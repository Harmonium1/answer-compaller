using System.Net;
using AnswerCompiler.Controllers;
using AnswerCompiler.DataAccess;
using AnswerCompiler.Extensions;
using AnswerCompiler.LineApi;
using AnswerCompiler.LineApi.Models;
using AnswerCompiler.LineApi.Models.Events;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace AnswerCompiler;

public class Test(UserController userController, SurveyController surveyController, DataContext context, LineApiClient lineClient)
{
    [Function("Test")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        WebhookBody webhook = lineClient.ReadRequest(req.Body);

        if (webhook.Events is null || webhook.Events.Count == 0)
        {
            return Ok(req);
        }

        foreach (BaseEvent baseEvent in webhook.Events)
        {
            UserSource? userSource = baseEvent.Source as UserSource;
            UserEntity? user = await context.Users.FindAsync(userSource!.UserId);
            HttpStatusCode actionResult = baseEvent switch
            {
                FollowEvent followEvent 
                    => await userController.Create(new(followEvent)),
                MessageEvent messageEvent when user!.Status is UserStatus.ProfileCreate
                    => await userController.Edit(new(messageEvent)),
                MessageEvent messageEvent when user.Status is UserStatus.Standby
                    => await surveyController.Assign(new(messageEvent)),
                PostbackEvent postbackEvent when Route.Parse(postbackEvent.Postback.Data) is { Control: Controls.Survey, Action: Actions.Create }
                    => await surveyController.Create(new(postbackEvent)),
                PostbackEvent postbackEvent when Route.Parse(postbackEvent.Postback.Data) is { Control: Controls.Survey, Action: Actions.Ask }
                    => await surveyController.Ask(new(postbackEvent)),
                PostbackEvent postbackEvent when Route.Parse(postbackEvent.Postback.Data) is { Control: Controls.Survey, Action: Actions.Read }
                    => await surveyController.Read(new(postbackEvent)),
                PostbackEvent postbackEvent when Route.Parse(postbackEvent.Postback.Data) is { Control: Controls.Survey, Action: Actions.Stop }
                    => await surveyController.Stop(new(postbackEvent)),
                PostbackEvent postbackEvent when Route.Parse(postbackEvent.Postback.Data) is { Control: Controls.User, Action: Actions.Poll }
                    => await surveyController.Poll(new(postbackEvent)),
                null => throw new ArgumentException("Got the unknown request"),
                _ => throw new ArgumentException("Got the unknown request")
            };
        }
        
        return req.CreateResponse(HttpStatusCode.OK);
    }
    
    private static HttpResponseData Ok(HttpRequestData req) => req.CreateResponse(HttpStatusCode.OK);
}

