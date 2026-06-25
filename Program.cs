using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using ExcelDataReader;
using static System.Collections.Specialized.BitVector32;

namespace Választás_2026 {
  class Program {
    static TextInfo Hungarian = new CultureInfo("hu-HU", false).TextInfo;
    static string InputFolder = string.Empty;
    static string OutputFolder = string.Empty;
    static string CacheFolder = string.Empty;
    static HttpClient http = new();
    static bool PartiesDone = false;

    static public List<County> Counties = [
       new County("01", "Budapest"),
       new County("02", "Baranya"),
       new County("03", "Bács-Kiskun"),
       new County("04", "Békés"),
       new County("05", "Borsod-Abaúj-Zemplén"),
       new County("06", "Csongrád-Csanád"),
       new County("07", "Fejér"),
       new County("08", "Győr-Moson-Sopron"),
       new County("09", "Hajdú-Bihar"),
       new County("10", "Heves"),
       new County("11", "Jász-Nagykun-Szolnok"),
       new County("12", "Komárom-Esztergom"),
       new County("13", "Nógrád"),
       new County("14", "Pest"),
       new County("15", "Somogy"),
       new County("16", "Szabolcs-Szatmár-Bereg"),
       new County("17", "Tolna"),
       new County("18", "Vas"),
       new County("19", "Veszprém"),
       new County("20", "Zala"),
    ];

    static public List<Nationality> Nationalities = [
       new Nationality(1, "Bolgár Országos Önkormányzat", "Bolgár Országos Önkormányzat", 1922, "Bolgár"),
       new Nationality(2, "Magyarországi Romák Országos", "Magyarországi Romák Országos Önkormányzata", 1921, "Roma"),
       new Nationality(3, "MGOÖ", "Magyarországi Görögök Országos Önkormányzata", 1928, "Görög"),
       new Nationality(4, "MNOÖ", "Magyarországi Németek Országos Önkormányzata", 1931, "Német"),
       new Nationality(5, "MROÖ", "Magyarországi Románok Országos Önkormányzata", 1924, "Román"),
       new Nationality(6, "OHÖ", "Országos Horvát Önkormányzat", 1926, "Horvát"),
       new Nationality(7, "OLÖ", "Országos Lengyel Önkormányzat", 1929, "Lengyel"),
       new Nationality(8, "OÖÖ", "Országos Örmény Önkormányzat", 1920, "Örmény"),
       new Nationality(9, "ORÖ", "Országos Ruszin Önkormányzat", 1925, "Ruszin"),
       new Nationality(10, "Országos Szlovén", "Országos Szlovén Önkormányzat", 1927, "Szlovén"),
       new Nationality(11, "Országos Ukrán", "Országos Ukrán Nemzetiségi Önkormányzat", 1919, "Ukrán"),
       new Nationality(12, "OSZÖ", "Országos Szlovák Önkormányzat", 1933, "Szlovák"),
    ];

    private static Election election = new();
    private static SortedDictionary<string, Settlement> AllSettlements = [];
    private static Dictionary<string, PollingStation> AllStations = [];
    private static Dictionary<string, Candidate> PartyCandidates = [];
    private static Dictionary<string, string> StationAddresses = [];
    private static Dictionary<string, string> StationKeys = [];
    private static Dictionary<string, GeoOEVK> OEVKData = [];

    static void Main(string[] args) {
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

      string root = FindProjectRoot();
      InputFolder = Path.Combine(root, "Adatok", "Eredeti"); ;
      OutputFolder = Path.Combine(root, "Adatok", "Feldolgozott");
      CacheFolder = Path.Combine(root, "Adatok", "Cache");
      Directory.CreateDirectory(CacheFolder);

      var AllNationalities = ProcessNationalities();
      election.Nationalities = AllNationalities
        .ToDictionary(nat => nat.Code, nat => nat);

      ProcessCandidates();
      ProcessStations();
      ProcessOEVK();
      ProcessIndividual();
      ProcessList();
      ProcessOEVKStats();
      ProcessCountiesAsync().GetAwaiter().GetResult();
      ProcessStationsAsync().GetAwaiter().GetResult();

      string OutputPath = Path.Combine(OutputFolder, $"ogy{election.Year}.json");
      var options = new JsonSerializerOptions {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      };
      File.WriteAllText(OutputPath, JsonSerializer.Serialize(election, options));

      OutputPath = Path.Combine(OutputFolder, $"ogy{election.Year}_jeloltek.json");
      File.WriteAllText(OutputPath, JsonSerializer.Serialize(PartyCandidates, options));

      OutputPath = Path.Combine(OutputFolder, $"ogy{election.Year}_telepulesek.json");
      File.WriteAllText(OutputPath, JsonSerializer.Serialize(AllSettlements, options));
    }

    private static void ProcessCandidates() {
      string InputPath = Path.Combine(InputFolder, "jeloltek_20260531.xls");
      Trace.WriteLine(InputPath);
      using var @in = File.Open(InputPath, FileMode.Open, FileAccess.Read);
      using var reader = ExcelReaderFactory.CreateReader(@in);
      ProcessCandidatesTable(reader.AsDataSet().Tables[0]);
    }

    private static void ProcessStations() {
      string InputPath = Path.Combine(InputFolder, "korzet.xls");
      Trace.WriteLine(InputPath);
      using var @in = File.Open(InputPath, FileMode.Open, FileAccess.Read);
      using var reader = ExcelReaderFactory.CreateReader(@in);
      ProcessStationsTable(reader.AsDataSet().Tables[0]);
    }

    private static void ProcessOEVK() {
      string InputPath = Path.Combine(InputFolder, "oevk.json");
      Trace.WriteLine(InputPath);
      using var @in = File.Open(InputPath, FileMode.Open, FileAccess.Read);
      var Data = JsonSerializer.Deserialize<List<GeoOEVK>>(@in) ?? [];
      OEVKData = Data.ToDictionary(item => $"{item.County}|{item.OEVK}", item => item);
    }

    private static void ProcessOEVKStats() {
      string InputPath = Path.Combine(InputFolder, "oevk-valasztopolgarok_2026062.xls");
      Trace.WriteLine(InputPath);
      using var @in = File.Open(InputPath, FileMode.Open, FileAccess.Read);
      using var reader = ExcelReaderFactory.CreateReader(@in);
      ProcessOEVKStatsTable(reader.AsDataSet().Tables[0]);
    }

    private static void ProcessIndividual() {
      foreach (var county in Counties) {
        string InputPath = Path.Combine(InputFolder, $"{county.Name} OEVK egyéni 2026.xls");
        Trace.WriteLine(InputPath);
        using var @in = File.Open(InputPath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(@in);
        ProcessOEVKTables(county, reader.AsDataSet().Tables);
        election.Counties[county.Code] = county;
      }
    }

    private static void ProcessList() {
      foreach (var county in election.Counties.Values) {
        string InputPath = Path.Combine(InputFolder, $"{county.Name} listás 2026.xls");
        Trace.WriteLine(InputPath);
        using var @in = File.Open(InputPath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(@in);
        var tables = reader.AsDataSet().Tables;
        ProcessLists(county, tables);
        ProcessNationalities(county, tables);
      }
    }

    private static void ProcessCandidatesTable(DataTable table) {
      foreach (var row in table.Rows.Cast<DataRow>().Skip(1)) {
        string OEVK = row.CellString(3);
        if (!string.IsNullOrEmpty(OEVK)) {
          var candidate = Candidate.Identify(row);
          if (PartyCandidates.ContainsKey(candidate.Key))
            throw new DataException($"Ismétlődő név: {candidate.Key}");
          PartyCandidates[candidate.Key] = candidate;
        }
      }
    }

    private static void ProcessStationsTable(DataTable table) {
      foreach (var row in table.Rows.Cast<DataRow>().Skip(6)) {
        string County = Candidate.GetCounty(Hungarian.ToUpper(row.CellString(0)));
        string Settlement = Constituency.ExtractSettlement(row.CellString(1));
        string Id = row.CellString(2);
        string EVK = row.CellString(3);
        string Key = $"{County}|{Settlement}|{EVK}|{Id}";
        if (StationAddresses.ContainsKey(Key))
          throw new DataException($"Ismétlődő szavazókör: {Key}");
        StationAddresses[Key] = row.CellString(5);
      }
    }

    private static string ExtractSettlement(string name) {
      return name switch {
        "Budapest I. kerület" => "01",
        "Budapest II. kerület" => "02",
        "Budapest III. kerület" => "03",
        "Budapest IV. kerület" => "04",
        "Budapest V. kerület" => "05",
        "Budapest VI. kerület" => "06",
        "Budapest VII. kerület" => "07",
        "Budapest VIII. kerület" => "08",
        "Budapest IX. kerület" => "09",
        "Budapest X. kerület" => "10",
        "Budapest XI. kerület" => "11",
        "Budapest XII. kerület" => "12",
        "Budapest XIII. kerület" => "13",
        "Budapest XIV. kerület" => "14",
        "Budapest XV. kerület" => "15",
        "Budapest XVI. kerület" => "16",
        "Budapest XVII. kerület" => "17",
        "Budapest XVIII. kerület" => "18",
        "Budapest XIX. kerület" => "19",
        "Budapest XX. kerület" => "20",
        "Budapest XXI. kerület" => "21",
        "Budapest XXII. kerület" => "22",
        "Budapest XXIII. kerület" => "23",
        _ => name,
      };
    }

    private static void ProcessOEVKTables(County county, DataTableCollection tables) {
      int count = 1;
      var AllCandidates = ProcessCandidates(tables[0]);
      foreach (var table in tables.Cast<DataTable>().Skip(1)) {
        var OEVK = new Constituency(count.ToString("00"), table.TableName);
        OEVK.Candidates = AllCandidates
          .Where(cand => cand.OEVK == OEVK.Code)
          .Select(LookupCandidate)
          .ToDictionary(cand => cand.Code, cand => cand);

        string Key = $"{county.Code}|{OEVK.Code}";
        if (OEVKData.TryGetValue(Key, out var geo)) {
          OEVK.Center = geo.Center;
          OEVK.Border = geo.Border;
        }
        else
          throw new DataException($"Ismeretlen OEVK: {Key}");

        string OpenDataFile = string.Empty;
        foreach (var row in table.Rows.Cast<DataRow>().Skip(2)) {
          var station = PollingStation.Create(row, OEVK.Code);
          AssertError(station.Individual.Total, station.Individual.Register + station.Transfer + station.Absentee, $"SZK {station.Code} EE = AE + B + C");
          if (station.Tally) {
            AssertError(station.Individual.Voters.Value, station.Individual.InPerson + station.Envelope, $"{station.Code} JE = FE + I");
            AssertError(station.Individual.Difference, station.Individual.Stamped - station.Individual.Voters.Value, $"SZK {station.Code} LE = KE - JE");
          }
          else
            AssertError(station.Individual.Difference, station.Individual.Stamped - station.Individual.InPerson, $"SZK {station.Code} LE = KE - FE");
          AssertError(station.Individual.Stamped, station.Individual.Invalid.Value + station.Individual.Valid.Value, $"SZK {station.Code} KE = ME + NE");
          AssertError(station.Individual.Valid.Value, station.Individual.Votes.Sum(vote => vote.Value.Value), $"{station.Code} NE");

          Key = $"{county.Code}|{ExtractSettlement(station.Settlement)}|{OEVK.Code}|{station.StationId}";
          if (StationAddresses.TryGetValue(Key, out string? value))
            station.Description = value.Replace("\"\"", "\"");
          else
            throw new DataException($"Ismeretlen szavazókör: {Key}");

          AllStations[station.Code] = OEVK.PollingStations[station.Code] = station;
          StationKeys[$"{station.County}-{station.SettlementId}-{station.StationId}"] = station.Code;

          Key = $"{station.County}|{station.SettlementId}";
          AllSettlements[Key] = new(Key, station.County, station.SettlementId, station.Settlement);
        }
        county.OEVKs[OEVK.Code] = OEVK;
        count++;
      }
    }

    private static void ProcessOEVKStatsTable(DataTable table) {
      foreach (var row in table.Rows.Cast<DataRow>().Skip(1)) {
        string[] Parts = row.CellString(1).Split(", ");
        string CountyId = Candidate.ExtractCounty(Parts.First());
        string OEVKId = Candidate.ExtractOEVK(Parts.Last());
        var OEVK = election.Counties[CountyId].OEVKs[OEVKId];
        OEVK.Total = row.CellInt(2); // Választókerület jelöltjeire szavazók aktuális száma
        OEVK.Register = row.CellInt(3); // Hazai szavazókörben lakcím szerint szavazók száma
        OEVK.Transfer = row.CellInt(4); // Belföldön más OEVK-ba átjelentkezettek száma
        OEVK.Absentee = row.CellInt(5); // Külképviseleten szavazók száma
        OEVK.TransferOut = row.CellInt(8); // OEVK-ba átjelentkezett szavazók száma
      }
    }

    private static Candidate LookupCandidate(Candidate candidate) {
      if (candidate.Name != "KIESETT") {
        if (PartyCandidates.TryGetValue(candidate.Key, out var value))
          candidate.Party = value.Party;
        else
          throw new DataException($"Ismeretlen jelölt: {candidate.Key}");
      }
      return candidate;
    }

    private static List<Candidate> ProcessCandidates(DataTable table) {
      if (table.Rows[28].CellString(0) != "VÁRMEGYE")
        throw new DataException("Hibás XLS (első oldalon nem található jelöltlista)");

      List<Candidate> allCandidates = [];
      foreach (var row in table.Rows.Cast<DataRow>().Skip(29))
        allCandidates.Add(Candidate.Create(row));
      return allCandidates;
    }

    private static void ProcessLists(County county, DataTableCollection tables) {
      if (!PartiesDone) {
        var AllParties = ProcessParties(tables[0]);
        election.Parties = AllParties
          .ToDictionary(party => party.Code, party => party);
        PartiesDone = true;
      }

      var table = tables[1];
      foreach (var row in table.Rows.Cast<DataRow>().Skip(2))
        AllStations[row.CellString(1)].AddParties(row);
    }

    private static List<Nationality> ProcessNationalities() {
      string InputPath = Path.Combine(InputFolder, "nemzetisegi-listak_2026067.xls");
      Trace.WriteLine(InputPath);
      using var @in = File.Open(InputPath, FileMode.Open, FileAccess.Read);
      using var reader = ExcelReaderFactory.CreateReader(@in);
      var tables = reader.AsDataSet().Tables;
      var allNationalities = Nationalities;
      foreach (var row in tables[0].Rows.Cast<DataRow>().Skip(1)) {
        var nationality = allNationalities.Find(nat => nat.Abbreviation == row.CellString(0));
        nationality!.Candidates = row.CellInt(5);
        nationality!.Mandates = row.CellInt(6);
        nationality!.Valid = row.CellInt(7);
        nationality!.Voters = row.CellInt(8);
      }

      return allNationalities;
    }

    private static List<Party> ProcessParties(DataTable table) {
      var header = table.Rows[27];
      if (header.CellString(0) != "SORSZÁM")
        throw new DataException("Hibás XLS (első oldalon nem található jelöltlista)");

      List<Party> allParties = [];
      foreach (var row in table.Rows.Cast<DataRow>().Skip(28))
        allParties.Add(Party.Create(row));

      string InputPath = Path.Combine(InputFolder, "partlistak_2026067.xls");
      Trace.WriteLine(InputPath);
      using var @in = File.Open(InputPath, FileMode.Open, FileAccess.Read);
      using var reader = ExcelReaderFactory.CreateReader(@in);
      var tables = reader.AsDataSet().Tables;
      foreach (var row in tables[0].Rows.Cast<DataRow>().Skip(1)) {
        var party = allParties.Find(party => party.Abbreviation == row.CellString(0));
        party!.Candidates = row.CellInt(5);
        party!.Mandates = row.CellInt(6);
        party!.Votes = new(row.CellInt(7), row.CellReal(8));
        party!.InlandVotes = new(row.CellInt(9), row.CellReal(10));
        party!.AbsenteeVotes = new(row.CellInt(11), row.CellReal(12));
        AssertError(party!.Votes.Value, party!.InlandVotes.Value + party!.AbsenteeVotes.Value, $"Párt {party.Abbreviation} szavazatai");
      }

      return allParties;
    }

    private static void ProcessNationalities(County county, DataTableCollection tables) {
      var table = tables[2];
      var header = table.Rows[0];
      List<int> NationalityCodes = [];
      for (int cell = 16; cell < header.Table.Columns.Count; cell += 7) {
        string id = header.CellString(cell);
        if (!string.IsNullOrEmpty(id))
          NationalityCodes.Add(Nationalities.Find(nat => nat.ID == id)!.Code);
      }
      foreach (var row in table.Rows.Cast<DataRow>().Skip(2)) {
        var station = AllStations[row.CellString(1)];
        station.AddNationalities(row, NationalityCodes);
        AssertError(station.Lists.Register, station.Lists.Parties.Register + station.Lists.Nationalities.Register, $"SZK {station.Code} AL = AP + AN");
        AssertError(station.Lists.Total, station.Lists.Register + station.Transfer + station.Absentee, $"SZK {station.Code} EL = AL + B + C");
        AssertError(station.Lists.Parties.Stamped, station.Lists.Parties.Invalid.Value + station.Lists.Parties.Valid.Value, $"SZK {station.Code} KP = MP + NP");
        AssertError(station.Lists.Stamped, station.Lists.Parties.Stamped + station.Lists.Nationalities.Stamped, $"SZK {station.Code} KL = KP + KN");
        AssertError(station.Lists.Valid.Value, station.Lists.Parties.Valid.Value + station.Lists.Nationalities.Valid.Value, $"SZK {station.Code} NL = NP + NN");
        if (station.Tally)
          AssertError(station.Lists.Voters.Value, station.Lists.InPerson + station.Lists.Envelope, $"SZK {station.Code} JL = FL + IL");
        else {
          AssertError(station.Lists.InPerson, station.Lists.Parties.InPerson + station.Lists.Nationalities.InPerson, $"SZK {station.Code} FL = FP + FN");
          AssertError(station.Lists.NotStamped, station.Lists.Parties.NotStamped + station.Lists.Nationalities.NotStamped, $"SZK {station.Code} OL = OP + ON");
          AssertError(station.Lists.Parties.Difference, station.Lists.Parties.Stamped - station.Lists.Parties.InPerson, $"SZK {station.Code} LP = KP - FP");
        }
        AssertError(station.Lists.Nationalities.Difference, 0, $"SZK {station.Code} LN = 0");
        AssertError(station.Lists.Parties.Valid.Value, station.Lists.Parties.Votes.Sum(vote => vote.Value.Value), $"SZK {station.Code} NP = Σ párt");
        AssertError(station.Lists.Nationalities.Valid.Value, station.Lists.Nationalities.Votes.Sum(vote => vote.Value.Valid.Value), $"SZK {station.Code} NN = Σ nemzetiség");
      }
    }

    private static async Task ProcessCountiesAsync() {
      string CountyPath = Path.Combine(CacheFolder, "Megyek.json");
      if (!File.Exists(CountyPath))
        await File.WriteAllTextAsync(CountyPath, await http.GetStringAsync("https://vtr.valasztas.hu/ogy2026/data/04112100/ver/Megyek.json"));
      Trace.WriteLine(CountyPath);
      using var inCounty = File.Open(CountyPath, FileMode.Open, FileAccess.Read);
      var CountyData = JsonSerializer.Deserialize<GeoCounties>(inCounty) ?? new();
      foreach (var county in CountyData.Counties) {
        var County = election.Counties[county.Geo.County];
        County.Center = county.Geo.Center;
        County.Border = county.Geo.Border;
        County.Register = county.Stats.Domicile;
        County.TransferOut = county.Stats.TransferOut;
        County.Transfer = county.Stats.Transfer;
        County.Absentee = county.Stats.Absentee;
        County.Total = county.Stats.Total;
      }

      string DataPath = Path.Combine(CacheFolder, "OevkAdatok.json");
      if (!File.Exists(DataPath))
        await File.WriteAllTextAsync(DataPath, await http.GetStringAsync("https://vtr.valasztas.hu/ogy2026/data/04112100/ver/OevkAdatok.json"));
      Trace.WriteLine(DataPath);
      using var inData = File.Open(DataPath, FileMode.Open, FileAccess.Read);
      var DataData = JsonSerializer.Deserialize<JsonConstituencies>(inData) ?? new();
      foreach (var oevk in DataData.Constituencies) {
        var OEVK = election.Counties[oevk.County].OEVKs[oevk.OEVK];
        AssertError(OEVK.Transfer, oevk.Stats.TransferOut, $"Megye {oevk.County} OEVK {oevk.OEVK} átjelentkezés");
        AssertError(OEVK.Absentee, oevk.Stats.Absentee, $"Megye {oevk.County} OEVK {oevk.OEVK} külképviselet");
        AssertError(OEVK.Total, oevk.Total, $"Megye {oevk.County} OEVK {oevk.OEVK} választópolgár");
      }

      string OEVKPath = Path.Combine(CacheFolder, "OevkJkv.json");
      if (!File.Exists(OEVKPath))
        await File.WriteAllTextAsync(OEVKPath, await http.GetStringAsync("https://vtr.valasztas.hu/ogy2026/data/05071600/szavossz/OevkJkv.json"));
      Trace.WriteLine(OEVKPath);
      using var inOEVK = File.Open(OEVKPath, FileMode.Open, FileAccess.Read);
      var OEVKData = JsonSerializer.Deserialize<JsonOEVKReports>(inOEVK) ?? new();
      foreach (var report in OEVKData.Reports) {
        var OEVK = election.Counties[report.County].OEVKs[report.OEVK];
        AssertError(OEVK.Register, report.Stats.Register, $"{report.County} OEVK {report.OEVK} névjegyzék");
        AssertError(OEVK.Transfer, report.Stats.Transfer, $"{report.County} OEVK {report.OEVK} átjelentkezés");
        OEVK.Absentee = report.Stats.Absentee;
        OEVK.Total = report.Stats.Total;
        AssertError(OEVK.Total, OEVK.Register + OEVK.Transfer + OEVK.Absentee, $"{report.County} OEVK {OEVK.Code} E = A + B + C");
        OEVK.Envelope = report.Stats.Envelope;
        OEVK.Valid = new(report.Stats.Valid, report.Stats.ValidPc);
        OEVK.Invalid = new(report.Stats.Invalid, report.Stats.InvalidPc);
        AssertError(OEVK.Envelope, OEVK.Valid.Value + OEVK.Invalid.Value, $"{report.County} OEVK {OEVK.Code} I = M + N");
        OEVK.Voters = new(report.Stats.Voters, report.Stats.VotersPc);
        foreach (var candidate in report.Stats.Votes) {
          var Candidate = OEVK.Candidates[candidate.Candidate];
          Candidate.Votes = new(candidate.Votes, candidate.VotesPc);
          Candidate.Won = (candidate.Won == 1);
        }
        AssertError(OEVK.Valid.Value, OEVK.Candidates.Sum(candidate => candidate.Value.Votes.Value), $"{report.County} OEVK {OEVK.Code} N = Σ jelölt");
        // OEVK egyéni szavazatok = szavazókörök összege
        foreach (var candidate in OEVK.Candidates.Values)
          AssertError(candidate.Votes.Value, OEVK.PollingStations.Sum(szk => szk.Value.IndividualVote(candidate.Code)), $"{report.County} OEVK {OEVK.Code} Σ {candidate.Name2}");
      }

      string ListPath = Path.Combine(CacheFolder, "ListasJkv.json");
      if (!File.Exists(ListPath))
        await File.WriteAllTextAsync(ListPath, await http.GetStringAsync("https://vtr.valasztas.hu/ogy2026/data/05071600/szavossz/ListasJkv.json"));
      Trace.WriteLine(ListPath);
      using var inList = File.Open(ListPath, FileMode.Open, FileAccess.Read);
      var ListData = JsonSerializer.Deserialize<JsonListReports>(inList) ?? new();
      foreach (var report in ListData.Reports) {
        if (string.IsNullOrEmpty(report.County)) {
          // országos
          election.Register = report.Register;
          election.Transfer = report.Transfer;
          election.Absentee = report.Absentee;
          election.Mail = report.Mail;
          election.Total = report.Total;
          AssertError(election.Total, election.Register + election.Transfer + election.Absentee + election.Mail, "Országos E = A + B + C");
          election.Envelope = report.Envelope;
          election.Valid = new(report.Valid, report.ValidPc);
          election.Invalid = new(report.Invalid, report.InvalidPc);
          AssertError(election.Envelope, election.Valid.Value + election.Invalid.Value, "Országos I = M + N");
          election.Voters = new(report.Votes, report.VotesPc);
          election.ListValid = new(report.ListValid, report.ListValidPc);
          election.NationalityValid = new(report.NationalityValid, report.NationalityValidPc);
          AssertError(election.Valid.Value, election.ListValid.Value + election.NationalityValid.Value, "Országos NL = NP + NN");
          // érvényes = listák összege
          AssertError(election.ListValid.Value, election.Parties.Sum(party => party.Value.Votes.Value), $"Országos NP = Σ párt");
          AssertError(election.NationalityValid.Value, election.Nationalities.Sum(nat => nat.Value.Valid), $"Országos NN = Σ nemzetiség");
        }
        else {
          // megyei
          var County = election.Counties[report.County];
          County.Register = report.Register;
          County.Transfer = report.Transfer;
          County.Absentee = report.Absentee;
          County.Total = report.Total;
          AssertError(County.Total, County.Register + County.Transfer + County.Absentee, $"{County.Name} E = A + B + C");
          County.Envelope = report.Envelope;
          County.Valid = new(report.Valid, report.ValidPc);
          County.Invalid = new(report.Invalid, report.InvalidPc);
          AssertError(County.Envelope, County.Valid.Value + County.Invalid.Value, $"{County.Name} I = M + N");
          County.Voters = new(report.Votes, report.VotesPc);
          County.ListValid = new(report.ListValid, report.ListValidPc);
          County.NationalityValid = new(report.NationalityValid, report.NationalityValidPc);
          AssertError(County.Valid.Value, County.ListValid.Value + County.NationalityValid.Value, $"{County.Name} NL = NP + NN");
          foreach (var list in report.Lists) {
            if (list.Nationalities == 0)
              County.Parties.TryAdd(list.Party, new(list.Votes, list.VotesPc));
            else {
              var nationality = Nationalities.Find(nat => nat.TLid == list.TLid);
              County.Nationalities.TryAdd(nationality!.Code, new(list.Votes, list.VotesPc));
            }
          }
          AssertError(County.Valid.Value, County.ListValid.Value + County.NationalityValid.Value, $"{County.Name} NL = NP + NN");
          // megyei érvényes = megyei listák összege
          AssertError(County.ListValid.Value, County.Parties.Sum(vote => vote.Value.Value), $"{County.Name} NP = Σ párt");
          AssertError(County.NationalityValid.Value, County.Nationalities.Sum(vote => vote.Value.Value), $"{County.Name} NN = Σ nemzetiség");
          // megyei statisztikai adatok = OEVK-k összegzése
          AssertError(County.Absentee, County.OEVKs.Sum(oevk => oevk.Value.Absentee), $"{County.Name} külképviselet");
          AssertError(County.Transfer, County.OEVKs.Sum(oevk => oevk.Value.Transfer), $"{County.Name} átjelentkezés");
          AssertError(County.ListValid.Value, County.OEVKs.Sum(oevk => oevk.Value.PartyValid()), $"{County.Name} NP = Σ oevk");
          AssertError(County.NationalityValid.Value, County.OEVKs.Sum(oevk => oevk.Value.NationalityValid()), $"{County.Name} NN = Σ oevk");
          // megyei szavazatok = OEVK szavazatok összege
          foreach (var party in election.Parties.Values)
            AssertError(County.Parties[party.Code].Value, County.OEVKs.Sum(oevk => oevk.Value.PartyVotes(party.Code)), $"{County.Name} {party.Abbreviation} = Σ oevk");
          foreach (var nationality in election.Nationalities.Values)
            if (County.Nationalities.TryGetValue(nationality.Code, out var nat))
              AssertError(nat.Value, County.OEVKs.Sum(oevk => oevk.Value.NationalityVotes(nationality.Code)), $"{County.Name} {nationality.Abbreviation} = Σ oevk");
        }
      }

      string MailPath = Path.Combine(CacheFolder, "LevelJkv.json");
      if (!File.Exists(MailPath))
        await File.WriteAllTextAsync(MailPath, await http.GetStringAsync("https://vtr.valasztas.hu/ogy2026/data/05071600/szavossz/LevelJkv.json"));
      Trace.WriteLine(MailPath);
      using var inMail = File.Open(MailPath, FileMode.Open, FileAccess.Read);
      var MailData = JsonSerializer.Deserialize<JsonMailReports>(inMail) ?? new();
      foreach (var report in MailData.Reports) {
        AssertError(election.Mail, report.Mail, "Levélszavazatok");
        election.MailList.Incoming = new(report.Incoming, report.IncomingPc);
        election.MailList.IncomingValid = report.IncomingValid.ToVote(report.Incoming);
        election.MailList.IncomingInvalid = report.IncomingInvalid.ToVote(report.Incoming);
        AssertError(election.MailList.Incoming.Value, election.MailList.IncomingValid.Value + election.MailList.IncomingInvalid.Value, "Levél bejövő");
        election.MailList.Envelope = report.Envelope;
        election.MailList.Difference = report.Difference;
        election.MailList.Valid = new(report.Valid, report.ValidPc);
        election.MailList.Invalid = new(report.Invalid, report.InvalidPc);
        AssertError(election.MailList.Envelope, election.MailList.Valid.Value + election.MailList.Invalid.Value, "Levél I = M + N");
        foreach (var list in report.Lists)
          election.MailList.Lists.TryAdd(list.Party, new(list.Votes, list.VotesPc));
        // levél érvényes = levél listák összege
        AssertError(election.MailList.Valid.Value, election.MailList.Lists.Sum(party => party.Value.Value), $"Levél N = Σ párt");
      }

      AssertError(election.Valid.Value, election.ListValid.Value + election.NationalityValid.Value, $"Országos NL = NP + NN");
      // országos érvényes = országos listák összege
      AssertError(election.ListValid.Value, election.Parties.Sum(party => party.Value.Votes.Value), "Országos NP = Σ párt");
      AssertError(election.NationalityValid.Value, election.Nationalities.Sum(nationality => nationality.Value.Valid), "Országos NN = Σ nemzetiség");
      // országos érvényes = megyei (+ levélszavazat) érvényesek
      AssertError(election.ListValid.Value, election.MailList.Valid.Value + election.Counties.Sum(county => county.Value.ListValid.Value), "Országos NP = levél + Σ megye");
      AssertError(election.NationalityValid.Value, election.Counties.Sum(county => county.Value.NationalityValid.Value), "Országos NN = Σ megye");
      // országos szavazatok = megyei (+ levélszavazatok) összege
      foreach (var party in election.Parties.Values)
        AssertError(party.Votes.Value, election.MailList.Lists[party.Code].Value + election.Counties.Sum(county => county.Value.PartyVotes(party.Code)), $"Országos {party.Abbreviation} = Σ megye");
      foreach (var nationality in election.Nationalities.Values)
        AssertError(nationality.Valid, election.Counties.Sum(county => county.Value.NationalityVotes(nationality.Code)), $"Országos {nationality.Abbreviation} = Σ megye");
    }

    private static async Task ProcessStationsAsync() {
      foreach (var settlement in AllSettlements.Values)
        await ProcessStationsAsync(settlement.County, settlement.SettlementId);
    }

    private static async Task ProcessStationsAsync(string county, string settlement) {
      string GeoFile = $"Szavkor-Topo-{county}-{settlement}.json";
      string GeoPath = Path.Combine(CacheFolder, "Topo", GeoFile);
      if (!File.Exists(GeoPath)) {
        try {
          Trace.WriteLine(GeoPath);
          await File.WriteAllTextAsync(GeoPath, await http.GetStringAsync(@$"https://vtr.valasztas.hu/ogy2026/data/04112100/ver/{county}/{GeoFile}"));
        }
        catch (Exception) {
        }
      }

      using var inGeo = File.Open(GeoPath, FileMode.Open, FileAccess.Read);
      var GeoData = JsonSerializer.Deserialize<GeoStations>(inGeo) ?? new();
      foreach (var st in GeoData.Stations)
        if (StationKeys.TryGetValue($"{st.County}-{st.Settlement}-{st.Station}", out string? Key))
          if (AllStations.TryGetValue(Key, out var station)) {
            station.Location = st.Center;
            station.Border = st.Border;
          }

      string DataFile = $"Szavazokorok-{county}-{settlement}.json";
      string DataPath = Path.Combine(CacheFolder, "Szk", DataFile);
      if (!File.Exists(DataPath)) {
        try {
          Trace.WriteLine(DataPath);
          await File.WriteAllTextAsync(DataPath, await http.GetStringAsync(@$"https://vtr.valasztas.hu/ogy2026/data/04112100/ver/{county}/{DataFile}"));
        }
        catch (Exception) {
        }
      }

      using var inData = File.Open(DataPath, FileMode.Open, FileAccess.Read);
      var DataData = JsonSerializer.Deserialize<JsonStations>(inData) ?? new();
      foreach (var st in DataData.Data.Stations) {
        if (StationKeys.TryGetValue($"{county}-{settlement}-{st.Desc.Code}", out string? Key)) {
          if (AllStations.TryGetValue(Key, out var station)) {
            station.SettlementFull = st.Desc.Settlement;
            station.Tally = (st.Desc.Tally == "I");
            if (!station.Tally && (station.Transfer > 0 || station.Absentee > 0))
              throw new DataException($"SZK {Key} nincs kijelölve számlálásra");
            if (station.Description.Trim() != st.Desc.Description.Trim()) {
              Trace.WriteLine($"Szavazókör {station.County}-{station.SettlementId}-{station.StationId} leírás: {station.Description} <> {st.Desc.Description}");
              station.Description = st.Desc.Description.Trim();
            }
          }
        }
      }
    }

    private static string FindProjectRoot() {
      string? dir = Directory.GetCurrentDirectory();

      while (dir is not null) {
        bool looksLikeRoot = File.Exists(Path.Combine(dir, "README.md")) && Directory.Exists(Path.Combine(dir, "Adatok"));
        if (looksLikeRoot)
          return dir;
        dir = Directory.GetParent(dir)?.FullName;
      }

      throw new DirectoryNotFoundException("Nem találom a projekt gyökerét.");
    }

    private static void AssertError(int data, int computed, string message) {
      if (data != computed)
        throw new DataException($"{message}: adat={data} számított={computed} eltérés={data - computed}");
    }
  }
}
