using AnswerCompiler.DataAccess;
using AnswerCompiler.LineApi;
using AnswerCompiler.LineApi.Models;

namespace AnswerCompiler.States;

public class StandbyState : BaseState, IState
{
    public StandbyState(LineApiClient apiClient, DataContext dataContext, BaseEvent baseEvent) 
        : base(apiClient, dataContext, baseEvent)
    {
    }
    
    public async Task OnEnter()
    {
        await UserStatusTo(UserStatus.Standby);
        await LinePush("You are registered. Please, enter a survey number.");
    }

    public Task<IState> Promote()
    {
        throw new NotImplementedException();
    }
}