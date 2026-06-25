using System.Data;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Választás_2026 {
  internal partial class PollingStation(string code, string oevk) {
    [JsonPropertyName("azon")]
    public string Code { get; set; } = code;

    [JsonPropertyName("maz")]
    public string County => Code.Split('-')[0];

    [JsonPropertyName("taz")]
    public string SettlementId => Code.Split('-')[1];

    [JsonPropertyName("szk")]
    public string StationId => Code.Split('-')[2];

    [JsonPropertyName("oevk")]
    public string OEVK { get; set; } = oevk;

    [JsonPropertyName("irsz")]
    public string? PostCode {
      get {
        string PostCode = PostCodePart().Match(SettlementFull).Value;
        return string.IsNullOrEmpty(PostCode) ? null : PostCode;
      }
    }

    [JsonPropertyName("telepules")]
    public string Settlement => Extensions.FirstValid(
      SettlementPart().Match(SettlementFull).Value,
      SettlementFull);

    [JsonIgnore]
    public string SettlementFull { get; set; } = string.Empty;

    [JsonPropertyName("cim")]
    public string Address => FixPeriod(Extensions.FirstValid(
      AddressPartParens().Match(Description).Value,
      AddressPartSlash().Match(Description).Value,
      Description));

    [JsonPropertyName("intezmeny")]
    public string Name => FixPeriod(Extensions.FirstValid(
      NamePartParens().Match(Description).Value,
      NamePartSlash().Match(Description).Value));

    [JsonPropertyName("leiras")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("szamlalo_szk")]
    public bool Tally { get; set; } = false;

    [JsonPropertyName("pozicio")]
    public string? Location { get; set; } = null;

    [JsonPropertyName("korvonal")]
    public string? Border { get; set; } = null;

    [JsonPropertyName("atjelent")]
    public int Transfer { get; set; } = 0;

    [JsonPropertyName("kulkepv")]
    public int Absentee { get; set; } = 0;

    [JsonPropertyName("boritek")]
    public int Envelope { get; set; } = 0;

    [JsonPropertyName("egyeni")]
    [JsonPropertyOrder(1000)]
    public IndividualBallots Individual { get; set; } = new();

    [JsonPropertyName("listas")]
    [JsonPropertyOrder(1001)]
    public ListStats Lists { get; set; } = new();

    public static PollingStation Create(DataRow row, string OEVK) {
      PollingStation self = new(row.CellString(1), OEVK) {
        SettlementFull = row.CellString(0),
      };
      self.Individual.Register = row.CellInt(2); // AE = Szavazóköri névjegyzékben lévő választópolgárok száma
      self.Transfer = row.CellInt(3); // B = Az átjelentkezett választópolgárok száma
      self.Absentee = row.CellInt(4); // C = Külképviseleti névjegyzékben lévő választópolgárok száma
      self.Individual.Total = row.CellInt(5); // EE = Választópolgárok száma összesen = AE + B + C
      self.Individual.InPerson = row.CellInt(6); // FE = Szavazókörben szavazó választópolgárok száma
      self.Envelope = row.CellInt(7); // I = Átjelentkezéssel és külképviseleten szavazó választópolgárok beérkezett lezárt borítékjainak száma
      self.Individual.Voters = row.CellInt(8).ToVote(self.Individual.Total); // JE = Szavazó választópolgárok száma összesen
      self.Individual.NotStamped = row.CellInt(9); // OE = Urnában és a beérkezett lezárt borítékokban lévő, bélyegzőlenyomat nélküli szavazólapok száma
      self.Individual.Stamped = row.CellInt(10); // KE = Urnában és a beérkezett lezárt borítékokban lévő, lebélyegzett szavazólapok száma
      self.Individual.Difference = row.CellInt(11); // LE = Eltérés a szavazóként megjelentek számától (LE=KE-FE; többlet: +/hiány:-)
      self.Individual.Invalid = row.CellInt(12).ToVote(self.Individual.Stamped); // ME = Érvénytelen lebélyegzett szavazólapok száma
      self.Individual.Valid = row.CellInt(13).ToVote(self.Individual.Stamped); // NE = Érvényes szavazólapok száma
      self.Tally = (self.Transfer > 0 && self.Absentee > 0);
      for (int cell = 14, candidate = 1; cell < row.Table.Columns.Count; cell++, candidate++)
        self.Individual.Votes[candidate] = row.CellInt(cell).ToVote(self.Individual.Valid.Value);
      return self;
    }

    public void AddParties(DataRow row) {
      if (SettlementFull != row.CellString(0)) throw new DataException($"Település neve eltér: {Settlement} [szavazókör: {Code}]");
      Lists.Register = row.CellInt(2); // AL = A szavazókör névjegyzékében lévő választópolgárok száma
      if (Transfer != row.CellInt(3)) throw new DataException($"Adateltérés B [szavazókör: {Code}]"); // B = Az átjelentkezett választópolgárok száma
      if (Absentee != row.CellInt(4)) throw new DataException($"Adateltérés C [szavazókör: {Code}]"); // C = Külképviseleti névjegyzékben lévő választópolgárok száma
      Lists.Total = row.CellInt(5); // EL = Választópolgárok száma összesen = AL + B + C
      Lists.InPerson = row.CellInt(6); // FL = Szavazókörben szavazó választópolgárok száma
      Lists.Envelope = row.CellInt(7); // IL = Átjelentkezéssel és külképviseleten szavazó választópolgárok beérkezett lezárt borítékjainak száma
      Lists.Voters = row.CellInt(8).ToVote(Lists.Total); // JL = A szavazó választópolgárok száma összesen
      Lists.NotStamped = row.CellInt(9); // OL = Urnában és beérkezett lezárt borítékban lévő, bélyegzőlenyomat nélküli szavazólapok száma
      Lists.Stamped = row.CellInt(10); // KL = Urnában és beérkezett lezárt borítékban lévő, lebélyegzett szavazólapok száma
      if (Lists.Difference != row.CellInt(11)) throw new DataException($"Adateltérés L [szavazókör: {Code}]"); // LL = Eltérés a szavazóként megjelentek számától (LL=KL-FL; többlet: +/hiány:-)
      Lists.Invalid = row.CellInt(12).ToVote(Lists.Stamped); // ML = Érvénytelen lebélyegzett szavazólapok száma
      Lists.Valid = row.CellInt(13).ToVote(Lists.Stamped); // NL = Érvényes szavazólapok száma
      for (int cell = 14, list = 1; cell < row.Table.Columns.Count; cell++, list++)
        Lists.Parties.Votes[list] = row.CellInt(cell).ToVote(Lists.Valid.Value);
    }

    public void AddNationalities(DataRow row, List<int> NationalityCodes) {
      if (SettlementFull != row.CellString(0)) throw new DataException($"Település neve eltér: {Settlement} [szavazókör: {Code}]");
      Lists.Parties.Register = row.CellInt(2); // AL = A szavazókör névjegyzékében lévő választópolgárok száma
      Lists.Parties.InPerson = row.CellInt(3); // FL = Szavazókörben szavazó választópolgárok száma
      Lists.Parties.NotStamped = row.CellInt(4); // OL = Urnában és beérkezett lezárt borítékban lévő, bélyegzőlenyomat nélküli szavazólapok száma
      Lists.Parties.Stamped = row.CellInt(5); // KL = Urnában és beérkezett lezárt borítékban lévő, lebélyegzett szavazólapok száma
      Lists.Parties.Difference = row.CellInt(6); // L = Eltérés a szavazóként megjelentek számától (többlet: +/hiány:-)
      Lists.Parties.Invalid = row.CellInt(7).ToVote(Lists.Parties.Stamped); // M = Érvénytelen lebélyegzett szavazólapok száma
      Lists.Parties.Valid = row.CellInt(8).ToVote(Lists.Parties.Stamped); // NL = Érvényes szavazólapok száma

      Lists.Nationalities.Register = row.CellInt(9); // A = A szavazókör névjegyzékében lévő választópolgárok száma
      Lists.Nationalities.InPerson = row.CellInt(10); // F = Szavazókörben szavazó választópolgárok száma
      Lists.Nationalities.NotStamped = row.CellInt(11); // O = Urnában és beérkezett lezárt borítékban lévő, bélyegzőlenyomat nélküli szavazólapok száma
      Lists.Nationalities.Stamped = row.CellInt(12); // K = Urnában és beérkezett lezárt borítékban lévő, lebélyegzett szavazólapok száma
      Lists.Nationalities.Difference = row.CellInt(13); // L = Eltérés a szavazóként megjelentek számától (többlet: +/hiány:-)
      Lists.Nationalities.Invalid = row.CellInt(14).ToVote(Lists.Parties.Stamped); // M = Érvénytelen lebélyegzett szavazólapok száma
      Lists.Nationalities.Valid = row.CellInt(15).ToVote(Lists.Parties.Stamped); // N = Érvényes szavazólapok száma
      for (int cell = 16, nationality = 0; cell < row.Table.Columns.Count && nationality < NationalityCodes.Count; cell += 7, nationality++) {
        var vote = new NationalityVotes {
          Register = row.CellInt(cell), // A = A szavazókör névjegyzékében lévő választópolgárok száma
          InPerson = row.CellInt(cell + 1), // F = Szavazókörben szavazó választópolgárok száma
          NotStamped = row.CellInt(cell + 2), // O = Urnában és beérkezett lezárt borítékban lévő, bélyegzőlenyomat nélküli szavazólapok száma
          Stamped = row.CellInt(cell + 3), // K = Urnában és beérkezett lezárt borítékban lévő, lebélyegzett szavazólapok száma
          Difference = row.CellInt(cell + 4), // L = Eltérés a szavazóként megjelentek számától (többlet: +/hiány:-)
        };
        vote.Invalid = row.CellInt(cell + 5).ToVote(vote.Stamped); // M = Érvénytelen lebélyegzett szavazólapok száma
        vote.Valid = row.CellInt(cell + 6).ToVote(vote.Stamped); // N = Érvényes szavazólapok száma
        if (vote.IsNonZero())
          Lists.Nationalities.Votes[NationalityCodes[nationality]] = vote;
      }
    }

    [GeneratedRegex(@"^(\d\d\d\d)(?=\s+)")]
    private static partial Regex PostCodePart();

    [GeneratedRegex(@"(?<=\d\d\d\d\s+).+")]
    private static partial Regex SettlementPart();

    [GeneratedRegex(@"(?<=\().+?(?=\))")]
    private static partial Regex NamePartParens();

    [GeneratedRegex(@"(?<=\s*/\s*).+")]
    private static partial Regex NamePartSlash();

    [GeneratedRegex(@"^.*?(?=\s*\()")]
    private static partial Regex AddressPartParens();

    [GeneratedRegex(@"^.*?(?=\s*/\s*)")]
    private static partial Regex AddressPartSlash();

    [GeneratedRegex(@"\.(?=\S)")]
    private static partial Regex WrongPeriod();

    private string FixPeriod(string s) => WrongPeriod().Replace(s, ". ");

    public int PartyVote(int party) => Lists.Parties.Votes[party].Value;
    public int NationalityVote(int nationality) => Lists.Nationalities.Votes.TryGetValue(nationality, out var vote) ? vote.Valid.Value : 0;
    public int IndividualVote(int candidate) => Individual.Votes[candidate].Value;

    internal class ListStats {
      [JsonPropertyName("nevjegyzek")]
      public int Register { get; set; } = 0;

      [JsonPropertyName("megjelent")]
      public int InPerson { get; set; } = 0;

      [JsonPropertyName("valaszto")]
      public int Total { get; set; } = 0;

      [JsonPropertyName("szavazo")]
      public Vote Voters { get; set; } = new Vote(0, 0);

      [JsonPropertyName("nemszavazo")]
      public Vote Absent => new(Total - Voters.Value, 100 - Voters.Percentage);

      [JsonPropertyName("boritek")]
      public int Envelope { get; set; } = 0;

      [JsonPropertyName("belyegzo")]
      public int Stamped { get; set; } = 0;

      [JsonPropertyName("belyegzo_nelkul")]
      public int NotStamped { get; set; } = 0;

      [JsonPropertyName("elteres")]
      public int Difference { get; set; } = 0;

      [JsonPropertyName("ervenyes")]
      public Vote Valid { get; set; } = new(0, 0);

      [JsonPropertyName("ervenytelen")]
      public Vote Invalid { get; set; } = new(0, 0);

      [JsonPropertyName("partok")]
      [JsonPropertyOrder(1002)]
      public ListBallots Parties { get; set; } = new();

      [JsonPropertyName("nemzetisegi")]
      [JsonPropertyOrder(1003)]
      public NationalityBallots Nationalities { get; set; } = new();
    }
  }
}