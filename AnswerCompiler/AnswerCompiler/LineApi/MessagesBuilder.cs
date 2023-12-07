using AnswerCompiler.Controllers;
using AnswerCompiler.DataAccess;
using AnswerCompiler.Extensions;
using AnswerCompiler.LineApi.Models;
using AnswerCompiler.LineApi.Models.Messages;

namespace AnswerCompiler.LineApi;

public static class MessagesBuilder
{
    public static TemplateMessage SurveyCreate() => new()
    {
        AltText = "Survey's create confirmation",
        Template = new ButtonsTemplate
        {
            Text = "Press button, when you want to start a survey",
            Actions = new()
            {
                new PostbackAction
                {
                    Label = "Start a survey",
                    Data = new Route
                    {
                        Control = Controls.Survey,
                        Action = Actions.Create
                    }.ToString()
                }
            }
        }
    };
    
    public static TemplateMessage SurveyStartConfirm(SurveyEntity survey) => new()
    {
        AltText = "Survey's start confirmation",
        Template = new ButtonsTemplate
        {
            Text = "Survey number: " + survey.EnterNumber,
            Actions = new()
            {
                new PostbackAction()
                {
                    Label = "Ask",
                    Data = new Route
                    {
                        Control = Controls.Survey,
                        Action = Actions.Ask,
                        Properties = new Dictionary<string, string>
                        {
                            {
                                nameof(SurveyStartRequest.SurveyId), survey.SurveyId.ToString()
                            }
                        }
                    }.ToString()
                }
            }
        }
    };

    public static TemplateMessage SurveyNextAction(SurveyEntity survey) => new()
    {
        AltText = "Survey's next action confirmation",
        Template = new ButtonsTemplate
        {
            Text = "What do you want to do next?",
            Actions = new()
            {
                new PostbackAction()
                {
                    Label = "Next question",
                    Data = new Route
                    {
                        Control = Controls.Survey,
                        Action = Actions.Ask,
                        Properties = new Dictionary<string, string>
                        {
                            {
                                nameof(SurveyStartRequest.SurveyId), survey.SurveyId.ToString()
                            }
                        }
                    }.ToString()
                },
                new PostbackAction()
                {
                    Label = "Check answers",
                    Data = new Route
                    {
                        Control = Controls.Survey,
                        Action = Actions.Read,
                        Properties = new Dictionary<string, string>
                        {
                            { nameof(SurveyReadRequest.SurveyId), survey.SurveyId.ToString() },
                            { nameof(SurveyReadRequest.QuestionId), (survey.LastQuestionId+1).ToString() }
                        }
                    }.ToString()
                },
                new PostbackAction()
                {
                    Label = "Stop survey",
                    Data = new Route
                    {
                        Control = Controls.Survey,
                        Action = Actions.Stop,
                        Properties = new Dictionary<string, string>
                        {
                            { nameof(SurveyReadRequest.SurveyId), survey.SurveyId.ToString() }
                        }
                    }.ToString()
                },
            }
        }
    };

    public static TemplateMessage SurveyQuestionToUsers(SurveyEntity survey)
    {
        int questionId = survey.LastQuestionId + 1;
        var buttons = Enumerable.Range(1, survey.VariantsAmount)
            .Select(i => new PostbackAction
            {
                Label = i.ToString(),
                Data = new Route
                {
                    Control = Controls.User,
                    Action = Actions.Poll,
                    Properties = new Dictionary<string, string>
                    {
                        { nameof(SurveyPollRequest.SurveyId), survey.SurveyId.ToString() },
                        { nameof(SurveyPollRequest.QuestionId), questionId.ToString() },
                        { nameof(SurveyPollRequest.Answer), i.ToString() },
                    }
                }.ToString()
            });
        
        return new()
        {
            AltText = $"Survey's question {questionId}",
            Template = new ButtonsTemplate()
            {
                Text = $"Question number {questionId}. Select answer: ",
                Actions = buttons.Cast<MessageAction>().ToList()
            }
        };
    }

    public static IEnumerable<TextMessage> AnswersMessages(SurveyEntity survey, int questionId, UserEntity[] currentUsers)
    {
        var currentAnswers = survey.Answers.Where(a => a.QuestionId == questionId);
        var messages = currentAnswers.ToLookup(a => a.Value);
        foreach (var group in messages)
        {
            string names = group.ToList()
                .Select(answer => currentUsers.First(user => user.UserId == answer.AuthorId))
                .Select(user => user.Name)
                .Order()
                .Aggregate((name1, name2) => name1 + "\n" + name2);
            
            yield return new TextMessage($"{group.Key}:\nAmount is {group.Count()}\n{names}");
        }
    }
}