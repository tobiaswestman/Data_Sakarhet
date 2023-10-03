using Microsoft.AspNetCore.SignalR;

public class TemperatureHub : Hub
{
	public async Task SendTemperature(string deviceId, string encryptedTemperature)
	{
		// Dekryptera och processa temperatur här om nödvändigt
		await Clients.All.SendAsync("ReceiveTemperature", deviceId, encryptedTemperature);
	}
}
