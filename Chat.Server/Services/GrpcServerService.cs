using Chat.Server.Configuration;
using Chat.Services;
using Grpc.Core;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chat.Server.Services
{
    internal class GrpcServerService : IHostedService
    {
        private readonly ILogger<GrpcServerService> _logger;
        private readonly ILogger<IChannelService> _channelLogger;
        private readonly IChannelService _channelService;
        private readonly AppConfig _appConfig;

        private Grpc.Core.Server _serverInstance;

        public GrpcServerService(ILogger<GrpcServerService> logger,
            ILogger<IChannelService> channelLogger,
            IChannelService channelService,
            IOptions<AppConfig> appConfig)
        {
            _logger = logger;
            _channelLogger = channelLogger;
            _channelService = channelService;
            _appConfig = appConfig.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting the Server");
            try
            {
                var reflectionServiceImpl = new ReflectionServiceImpl(ChatService.Descriptor, ServerReflection.Descriptor);
                _serverInstance = new Grpc.Core.Server()
                {
                    Ports = { new ServerPort(_appConfig.Host, _appConfig.Port, ServerCredentials.Insecure) },
                    Services = {
                        Chat.ChatService.BindService(new Chat.Services.GrpcImpl.ChatService(_channelService, _channelLogger)),
                        ServerReflection.BindService(reflectionServiceImpl)
                    }
                };

                _serverInstance.Start();
                _logger.LogInformation("Server has started");
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Server failed to start: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _serverInstance.ShutdownAsync();
        }
    }
}
