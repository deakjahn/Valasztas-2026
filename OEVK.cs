using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class OEVK(string code, string name) {
    [JsonPropertyName("code")]
    public string Code { get; set; } = code;

    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("center")]
    public string? Center { get; set; } = null;

    [JsonPropertyName("border")]
    public string? Border { get; set; } = null;

    [JsonPropertyName("voted")]
    public int Voted { get; set; } = 0;

    [JsonPropertyName("domicile")]
    public int Domicile { get; set; } = 0;

    [JsonPropertyName("transferOut")]
    public int TransferOut { get; set; } = 0;

    [JsonPropertyName("transferIn")]
    public int TransferIn { get; set; } = 0;

    [JsonPropertyName("absentee")]
    public int Absentee { get; set; } = 0;

    [JsonPropertyName("register")]
    public int Register { get; set; } = 0;

    [JsonPropertyName("inland")]
    public int Inland { get; set; } = 0;

    [JsonPropertyName("candidates")]
    public Dictionary<int, Candidate> Candidates { get; set; } = [];

    [JsonPropertyName("stations")]
    public Dictionary<string, PollingStation> PollingStations { get; set; } = [];
  }
}
