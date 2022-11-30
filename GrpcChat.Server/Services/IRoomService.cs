using GrpcChat.Data;

namespace GrpcChat.Services;

public interface IRoomService
{
    int JoinRoom(Client client);
    Room GetRoom(int index);
}