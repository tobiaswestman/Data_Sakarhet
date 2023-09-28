using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

Console.WriteLine("Tryck på en knapp för att starta applikationen:");
Console.ReadKey();

// Skapa en ny WebSocket-klient
using var clientSocket = new ClientWebSocket();

// Försök ansluta till servern via WebSocket
Console.WriteLine("Ansluter till servern...");
await clientSocket.ConnectAsync(new Uri("ws://localhost:5000/ws"), CancellationToken.None);

// Skapa ett objekt att skicka
var message = new MessageData
{
    Content = "Detta är ett meddelande till servern"
};

// Serialisera objektet till JSON och konvertera det till en byte-array för att skicka
Console.WriteLine("Skickar meddelande till server...");
var sendBuffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
await clientSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

// Vänta på ett meddelande från servern
Console.WriteLine("Tar emot svar från servern...");
var receiveBuffer = new byte[1024];
var receivedMessage = await clientSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

// Avkoda och deserialisera det mottagna meddelandet
var receivedTextJson = Encoding.UTF8.GetString(receiveBuffer, 0, receivedMessage.Count);
var receivedData = JsonSerializer.Deserialize<MessageData>(receivedTextJson);

// Skriv ut det mottagna meddelandet
Console.WriteLine($"Mottaget meddelande: {receivedData.Content}");
Console.ReadKey();

public class MessageData
{
    public string Content { get; set; }
}