using AnswerCompiler.DataAccess;
using AnswerCompiler.LineApi;
using AnswerCompiler.LineApi.Models;

namespace AnswerCompiler.Controllers;

public abstract class BaseController(LineApiClient apiClient, DataContext dataContext)
{
    protected readonly DataContext DataContext = dataContext;

    protected async Task Push(UserEntity user, params Message[] messages)
        => await apiClient.Push(user.UserId, false, messages);

    protected async Task Push(string userId, params string[] messages)
        => await apiClient.Push(userId, false,
            messages.Select(text => new TextMessage(text)).Cast<Message>().ToArray());
    protected async Task Push(UserEntity user, params string[] messages)
        => await apiClient.Push(user.UserId, false,
            messages.Select(text => new TextMessage(text)).Cast<Message>().ToArray());

    protected async Task Multicast(IEnumerable<string> userIds, params Message[] messages)
        => await apiClient.Multicast(userIds.ToArray(), false, messages);
}