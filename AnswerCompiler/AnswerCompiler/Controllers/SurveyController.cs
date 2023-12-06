using System.Net;
using AnswerCompiler.DataAccess;
using AnswerCompiler.LineApi;
using AnswerCompiler.LineApi.Models;
// ReSharper disable ClassNeverInstantiated.Global

namespace AnswerCompiler.Controllers;

public class SurveyController(LineApiClient apiClient, DataContext dataContext) : BaseController(apiClient, dataContext)
{
    public async Task<HttpStatusCode> Create(SurveyCreateRequest request)
    {
        UserEntity user = await request.GetUser(DataContext);
        if (user.Status != UserStatus.Standby)
        {
            throw new ArgumentException("User already called another action.");
        }
        if (user.Role <= UserRole.Student)
        {
            throw new ArgumentException("Students cannot create a survey.");
        }

        SurveyEntity survey = new(request.UserId, request.QuestionsAmount);
        DataContext.Surveys.Add(survey);
        user.Status = UserStatus.SurveyRunning;
        await DataContext.SaveChangesAsync();
        
        await Push(user, MessagesBuilder.SurveyStartConfirm(survey));

        return HttpStatusCode.OK;
    }
    
    public async Task<HttpStatusCode> Assign(SurveyAssignRequest request)
    {
        UserEntity user = await request.GetUser(DataContext);
        if (user.Status != UserStatus.Standby)
        {
            throw new ArgumentException("User already called another action.");
        }

        SurveyEntity? survey = DataContext.Surveys
                .Where(s => s.EnterNumber == request.SurveyEnterNumber)
                .FirstOrDefault(s => s.Closed == null);

        if (survey is null)
        {
            await Push(user, "There is no survey with this number. Please, try enter correct one.");
            return HttpStatusCode.OK;
        }

        if (survey.AppliedUserIds.Contains(user.UserId))
        {
            await Push(user, "You already assigned to this survey. Please, wait the polling begins.");
            return HttpStatusCode.OK;
        }
        survey.AppliedUserIds.Add(user.UserId);
        await DataContext.SaveChangesAsync();
        
        await Push(user, "You was assigned to this survey. Please, wait the polling begins.");

        return HttpStatusCode.OK;
    }
    
    public async Task<HttpStatusCode> Ask(SurveyStartRequest request)
    {
        UserEntity user = await request.GetUser(DataContext);
        if (user.Status != UserStatus.SurveyRunning)
        {
            throw new ArgumentException("User already called another action.");
        }

        SurveyEntity? currentSurvey = await DataContext.Surveys.FindAsync(request.SurveyId);
        if (currentSurvey is null)
        {
            throw new ArgumentException("Cant find survey to start");
        }

        TemplateMessage questionMessage = MessagesBuilder.QuestionToUsers(currentSurvey);
        var sendingUsers = currentSurvey.AppliedUserIds;

        await Multicast(sendingUsers, questionMessage);

        //TODO: поправить модель на продолжение
        TemplateMessage messageToTeacher = MessagesBuilder.SurveyStartConfirm(currentSurvey);
        await Push(user, messageToTeacher);
        
        return HttpStatusCode.OK;
    }

    public async Task<HttpStatusCode> Poll(SurveyPollRequest request)
    {
        UserEntity user = await request.GetUser(DataContext);
        SurveyEntity? currentSurvey = await DataContext.Surveys.FindAsync(request.SurveyId);
        if (currentSurvey is null)
        {
            throw new ArgumentException("Cant find survey to start");
        }

        var existedAnswer = currentSurvey.Answers
            .Where(a => a.AuthorId == user.UserId)
            .FirstOrDefault(a => a.QuestionId == request.QuestionId);

        if (existedAnswer is not null)
        {
            await Push(user,
                "You have already answer to this question. You cant change your answer, that is: " +
                existedAnswer.Value);
        }
        
        currentSurvey.Answers.Add(new(user, request.QuestionId, request.Answer));
        await DataContext.SaveChangesAsync();
        await Push(user, "Your answer was received. You have selected: " + request.Answer);
        
        return HttpStatusCode.OK;
    }
}