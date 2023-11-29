using System.Text.Json;
using AnswerCompiler.DataAccess;
using AnswerCompiler.Extensions;
using AnswerCompiler.LineApi;
using AnswerCompiler.LineApi.Models;
using AnswerCompiler.States;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AnswerCompiler;

public class Test
{
    private readonly StateMachine _stateMachine;

    public Test(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }
    
    [Function("Test")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        WebhookBody webhook = req.GetData();

        if (webhook.Events is null || webhook.Events.Count == 0)
        {
            return req.Ok();
        }

        foreach (BaseEvent baseEvent in webhook.Events)
        {
            IState currentState = await _stateMachine.GetState(baseEvent);
            IState nextState = await currentState.Promote();
            if (currentState != nextState)
                await nextState.OnEnter();
        }
        
        return req.Ok();
        
    }
}

