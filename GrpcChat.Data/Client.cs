using Grpc.Core;

namespace GrpcChat.Data;

public class Client
{
    public string Name { get; set; }
    public IServerStreamWriter<ChatMessage> Stream { get; set; }
}