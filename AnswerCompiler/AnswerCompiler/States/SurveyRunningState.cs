using AnswerCompiler.DataAccess;
using AnswerCompiler.LineApi;
using AnswerCompiler.LineApi.Models;
using AnswerCompiler.LineApi.Models.Events;
using AnswerCompiler.LineApi.Models.Messages;

namespace AnswerCompiler.States;

public class SurveyRunningState: BaseState, IState
{
    public SurveyRunningState(LineApiClient apiClient, DataContext dataContext, BaseEvent baseEvent) : base(apiClient, dataContext, baseEvent)
    {
    }

    public async Task OnEnter()
    {
        await UserStatusTo(UserStatus.SurveyRunning);

        UserEntity user = await GetUser();
        var lastSurvey = DataContext.Surveys.Where(s => s.AuthorId == user.UserId).ToList();
        if (lastSurvey is null)
        {
            throw new ArgumentException("Cant find started surveys");
        }
        
        int questionNum = lastSurvey.MaxBy(x=>x.Created)!.Answers.Count != 0 ?
            lastSurvey.MaxBy(x=>x.Created)!.Answers.Select(a => a.QuestionId).Max()
            : 1;
        var message = new TemplateMessage()
        {
            AltText = $"Survey's question {questionNum}",
            Template = new ButtonsTemplate()
            {
                Text = $"Question number {questionNum}. Select answer: ",
                Actions = new()
            }
        };
        for (int i = 0; i < lastSurvey.MaxBy(x=>x.Created)!.VariantsAmount; i++)
        {
            var template = message.Template as ButtonsTemplate;
            template!.Actions.Add(new PostbackAction()
            {
                Label = $"Answer is {i}",
                Data = $"action=poll&surveyId={lastSurvey.MaxBy(x=>x.Created)!.SurveyId}&questionNum={questionNum}&answer={i}"
            });
        }

        string[] sendingUsers = lastSurvey.MaxBy(x=>x.Created)!.AppliedUserIds.ToArray();

        await LineMulticast(sendingUsers, message);

        var messageToTeacher = new TemplateMessage()
        {
            AltText = $"Get answers on question",
            Template = new ButtonsTemplate()
            {
                Text = $"Get answers on question",
                Actions = new()
                {
                    new PostbackAction()
                    {
                        Label = $"Get answers",
                        Data = $"action=getAnswers&surveyId={lastSurvey.MaxBy(x=>x.Created)!.SurveyId}&questionNum={questionNum}"
                    }
                }
            }
        };

        await LinePush(messageToTeacher);
    }

    public Task<IState> Promote()
    {
        throw new NotImplementedException();
    }
}