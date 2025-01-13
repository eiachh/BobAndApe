using Godot;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class ClientSingleton
{
    public static ClientSocket Socket { get; set; }
    static ClientSingleton()
    {
        Socket = new ClientSocket();
    }
}

public partial class ClientSocket : Node
{
    private const string SERVER_IP = "127.0.0.1";  // Change this to your server's IP
    private const int SERVER_PORT = 6969;

    private static ClientWebSocket socketToServer = new();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Start connection attempt as soon as the node is ready
        GD.Print("Attempting to connect to server...");
        ClientConn().Wait();
        SendMessage("fuk u").Wait();
        DoRecieve();
    }
    public async Task ClientConn()
    {
        var serverUri = new Uri("ws://127.0.0.1:6969/ws");
        try
        {
            // Connect to the WebSocket server
            Console.WriteLine("Connecting to WebSocket server...");
            socketToServer.ConnectAsync(serverUri, CancellationToken.None).Wait();
            Console.WriteLine("Connection established!");

            // Send a message to the server
            //var message = "Hello, WebSocket server!";
            //var encodedMessage = Encoding.UTF8.GetBytes(message);
            //var buffer = new ArraySegment<byte>(encodedMessage);

            //await clientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            //Console.WriteLine("Message sent: " + message);

            // Receive a message from the server
            //var receiveBuffer = new ArraySegment<byte>(new byte[1024]);
            //var result = await clientWebSocket.ReceiveAsync(receiveBuffer, CancellationToken.None);

            //var receivedMessage = Encoding.UTF8.GetString(receiveBuffer.Array, 0, result.Count);
            //Console.WriteLine("Message received: " + receivedMessage);

        }
        catch (WebSocketException ex)
        {
            Console.WriteLine("WebSocket exception: " + ex.Message);
        }

    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
    public async Task SendMessage(string message)
    {
        var encodedMessage = Encoding.UTF8.GetBytes(message);
        var buffer = new ArraySegment<byte>(encodedMessage);

        await socketToServer.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        GD.Print($"Attemoted to send: {message}");
    }
    public async void DoRecieve()
    {
        while (true)
        {
            string message = await ReceiveMessage();
            if (message == null)
            {
                Console.WriteLine("Connection closed or error occurred.");
                break;
            }
            Console.WriteLine("Received message: " + message);
        }
    }
    public async Task<string> ReceiveMessage()
    {
        var receiveBuffer = new ArraySegment<byte>(new byte[1024]);
        var result = await socketToServer.ReceiveAsync(receiveBuffer, CancellationToken.None);
        return Encoding.UTF8.GetString(receiveBuffer.Array, 0, result.Count);
    }
}
