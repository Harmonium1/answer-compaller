using AnswerCompiler.DataAccess;
using AnswerCompiler.LineApi;
using AnswerCompiler.LineApi.Models;
using AnswerCompiler.LineApi.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace AnswerCompiler.States;

public class SurveyApplying: BaseState, IState
{
    public SurveyApplying(LineApiClient apiClient, DataContext dataContext, BaseEvent baseEvent) : base(apiClient, dataContext, baseEvent)
    {
    }

    public async Task OnEnter()
    {
        var textMessage = ((BaseEvent as MessageEvent)!.Message as TextMessage)!;
        if (!int.TryParse(textMessage.Text, out int enterNumber))
        {
            await LinePush("Wrong format: please, enter only digits");
            return;
        }

        UserEntity user = await GetUser();
        SurveyEntity? survey = await DataContext.Surveys.FirstOrDefaultAsync(s => s.EnterNumber == enterNumber);
        if (survey is null)
        {
            await LinePush("Wrong input: survey with this number is not exist. Try another one");
            return;
        }
        survey.AppliedUserIds.Add(user.UserId);
        await UserStatusTo(UserStatus.SurveyWaiting);
        
        await LinePush("You have connected to survey #"+enterNumber+". Please, wait until beginning");
    }

    public Task<IState> Promote()
    {
        throw new NotImplementedException();
    }
}