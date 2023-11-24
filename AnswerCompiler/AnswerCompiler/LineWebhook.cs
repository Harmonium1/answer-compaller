using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnswerCompiler;

public class LineWebhook
{
    public string Destination { get; set; } = null!;
    public IReadOnlyCollection<BaseEvent>? Events { get; set; }
    
    public static LineWebhook? Deserialize(Stream request, JsonSerializerOptions? options)
        => JsonSerializer.Deserialize<LineWebhook>(request, options);
}

public enum EventType
{
    Message,
    Unsend,Follow,Unfollow,Join,Leave,MemberJoin,MemberLeave,Postback,Beacon,AccountLink,DeviceLink,DeviceUnLink 
}

public enum Mode
{
    Active,Standby
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(FollowEvent), typeDiscriminator: "follow")]
[JsonDerivedType(typeof(MessageEvent), typeDiscriminator: "message")]
public class BaseEvent
{
    public EventType Type { get; set; }
    public Mode Mode { get; set; }
    public long Timestamp { get; set; }
    public BaseSource? Source { get; set;}
    public string WebhookEventId { get; set; }
    public DeliveryContext DeliveryContext { get; set;}
}

public class MessageEvent : BaseEvent
{
    public string ReplyToken { get; set; }
    public Message Message { get; set; }
}
public class FollowEvent : BaseEvent
{
    public string ReplyToken { get; set; }

    public ReplyAction CreateReply(params TextMessage[] messages)
    {
        return new ReplyAction { Messages = messages.ToList(), ReplyToken = ReplyToken };
    }
}
public class DeliveryContext
{
    public bool IsRedelivery { get; set; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(UserSource), typeDiscriminator: "user")]
public class BaseSource
{
    public string Type { get; set; }
    
}

public class UserSource : BaseSource
{
    public string UserId { get; set; } 
}
public class GroupChatSource : BaseSource
{
    public string GroupId { get; set; }
    public string? UserId { get; set; }
}

public class RoomChatSource : BaseSource
{
    public string RoomId { get; set; }
    public string? UserId { get; set; }
}