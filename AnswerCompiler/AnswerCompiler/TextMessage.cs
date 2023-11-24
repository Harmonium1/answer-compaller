using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnswerCompiler;

public class ReplyAction:Action
{
    public override string Url => "/v2/bot/message/reply";
    public string ReplyToken { get; set; }
    public IList<TextMessage> Messages { get; set; }
    public bool? NotificationDisabled { get; set;}
    public override string Serialize(JsonSerializerOptions options) =>JsonSerializer.Serialize(this,options);
    
    //public string takeReplyToken(LineWebhook lineWebhook)=>ReplyToken = lineWebhook.Events.First().ReplyToken;
}

public static class MessageType
{
    public static string Text => "text";
}

public class TextMessage : Message
{
    public TextMessage(string text)
    {
        Text = text;
        Type = "text";
    }
    public string Text { get; set;}
    public string? QuoteToken { get; set;}
}
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TextMessage), typeDiscriminator: "text")]
public class Message
{
    public string Type { get; set; }
}

public abstract class Action
{
    public abstract string Url { get; }
    public abstract string Serialize(JsonSerializerOptions options);
}