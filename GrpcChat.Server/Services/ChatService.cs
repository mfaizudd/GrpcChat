using Grpc.Core;
using GrpcChat.Data;

namespace GrpcChat.Services
{
    public class ChatService : Chat.ChatBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<ChatService> _logger;
        public ChatService(ILogger<ChatService> logger, IRoomService roomService)
        {
            _logger = logger;
            _roomService = roomService;
        }

        public override Task<JoinReply> Join(JoinRequest request, ServerCallContext context)
        {
            var roomId = _roomService.JoinRoom(new Client
            {
                Name = request.Name
            });
            return Task.FromResult(new JoinReply
            {
                RoomId = roomId
            });
        }

        public override async Task SendMessage(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
        {
            _logger.LogInformation("New connection with id: {ConnectionId}", context.GetHttpContext().Connection.Id);
            if (!await requestStream.MoveNext())
            {
                return;
            }

            var room = _roomService.GetRoom(requestStream.Current.RoomId);
            var client = room.Clients.FirstOrDefault(c => c.Name == requestStream.Current.Name);
            if (client == null)
            {
                return;
            }

            client.Stream = responseStream;

            while (await requestStream.MoveNext())
            {
                foreach (var c in room.Clients)
                {
                    await c.Stream.WriteAsync(requestStream.Current);
                }
            }
        }
    }
}