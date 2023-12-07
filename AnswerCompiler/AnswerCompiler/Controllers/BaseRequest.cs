using AnswerCompiler.DataAccess;
using AnswerCompiler.Extensions;
using AnswerCompiler.LineApi.Models;
using AnswerCompiler.LineApi.Models.Events;

namespace AnswerCompiler.Controllers;

public abstract record BaseRequest
{
    public string UserId { get; }
    public async Task<UserEntity> GetUser(DataContext context) 
        => await context.Users.FindAsync(UserId) ?? throw new InvalidOperationException();
    protected BaseRequest(BaseEvent baseEvent)
    {
        if (baseEvent.Source is not UserSource userSource)
            throw new ArgumentException("Event has wrong user source");
        
        UserId = userSource.UserId;
    }
}

public record UserCreateRequest : BaseRequest
{
    public UserCreateRequest(FollowEvent eventData) : base(eventData)
    {
    }
}

public record UserEditRequest : BaseRequest
{
    public string? Name { get; }
    public bool PromoteToTeacher { get; } = false;

    private const string TeacherPromoteCode = "kdsensei"; 
    public UserEditRequest(MessageEvent eventData) : base(eventData)
    {
        if (eventData.Message is not TextMessage textMessage)
            throw new ArgumentException("Wrong answer type. Text message is awaiting.");

        Name = textMessage.Text == TeacherPromoteCode ? null : textMessage.Text;
        PromoteToTeacher = textMessage.Text == TeacherPromoteCode;
    }
}

public record SurveyCreateRequest : BaseRequest
{
    public int? QuestionsAmount { get; }

    public SurveyCreateRequest(PostbackEvent eventData) : base(eventData)
    {
        Route route = Route.Parse(eventData.Postback.Data);
        QuestionsAmount = route.Properties.Keys.Contains(nameof(QuestionsAmount))
            ? int.Parse(route.Properties[nameof(QuestionsAmount)])
            : null;
    }
}

public record SurveyStartRequest : BaseRequest
{
    public Guid SurveyId { get; }
    public SurveyStartRequest(PostbackEvent eventData) : base(eventData)
    {
        Route route = Route.Parse(eventData.Postback.Data);
        SurveyId = route.Properties.Keys.Contains(nameof(SurveyId))
            ? Guid.Parse(route.Properties[nameof(SurveyId)])
            : throw new ArgumentException("Can't find the survey to start.");
    }
}

public record SurveyAssignRequest : BaseRequest
{
    public int SurveyEnterNumber { get; }
    public SurveyAssignRequest(MessageEvent eventData) : base(eventData)
    {
        if (eventData.Message is not TextMessage textMessage)
            throw new ArgumentException("Wrong answer type. Text message is awaiting.");

        SurveyEnterNumber = int.Parse(textMessage.Text);
    }
}
public record SurveyPollRequest : BaseRequest
{
    public Guid SurveyId { get; }
    public int QuestionId { get; }
    public string Answer { get; }
    public SurveyPollRequest(PostbackEvent eventData) : base(eventData)
    {
        Route route = Route.Parse(eventData.Postback.Data);
        SurveyId = route.Properties.Keys.Contains(nameof(SurveyId))
            ? Guid.Parse(route.Properties[nameof(SurveyId)])
            : throw new ArgumentException("Can't find the survey to start.");
        QuestionId = route.Properties.Keys.Contains(nameof(QuestionId))
            ? int.Parse(route.Properties[nameof(QuestionId)])
            : throw new ArgumentException("Can't find the question to poll.");
        Answer = route.Properties.Keys.Contains(nameof(Answer))
            ? route.Properties[nameof(Answer)]
            : throw new ArgumentException("Can't find the user's answer.");
    }
}
public record SurveyReadRequest : BaseRequest
{
    public Guid SurveyId { get; }
    public int QuestionId { get; }
    public SurveyReadRequest(PostbackEvent eventData) : base(eventData)
    {
        Route route = Route.Parse(eventData.Postback.Data);
        SurveyId = route.Properties.Keys.Contains(nameof(SurveyId))
            ? Guid.Parse(route.Properties[nameof(SurveyId)])
            : throw new ArgumentException("Can't find the survey to start.");
        QuestionId = route.Properties.Keys.Contains(nameof(QuestionId))
            ? int.Parse(route.Properties[nameof(QuestionId)])
            : throw new ArgumentException("Can't find the question to poll.");
    }
}

public record SurveyStopRequest : BaseRequest
{
    public Guid SurveyId { get; }
    public SurveyStopRequest(PostbackEvent eventData) : base(eventData)
    {
        Route route = Route.Parse(eventData.Postback.Data);
        SurveyId = route.Properties.Keys.Contains(nameof(SurveyId))
            ? Guid.Parse(route.Properties[nameof(SurveyId)])
            : throw new ArgumentException("Can't find the survey to start.");
    }
}