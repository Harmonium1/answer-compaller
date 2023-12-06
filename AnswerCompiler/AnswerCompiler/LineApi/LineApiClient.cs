using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AnswerCompiler.Configuration;
using AnswerCompiler.LineApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AnswerCompiler.LineApi;

public class LineApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private const string BaseUri = "https://api.line.me";
    
    private const string PushUri = "v2/bot/message/push";
    private const string ReplyUri= "v2/bot/message/reply";
    private const string MulticastUri = "v2/bot/message/multicast";

    public LineApiClient(
        HttpClient httpClient, 
        IOptions<BotOptions> options, 
        JsonSerializerOptions jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions;
        _httpClient.BaseAddress = new(BaseUri);
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
    
    public async Task Push(string? toUserId, bool notify, params Message[] messages)
    {
        if (toUserId is null)
        {
            throw new ArgumentException("Trying to send message, but UserId is null");
        }

        await _httpClient.PostAsJsonAsync(PushUri, new PushBody
        {
            To = toUserId,
            Messages = messages,
            NotificationDisabled = !notify
        });
    }

    public async Task Multicast(string[] toLineUserIds, bool notify, params Message[] messages)
    {
        if (toLineUserIds is null || toLineUserIds.Length == 0)
        {
            throw new ArgumentException("Trying to send message, but UserId is null");
        }
        
        await _httpClient.PostAsJsonAsync(MulticastUri, new MulticastBody
        {
            To = toLineUserIds,
            Messages = messages,
            NotificationDisabled = !notify
        });
    }
    
    public async Task Reply(string replyToken, bool notify, params Message[] messages)
    {
        await _httpClient.PostAsJsonAsync(ReplyUri, new ReplyBody
        {
            ReplyToken = replyToken,
            Messages = messages,
            NotificationDisabled = !notify
        });
    }
}