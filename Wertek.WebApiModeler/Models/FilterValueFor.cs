using System.Text.Json.Serialization;

namespace Wertek.WebApiModeler.Models;

public class FilterValueFor
{
    public FilterValueFor()
    {
        Direction = SortDirection.Asc;
        AutoComplete = "";
        KeyField = "";
        Filters = new List<Filter>();
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SortDirection Direction { get; set; }
    public string AutoComplete { get; set; }
    public string KeyField { get; set; }
    public IEnumerable<Filter> Filters { get; set; }
}