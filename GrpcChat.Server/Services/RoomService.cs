using GrpcChat.Data;

namespace GrpcChat.Services;

public class RoomService : IRoomService
{
    private readonly List<Room> _rooms = new();

    public int JoinRoom(Client client)
    {
        var room = _rooms.LastOrDefault();
        if (room == null || room.Clients.Count >= 2)
        {
            room = new Room();
            _rooms.Add(room);
        }

        room.Clients.Add(client);
        return _rooms.Count - 1;
    }

    public Room GetRoom(int index) => _rooms[index];
}