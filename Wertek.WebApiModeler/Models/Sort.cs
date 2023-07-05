using System.Text.Json.Serialization;

namespace Wertek.WebApiModeler.Models;
public class Sort
{
    public Sort()
    {
        Field = "";
        Dir = SortDirection.Asc;
        Capitalize = true;
    }
    public string Field { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SortDirection Dir { get; set; }    
    public bool? Capitalize { get; set; }
}

public enum SortDirection
{
    Asc,
    Desc
}