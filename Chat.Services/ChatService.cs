using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static Chat.ChatService;

namespace Chat.Services
{
    public class ChatService : ChatServiceBase
    {
        public override async Task ConnectToChannel(IAsyncStreamReader<MessageRequest> requestStream, IServerStreamWriter<MessageResponse> responseStream, ServerCallContext context)
        {
            while(await requestStream.MoveNext())
            {
                await responseStream.WriteAsync(new MessageResponse() { Text = requestStream.Current.Text });
            }
        }
    }
}