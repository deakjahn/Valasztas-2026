using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class Constituency(string code, string name) {
    [JsonPropertyName("oevk")]
    public string Code { get; set; } = code;

    [JsonPropertyName("nev")]
    public string Name { get; set; } = name;

    [JsonPropertyName("kozeppont")]
    public string? Center { get; set; } = null;

    [JsonPropertyName("korvonal")]
    public string? Border { get; set; } = null;

    [JsonPropertyName("nevjegyzek")]
    public int Register { get; set; } = 0;

    [JsonPropertyName("atjelent")]
    public int Transfer { get; set; } = 0;

    [JsonPropertyName("atjelent_mashova")]
    public int TransferOut { get; set; } = 0;

    [JsonPropertyName("kulkepv")]
    public int Absentee { get; set; } = 0;

    [JsonPropertyName("valaszto")]
    public int Total { get; set; } = 0;

    [JsonPropertyName("szavazo")]
    public Vote Voters { get; set; } = new Vote(0, 0);

    [JsonPropertyName("nemszavazo")]
    public Vote Absent => new(Total - Voters.Value, 100 - Voters.Percentage);

    [JsonPropertyName("boritek")]
    public int Envelope { get; set; } = 0;

    [JsonPropertyName("ervenyes")]
    public Vote Valid { get; set; } = new(0, 0);

    [JsonPropertyName("ervenytelen")]
    public Vote Invalid { get; set; } = new(0, 0);

    [JsonPropertyName("jeloltek")]
    public Dictionary<int, Candidate> Candidates { get; set; } = [];

    [JsonPropertyName("szavazokorok")]
    public Dictionary<string, PollingStation> PollingStations { get; set; } = [];

    public static string ExtractSettlement(string name) {
      return name switch {
        "Budapest 01" => "01",
        "Budapest 02" => "02",
        "Budapest 03" => "03",
        "Budapest 04" => "04",
        "Budapest 05" => "05",
        "Budapest 06" => "06",
        "Budapest 07" => "07",
        "Budapest 08" => "08",
        "Budapest 09" => "09",
        "Budapest 10" => "10",
        "Budapest 11" => "11",
        "Budapest 12" => "12",
        "Budapest 13" => "13",
        "Budapest 14" => "14",
        "Budapest 15" => "15",
        "Budapest 16" => "16",
        "Budapest 17" => "17",
        "Budapest 18" => "18",
        "Budapest 19" => "19",
        "Budapest 20" => "20",
        "Budapest 21" => "21",
        "Budapest 22" => "22",
        "Budapest 23" => "23",
        _ => name,
      };
    }

    public int PartyVotes(int party) => PollingStations.Values.Sum(szk => szk.PartyVote(party));
    public int NationalityVotes(int nationality) => PollingStations.Values.Sum(szk => szk.NationalityVote(nationality));

    public int PartyValid() => PollingStations.Values.Sum(szk => szk.Lists.Parties.Valid.Value);
    public int NationalityValid() => PollingStations.Values.Sum(szk => szk.Lists.Nationalities.Valid.Value);
  }
}