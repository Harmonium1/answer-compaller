namespace AnswerCompiler;

public class UserEntity
{
    private UserEntity() { }
    public UserEntity(BaseSource? source)
    {
        if (source is not UserSource userSource)
            throw new ArgumentException();

        LineUserId = userSource.UserId;
        Status = Status.RegisteringName;
    }

    public async Task<Action> Execute(BaseEvent baseEvent)
    {
        switch (Status)
        {
            case Status.Creating:
                await DataContext.Users.AddAsync(new UserEntity(baseEvent.Source));
                await DataContext.SaveChangesAsync();
                new RegistrationState().OnEnter(baseEvent, this);
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

    public string LineUserId { get; set; } = null!;
    public string? Name { get; set; }
    public Status Status { get; set; }
}

public interface IState {}
class RegistrationState: IState
{
    private readonly LineApiClient _apiClient;

    public RegistrationState(LineApiClient apiClient)
    {
        _apiClient = apiClient;
    }
    public async Task OnEnter(FollowEvent followEvent, UserEntity userEntity)
    {
        userEntity.Status = Status.RegisteringName;
        
        TextMessage textMessage1 = new("Registration is started.");
        TextMessage textMessage2 = new("Please, enter your full Name.");
        ReplyAction replyMessage = followEvent.CreateReply(textMessage1, textMessage2);
        await _apiClient.Reply(replyMessage);
    }
    public async Task<IState> OnOut(MessageEvent messageEvent, UserEntity userEntity)
    {
        userEntity.Status = Status.RegisteringName;
        
        TextMessage textMessage1 = new("Registration is started.");
        TextMessage textMessage2 = new("Please, enter your full Name.");
        ReplyAction replyMessage = messageEvent.CreateReply(textMessage1, textMessage2);
        await _apiClient.Reply(replyMessage);

        return new StandbyState();
    }
}

class StandbyState: IState
{
    
}