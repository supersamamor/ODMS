namespace FBSC.ODMS.Infrastructure.Services.Dashboard;

/// <summary>
/// Turns raw, untrusted spreadsheet/CSV header text into safe SQL identifiers. Every header
/// value that ends up in generated DDL/DML for a staging table goes through this first - no
/// exceptions. Allow-lists [A-Za-z0-9_], de-duplicates collisions (Name, Name_2, Name_3, ...),
/// and falls back to Column_N for anything empty or a reserved word once sanitized.
/// </summary>
public static class UploadedFileHeaderSanitizer
{
    // Not exhaustive, but covers the T-SQL reserved words most likely to appear verbatim as a
    // spreadsheet column header (a real header being one of these exactly is rare, but if a
    // sanitized header lands on one, silently emitting it into DDL would be a real bug).
    private static readonly HashSet<string> ReservedWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "ADD", "ALL", "ALTER", "AND", "ANY", "AS", "ASC", "BACKUP", "BEGIN", "BETWEEN", "BY",
        "CASE", "CHECK", "COLUMN", "CONSTRAINT", "CREATE", "DATABASE", "DEFAULT", "DELETE",
        "DESC", "DISTINCT", "DROP", "ELSE", "END", "EXEC", "EXECUTE", "EXISTS", "FOREIGN",
        "FROM", "FULL", "FUNCTION", "GO", "GRANT", "GROUP", "HAVING", "IDENTITY", "IF", "IN",
        "INDEX", "INNER", "INSERT", "INTO", "IS", "JOIN", "KEY", "LEFT", "LIKE", "NOT", "NULL",
        "ON", "OR", "ORDER", "OUTER", "PRIMARY", "PROCEDURE", "REFERENCES", "RIGHT", "ROW",
        "ROWS", "SCHEMA", "SELECT", "SET", "TABLE", "TOP", "TRIGGER", "TRUNCATE", "UNION",
        "UNIQUE", "UPDATE", "USER", "VALUES", "VIEW", "WHERE", "WHILE", "WITH",
    };

    public static IReadOnlyList<string> Sanitize(IReadOnlyList<string?> rawHeaders)
    {
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var result = new List<string>(rawHeaders.Count);

        for (var i = 0; i < rawHeaders.Count; i++)
        {
            var candidate = SanitizeOne(rawHeaders[i], i);
            var finalName = candidate;
            var suffix = 2;
            while (!used.Add(finalName))
            {
                finalName = $"{candidate}_{suffix}";
                suffix++;
            }
            result.Add(finalName);
        }

        return result;
    }

    private static string SanitizeOne(string? raw, int zeroBasedOrdinal)
    {
        var fallback = $"Column_{zeroBasedOrdinal + 1}";
        if (string.IsNullOrWhiteSpace(raw))
        {
            return fallback;
        }

        var allowListed = raw.Trim()
            .Select(c => c is (>= 'A' and <= 'Z') or (>= 'a' and <= 'z') or (>= '0' and <= '9') or '_' ? c : '_')
            .ToArray();
        var collapsed = CollapseUnderscores(new string(allowListed)).Trim('_');

        if (collapsed.Length == 0)
        {
            return fallback;
        }
        if (char.IsDigit(collapsed[0]))
        {
            collapsed = "Col_" + collapsed;
        }
        if (collapsed.Length > 128)
        {
            collapsed = collapsed[..128];
        }
        return ReservedWords.Contains(collapsed) ? fallback : collapsed;
    }

    private static string CollapseUnderscores(string value)
    {
        var result = new System.Text.StringBuilder(value.Length);
        var lastWasUnderscore = false;
        foreach (var c in value)
        {
            if (c == '_')
            {
                if (!lastWasUnderscore)
                {
                    result.Append(c);
                }
                lastWasUnderscore = true;
            }
            else
            {
                result.Append(c);
                lastWasUnderscore = false;
            }
        }
        return result.ToString();
    }
}
