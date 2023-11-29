using System.Net;
using Microsoft.Azure.Functions.Worker.Http;

namespace AnswerCompiler.Extensions;

public static class HttpRequestDataExtensions
{
    public static HttpResponseData Ok(this HttpRequestData req) => req.CreateResponse(HttpStatusCode.OK);
}