using System.Net;
using AnswerCompiler.DataAccess;
using AnswerCompiler.LineApi;
// ReSharper disable ClassNeverInstantiated.Global

namespace AnswerCompiler.Controllers;

public class UserController(LineApiClient apiClient, DataContext dataContext) : BaseController(apiClient, dataContext)
{
    public async Task<HttpStatusCode> Create(UserCreateRequest request)
    {
        DataContext.Users.Add(new(request.UserId));
        await DataContext.SaveChangesAsync();
        await Push(request.UserId, 
            "User has been registered.", 
            "Now, please, enter your name with kanji or katakana.");

        return HttpStatusCode.OK;
    }
    
    public async Task<HttpStatusCode> Edit(UserEditRequest request)
    {
        UserEntity user = await request.GetUser(DataContext);

        if (!string.IsNullOrEmpty(request.Name))
        {
            user.Name = request.Name;
            user.Status = UserStatus.Standby;
            await DataContext.SaveChangesAsync();
            await Push(user, "Name is registered.", "Please, enter a survey number.");
        }
        else if (request.PromoteToTeacher)
        {
            user.Role = UserRole.Teacher;
            await DataContext.SaveChangesAsync();
            await Push(user, "Now you have a teacher role.");
            await Push(user, MessagesBuilder.SurveyCreate());
        }
        else
        {
            await Push(user, "Unknown command.");
        }

        return HttpStatusCode.OK;;
    }
}