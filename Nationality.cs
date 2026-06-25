using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class Nationality(int code, string abbreviation, string name, int tl_id, string id) {
    [JsonPropertyName("azon")]
    public int Code { get; set; } = code;

    [JsonIgnore]
    public int TLid { get; set; } = tl_id;

    [JsonIgnore]
    public string ID { get; set; } = id;

    [JsonPropertyName("rovid")]
    public string Abbreviation { get; set; } = abbreviation;

    [JsonPropertyName("nev")]
    public string Name { get; set; } = name;

    [JsonPropertyName("jelolt")]
    public int Candidates { get; set; } = 0;

    [JsonPropertyName("mandatum")]
    public int Mandates { get; set; } = 0;

    [JsonPropertyName("ervenyes")]
    public int Valid { get; set; } = 0;

    [JsonPropertyName("szavazo")]
    public int Voters { get; set; } = 0;
  }
}
