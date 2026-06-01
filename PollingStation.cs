using System.Data;
using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class PollingStation(string code, string oevk) : BallotStats {
    [JsonPropertyName("code")]
    public string Code { get; set; } = code;

    [JsonIgnore]
    public string Id {
      get {
        string[] parts = Code.Split('-');
        if (parts.Length < 3)
          throw new DataException($"Érvénytelen szavazókörkód: {Code}");
        return parts[2];
      }
    }

    [JsonPropertyName("oevk")]
    public string OEVK { get; set; } = oevk;

    [JsonPropertyName("settlement")]
    public string Settlement { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public string? Address { get; set; } = null;

    [JsonPropertyName("absentee")]
    public int Absentee { get; set; } = 0;

    [JsonPropertyName("transfer")]
    public int Transfer { get; set; } = 0;

    [JsonPropertyName("voters")]
    public int Voters { get; set; } = 0;

    [JsonPropertyName("voted")]
    public int Voted { get; set; } = 0;

    [JsonPropertyName("envelope")]
    public int Envelope { get; set; } = 0;

    [JsonPropertyName("list")]
    [JsonPropertyOrder(1000)]
    public ListBallots List { get; set; } = new();

    [JsonPropertyName("individual")]
    [JsonPropertyOrder(1001)]
    public IndividualBallots Individual { get; set; } = new();

    [JsonPropertyName("nationalities")]
    [JsonPropertyOrder(1002)]
    public NationalityBallots Nationalities { get; set; } = new();

    public static PollingStation Create(DataRow row, string OEVK) {
      PollingStation self = new(row.CellString(1), OEVK) {
        Settlement = row.CellString(0),
        Transfer = row.CellInt(3), // B = Az átjelentkezett választópolgárok száma
        Absentee = row.CellInt(4), // C = Külképviseleti névjegyzékben lévő választópolgárok száma
        Envelope = row.CellInt(7), // I = Átjelentkezéssel és külképviseleten szavazó választópolgárok beérkezett lezárt borítékjainak száma
        Individual = new() {
          Register = row.CellInt(2), // AE = Szavazóköri névjegyzékben lévő választópolgárok száma
          Voters = row.CellInt(5), // EE = Választópolgárok száma összesen
          InPerson = row.CellInt(6), // FE = Szavazókörben szavazó választópolgárok száma
          Voted = row.CellInt(8), // JE = Szavazó választópolgárok száma összesen
          NotStamped = row.CellInt(9), // OE = Urnában és a beérkezett lezárt borítékokban lévő, bélyegzőlenyomat nélküli szavazólapok száma
          Stamped = row.CellInt(10), // KE = Urnában és a beérkezett lezárt borítékokban lévő, lebélyegzett szavazólapok száma
          Difference = row.CellInt(11), // LE = Eltérés a szavazóként megjelentek számától (LE=KE-JE; többlet: +/hiány:-)
          Invalid = row.CellInt(12), // ME = Érvénytelen lebélyegzett szavazólapok száma
          Valid = row.CellInt(13), // NE = Érvényes szavazólapok száma
        },
      };
      for (int cell = 14, candidate = 1; cell < row.Table.Columns.Count; cell++, candidate++)
        self.Individual.Votes[candidate] = row.CellInt(cell);
      return self;
    }

    public void AddParties(DataRow row) {
      if (Settlement != row.CellString(0)) throw new DataException($"Település neve eltér: {Settlement} [szavazókör: {Code}]");
      Register = row.CellInt(2); // AL = A szavazókör névjegyzékében lévő választópolgárok száma
      if (Transfer != row.CellInt(3)) throw new DataException($"Adateltérés B [szavazókör: {Code}]"); // B = Az átjelentkezett választópolgárok száma
      if (Absentee != row.CellInt(4)) throw new DataException($"Adateltérés C [szavazókör: {Code}]"); // C = Külképviseleti névjegyzékben lévő választópolgárok száma
      Voters = row.CellInt(5); // EL = Választópolgárok száma összesen
      InPerson = row.CellInt(6); // FL = Szavazókörben szavazó választópolgárok száma
      Envelope = row.CellInt(7); // IL = Átjelentkezéssel és külképviseleten szavazó választópolgárok beérkezett lezárt borítékjainak száma
      Voted = row.CellInt(8); // JL = A szavazó választópolgárok száma összesen
      NotStamped = row.CellInt(9); // OL = Urnában és beérkezett lezárt borítékban lévő, bélyegzőlenyomat nélküli szavazólapok száma
      Stamped = row.CellInt(10); // KL = Urnában és beérkezett lezárt borítékban lévő, lebélyegzett szavazólapok száma
      Difference = row.CellInt(11); // L = Eltérés a szavazóként megjelentek számától (többlet: +/hiány:-)
      Invalid = row.CellInt(12); // M = Érvénytelen lebélyegzett szavazólapok száma
      Valid = row.CellInt(13); // NL = Érvényes szavazólapok száma
      for (int cell = 14, list = 1; cell < row.Table.Columns.Count; cell++, list++)
        List.Votes[list] = row.CellInt(cell);
    }

    public void AddNationalities(DataRow row) {
      if (Settlement != row.CellString(0)) throw new DataException($"Település neve eltér: {Settlement} [szavazókör: {Code}]");
      List.Register = row.CellInt(2); // AL = A szavazókör névjegyzékében lévő választópolgárok száma
      List.InPerson = row.CellInt(3); // FL = Szavazókörben szavazó választópolgárok száma
      List.NotStamped = row.CellInt(4); // OL = Urnában és beérkezett lezárt borítékban lévő, bélyegzőlenyomat nélküli szavazólapok száma
      List.Stamped = row.CellInt(5); // KL = Urnában és beérkezett lezárt borítékban lévő, lebélyegzett szavazólapok száma
      List.Difference = row.CellInt(6); // L = Eltérés a szavazóként megjelentek számától (többlet: +/hiány:-)
      List.Invalid = row.CellInt(7); // M = Érvénytelen lebélyegzett szavazólapok száma
      List.Valid = row.CellInt(8); // NL = Érvényes szavazólapok száma
      Nationalities.Register = row.CellInt(9); // A = A szavazókör névjegyzékében lévő választópolgárok száma
      Nationalities.InPerson = row.CellInt(10); // F = Szavazókörben szavazó választópolgárok száma
      Nationalities.NotStamped = row.CellInt(11); // O = Urnában és beérkezett lezárt borítékban lévő, bélyegzőlenyomat nélküli szavazólapok száma
      Nationalities.Stamped = row.CellInt(12); // K = Urnában és beérkezett lezárt borítékban lévő, lebélyegzett szavazólapok száma
      Nationalities.Difference = row.CellInt(13); // L = Eltérés a szavazóként megjelentek számától (többlet: +/hiány:-)
      Nationalities.Invalid = row.CellInt(14); // M = Érvénytelen lebélyegzett szavazólapok száma
      Nationalities.Valid = row.CellInt(15); // N = Érvényes szavazólapok száma
      for (int cell = 16, nationality = 1; cell < row.Table.Columns.Count; cell += 7, nationality++) {
        var vote = new NationalityVotes {
          Register = row.CellInt(cell), // A = A szavazókör névjegyzékében lévő választópolgárok száma
          InPerson = row.CellInt(cell + 1), // F = Szavazókörben szavazó választópolgárok száma
          NotStamped = row.CellInt(cell + 2), // O = Urnában és beérkezett lezárt borítékban lévő, bélyegzőlenyomat nélküli szavazólapok száma
          Stamped = row.CellInt(cell + 3), // K = Urnában és beérkezett lezárt borítékban lévő, lebélyegzett szavazólapok száma
          Difference = row.CellInt(cell + 4), // L = Eltérés a szavazóként megjelentek számától (többlet: +/hiány:-)
          Invalid = row.CellInt(cell + 5), // M = Érvénytelen lebélyegzett szavazólapok száma
          Valid = row.CellInt(cell + 6), // N = Érvényes szavazólapok száma
        };
        if (vote.IsNonZero())
          Nationalities.Votes[nationality] = vote;
      }
    }
  }
}