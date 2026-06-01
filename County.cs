using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class County(string code, string name) {
    [JsonPropertyName("code")]
    public string Code { get; set; } = code;

    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("oevks")]
    public Dictionary<string, OEVK> OEVKs { get; set; } = [];
  }
}
