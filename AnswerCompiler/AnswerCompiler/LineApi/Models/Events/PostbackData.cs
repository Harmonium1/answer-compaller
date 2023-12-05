namespace AnswerCompiler.LineApi.Models.Events;

public class PostbackData
{
    public string Data { get; set; }
    public PostbackParams Params { get; set; }
    
}

public class PostbackParams
{
    public DateTimeOffset Datetime { get; set; }
}