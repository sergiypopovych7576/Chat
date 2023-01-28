using Chat.Server.Configuration;
using Chat.Server.Services;
using Chat.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

await Host.CreateDefaultBuilder(args)
    .UseConsoleLifetime()
    .ConfigureLogging(logging =>
    {
        logging.AddSimpleConsole(options =>
        {
            options.IncludeScopes = false;
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        });
    })
    .ConfigureServices((context, services) => ConfigureServices(context.Configuration, services))
    .ConfigureAppConfiguration(app =>
    {
        app.AddJsonFile("appsettings.json", optional: false);
    })
    .RunConsoleAsync();

static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
{
    services.AddHostedService<GrpcServerService>();

    services.AddSingleton<IChannelService, ChannelService>();

    services.Configure<AppConfig>(configuration.GetSection("AppConfig"));
}