namespace Wertek.WebApiModeler.Models;

public class FilterDTO
{
    public FilterDTO()
    {
        Page = 0;
        PageSize = 0;
        Sort = new List<Sort>();
        Filters = new List<Filter>();
    }
    public int Page {get; set;}
    public int PageSize { get; set; }
    public IEnumerable<Sort> Sort { get; set; }
    public IEnumerable<Filter> Filters { get; set; }
}