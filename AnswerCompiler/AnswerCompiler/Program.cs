using System.Text.Json;
using System.Text.Json.Serialization;
using AnswerCompiler.Configuration;
using AnswerCompiler.Controllers;
using AnswerCompiler.DataAccess;
using AnswerCompiler.LineApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost? host = new HostBuilder()
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
        services.AddScoped<UserController>();
        services.AddScoped<SurveyController>();
    })
    .Build();

host.Run();