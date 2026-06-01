using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using ExcelDataReader;

namespace Választás_2026 {
  class Program {
    static TextInfo Hungarian = new CultureInfo("hu-HU", false).TextInfo;
    static string InputFolder = string.Empty;
    static string OutputFolder = string.Empty;

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
       new Nationality(1, "OLÖ", "Országos Lengyel Önkormányzat"),
       new Nationality(2, "OÖÖ", "Országos Örmény Önkormányzat"),
       new Nationality(3, "Bolgár Országos Önkormányzat", "Bolgár Országos Önkormányzat"),
       new Nationality(4, "MGOÖ", "Magyarországi Görögök Országos Önkormányzata"),
       new Nationality(5, "OHÖ", "Országos Horvát Önkormányzat"),
       new Nationality(6, "Magyarországi Romák Országos", "Magyarországi Romák Országos Önkormányzata"),
       new Nationality(7, "MNOÖ", "Magyarországi Németek Országos Önkormányzata"),
       new Nationality(8, "ORÖ", "Országos Ruszin Önkormányzat"),
       new Nationality(9, "OSZÖ", "Országos Szlovák Önkormányzat"),
       new Nationality(10, "Országos Ukrán", "Országos Ukrán Nemzetiségi Önkormányzat"),
       new Nationality(11, "MROÖ", "Magyarországi Románok Országos Önkormányzata"),
       new Nationality(12, "Országos Szlovén", "Országos Szlovén Önkormányzat"),
    ];

    private static Election election = new() {
      Nationalities = Nationalities.ToDictionary(nat => nat.Code, nat => nat),
    };

    private static Dictionary<string, PollingStation> AllStations = [];
    private static Dictionary<string, Candidate> PartyCandidates = [];
    private static Dictionary<string, string> StationAddresses = [];
    private static Dictionary<string, Geo> OEVKData = [];

    static void Main(string[] args) {
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

      string root = FindProjectRoot();
      InputFolder = Path.Combine(root, "Adatok", "Eredeti"); ;
      OutputFolder = Path.Combine(root, "Adatok", "Feldolgozott");

      ProcessCandidates();
      ProcessStations();
      ProcessOEVK();
      ProcessIndividual();
      ProcessList();

      string OutputPath = Path.Combine(OutputFolder, $"ogy{election.Year}.json");
      var options = new JsonSerializerOptions {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
      };
      File.WriteAllText(OutputPath, JsonSerializer.Serialize(election, options));

      OutputPath = Path.Combine(OutputFolder, $"ogy{election.Year}_jeloltek.json");
      File.WriteAllText(OutputPath, JsonSerializer.Serialize(PartyCandidates, options));
    }

    private static void ProcessCandidates() {
      string InputPath = Path.Combine(InputFolder, "jeloltek_20260531.xls");
      Trace.WriteLine(InputPath);
      using var @in = File.Open(InputPath, FileMode.Open, FileAccess.Read);
      using var reader = ExcelReaderFactory.CreateReader(@in);
      ProcessPartyCandidates(reader.AsDataSet().Tables[0]);
    }

    private static void ProcessStations() {
      string InputPath = Path.Combine(InputFolder, "korzet.xls");
      Trace.WriteLine(InputPath);
      using var @in = File.Open(InputPath, FileMode.Open, FileAccess.Read);
      using var reader = ExcelReaderFactory.CreateReader(@in);
      ProcessPollingStations(reader.AsDataSet().Tables[0]);
    }

    private static void ProcessOEVK() {
      string InputPath = Path.Combine(InputFolder, "oevk.json");
      Trace.WriteLine(InputPath);
      using var @in = File.Open(InputPath, FileMode.Open, FileAccess.Read);
      var Data = JsonSerializer.Deserialize<List<Geo>>(@in) ?? [];
      OEVKData = Data.ToDictionary(item => $"{item.County}|{item.OEVK}", item => item);
    }

    private static void ProcessIndividual() {
      foreach (var county in Counties) {
        string InputPath = Path.Combine(InputFolder, $"{county.Name} OEVK egyéni 2026.xls");
        Trace.WriteLine(InputPath);
        using var @in = File.Open(InputPath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(@in);
        ProcessOEVKs(county, reader.AsDataSet().Tables);
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

    private static void ProcessPartyCandidates(DataTable table) {
      foreach (var row in table.Rows.Cast<DataRow>().Skip(1)) {
        string OEVK = row.CellString(3);
        if (!string.IsNullOrEmpty(OEVK)) {
          var candidate = Candidate.Identify(row);
          if (PartyCandidates.ContainsKey(candidate.Key)) throw new DataException($"Ismétlődő név: {candidate.Key}");
          PartyCandidates[candidate.Key] = candidate;
        }
      }
    }

    private static void ProcessPollingStations(DataTable table) {
      foreach (var row in table.Rows.Cast<DataRow>().Skip(6)) {
        string County = Candidate.GetCounty(Hungarian.ToUpper(row.CellString(0)));
        string Id = row.CellString(2);
        string OEVK = row.CellString(3);
        string Key = $"{County}|{OEVK}|{Id}";
        StationAddresses[Key] = row.CellString(5);
      }
    }

    private static void ProcessOEVKs(County county, DataTableCollection tables) {
      int count = 1;
      var AllCandidates = ProcessCandidates(tables[0]);
      foreach (var table in tables.Cast<DataTable>().Skip(1)) {
        var OEVK = new OEVK(count.ToString("00"), table.TableName);
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

        foreach (var row in table.Rows.Cast<DataRow>().Skip(2)) {
          var station = PollingStation.Create(row, OEVK.Code);

          Key = $"{county.Code}|{OEVK.Code}|{station.Id}";
          if (StationAddresses.TryGetValue(Key, out string? value))
            station.Description = value;
          else
            throw new DataException($"Ismeretlen szavazókör: {Key}");

          AllStations[station.Code] = OEVK.PollingStations[station.Code] = station;
        }
        county.OEVKs[OEVK.Code] = OEVK;
        count++;
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
      if (table.Rows[28].CellString(0) != "VÁRMEGYE") throw new DataException("Hibás XLS (első oldalon nem található jelöltlista)");

      List<Candidate> allCandidates = [];
      foreach (var row in table.Rows.Cast<DataRow>().Skip(29))
        allCandidates.Add(Candidate.Create(row));
      return allCandidates;
    }

    private static void ProcessLists(County county, DataTableCollection tables) {
      var AllParties = ProcessParties(tables[0]);
      election.Parties = AllParties
        .ToDictionary(party => party.Code, party => party);
      var table = tables[1];
      foreach (var row in table.Rows.Cast<DataRow>().Skip(2)) {
        var station = AllStations[row.CellString(1)];
        station.AddParties(row);
      }
    }

    private static List<Party> ProcessParties(DataTable table) {
      var header = table.Rows[27];
      if (header.CellString(0) != "SORSZÁM") throw new DataException("Hibás XLS (első oldalon nem található jelöltlista)");

      List<Party> allParties = [];
      foreach (var row in table.Rows.Cast<DataRow>().Skip(28))
        allParties.Add(Party.Create(row));
      return allParties;
    }

    private static void ProcessNationalities(County county, DataTableCollection tables) {
      var table = tables[2];
      foreach (var row in table.Rows.Cast<DataRow>().Skip(2)) {
        var station = AllStations[row.CellString(1)];
        station.AddNationalities(row);
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
  }
}
