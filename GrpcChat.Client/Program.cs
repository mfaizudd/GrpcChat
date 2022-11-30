using Grpc.Core;
using Grpc.Net.Client;
using GrpcChat;

Console.Write("Enter your name: ");
var name = Console.ReadLine() ?? "Bandeng";
var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new Chat.ChatClient(channel);
var reply = await client.JoinAsync(new JoinRequest
{
    Name = name
});
var roomId = reply.RoomId;
using var streaming = client.SendMessage(new Metadata { new Metadata.Entry("Name", name) });
var _ =  Task.Run(async () =>
{
    while (await streaming.ResponseStream.MoveNext())
    {
        var chat = streaming.ResponseStream.Current;
        Console.WriteLine($"{chat.Name}: {chat.Body}");
    }
});
await streaming.RequestStream.WriteAsync(new ChatMessage
{
    Name = name,
    RoomId = roomId,
    Body = ""
});
Console.WriteLine($"Joined chat as {name}");

void DeletePreviousLine()
{
    if (Console.CursorTop == 0) return;
    Console.SetCursorPosition(0, Console.CursorTop - 1);
    Console.Write(new string(' ', Console.WindowWidth));
    Console.SetCursorPosition(0, Console.CursorTop);
}

while (true)
{
    var line = Console.ReadLine();
    DeletePreviousLine();
    await streaming.RequestStream.WriteAsync(new ChatMessage
    {
        Name = name,
        RoomId = roomId,
        Body = line
    });
}