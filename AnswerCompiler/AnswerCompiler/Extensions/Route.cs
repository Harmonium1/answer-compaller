using Microsoft.AspNetCore.WebUtilities;

namespace AnswerCompiler.Extensions;

public record Route
{
    public Controls Control { get; set; }
    public Actions Action { get; set; }
    public IReadOnlyDictionary<string, string> Properties { get; init; } = new Dictionary<string, string>();

    public override string ToString()
    {
        string? controlName = Enum.GetName(typeof(Controls), Control);
        string? actionName = Enum.GetName(typeof(Actions), Action);
        string url = $"{controlName}/{actionName}/";
        var dictionary = Properties.ToDictionary(pair => pair.Key, pair => pair.Value);

        return QueryHelpers.AddQueryString(url, dictionary!);
    }

    public static Route Parse(string data)
    {
        var control = Enum.Parse<Controls>(data.Split("/")[0]);
        var action = Enum.Parse<Actions>(data.Split("/")[1]);
        string queryString = data.Split("/")[2];
        var dictionary = QueryHelpers.ParseQuery(queryString).ToDictionary(pair=>pair.Key, pair=>pair.Value.ToString());

        return new() { Control = control, Action = action, Properties = dictionary };
    }
}

public enum Controls
{
    User,
    Survey
}

public enum Actions
{
    Create,
    Read,
    Edit,
    Stop,
    Ask,
    Poll
}