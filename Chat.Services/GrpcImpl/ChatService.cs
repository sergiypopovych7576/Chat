using Grpc.Core;
using Microsoft.Extensions.Logging;
using static Chat.ChatService;

namespace Chat.Services.GrpcImpl
{
    public class ChatService : ChatServiceBase
    {
        private readonly IChannelService _channelService;
        private readonly ILogger<IChannelService> _logger;

        public ChatService(IChannelService channelService,
            ILogger<IChannelService> logger)
        {
            _channelService = channelService;
            _logger = logger;
        }

        public override async Task Connect(IAsyncStreamReader<MessageRequest> requestStream, IServerStreamWriter<MessageResponse> responseStream, ServerCallContext context)
        {
            var clientId = Guid.NewGuid();
            _logger.LogInformation($"Connected new client with id: {clientId}");
            _channelService.RegisterClient(clientId, responseStream);

            while (await requestStream.MoveNext())
            {
                _logger.LogInformation($"Client with id: {clientId} has sent a message");
                await _channelService.SendMessage(clientId, new MessageResponse() { Text = requestStream.Current.Text, Author = clientId.ToString() });
            }

            _channelService.RemoveClient(clientId);
            _logger.LogInformation($"Client with id: {clientId} has disconnected");
        }
    }
}