using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class Mail() {
    [JsonPropertyName("beerkezett")]
    public Vote Incoming { get; set; } = new(0, 0);

    [JsonPropertyName("beerkezett_ervenyes")]
    public Vote IncomingValid { get; set; } = new(0, 0);

    [JsonPropertyName("beerkezett_ervenytelen")]
    public Vote IncomingInvalid { get; set; } = new(0, 0);

    [JsonPropertyName("boritek")]
    public int Envelope { get; set; } = 0;

    [JsonPropertyName("elteres")]
    public int Difference { get; set; } = 0;

    [JsonPropertyName("ervenyes")]
    public Vote Valid { get; set; } = new(0, 0);

    [JsonPropertyName("ervenytelen")]
    public Vote Invalid { get; set; } = new(0, 0);

    [JsonPropertyName("listas")]
    public Dictionary<int, Vote> Lists { get; set; } = [];
  }
}
