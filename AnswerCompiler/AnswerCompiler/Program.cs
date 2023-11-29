using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using AnswerCompiler;
using AnswerCompiler.Configuration;
using AnswerCompiler.DataAccess;
using AnswerCompiler.Extensions;
using AnswerCompiler.LineApi;
using AnswerCompiler.States;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddOptions<BotOptions>().Configure<IConfiguration>((options, configuration) =>
        {
            configuration.GetSection("BotOptions").Bind(options);
        });
        services.AddDbContext<DataContext>();
        services.AddHttpClient<LineApiClient>();
        services.AddSingleton<JsonSerializerOptions>(_ => new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = {new JsonStringEnumConverter()}
        });
        services.AddScoped<StateMachine>();
        
        HttpRequestDataExtensions.JsonSerializerOptions = services.BuildServiceProvider().GetService<JsonSerializerOptions>()!;
    })
    .Build();

host.Run();