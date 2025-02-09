using Godot;
using System;
using Package;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

public static class ClientSocket
{

	private static readonly ClientWebSocket socketToServer = new();
	private static bool isConnected = false;

	public static void Init(string ip, int port)
	{
        GD.Print("Attempting to connect to server...");
		var serverUri = new Uri($"ws://{ip}:{port}/ws");
		try
		{
			ConnectToServer(serverUri);
			if (socketToServer.State == WebSocketState.Open)
			{
                isConnected = true;
				GD.Print("WebSocket connection is now open.");
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Failed to connect to the server: {ex.Message}");
		}
	}

	private static void ConnectToServer(Uri serverUri)
	{
		try
		{
			socketToServer.ConnectAsync(serverUri, CancellationToken.None).Wait();
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
