using AnswerCompiler.DataAccess;
using AnswerCompiler.LineApi;
using AnswerCompiler.LineApi.Models.Events;
using Microsoft.EntityFrameworkCore;
// ReSharper disable ClassNeverInstantiated.Global

namespace AnswerCompiler.States;

public class StateMachine
{
    private readonly LineApiClient _client;
    private readonly DataContext _dataContext;

    public StateMachine(LineApiClient client, DataContext dataContext)
    {
        _client = client;
        _dataContext = dataContext;
    }

    public async Task<IState> GetState(BaseEvent incomeEvent)
    {
        if (incomeEvent.Source is not UserSource userSource)
            throw new ArgumentException("Event has wrong user source");
        UserEntity? user = await _dataContext.Users.FindAsync(userSource.UserId);
        
        IState currentState = user switch
        {
            null when incomeEvent is FollowEvent
                => new UserCreatingState(_client, _dataContext, incomeEvent),
            null => throw new ArgumentException("Got the request from unknown user"),
            _ when user.Status == UserStatus.RegisteringName
                => new UsernameRegistrationState(_client, _dataContext, incomeEvent),
            _ when user.Status == UserStatus.Standby
                => new StandbyState(_client, _dataContext, incomeEvent),
            _ when user.Status == UserStatus.SurveyRequested
                => new SurveyCreatingState(_client, _dataContext, incomeEvent),
            _ => throw new NotImplementedException()
        };

        return currentState;
    }
}