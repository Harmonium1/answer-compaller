using System.Net;
using Microsoft.EntityFrameworkCore;

namespace AnswerCompiler;

public interface IStrategy
{
    Task<HttpStatusCode> ExecuteAsync(BaseEvent baseEvent);
}

public abstract class BaseStrategy
{
    protected readonly DataContext DataContext;
    protected BaseStrategy(DataContext dataContext)
    {
        DataContext = dataContext;
    }

    protected UserEntity? GetUser(BaseEvent baseEvent)
    {
        if (baseEvent.Source is not UserSource source)
            throw new ArgumentException();

        return DataContext.Users.FirstOrDefault(user => user.LineUserId == source.UserId);
    }
}
public class RegistrationStrategy : BaseStrategy, IStrategy
{
    private readonly LineApiClient _client;

    public RegistrationStrategy(DataContext dataContext, LineApiClient client) : base(dataContext)
    {
        _client = client;
    }
    public async Task<HttpStatusCode> ExecuteAsync(BaseEvent baseEvent)
    {
        switch (baseEvent)
        {
            case FollowEvent followEvent:
                await DataContext.Users.AddAsync(new UserEntity(baseEvent.Source));
                await DataContext.SaveChangesAsync();
                TextMessage textMessage1 = new("Registration is started.");
                TextMessage textMessage2 = new("Please, enter your full Name.");
                ReplyAction replyMessage = followEvent.CreateReply(textMessage1, textMessage2);
                await _client.Reply(replyMessage);
                break;
            case MessageEvent messageEvent:
                UserEntity? user = GetUser(baseEvent);
                if (user is null)
                {
                    throw new ArgumentException("User not found.");
                }
                if (messageEvent.Message is not TextMessage message) 
                    throw new ArgumentException("Wrong message type");
                
                user.Name = message.Text;
                user.Status = Status.Standby;
                break;
                    
        }
    }
}