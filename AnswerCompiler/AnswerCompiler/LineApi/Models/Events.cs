using System.Text.Json.Serialization;
using AnswerCompiler.LineApi.Models.Common;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace AnswerCompiler.LineApi.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(FollowEvent), typeDiscriminator: "follow")]
[JsonDerivedType(typeof(MessageEvent), typeDiscriminator: "message")]
public class BaseEvent
{
    public EventType Type { get; set; }
    public EventMode EventMode { get; set; }
    public long Timestamp { get; set; }
    public BaseSource? Source { get; set;}
    public string WebhookEventId { get; set; } = null!;
    public DeliveryContext DeliveryContext { get; set;} = null!;
}

public class MessageEvent : BaseEvent
{
    public string ReplyToken { get; set; } = null!;
    public Message Message { get; set; } = null!;
}
public class FollowEvent : BaseEvent
{
    public string ReplyToken { get; set; } = null!;
}