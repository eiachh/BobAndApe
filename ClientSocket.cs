using Godot;
using System;
using Package;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

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
	private const string ServerIP = "84.3.90.43";
	private const int ServerPort = 7070;

	private static readonly ClientWebSocket socketToServer = new();
	private bool isLogged = false;

	public async override void _Ready()
	{
		GD.Print("Attempting to connect to server...");
		var serverUri = new Uri($"ws://{ServerIP}:{ServerPort}/ws");
		try
		{
			await ConnectToServer(serverUri);
			if (socketToServer.State == WebSocketState.Open)
			{
				isLogged = true;
				GD.Print("WebSocket connection is now open.");
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Failed to connect to the server: {ex.Message}");
		}
	}

	public async Task ConnectToServer(Uri serverUri)
	{
		try
		{
			await socketToServer.ConnectAsync(serverUri, CancellationToken.None);
			if (socketToServer.State == WebSocketState.Open)
			{
				GD.Print("WebSocket connection is now open.");
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Error during connection: {ex.Message}");
			throw;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}


	public async static Task SendMessage<T>(Package<T> package)
	{
		if (socketToServer.State != WebSocketState.Open)
		{
			return;
		}

		string message = JsonSerializer.Serialize(package);
		var encodedMessage = Encoding.UTF8.GetBytes(message);
		var buffer = new ArraySegment<byte>(encodedMessage);

		try
		{
			await socketToServer.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
			GD.Print($"Message sent: {message}");
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Error sending message: {ex.Message}");
		}
	}

	public async static Task<object> ReceiveMessage()
	{

		var receiveBuffer = new ArraySegment<byte>(new byte[1024]);
		var result = await socketToServer.ReceiveAsync(receiveBuffer, CancellationToken.None);
		var jsonString = Encoding.UTF8.GetString(receiveBuffer.Array, 0, result.Count);

		using var jsonDoc = JsonDocument.Parse(jsonString);
		string? packageName = jsonDoc.RootElement.GetProperty("name").GetString();

		if (packageName == null || !PackageTypeMap.packageTypeMap.TryGetValue(packageName, out Type? packageType))
		{
			throw new InvalidOperationException($"Unknown package type: {packageName}");
		}

		object deserializedPackage = JsonSerializer.Deserialize(jsonString, packageType);
		return deserializedPackage;
	}
}
