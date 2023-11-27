using System.Text.Json.Serialization;

namespace AnswerCompiler.LineApi.Models.Common;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(UserSource), typeDiscriminator: "user")]
public class BaseSource
{
    public string Type { get; set; } = null!;
}

public class UserSource : BaseSource
{
    public string UserId { get; set; } = null!;
}

public class GroupChatSource : BaseSource
{
    public string GroupId { get; set; } = null!;
    public string? UserId { get; set; }
}

public class RoomChatSource : BaseSource
{
    public string RoomId { get; set; } = null!;
    public string? UserId { get; set; }
}