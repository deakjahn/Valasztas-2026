using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class BallotStats {
    [JsonPropertyName("register")]
    public int Register { get; set; } = 0;

    [JsonPropertyName("inperson")]
    public int InPerson { get; set; } = 0;

    [JsonPropertyName("notstamped")]
    public int NotStamped { get; set; } = 0;

    [JsonPropertyName("stamped")]
    public int Stamped { get; set; } = 0;

    [JsonPropertyName("difference")]
    public int Difference { get; set; } = 0;

    [JsonPropertyName("invalid")]
    public int Invalid { get; set; } = 0;

    [JsonPropertyName("valid")]
    public int Valid { get; set; } = 0;
  }

  internal class ListBallots : BallotStats {
    [JsonPropertyName("votes")]
    [JsonPropertyOrder(1000)]
    public Dictionary<int, int> Votes { get; set; } = [];
  }

  internal class IndividualBallots : BallotStats {
    [JsonPropertyName("voters")]
    [JsonPropertyOrder(999)]
    public int Voters { get; set; } = 0;

    [JsonPropertyName("voted")]
    [JsonPropertyOrder(999)]
    public int Voted { get; set; } = 0;

    [JsonPropertyName("votes")]
    [JsonPropertyOrder(1000)]
    public Dictionary<int, int> Votes { get; set; } = [];
  }

  internal class NationalityBallots : BallotStats {
    [JsonPropertyName("byList")]
    [JsonPropertyOrder(1000)]
    public Dictionary<int, NationalityVotes> Votes { get; set; } = [];
  }

  internal class NationalityVotes : BallotStats {
    public bool IsNonZero() => Register != 0 || InPerson != 0 || NotStamped != 0 || Stamped != 0 || Difference != 0 || Invalid != 0 || Valid != 0;
  }
}
