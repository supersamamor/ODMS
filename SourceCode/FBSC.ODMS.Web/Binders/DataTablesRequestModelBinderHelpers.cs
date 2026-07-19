using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

internal static class DataTablesRequestModelBinderHelpers
{
    public static IEnumerable<DataTableColumn> TryGetColumns(IValueProvider valueProvider)
    {
        int i = 0;
        List<DataTableColumn> list = [];
        for (; valueProvider.GetValue($"columns[{i}][data]").FirstValue != null; i++)
        {
            _ = TryParse<string>(valueProvider.GetValue($"columns[{i}][data]"), out string? result);
            _ = TryParse<string>(valueProvider.GetValue($"columns[{i}][name]"), out string? result2);
            _ = TryParse<bool>(valueProvider.GetValue($"columns[{i}][searchable]"), out bool result3);
            _ = TryParse<bool>(valueProvider.GetValue($"columns[{i}][orderable]"), out bool result4);
            _ = TryParse<string>(valueProvider.GetValue($"columns[{i}][search][value]"), out string? result5);
            _ = TryParse<bool>(valueProvider.GetValue($"columns[{i}][search][regex]"), out bool result6);
            list.Add(new DataTableColumn(result, result2, result3, result4, result5, result6));
        }

        return list;
    }
    public static bool TryParse<T>(ValueProviderResult value, out T? result)
    {
        result = default;
        if (value.FirstValue == null)
        {
            return false;
        }

        try
        {
            result = (T)Convert.ChangeType(value.FirstValue, typeof(T));
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static IEnumerable<Order> TryGetOrders(IValueProvider valueProvider)
    {
        int i = 0;
        List<Order> list = [];
        for (; valueProvider.GetValue($"order[{i}][column]").FirstValue != null; i++)
        {
            _ = DataTablesRequestModelBinderHelpers.TryParse<int>(valueProvider.GetValue($"order[{i}][column]"), out int result);
            _ = DataTablesRequestModelBinderHelpers.TryParse<string>(valueProvider.GetValue($"order[{i}][dir]"), out string? result2);
            list.Add(new Order(result, result2));
        }

        return list;
    }
}