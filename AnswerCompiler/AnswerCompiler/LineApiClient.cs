using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace AnswerCompiler;

public class LineApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public LineApiClient(HttpClient httpClient, IOptions<BotOptions> options, JsonSerializerOptions jsonSerializerOptions)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = jsonSerializerOptions;
        _httpClient.BaseAddress = new Uri("https://api.line.me");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.Value.LineApiKey);
    }

    public async Task<string> Reply(Action message)
    {
        string reply = message.Serialize(_jsonSerializerOptions);
        var body = new StringContent(reply, new MediaTypeHeaderValue("application/json"));
        var result = await _httpClient.PostAsync("v2/bot/message/reply", body);
        return await result.Content.ReadAsStringAsync();
    }
}