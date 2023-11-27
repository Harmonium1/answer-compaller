using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace AnswerCompiler;

public class Test
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly ILogger<Test> _logger;
    private readonly LineApiClient _client;
    private readonly DataContext _dataContext;

    public Test(JsonSerializerOptions jsonSerializerOptions, ILogger<Test> logger, LineApiClient client, DataContext dataContext)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
        _logger = logger;
        _client = client;
        _dataContext = dataContext;
    }
    
    [Function("Test")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        FunctionContext executionContext)
    {
        var webhook = CreateWebhook(req);

        if (webhook.Events is null || !webhook.Events.Any())
        {
            return req.CreateResponse(HttpStatusCode.OK);
        }

        foreach (var baseEvent in webhook.Events)
        {
            if (baseEvent.Source is not UserSource source)
                throw new ArgumentException();
            switch (baseEvent)
            {
                case FollowEvent followEvent:
                    await _dataContext.Users.AddAsync(new UserEntity(){ UserId = 1, LineUserId = source.UserId});
                    await _dataContext.SaveChangesAsync();
                    TextMessage textMessage = new("Hello from Oleg with Love!");
                    ReplyAction replyMessage = followEvent.CreateReply(textMessage);
                    string result = await _client.Reply(replyMessage);
                    break;
                case MessageEvent messageEvent:
                    UserEntity user = await _dataContext.Users.FirstAsync(user => user.LineUserId == source.UserId);
                    if (messageEvent.Message is not TextMessage message) 
                        throw new ArgumentException();
                    user.Name = message.Text;
                    break;
                    
            }
        }
        
        var response = req.CreateResponse(HttpStatusCode.OK);
        return response;
        
    }

    private LineWebhook CreateWebhook(HttpRequestData req)
    {
        var lineWebhook = LineWebhook.Deserialize(req.Body, _jsonSerializerOptions);
        if (lineWebhook is null)
        {
            using StreamReader reader = new StreamReader(req.Body);
            _logger.LogError("Не получилось десериализовать вебхук. Текст: {0}", reader.ReadToEnd());
        }

        return lineWebhook!;
    }
}