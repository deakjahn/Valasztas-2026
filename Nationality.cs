using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class Nationality(int code, string abbreviation, string name) {
    [JsonPropertyName("code")]
    public int Code { get; set; } = code;

    [JsonPropertyName("abbreviation")]
    public string Abbreviation { get; set; } = abbreviation;

    [JsonPropertyName("name")]
    public string Name { get; set; } = name;
  }
}
