using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class Geo {
    [JsonPropertyName("maz")]
    public string County { get; set; } = string.Empty;

    [JsonPropertyName("evk")]
    public string OEVK { get; set; } = string.Empty;

    [JsonPropertyName("centrum")]
    public string Center { get; set; } = string.Empty;

    [JsonPropertyName("poligon")]
    public string Border { get; set; } = string.Empty;
  }
}