using Chat;
using Chat.Server.Configuration;
using Chat.Services;
using Google.Api;
using Grpc.Core;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var appConfig = config.GetRequiredSection(nameof(AppConfig)).Get<AppConfig>();


var reflectionServiceImpl = new ReflectionServiceImpl(Chat.ChatService.Descriptor, ServerReflection.Descriptor);
var server = new Server()
{
    Ports = { new ServerPort(appConfig.Host, appConfig.Port, ServerCredentials.Insecure) },
    Services = {
        Chat.ChatService.BindService(new Chat.Services.ChatService()),
        ServerReflection.BindService(reflectionServiceImpl)
    }
};

Console.WriteLine("Starting the server ...");
server.Start();
Console.ReadKey();