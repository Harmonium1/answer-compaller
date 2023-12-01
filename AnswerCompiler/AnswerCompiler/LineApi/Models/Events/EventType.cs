// ReSharper disable IdentifierTypo
namespace AnswerCompiler.LineApi.Models.Events;

public enum EventType
{
    Message,
    Unsend,
    Follow,
    Unfollow,
    Join,
    Leave,
    MemberJoin,
    MemberLeave,
    Postback,
    Beacon,
    AccountLink,
    DeviceLink,
    DeviceUnLink 
}