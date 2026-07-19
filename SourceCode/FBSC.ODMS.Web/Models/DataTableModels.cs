using Newtonsoft.Json;

namespace FBSC.ODMS.Web.Models
{
    public class DataTablesResponse<T>
    {
        [JsonProperty(PropertyName = "draw")]
        public int Draw { get; set; }
        [JsonProperty(PropertyName = "recordsTotal")]
        public int RecordsTotal { get; set; }
        [JsonProperty(PropertyName = "recordsFiltered")]
        public int RecordsFiltered { get; set; }
        [JsonProperty(PropertyName = "data")]
        public IEnumerable<T>? Data { get; set; }
        [JsonProperty(PropertyName = "error", NullValueHandling = NullValueHandling.Ignore)]
        public string? Error { get; set; }
    }
    public class DataTablesRequest(int draw, int start, int length, Search? search, IEnumerable<Order> orders, IEnumerable<DataTableColumn> columns)
    {
        public int Draw { get; } = draw;
        public int Start { get; } = start;
        public int Length { get; } = length;
        public Search? Search { get; } = search;
        public IEnumerable<Order> Orders { get; } = orders;
        public IEnumerable<DataTableColumn> Columns { get; } = columns;
    }
    public class Search(string value, bool regex)
    {
        public string Value { get; } = value;
        public bool Regex { get; } = regex;
    }
    public class DataTableColumn(string? data, string? name, bool searchable, 
        bool orderable, string? searchValue, bool regex)
    {
        public string? Data { get; } = data;
        public string? Name { get; } = name;
        public bool Searchable { get; } = searchable;
        public bool Orderable { get; } = orderable;
        public string? SearchValue { get; } = searchValue;
        public bool SearchRegEx { get; } = regex;
    }

    public class Order(int column, string? dir)
    {
        public int Column { get; } = column;
        public string? Dir { get; } = dir;
    }
}
