using System.Net;
using System.Text.Json;
using AnswerCompiler.LineApi.Models;
using Microsoft.Azure.Functions.Worker.Http;

namespace AnswerCompiler.Extensions;

public static class HttpRequestDataExtensions
{
    public static JsonSerializerOptions JsonSerializerOptions { get; set; } = null!;
    public static HttpResponseData Ok(this HttpRequestData req) => req.CreateResponse(HttpStatusCode.OK);

    public static WebhookBody GetData(this HttpRequestData req)
    {
        WebhookBody? lineWebhook = JsonSerializer.Deserialize<WebhookBody>(req.Body, JsonSerializerOptions);
        if (lineWebhook is not null)
            return lineWebhook;

        using var reader = new StreamReader(req.Body);
        throw new JsonException("Cant deserialize the Webhook object. String value: " + reader.ReadToEnd());
    }
}