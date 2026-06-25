using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Választás_2026 {
  [JsonConverter(typeof(VoteConverter))]
  public class Vote(int votes, decimal votesPc) {
    public decimal[] Votes { get; set; } = [votes, votesPc];

    [JsonIgnore]
    public int Value {
      get => (int)Votes[0];
      set => Votes[0] = value;
    }

    [JsonIgnore]
    public decimal Percentage {
      get => Votes[1];
      set => Votes[1] = value;
    }
  }

  internal class VoteConverter : JsonConverter<Vote> {
    static JsonSerializerOptions noIndentOptions = new() { WriteIndented = false };

    public override Vote? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, Vote vote, JsonSerializerOptions options) {
      var bufferWriter = new ArrayBufferWriter<byte>();
      using (var innerWriter = new Utf8JsonWriter(bufferWriter)) {
        innerWriter.WriteStartArray();
        JsonSerializer.Serialize(innerWriter, vote.Value, noIndentOptions);
        JsonSerializer.Serialize(innerWriter, vote.Percentage, noIndentOptions);
        innerWriter.WriteEndArray();
      }
      writer.WriteRawValue(bufferWriter.WrittenSpan, skipInputValidation: true);
    }
  }
}
