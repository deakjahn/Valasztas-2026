using System.Data;
using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class OEVK(string code, string name) {
    [JsonPropertyName("code")]
    public string Code { get; set; } = code;

    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("center")]
    public string? Center { get; set; } = null;

    [JsonPropertyName("border")]
    public string? Border { get; set; } = null;

    [JsonPropertyName("voted")]
    public int Voted { get; set; } = 0;

    [JsonPropertyName("domicile")]
    public int Domicile { get; set; } = 0;

    [JsonPropertyName("transferOut")]
    public int TransferOut { get; set; } = 0;

    [JsonPropertyName("transferIn")]
    public int TransferIn { get; set; } = 0;

    [JsonPropertyName("absentee")]
    public int Absentee { get; set; } = 0;

    [JsonPropertyName("register")]
    public int Register { get; set; } = 0;

    [JsonPropertyName("inland")]
    public int Inland { get; set; } = 0;

    [JsonPropertyName("candidates")]
    public Dictionary<int, Candidate> Candidates { get; set; } = [];

    [JsonPropertyName("stations")]
    public Dictionary<string, PollingStation> PollingStations { get; set; } = [];

    public static string ExtractSettlement(string name) {
      return name switch {
        "Budapest 01" => "01",
        "Budapest 02" => "02",
        "Budapest 03" => "03",
        "Budapest 04" => "04",
        "Budapest 05" => "05",
        "Budapest 06" => "06",
        "Budapest 07" => "07",
        "Budapest 08" => "08",
        "Budapest 09" => "09",
        "Budapest 10" => "10",
        "Budapest 11" => "11",
        "Budapest 12" => "12",
        "Budapest 13" => "13",
        "Budapest 14" => "14",
        "Budapest 15" => "15",
        "Budapest 16" => "16",
        "Budapest 17" => "17",
        "Budapest 18" => "18",
        "Budapest 19" => "19",
        "Budapest 20" => "20",
        "Budapest 21" => "21",
        "Budapest 22" => "22",
        "Budapest 23" => "23",
        _ => name,
      };
    }
  }
}
