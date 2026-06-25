using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class County(string code, string name) {
    [JsonPropertyName("maz")]
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

    [JsonPropertyName("part_ervenyes")]
    public Vote ListValid { get; set; } = new(0, 0);

    [JsonPropertyName("partok")]
    public Dictionary<int, Vote> Parties { get; set; } = [];

    [JsonPropertyName("nemz_ervenyes")]
    public Vote NationalityValid { get; set; } = new(0, 0);

    [JsonPropertyName("nemzetisegek")]
    public Dictionary<int, Vote> Nationalities { get; set; } = [];

    [JsonPropertyName("oevkk")]
    public Dictionary<string, Constituency> OEVKs { get; set; } = [];

    public int PartyVotes(int party) => Parties[party].Value;
    public int NationalityVotes(int nationality) => Nationalities.TryGetValue(nationality, out var vote) ? vote.Value : 0;
  }
}
