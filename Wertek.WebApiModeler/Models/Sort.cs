using System.Text.Json.Serialization;

namespace Wertek.WebApiModeler.Models;
public class Sort
{
    public Sort()
    {
        Field = "";
        Dir = SortDirection.Asc;
    }
    public string Field { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SortDirection Dir { get; set; }    
}

public enum SortDirection
{
    Asc,
    Desc
}