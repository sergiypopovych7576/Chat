using Grpc.Core;

namespace Chat.Services
{
    public interface IChannelService
    {
        void RegisterClient(Guid clientId, IServerStreamWriter<MessageResponse> responseStream);
        void RemoveClient(Guid clientId);
        Task SendMessage(Guid clientId, MessageResponse response);
    }

    public class ChannelService : IChannelService
    {
        private readonly Dictionary<Guid, IServerStreamWriter<MessageResponse>> _streams
            = new Dictionary<Guid, IServerStreamWriter<MessageResponse>>();

        public void RegisterClient(Guid clientId, IServerStreamWriter<MessageResponse> responseStream)
        {
            _streams.Add(clientId, responseStream);
        }

        public void RemoveClient(Guid clientId)
        {
            _streams.Remove(clientId);
        }

        public async Task SendMessage(Guid clientId, MessageResponse response)
        {
            foreach (var stream in _streams.Values)
            {
                await stream.WriteAsync(response);
            }
        }
    }
}
