using AnswerCompiler.DataAccess;
using AnswerCompiler.LineApi;
using AnswerCompiler.LineApi.Models.Events;

namespace AnswerCompiler.States;

public class UserCreatingState : BaseState, IState
{
    public UserCreatingState(LineApiClient apiClient, DataContext dataContext, BaseEvent incomeEvent)
        : base(apiClient, dataContext, incomeEvent)
    {
    }

    public Task OnEnter() => Task.CompletedTask;

    public async Task<IState> Promote()
    {
        if (BaseEvent is not FollowEvent followEvent)
            throw new ArgumentException("Try to promote state with wrong event");

        DataContext.Users.Add(new()
        {
            UserId = LineUserId,
            Status = UserStatus.Creating,
            Role = UserRole.Student
        });
        await DataContext.SaveChangesAsync();

        await LinePush("User has been registered.");

        return new UsernameRegistrationState(ApiClient, DataContext, BaseEvent);
    }
}