using System.Data;
using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class Candidate(int code, string name) {
    [JsonPropertyName("code")]
    public int Code { get; set; } = code;

    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonPropertyName("county")]
    public string County { get; set; } = string.Empty;

    [JsonPropertyName("oevk")]
    public string OEVK { get; set; } = string.Empty;

    [JsonPropertyName("party")]
    public string? Party { get; set; } = null;

    [JsonIgnore]
    public string Key => $"{Name}|{County}|{OEVK}";

    public static Candidate Create(DataRow row) {
      return new(row.CellInt(2), row.CellString(3)) {
        County = GetCounty(row.CellString(0)),
        OEVK = row.CellString(1),
      };
    }

    public static Candidate Identify(DataRow row) {
      string[] OEVK = row.CellString(3).Split(", ");
      return new(0, row.CellString(0)) {
        County = ExtractCounty(OEVK.First()),
        OEVK = ExtractOEVK(OEVK.Last()),
        Party = GetParty(row.CellString(5))
      };
    }

    public static string GetCounty(string name) {
      return name switch {
        "BUDAPEST" => "01",
        "BARANYA" => "02",
        "BÁCS-KISKUN" => "03",
        "BÉKÉS" => "04",
        "BORSOD-ABAÚJ-ZEMPLÉN" => "05",
        "CSONGRÁD-CSANÁD" => "06",
        "FEJÉR" => "07",
        "GYŐR-MOSON-SOPRON" => "08",
        "HAJDÚ-BIHAR" => "09",
        "HEVES" => "10",
        "JÁSZ-NAGYKUN-SZOLNOK" => "11",
        "KOMÁROM-ESZTERGOM" => "12",
        "NÓGRÁD" => "13",
        "PEST" => "14",
        "SOMOGY" => "15",
        "SZABOLCS-SZATMÁR-BEREG" => "16",
        "TOLNA" => "17",
        "VAS" => "18",
        "VESZPRÉM" => "19",
        "ZALA" => "20",
        _ => throw new DataException($"Ismeretlen vármegye: {name}"),
      };
    }

    private static string ExtractCounty(string name) {
      return name switch {
        "Budapest főváros" => "01",
        "Baranya vármegye" => "02",
        "Bács-Kiskun vármegye" => "03",
        "Békés vármegye" => "04",
        "Borsod-Abaúj-Zemplén vármegye" => "05",
        "Csongrád-Csanád vármegye" => "06",
        "Fejér vármegye" => "07",
        "Győr-Moson-Sopron vármegye" => "08",
        "Hajdú-Bihar vármegye" => "09",
        "Heves vármegye" => "10",
        "Jász-Nagykun-Szolnok vármegye" => "11",
        "Komárom-Esztergom vármegye" => "12",
        "Nógrád vármegye" => "13",
        "Pest vármegye" => "14",
        "Somogy vármegye" => "15",
        "Szabolcs-Szatmár-Bereg vármegye" => "16",
        "Tolna vármegye" => "17",
        "Vas vármegye" => "18",
        "Veszprém vármegye" => "19",
        "Zala vármegye" => "20",
        _ => throw new DataException($"Ismeretlen vármegye: {name}"),
      };
    }

    private static string ExtractOEVK(string name) {
      return name switch {
        "01. számú egyéni választókerület" => "01",
        "02. számú egyéni választókerület" => "02",
        "03. számú egyéni választókerület" => "03",
        "04. számú egyéni választókerület" => "04",
        "05. számú egyéni választókerület" => "05",
        "06. számú egyéni választókerület" => "06",
        "07. számú egyéni választókerület" => "07",
        "08. számú egyéni választókerület" => "08",
        "09. számú egyéni választókerület" => "09",
        "10. számú egyéni választókerület" => "10",
        "11. számú egyéni választókerület" => "11",
        "12. számú egyéni választókerület" => "12",
        "13. számú egyéni választókerület" => "13",
        "14. számú egyéni választókerület" => "14",
        "15. számú egyéni választókerület" => "15",
        "16. számú egyéni választókerület" => "16",
        _ => throw new DataException($"Ismeretlen OEVK: {name}"),
      };
    }

    private static string? GetParty(string name) {
      return name switch {
        "Magyar Kétfarkú Kutya Párt" => "MKKP",
        "Tisztelet és Szabadság Párt" => "TISZA",
        "Mi Hazánk Mozgalom" => "Mi Hazánk",
        "Demokratikus Koalíció" => "DK",
        "FIDESZ - Magyar Polgári Szövetség-Kereszténydemokrata Néppárt" => "FIDESZ-KDNP",
        "FIDESZ - Magyar Polgári Szövetség, Kereszténydemokrata Néppárt" => "FIDESZ-KDNP",
        "Jobbik Magyarországért Mozgalom" => "Jobbik",
        "A SZOLIDARITÁS PÁRTJA, Magyar Munkáspárt" => "Munkáspárt",
        "Nemzetegyesítő Mozgalom a Szentkorona Országaiért" => "Szentkorona",
        "Normális Élet Pártja" => "NÉP",
        "Középen Állók Pártja" => "KÁP",
        "IRÁNY a Jövő Párt" => "IRÁNY",
        "Magyar Igazság és Élet Pártja" => "MIÉP",
        "LMP- Magyarország Zöld Pártja" => "LMP",
        "Független jelölt" => null,
        _ => throw new DataException($"Ismeretlen párt: {name}"),
      };
    }
  }
}
