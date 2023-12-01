using AnswerCompiler.DataAccess;
using AnswerCompiler.LineApi;
using AnswerCompiler.LineApi.Models;
using AnswerCompiler.LineApi.Models.Events;

namespace AnswerCompiler.States;

public class UsernameRegistrationState: BaseState, IState
{
    public UsernameRegistrationState(LineApiClient apiClient, DataContext dataContext, BaseEvent baseEvent)
    :base(apiClient, dataContext, baseEvent)
    {

    }
    public async Task OnEnter()
    {
        await UserStatusTo(UserStatus.RegisteringName);
        await LinePush("Now, please, enter your name with kanji or katakana.");
    }

    public async Task<IState> Promote()
    {
        if (BaseEvent is not MessageEvent messageEvent)
            throw new ArgumentException("Try to promote state with wrong event");
        
        if (messageEvent.Message is not TextMessage textMessage)
            throw new ArgumentException("Wrong answer type. Text message is awaiting.");
        
        UserEntity user = await GetUser();
        user.Name = textMessage.Text;
        await DataContext.SaveChangesAsync();
        
        await LineReply(messageEvent.ReplyToken, "Name is registered.");

        return new StandbyState(ApiClient, DataContext, BaseEvent);
    }

    
}