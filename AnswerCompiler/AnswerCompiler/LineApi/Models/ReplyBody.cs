// ReSharper disable ClassNeverInstantiated.Global
namespace AnswerCompiler.LineApi.Models;

public class ReplyBody
{
    public string ReplyToken { get; set; } = null!;
    public IList<Message> Messages { get; set; } = null!;
    public bool? NotificationDisabled { get; set;}
}