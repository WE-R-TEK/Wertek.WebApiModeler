namespace Wertek.WebApiModeler.Models;

public class PaginatedResult<T> where T : class
{
    public PaginatedResult()
    {
        Items = new List<T>();
    }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public IEnumerable<T> Items { get; set; }
    public int TotalFiltered { get; set; }
}