using System.Globalization;

namespace FBSC.ODMS.Infrastructure.Services.Dashboard;

/// <summary>
/// Infers a SQL column type by sampling a column's raw string values from an uploaded file -
/// the inverse of DataSourceSchemaDiscoveryService.InferSemanticType (which infers a semantic
/// *role* from an already-known SQL type). No equivalent "guess the SQL type from sample
/// values" heuristic exists elsewhere in the solution to reuse, so this is new.
/// int -> decimal -> datetime -> nvarchar fallback, in that order.
/// </summary>
public static class UploadedColumnTypeInferrer
{
    private const int MaxSampleSize = 500;

    public static string InferSqlType(IEnumerable<string?> columnValues)
    {
        var sample = columnValues
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Take(MaxSampleSize)
            .ToList();

        if (sample.Count == 0)
        {
            return "nvarchar(450)";
        }
        if (sample.All(v => int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)))
        {
            return "int";
        }
        if (sample.All(v => decimal.TryParse(v, NumberStyles.Number, CultureInfo.InvariantCulture, out _)))
        {
            return "decimal(18,4)";
        }
        if (sample.All(v => DateTime.TryParse(v, CultureInfo.InvariantCulture, DateTimeStyles.None, out _)))
        {
            return "datetime2";
        }
        return "nvarchar(450)";
    }

    public static Type ToClrType(string sqlType) => sqlType switch
    {
        "int" => typeof(int),
        "decimal(18,4)" => typeof(decimal),
        "datetime2" => typeof(DateTime),
        _ => typeof(string),
    };
}
