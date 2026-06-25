using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class BallotStats {
    [JsonPropertyName("nevjegyzek")]
    public int Register { get; set; } = 0; // A

    [JsonPropertyName("megjelent")]
    public int InPerson { get; set; } = 0; // F

    [JsonPropertyName("belyegzo")]
    public int Stamped { get; set; } = 0; // K

    [JsonPropertyName("belyegzo_nelkul")]
    public int NotStamped { get; set; } = 0; // O

    [JsonPropertyName("elteres")]
    public int Difference { get; set; } = 0; // L

    [JsonPropertyName("ervenyes")]
    public Vote Valid { get; set; } = new(0, 0); // N

    [JsonPropertyName("ervenytelen")]
    public Vote Invalid { get; set; } = new(0, 0); // M
  }

  internal class IndividualBallots : BallotStats {
    [JsonPropertyName("valaszto")]
    public int Total { get; set; } = 0;

    [JsonPropertyName("szavazo")]
    public Vote Voters { get; set; } = new(0, 0);

    [JsonPropertyName("nemszavazo")]
    public Vote Absent => new(Total - Voters.Value, 100 - Voters.Percentage);

    [JsonPropertyName("szav")]
    [JsonPropertyOrder(1000)]
    public Dictionary<int, Vote> Votes { get; set; } = [];
  }

  internal class ListBallots : BallotStats {
    [JsonPropertyName("szav")]
    [JsonPropertyOrder(1000)]
    public Dictionary<int, Vote> Votes { get; set; } = [];
  }

  internal class NationalityBallots : BallotStats {
    [JsonPropertyName("szav")]
    [JsonPropertyOrder(1000)]
    public Dictionary<int, NationalityVotes> Votes { get; set; } = [];
  }

  internal class NationalityVotes : BallotStats {
    public bool IsNonZero() => Register != 0 || InPerson != 0 || NotStamped != 0 || Stamped != 0 || Difference != 0 || Invalid.Value != 0 || Valid.Value != 0;
  }
}
