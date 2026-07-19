using System.Collections;
using System.Collections.Concurrent;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FBSC.Common.Utility.Helpers
{
    public static class HTMLTemplateHelper
    {
        // Cache compiled regexes
        private static readonly Regex PlaceholderRegex = new Regex(@"\{([a-zA-Z0-9_.]+)\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex ForeachPattern = new Regex(@"\{#foreach\s+([a-zA-Z0-9_.]+)\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex EndForeachPattern = new Regex(@"\{/foreach\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex PlaceholderCommentStart = new Regex(@"<!--\s*\{Placeholder\}\s*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex PlaceholderCommentEnd = new Regex(@"\{Placeholder\}\s*-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex PropertyRegex = new Regex(@"\{(?!#foreach|/foreach)([a-zA-Z0-9_.]+)\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Cache reflection lookups
        private static readonly ConcurrentDictionary<string, PropertyInfo?> PropertyCache = new ConcurrentDictionary<string, PropertyInfo?>();
        private static readonly ConcurrentDictionary<string, DataColumn?> ColumnCache = new ConcurrentDictionary<string, DataColumn?>();

        /// <summary>
        /// Process HTML template and replace shortcodes {PropertyName} with actual values from model
        /// Case-insensitive matching with support for nested foreach loops on collections (up to any level)
        /// Supports: Objects, DataSets, DataTables, DataRows, Collections
        /// </summary>
        public static string ProcessTemplate(string? template, object? model)
        {
            // Handle null or empty template early
            if (string.IsNullOrEmpty(template))
                return string.Empty;

            // If model is null, we can't resolve placeholders, so just return the raw template
            if (model == null)
                return template;

            try
            {
                // Remove placeholder comments before processing
                template = RemovePlaceholderComments(template);

                // Process all foreach loops recursively first
                template = ProcessForeachLoops(template, model, 0);

                // Then process remaining simple placeholders
                return PropertyRegex.Replace(template, match =>
                {
                    string propertyPath = match.Groups[1].Value;

                    // Skip special variables like $Date, $Now, etc.
                    if (propertyPath.StartsWith("$", StringComparison.OrdinalIgnoreCase))
                        return match.Value;

                    object? value = GetPropertyValue(model, propertyPath);

                    // Handle null, DBNull, or unexpected empty values
                    return value switch
                    {
                        null => string.Empty,
                        DBNull => string.Empty,
                        _ => value.ToString() ?? string.Empty
                    };
                });
            }
            catch (Exception ex)
            {
                // Optionally log or handle errors gracefully
                Console.WriteLine($"[ProcessTemplate] Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return template; // Return the partially processed template
            }
        }


        /// <summary>
        /// Remove placeholder comments from template
        /// Removes patterns like <!--{Placeholder} and {Placeholder}-->
        /// </summary>
        private static string RemovePlaceholderComments(string template)
        {
            template = PlaceholderCommentStart.Replace(template, string.Empty);
            template = PlaceholderCommentEnd.Replace(template, string.Empty);
            return template;
        }

        /// <summary>
        /// Process foreach loops in the template with support for nesting (up to any reasonable depth)
        /// Syntax: {#foreach CollectionName} ... {/foreach}
        /// Supports: Collections, DataSets (by table name), DataTables
        /// </summary>
        private static string ProcessForeachLoops(string template, object model, int nestingLevel)
        {
            const int maxNestingLevel = 5;
            const int foreachEndTagLength = 10; // "{/foreach}".Length

            if (model == null || nestingLevel > maxNestingLevel)
                return template;

            int searchIndex = 0;

            while (true)
            {
                var startMatch = ForeachPattern.Match(template, searchIndex);
                if (!startMatch.Success)
                    break;

                string collectionPath = startMatch.Groups[1].Value.Trim();
                int startIndex = startMatch.Index;
                int contentStartIndex = startMatch.Index + startMatch.Length;

                int endIndex = FindMatchingEndForeach(template, contentStartIndex);
                if (endIndex == -1)
                {
                    searchIndex = startMatch.Index + startMatch.Length;
                    continue;
                }

                string loopTemplate = template.Substring(contentStartIndex, endIndex - contentStartIndex);
                object? collection = GetPropertyValue(model, collectionPath);
                int blockLength = (endIndex - startIndex) + foreachEndTagLength;

                string replacement = string.Empty;

                if (collection == null)
                {
                    // If collection is null, just remove the foreach block
                    template = template.Remove(startIndex, blockLength);
                    searchIndex = startIndex;
                    continue;
                }

                // Handle DataTable (check this BEFORE IEnumerable since DataTable implements both)
                if (collection is DataTable dataTable)
                {
                    replacement = ProcessDataTableLoop(loopTemplate, dataTable, nestingLevel);
                }
                // Handle DataSet - use first table
                else if (collection is DataSet dataSet && dataSet.Tables.Count > 0)
                {
                    replacement = ProcessDataTableLoop(loopTemplate, dataSet.Tables[0], nestingLevel);
                }
                // Handle DataRowCollection
                else if (collection is DataRowCollection rowCollection)
                {
                    replacement = ProcessDataRowCollectionLoop(loopTemplate, rowCollection, nestingLevel);
                }
                // Handle regular collections (AFTER DataTable check)
                else if (collection is IEnumerable enumerable && collection is not string)
                {
                    replacement = ProcessCollectionLoop(loopTemplate, enumerable, nestingLevel);
                }

                // Replace the entire foreach block
                template = template.Remove(startIndex, blockLength).Insert(startIndex, replacement);

                searchIndex = startIndex + replacement.Length;
            }

            return template;
        }

        /// <summary>
        /// Process a collection in a foreach loop
        /// </summary>
        private static string ProcessCollectionLoop(string loopTemplate, IEnumerable enumerable, int nestingLevel)
        {
            var resultBuilder = new StringBuilder();
            int index = 0;

            foreach (var item in enumerable)
            {
                if (item == null) continue;

                // Recursively process inner foreach loops
                string processedTemplate = ProcessForeachLoops(loopTemplate, item, nestingLevel + 1);

                // Process property placeholders for this loop item
                string processedItem = ProcessLoopItem(processedTemplate, item, index, nestingLevel);
                resultBuilder.Append(processedItem);
                index++;
            }

            return resultBuilder.ToString();
        }

        /// <summary>
        /// Process a DataTable in a foreach loop
        /// </summary>
        private static string ProcessDataTableLoop(string loopTemplate, DataTable dataTable, int nestingLevel)
        {
            var resultBuilder = new StringBuilder();
            int index = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                    continue;

                // Recursively process inner foreach loops
                string processedTemplate = ProcessForeachLoops(loopTemplate, row, nestingLevel + 1);

                // Process property placeholders for this DataRow
                string processedItem = ProcessLoopItem(processedTemplate, row, index, nestingLevel);
                resultBuilder.Append(processedItem);
                index++;
            }

            return resultBuilder.ToString();
        }

        /// <summary>
        /// Process a DataRowCollection in a foreach loop
        /// </summary>
        private static string ProcessDataRowCollectionLoop(string loopTemplate, DataRowCollection rows, int nestingLevel)
        {
            var resultBuilder = new StringBuilder();
            int index = 0;

            foreach (DataRow row in rows)
            {
                if (row.RowState == DataRowState.Deleted)
                    continue;

                // Recursively process inner foreach loops
                string processedTemplate = ProcessForeachLoops(loopTemplate, row, nestingLevel + 1);

                // Process property placeholders for this DataRow
                string processedItem = ProcessLoopItem(processedTemplate, row, index, nestingLevel);
                resultBuilder.Append(processedItem);
                index++;
            }

            return resultBuilder.ToString();
        }

        /// <summary>
        /// Find the matching {/foreach} for a {#foreach} block, handling nested ones
        /// </summary>
        private static int FindMatchingEndForeach(string template, int startIndex)
        {
            int depth = 1;
            int currentIndex = startIndex;

            while (currentIndex < template.Length)
            {
                var nextForeach = ForeachPattern.Match(template, currentIndex);
                var nextEnd = EndForeachPattern.Match(template, currentIndex);

                if (!nextEnd.Success)
                    return -1;

                if (nextForeach.Success && nextForeach.Index < nextEnd.Index)
                {
                    depth++;
                    currentIndex = nextForeach.Index + nextForeach.Length;
                }
                else
                {
                    depth--;
                    if (depth == 0)
                        return nextEnd.Index;

                    currentIndex = nextEnd.Index + nextEnd.Length;
                }
            }

            return -1;
        }

        /// <summary>
        /// Process one item in a foreach loop
        /// Supports: {PropertyName}, {$index}, {$index1}, nested {$index0_1}, etc.
        /// Works with objects, DataRows, and dictionaries
        /// </summary>
        private static string ProcessLoopItem(string template, object item, int index, int nestingLevel)
        {
            // Pre-calculate index strings
            string indexValue = index.ToString();
            string index1Value = (index + 1).ToString();

            // Replace index variables
            string indexVar = nestingLevel == 0 ? "$index" : $"$index{nestingLevel}";
            string index1Var = nestingLevel == 0 ? "$index1" : $"$index{nestingLevel}_1";

            var result = template
                .Replace("{" + indexVar + "}", indexValue, StringComparison.OrdinalIgnoreCase)
                .Replace("{" + index1Var + "}", index1Value, StringComparison.OrdinalIgnoreCase)
                .Replace("{$index}", indexValue, StringComparison.OrdinalIgnoreCase)
                .Replace("{$index1}", index1Value, StringComparison.OrdinalIgnoreCase);

            // Replace properties
            return PropertyRegex.Replace(result, match =>
            {
                string propertyPath = match.Groups[1].Value;
                if (propertyPath.StartsWith("$", StringComparison.OrdinalIgnoreCase))
                    return match.Value;

                object? value = GetPropertyValue(item, propertyPath);
                return value?.ToString() ?? string.Empty;
            });
        }

        /// <summary>
        /// Get property value from object, supports nested paths and dynamic dictionaries
        /// Extended to support DataRow, DataTable, and DataSet
        /// Supports collection methods: Count, Length, First(), Last(), Sum(), Average(), Min(), Max()
        /// </summary>
        private static object? GetPropertyValue(object obj, string propertyPath)
        {
            if (obj == null || string.IsNullOrWhiteSpace(propertyPath))
                return null;

            string[] parts = propertyPath.Split('.');
            object? current = obj;

            foreach (string part in parts)
            {
                if (current == null)
                    return null;

                // Check if this is a collection method call
                if (part.EndsWith("()", StringComparison.OrdinalIgnoreCase))
                {
                    string methodName = part.Substring(0, part.Length - 2);
                    var methodResult = ExecuteCollectionMethod(current, methodName);
                    if (methodResult != null)
                    {
                        current = methodResult;
                        continue;
                    }
                    return null;
                }

                // Handle DataRow - access columns by name (with caching)
                if (current is DataRow dataRow)
                {
                    // Use a more robust cache key that includes table reference to avoid collisions
                    string cacheKey = $"{dataRow.Table.GetHashCode()}_{part}";
                    var column = ColumnCache.GetOrAdd(cacheKey, _ =>
                        dataRow.Table.Columns.Cast<DataColumn>()
                            .FirstOrDefault(c => c.ColumnName.Equals(part, StringComparison.OrdinalIgnoreCase))
                    );

                    if (column != null)
                    {
                        current = dataRow[column];
                        if (current == DBNull.Value)
                            current = null;
                        continue;
                    }
                    return null;
                }

                // Handle DataSet - access tables by name
                if (current is DataSet dataSet)
                {
                    var table = dataSet.Tables.Cast<DataTable>()
                        .FirstOrDefault(t => t.TableName.Equals(part, StringComparison.OrdinalIgnoreCase));

                    if (table != null)
                    {
                        current = table;
                        continue;
                    }
                    return null;
                }

                // Handle DataTable - access rows collection or table properties
                if (current is DataTable dataTable)
                {
                    if (part.Equals("Rows", StringComparison.OrdinalIgnoreCase))
                    {
                        current = dataTable.Rows;
                        continue;
                    }

                    // Check for Count property on DataTable itself
                    if (part.Equals("Count", StringComparison.OrdinalIgnoreCase))
                    {
                        current = dataTable.Rows.Count;
                        continue;
                    }

                    var tableType = current.GetType();
                    var tableProp = GetCachedProperty(tableType, part);
                    if (tableProp != null)
                    {
                        current = tableProp.GetValue(current);
                        continue;
                    }
                    return null;
                }

                // Handle DataRowCollection
                if (current is DataRowCollection rowCollection)
                {
                    if (part.Equals("Count", StringComparison.OrdinalIgnoreCase))
                    {
                        current = rowCollection.Count;
                        continue;
                    }
                    return null;
                }

                // Handle collection properties and methods (Count, Length, etc.)
                if (current is IEnumerable enumerable && current is not string)
                {
                    var collectionValue = GetCollectionProperty(enumerable, part);
                    if (collectionValue != null)
                    {
                        current = collectionValue;
                        continue;
                    }
                    return null;
                }

                var type = current.GetType();

                // Handle ExpandoObject/dynamic dictionary
                if (current is IDictionary<string, object> dict)
                {
                    var key = dict.Keys.FirstOrDefault(k => k.Equals(part, StringComparison.OrdinalIgnoreCase));
                    if (key != null)
                    {
                        current = dict[key];
                        continue;
                    }
                    return null;
                }

                // Reflection-based lookup for regular objects (with caching)
                var prop = GetCachedProperty(type, part);
                if (prop == null)
                    return null;

                current = prop.GetValue(current);
            }

            return current;
        }

        /// <summary>
        /// Get cached property info to avoid repeated reflection lookups
        /// </summary>
        private static PropertyInfo? GetCachedProperty(Type type, string propertyName)
        {
            string cacheKey = $"{type.FullName}_{propertyName}";
            return PropertyCache.GetOrAdd(cacheKey, _ =>
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            );
        }

        /// <summary>
        /// Execute collection methods like First(), Last(), Sum(), Average(), Min(), Max()
        /// </summary>
        private static object? ExecuteCollectionMethod(object obj, string methodName)
        {
            if (obj is string || obj is not IEnumerable enumerable)
                return null;

            // Use a more efficient approach - avoid multiple enumerations
            switch (methodName.ToLowerInvariant())
            {
                case "first":
                    foreach (var item in enumerable)
                        return item; // Return first item without creating list
                    return null;

                case "last":
                    {
                        object? last = null;
                        foreach (var item in enumerable)
                            last = item;
                        return last;
                    }

                case "sum":
                    try
                    {
                        decimal sum = 0;
                        foreach (var item in enumerable)
                            sum += Convert.ToDecimal(item);
                        return sum;
                    }
                    catch
                    {
                        return null;
                    }

                case "average":
                case "avg":
                    try
                    {
                        decimal sum = 0;
                        int count = 0;
                        foreach (var item in enumerable)
                        {
                            sum += Convert.ToDecimal(item);
                            count++;
                        }
                        return count > 0 ? sum / count : (decimal?)null;
                    }
                    catch
                    {
                        return null;
                    }

                case "min":
                    try
                    {
                        decimal? min = null;
                        foreach (var item in enumerable)
                        {
                            decimal value = Convert.ToDecimal(item);
                            if (!min.HasValue || value < min.Value)
                                min = value;
                        }
                        return min;
                    }
                    catch
                    {
                        return null;
                    }

                case "max":
                    try
                    {
                        decimal? max = null;
                        foreach (var item in enumerable)
                        {
                            decimal value = Convert.ToDecimal(item);
                            if (!max.HasValue || value > max.Value)
                                max = value;
                        }
                        return max;
                    }
                    catch
                    {
                        return null;
                    }

                default:
                    return null;
            }
        }

        /// <summary>
        /// Get property from collection (Count, Length) or execute collection methods
        /// </summary>
        private static object? GetCollectionProperty(IEnumerable enumerable, string propertyName)
        {
            // Handle Count
            if (propertyName.Equals("Count", StringComparison.OrdinalIgnoreCase))
            {
                // Try ICollection first (O(1) operation)
                if (enumerable is ICollection collection)
                    return collection.Count;

                // Fall back to enumeration (O(n))
                int count = 0;
                foreach (var _ in enumerable)
                    count++;
                return count;
            }

            // Handle Length (for arrays)
            if (propertyName.Equals("Length", StringComparison.OrdinalIgnoreCase))
            {
                if (enumerable is Array array)
                    return array.Length;

                if (enumerable is ICollection collection)
                    return collection.Count;

                int count = 0;
                foreach (var _ in enumerable)
                    count++;
                return count;
            }

            // Try to access actual properties on the collection type (with caching)
            var type = enumerable.GetType();
            var prop = GetCachedProperty(type, propertyName);
            return prop?.GetValue(enumerable);
        }

        /// <summary>
        /// Clear all caches - call this if memory usage becomes a concern
        /// </summary>
        public static void ClearCache()
        {
            PropertyCache.Clear();
            ColumnCache.Clear();
        }
    }
}