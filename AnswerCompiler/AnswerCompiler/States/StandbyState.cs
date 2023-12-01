using AnswerCompiler.DataAccess;
using AnswerCompiler.LineApi;
using AnswerCompiler.LineApi.Models;
using AnswerCompiler.LineApi.Models.Events;

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

    public async Task<IState> Promote()
    {
        UserEntity user = await GetUser();
        if (BaseEvent is MessageEvent messageEvent && user.Role == UserRole.Teacher && messageEvent.Message is TextMessage textMessage)
        {
            return new SurveyCreatingState(ApiClient, DataContext, BaseEvent);
        }

        throw new NotImplementedException();
    }
}