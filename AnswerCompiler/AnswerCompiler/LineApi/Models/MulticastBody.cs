namespace AnswerCompiler.LineApi.Models;

public class MulticastBody
{
    public string[] To { get; set; } = null!;
    public IList<Message> Messages { get; set; } = null!;
    public bool? NotificationDisabled { get; set;}
}