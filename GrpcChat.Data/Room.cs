namespace GrpcChat.Data;

public class Room
{
    public List<Client> Clients { get; } = new();
}