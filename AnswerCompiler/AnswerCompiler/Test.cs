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
    private readonly LineApiClient _client;

    public Test(StateMachine stateMachine, LineApiClient client)
    {
        _stateMachine = stateMachine;
        _client = client;
    }
    
    [Function("Test")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        WebhookBody webhook = _client.ReadRequest(req.Body);

        if (webhook.Events is null || webhook.Events.Count == 0)
        {
            return req.Ok();
        }

        foreach (BaseEvent baseEvent in webhook.Events)
        {
            IState currentState = await _stateMachine.GetState(baseEvent);
            IState nextState = await currentState.Promote();
            await nextState.OnEnter();
        }
        
        return req.Ok();
        
    }
}

