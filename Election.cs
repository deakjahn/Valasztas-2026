using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class Election {
    [JsonPropertyName("ev")]
    public int Year { get; set; } = 2026;

    [JsonPropertyName("idopont")]
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("nevjegyzek")]
    public int Register { get; set; } = 0;

    [JsonPropertyName("atjelent")]
    public int Transfer { get; set; } = 0;

    [JsonPropertyName("kulkepv")]
    public int Absentee { get; set; } = 0;

    [JsonPropertyName("level")]
    public int Mail { get; set; } = 0;

    [JsonPropertyName("valaszto")]
    public int Total { get; set; } = 0;

    [JsonPropertyName("szavazo")]
    public Vote Voters { get; set; } = new(0, 0);

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
    public Dictionary<int, Party> Parties { get; set; } = [];

    [JsonPropertyName("nemz_ervenyes")]
    public Vote NationalityValid { get; set; } = new(0, 0);

    [JsonPropertyName("nemzetisegek")]
    public Dictionary<int, Nationality> Nationalities { get; set; } = [];

    [JsonPropertyName("megyek")]
    public Dictionary<string, County> Counties { get; set; } = [];

    [JsonPropertyName("levelszav")]
    public Mail MailList { get; set; } = new();
  }
}
