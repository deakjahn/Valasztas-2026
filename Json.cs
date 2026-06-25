using System.Text.Json.Serialization;

namespace Választás_2026 {
  internal class JsonConstituencies {
    [JsonPropertyName("PvOnHeader")]
    public object? Header { get; set; }

    [JsonPropertyName("list")]
    public List<JsonConstituency> Constituencies { get; set; } = [];

    internal class JsonConstituency {
      [JsonPropertyName("maz")]
      public string County { get; set; } = string.Empty;

      [JsonPropertyName("maz_nev")]
      public string CountyName { get; set; } = string.Empty;

      [JsonPropertyName("maz_nev_en")]
      public string CountyNameEn { get; set; } = string.Empty;

      [JsonPropertyName("evk")]
      public string OEVK { get; set; } = string.Empty;

      [JsonPropertyName("evk_nev")]
      public string OEVKName { get; set; } = string.Empty;

      [JsonPropertyName("evk_nev_en")]
      public string OEVKNameEn { get; set; } = string.Empty;

      [JsonPropertyName("szekhely")]
      public string Settlement { get; set; } = string.Empty;

      [JsonPropertyName("szekhely_en")]
      public string SettlementEn { get; set; } = string.Empty;

      [JsonPropertyName("oevk_jeloltre_szavhat")]
      public int Total { get; set; } = 0;

      [JsonPropertyName("letszam")]
      public JsonConstituencyStats Stats { get; set; } = new();

      internal class JsonConstituencyStats {
        [JsonPropertyName("indulo")]
        public int Start { get; set; } = 0;

        [JsonPropertyName("honos")]
        public int Inland { get; set; } = 0;

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

  internal class JsonOEVKReports {
    [JsonPropertyName("PvOnHeader")]
    public object? Header { get; set; }

    [JsonPropertyName("list")]
    public List<JsonReport> Reports { get; set; } = [];

    internal class JsonReport {
      [JsonPropertyName("maz")]
      public string County { get; set; } = string.Empty;

      [JsonPropertyName("evk")]
      public string OEVK { get; set; } = string.Empty;

      [JsonPropertyName("egyeni_jkv")]
      public JsonReportStats Stats { get; set; } = new();

      internal class JsonReportStats {
        [JsonPropertyName("vp_belf_njben")]
        public int Register { get; set; } = 0;

        [JsonPropertyName("vp_atjel")]
        public int Transfer { get; set; } = 0;

        [JsonPropertyName("vp_kulkepv")]
        public int Absentee { get; set; } = 0;

        [JsonPropertyName("vp_level")]
        public int Mail { get; set; } = 0;

        [JsonPropertyName("vp_osszes")]
        public int Total { get; set; } = 0;

        [JsonPropertyName("szavazott_osszesen")]
        public int Voters { get; set; } = 0;

        [JsonPropertyName("szavazott_osszesen_szaz")]
        public decimal VotersPc { get; set; } = 0;

        [JsonPropertyName("szl_ervenyes")]
        public int Valid { get; set; } = 0;

        [JsonPropertyName("szl_ervenyes_szaz")]
        public decimal ValidPc { get; set; } = 0;

        [JsonPropertyName("szl_ervenytelen")]
        public int Invalid { get; set; } = 0;

        [JsonPropertyName("szl_ervenytelen_szaz")]
        public decimal InvalidPc { get; set; } = 0;

        [JsonPropertyName("szl_urna_boritek")]
        public int Envelope { get; set; } = 0;

        [JsonPropertyName("tetelek")]
        public List<JsonReportVotes> Votes { get; set; } = [];

        internal class JsonReportVotes {
          [JsonPropertyName("szavlap_sorsz")]
          public int Candidate { get; set; } = 0;

          [JsonPropertyName("szavazat")]
          public int Votes { get; set; } = 0;

          [JsonPropertyName("szavazat_szaz")]
          public decimal VotesPc { get; set; } = 0;

          [JsonPropertyName("mandatum")]
          public int Won { get; set; } = 0;
        }
      }
    }
  }

  internal class JsonListReports {
    [JsonPropertyName("PvOnHeader")]
    public object? Header { get; set; }

    [JsonPropertyName("list")]
    public List<JsonReport> Reports { get; set; } = [];

    internal class JsonReport {
      [JsonPropertyName("maz")]
      public string County { get; set; } = string.Empty;

      [JsonPropertyName("vp_belf_njben")]
      public int Register { get; set; } = 0;

      [JsonPropertyName("vp_atjel")]
      public int Transfer { get; set; } = 0;

      [JsonPropertyName("vp_kulkepv")]
      public int Absentee { get; set; } = 0;

      [JsonPropertyName("vp_level")]
      public int Mail { get; set; } = 0;

      [JsonPropertyName("vp_osszes")]
      public int Total { get; set; } = 0;

      [JsonPropertyName("szavazott_osszesen")]
      public int Votes { get; set; } = 0;

      [JsonPropertyName("szavazott_osszesen_szaz")]
      public decimal VotesPc { get; set; } = 0M;

      [JsonPropertyName("szl_urna_boritek")]
      public int Envelope { get; set; } = 0;

      [JsonPropertyName("szl_ervenyes")]
      public int Valid { get; set; } = 0;

      [JsonPropertyName("szl_ervenyes_szaz")]
      public decimal ValidPc { get; set; } = 0M;

      [JsonPropertyName("szl_ervenytelen")]
      public int Invalid { get; set; } = 0;

      [JsonPropertyName("szl_ervenytelen_szaz")]
      public decimal InvalidPc { get; set; } = 0M;

      [JsonPropertyName("partlistara_szl_ervenyes")]
      public int ListValid { get; set; } = 0;

      [JsonPropertyName("partlistara_szl_ervenyes_szaz")]
      public decimal ListValidPc { get; set; } = 0M;

      [JsonPropertyName("nemzlistara_szl_ervenyes")]
      public int NationalityValid { get; set; } = 0;

      [JsonPropertyName("nemzlistara_szl_ervenyes_szaz")]
      public decimal NationalityValidPc { get; set; } = 0M;

      [JsonPropertyName("tetelek")]
      public List<JsonReportVotes> Lists { get; set; } = [];

      internal class JsonReportVotes {
        [JsonPropertyName("sorsz")]
        public int Party { get; set; } = 0;

        [JsonPropertyName("tl_id")]
        public int TLid { get; set; } = 0;

        [JsonPropertyName("vpOsszesNemz")]
        public int Nationalities { get; set; } = 0;

        [JsonPropertyName("osszes_szavazat")]
        public int Votes { get; set; } = 0;

        [JsonPropertyName("osszes_szavazat_szaz")]
        public decimal VotesPc { get; set; } = 0;

        [JsonPropertyName("osszes_szavazat_partlistas_szaz")]
        public decimal PartyVotesPc { get; set; } = 0;
      }
    }
  }

  internal class JsonMailReports {
    [JsonPropertyName("PvOnHeader")]
    public object? Header { get; set; }

    [JsonPropertyName("list")]
    public List<JsonReport> Reports { get; set; } = [];

    internal class JsonReport {
      [JsonPropertyName("vp_level")]
      public int Mail { get; set; } = 0;

      [JsonPropertyName("beerkezett_irat")]
      public int Incoming { get; set; } = 0;

      [JsonPropertyName("beerkezett_irat_szaz")]
      public decimal IncomingPc { get; set; } = 0;

      [JsonPropertyName("ell_irat_ervenyes")]
      public int IncomingValid { get; set; } = 0;

      [JsonPropertyName("ell_irat_ervenytelen")]
      public int IncomingInvalid { get; set; } = 0;

      [JsonPropertyName("szl_belyegzett_urna")]
      public int Envelope { get; set; } = 0;

      [JsonPropertyName("szl_elteres")]
      public int Difference { get; set; } = 0;

      [JsonPropertyName("szl_ervenyes")]
      public int Valid { get; set; } = 0;

      [JsonPropertyName("szl_ervenyes_szaz")]
      public decimal ValidPc { get; set; } = 0;

      [JsonPropertyName("szl_ervenytelen")]
      public int Invalid { get; set; } = 0;

      [JsonPropertyName("szl_ervenytelen_szaz")]
      public decimal InvalidPc { get; set; } = 0;

      [JsonPropertyName("tetelek")]
      public List<JsonReportVotes> Lists { get; set; } = [];

      internal class JsonReportVotes {
        [JsonPropertyName("szavlap_sorsz")]
        public int Party { get; set; } = 0;

        [JsonPropertyName("szavazat")]
        public int Votes { get; set; } = 0;

        [JsonPropertyName("szavazat_szaz")]
        public decimal VotesPc { get; set; } = 0;
      }
    }
  }

  internal class JsonStations {
    [JsonPropertyName("PvOnHeader")]
    public object? Header { get; set; }

    [JsonPropertyName("data")]
    public JsonStationData Data { get; set; } = new();

    internal class JsonStationData {
      [JsonPropertyName("maz")]
      public string County { get; set; } = string.Empty;

      [JsonPropertyName("taz")]
      public string SettlementId { get; set; } = string.Empty;

      [JsonPropertyName("tel_nev")]
      public string Settlement { get; set; } = string.Empty;

      [JsonPropertyName("szavazokorok")]
      public List<JsonStation> Stations { get; set; } = [];

      internal class JsonStation {
        [JsonPropertyName("leiro")]
        public JsonStationDesc Desc { get; set; } = new();

        [JsonPropertyName("letszam")]
        public GeoCounties.GeoCounty.GeoCountyStats Stats { get; set; } = new();

        internal class JsonStationDesc {
          [JsonPropertyName("sorszam")]
          public string Code { get; set; } = string.Empty;

          [JsonPropertyName("szk_nev")]
          public string FullName { get; set; } = string.Empty;

          [JsonPropertyName("szk_nev_en")]
          public string FullNameEn { get; set; } = string.Empty;

          [JsonPropertyName("evk")]
          public string OEVK { get; set; } = string.Empty;

          [JsonPropertyName("evk_nev")]
          public string OEVKFullName { get; set; } = string.Empty;

          [JsonPropertyName("evk_nev_en")]
          public string OEVKFullNameEn { get; set; } = string.Empty;

          [JsonPropertyName("cim")]
          public string Settlement { get; set; } = string.Empty;

          [JsonPropertyName("kozter")]
          public string Description { get; set; } = string.Empty;

          [JsonPropertyName("szamlKijelolt")]
          public string Tally { get; set; } = "N";
        }
      }
    }
  }
}