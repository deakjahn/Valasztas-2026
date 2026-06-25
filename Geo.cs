using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class GeoOEVK {
    [JsonPropertyName("maz")]
    public string County { get; set; } = string.Empty;

    [JsonPropertyName("evk")]
    public string OEVK { get; set; } = string.Empty;

    [JsonPropertyName("centrum")]
    public string Center { get; set; } = string.Empty;

    [JsonPropertyName("poligon")]
    public string Border { get; set; } = string.Empty;
  }

  internal class GeoCounties {
    [JsonPropertyName("PvOnHeader")]
    public object? Header { get; set; }

    [JsonPropertyName("list")]
    public List<GeoCounty> Counties { get; set; } = [];

    internal class GeoCounty {
      [JsonPropertyName("leiro")]
      public GeoCountyGeo Geo { get; set; } = new();

      [JsonPropertyName("letszam")]
      public GeoCountyStats Stats { get; set; } = new();

      internal class GeoCountyGeo {
        [JsonPropertyName("maz")]
        public string County { get; set; } = string.Empty;

        [JsonPropertyName("nev")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("rovid_nev")]
        public string ShortName { get; set; } = string.Empty;

        [JsonPropertyName("nevi")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("nevi_en")]
        public string FullNameEn { get; set; } = string.Empty;

        [JsonPropertyName("centrum")]
        public string Center { get; set; } = string.Empty;

        [JsonPropertyName("megye_poligon")]
        public string Border { get; set; } = string.Empty;
      }

      internal class GeoCountyStats {
        [JsonPropertyName("indulo")]
        public int Voters { get; set; } = 0;

        [JsonPropertyName("honos")]
        public int Domicile { get; set; } = 0;

        [JsonPropertyName("atjel")]
        public int Transfer { get; set; } = 0;

        [JsonPropertyName("atjelInnen")]
        public int TransferOut { get; set; } = 0;

        [JsonPropertyName("kuvi")]
        public int Absentee { get; set; } = 0;

        [JsonPropertyName("osszesen")]
        public int Total { get; set; } = 0;
      }
    }
  }

  internal class GeoStations {
    [JsonPropertyName("PvOnHeader")]
    public object? Header { get; set; }

    [JsonPropertyName("list")]
    public List<GeoStation> Stations { get; set; } = [];

    internal class GeoStation {
      [JsonPropertyName("maz")]
      public string County { get; set; } = string.Empty;

      [JsonPropertyName("taz")]
      public string Settlement { get; set; } = string.Empty;

      [JsonPropertyName("szk")]
      public string Station { get; set; } = string.Empty;

      [JsonPropertyName("centrum")]
      public string Center { get; set; } = string.Empty;

      [JsonPropertyName("poligon")]
      public string Border { get; set; } = string.Empty;
    }
  }
}