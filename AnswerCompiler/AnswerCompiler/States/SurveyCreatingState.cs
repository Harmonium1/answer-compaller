using AnswerCompiler.DataAccess;
using AnswerCompiler.LineApi;
using AnswerCompiler.LineApi.Models;
using AnswerCompiler.LineApi.Models.Events;
using AnswerCompiler.LineApi.Models.Messages;

namespace AnswerCompiler.States;

public class SurveyCreatingState: BaseState, IState
{
    public SurveyCreatingState(LineApiClient apiClient, DataContext dataContext, BaseEvent baseEvent) : base(apiClient, dataContext, baseEvent)
    {
    }

    public async Task OnEnter()
    {
        TextMessage textMessage = ((BaseEvent as MessageEvent)!.Message as TextMessage)!;
        if (!int.TryParse(textMessage.Text, out int questionsAmount))
        {
            await LinePush("Please, enter questions amount");
            return;
        }

        UserEntity user = await GetUser();
        int surveyNumber = SurveyEntity.RandomNumber();
        DataContext.Surveys.Add(new()
        {
            SurveyId = Guid.NewGuid(),
            AuthorId = user.UserId,
            Created = DateTimeOffset.Now,
            VariantsAmount = questionsAmount,
            EnterNumber = surveyNumber
        });
        
        user.Status = UserStatus.SurveyRequested;

        await DataContext.SaveChangesAsync();

        var message = new TemplateMessage()
        {
            AltText = "Survey's start confirmation",
            Template = new ButtonsTemplate()
            {
                Text = "Survey number: " + surveyNumber,
                Actions = new()
                {
                    new PostbackAction()
                    {
                        Label = "Start",
                        Data = "action:survey_start"
                    }
                }
            }
        };

        await LinePush(message);
    }

    public async Task<IState> Promote()
    {
        return await Task.FromResult(new SurveyRunningState(ApiClient, DataContext, BaseEvent));
    }
}