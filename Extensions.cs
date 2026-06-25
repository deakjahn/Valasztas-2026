using System.Data;
using System.Globalization;

namespace Választás_2026 {
  public static class Extensions {

    public static string FirstValid(params string[] strings) {
      foreach (string s in strings)
        if (!string.IsNullOrEmpty(s))
          return s;
      return string.Empty;
    }

    public static object? Cell(this DataRow row, int index) {
      if (index < 0 || index >= row.ItemArray.Length)
        return null;

      object value = row[index];
      return (value == DBNull.Value) ? null : value;
    }

    public static string CellString(this DataRow row, int index) {
      object? value = row.Cell(index);
      return value switch {
        null => string.Empty,
        string s => string.IsNullOrWhiteSpace(s) ? string.Empty : s.Trim(),
        double d => d.ToString(CultureInfo.InvariantCulture),
        decimal d => d.ToString(CultureInfo.InvariantCulture),
        int i => i.ToString(CultureInfo.InvariantCulture),
        long l => l.ToString(CultureInfo.InvariantCulture),
        _ => value.ToString()?.Trim() ?? string.Empty
      };
    }

    public static int CellInt(this DataRow row, int index) {
      object? value = row.Cell(index);
      return value switch {
        null => 0,
        int i => i,
        long l when l is >= int.MinValue and <= int.MaxValue => (int)l,
        double d when Math.Abs(d % 1) < 0.0000001 => (int)d,
        decimal d when d == Math.Truncate(d) => (int)d,
        string s when int.TryParse(s.Trim().Replace(" ", ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out int i) => i,
        _ => 0
      };
    }

    public static decimal CellReal(this DataRow row, int index) {
      object? value = row.Cell(index);
      return value switch {
        null => 0,
        int i => i,
        long l when l is >= int.MinValue and <= int.MaxValue => l,
        double d => (decimal)d,
        decimal d => d,
        string s when decimal.TryParse(s.Trim().Replace(" ", ""), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal i) => i,
        _ => 0
      };
    }

    public static decimal ToPercent(this int value, int total) => (total == 0) ? 0 : (decimal)Math.Round((double)value / total * 100, 2);
    public static Vote ToVote(this int value, int total) => new(value, value.ToPercent(total));
  }
}