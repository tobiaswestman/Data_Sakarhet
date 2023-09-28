using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using WebSocketServer.Classes;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Aktivera användning av WebSockets i applikationen
app.UseWebSockets();

// Sätt upp "/ws" som vår WebSocket-anslutningspunkt
app.Map("/ws", HandleWebSocket);

async Task HandleWebSocket(HttpContext context)
{
    // Kontrollera om inkommande anslutning är en WebSocket-anslutning
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await Echo(context, webSocket);
    }
    else
    {
        // Svara med ett fel om det inte är en WebSocket-anslutning
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
}

async Task Echo(HttpContext context, WebSocket webSocket)
{
    var buffer = new byte[1024 * 4];
    while (true)
    {
        // Vänta på meddelanden från klienten
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Text)
        {
            // Avkoda och deserialisera det mottagna meddelandet
            var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var message = JsonSerializer.Deserialize<MessageData>(messageJson);
            message.Content = $"{message.Content}";

            // Skicka tillbaka meddelandet till klienten
            var responseJson = JsonSerializer.Serialize(message);
            var responseBuffer = Encoding.UTF8.GetBytes(responseJson);
            await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, result.EndOfMessage, CancellationToken.None);
        }
        else if (result.MessageType == WebSocketMessageType.Close)
        {
            // Avsluta loopen om klienten avslutar anslutningen
            break;
        }
    }
}

// Starta servern
app.Run();