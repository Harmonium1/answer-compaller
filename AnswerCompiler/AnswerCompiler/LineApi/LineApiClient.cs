using System.Net.Http.Headers;
using System.Text.Json;
using AnswerCompiler.Configuration;
using AnswerCompiler.LineApi.Models;
using Microsoft.Extensions.Options;

namespace AnswerCompiler.LineApi;

public class LineApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public LineApiClient(
        HttpClient httpClient, 
        IOptions<BotOptions> options, 
        JsonSerializerOptions jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions;
        _httpClient.BaseAddress = new("https://api.line.me");
        _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", options.Value.LineApiKey);
    }

    public WebhookBody ReadRequest(Stream stream)
    {
        var lineWebhook = JsonSerializer.Deserialize<WebhookBody>(stream, _jsonSerializerOptions);
        if (lineWebhook is not null) 
            return lineWebhook;
        
        
        using var reader = new StreamReader(stream);
        throw new JsonException("Cant deserialize the Webhook object. String value: " + reader.ReadToEnd());
    }
    
    public async Task<string> Push(string toLineUserId, bool notify, params string[] texts)
    {
        var messageObjects = texts.Select(text => new TextMessage(text)).Cast<Message>().ToArray();
        return await Push(toLineUserId, notify, messageObjects);
    }

    public async Task<string> Push(string toLineUserId, bool notify, params Message[] messages)
    {
        var model = new PushBody()
        {
            To = toLineUserId,
            Messages = messages,
            NotificationDisabled = !notify
        };
        string serializedModel = JsonSerializer.Serialize(model,_jsonSerializerOptions);
        var body = new StringContent(serializedModel, new MediaTypeHeaderValue("application/json"));
        var result = await _httpClient.PostAsync("v2/bot/message/push", body);
        return await result.Content.ReadAsStringAsync();
    }
    
    public async Task<string> Reply(string replyToken, bool notify, params string[] texts)
    {
        var messageObjects = texts.Select(text => new TextMessage(text)).Cast<Message>().ToArray();
        return await Reply(replyToken, notify, messageObjects);
    }
    public async Task<string> Reply(string replyToken, bool notify, params Message[] messages)
    {
        var model = new ReplyBody
        {
            ReplyToken = replyToken,
            Messages = messages,
            NotificationDisabled = !notify
        };
        string serializedModel = JsonSerializer.Serialize(model,_jsonSerializerOptions);
        var body = new StringContent(serializedModel, new MediaTypeHeaderValue("application/json"));
        var result = await _httpClient.PostAsync("v2/bot/message/reply", body);
        return await result.Content.ReadAsStringAsync();
    }
}