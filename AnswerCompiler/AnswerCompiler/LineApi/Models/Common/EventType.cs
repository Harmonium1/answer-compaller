// ReSharper disable IdentifierTypo
namespace AnswerCompiler.LineApi.Models.Common;

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