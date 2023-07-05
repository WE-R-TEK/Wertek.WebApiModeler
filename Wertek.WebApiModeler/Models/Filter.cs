using System.Text.Json.Serialization;

namespace Wertek.WebApiModeler.Models;

public class Filter
{
    public Filter()
    {
        Clauses = new List<Clause>();
        Logic = Logic.And;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Logic Logic { get; set; }
    public IEnumerable<Clause> Clauses { get; set; }
}

public class Clause
{
    public Clause()
    {
        Filters = new List<Filter>();
        Field = "";
        Operator = Operators.Equal;
        Value = "";
        Capitalize = true;
    }
    public string Field { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Operators Operator { get; set; }
    public string Value { get; set; }
    public IEnumerable<Filter> Filters { get; set; }
    public bool? Capitalize { get; set; }
}

public enum Operators
{
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Contains,
    StartsWith,
    EndsWith,
    In,
    NotIn,
    Between,
    NotBetween,
    IsNull,
    IsNotNull
}

public enum Logic
{
    And,
    Or
}