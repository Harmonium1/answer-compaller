using AnswerCompiler.DataAccess;
using AnswerCompiler.LineApi;
using AnswerCompiler.LineApi.Models;
using AnswerCompiler.LineApi.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace AnswerCompiler.States;

public abstract class BaseState
{
    protected readonly string LineUserId;
    protected readonly BaseEvent BaseEvent;
    protected readonly DataContext DataContext;
    protected readonly LineApiClient ApiClient;
    
    protected BaseState(LineApiClient apiClient, DataContext dataContext, BaseEvent baseEvent)
    {
        ApiClient = apiClient;
        DataContext = dataContext;
        BaseEvent = baseEvent;
        
        if (baseEvent.Source is not UserSource userSource)
            throw new ArgumentException("Event has wrong user source");
        
        LineUserId = userSource.UserId;
    }

    protected async Task LinePush(params Message[] messages) => await ApiClient.Push(LineUserId, false, messages);
    protected async Task LinePush(string message) => await ApiClient.Push(LineUserId, false, message);

    protected async Task LineMulticast(string[] lineUserIds, params Message[] messages) =>
        await ApiClient.Multicast(lineUserIds, false, messages);
    protected async Task LineReply(string replyToken, string message) => await ApiClient.Reply(replyToken, false, message);
    protected async Task<UserEntity> GetUser() => (await DataContext.Users.FindAsync(LineUserId))!;

    protected async Task UserStatusTo(UserStatus newStatus)
    {
        var user = await DataContext.Users.FirstAsync(u => u.LineUserId == LineUserId);
        user.Status = newStatus;
        await DataContext.SaveChangesAsync();
    }
}