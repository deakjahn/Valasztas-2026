using System.Data;
using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class Party(int code, string abbreviation, string name) {
    [JsonPropertyName("azon")]
    public int Code { get; set; } = code;

    [JsonPropertyName("rovid")]
    public string Abbreviation { get; set; } = abbreviation;

    [JsonPropertyName("nev")]
    public string Name { get; set; } = name;

    [JsonPropertyName("jelolt")]
    public int Candidates { get; set; } = 0;

    [JsonPropertyName("mandatum")]
    public int Mandates { get; set; } = 0;

    [JsonPropertyName("szavazat")]
    public Vote Votes { get; set; } = new(0, 0);

    [JsonPropertyName("hazai")]
    public Vote InlandVotes { get; set; } = new(0, 0);

    [JsonPropertyName("level")]
    public Vote AbsenteeVotes { get; set; } = new(0, 0);

    public static Party Create(DataRow row) {
      return new(row.CellInt(0), row.CellString(1), row.CellString(2));
    }
  }
}
