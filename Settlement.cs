using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class Settlement(string code, string county, string id, string name) {
    [JsonPropertyName("azon")]
    public string Code { get; set; } = code;

    [JsonPropertyName("maz")]
    public string County { get; set; } = county;

    [JsonPropertyName("taz")]
    public string SettlementId { get; set; } = id;

    [JsonPropertyName("nev")]
    public string Name { get; set; } = name;
  }
}
