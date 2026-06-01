using System.Data;
using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class Party(int code, string abbreviation, string name) {
    [JsonPropertyName("code")]
    public int Code { get; set; } = code;

    [JsonPropertyName("abbreviation")]
    public string Abbreviation { get; set; } = abbreviation;

    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    public static Party Create(DataRow row) {
      return new(row.CellInt(0), row.CellString(1), row.CellString(2));
    }
  }
}
