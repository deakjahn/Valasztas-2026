using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class Election {
    [JsonPropertyName("year")]
    public int Year { get; set; } = 2026;

    [JsonPropertyName("generated")]
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("parties")]
    public Dictionary<int, Party> Parties { get; set; } = [];

    [JsonPropertyName("nationalities")]
    public Dictionary<int, Nationality> Nationalities { get; set; } = [];

    [JsonPropertyName("counties")]
    public Dictionary<string, County> Counties { get; set; } = [];
  }
}
