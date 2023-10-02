using System;
using Microsoft.AspNetCore.SignalR.Client;

class Program
{
	static async Task Main(string[] args)
	{
		var hubConnection = new HubConnectionBuilder()
			.WithUrl("https://your-web-api-url/tempHub", options => {
				options.Headers.Add("Authorization", "Bearer YOUR_TOKEN_HERE"); // Om du har autentisering
			})
			.Build();

		await hubConnection.StartAsync();

		while (true)
		{
			string temperature = GenerateRandomTemperature(); // Implementera denna funktion
			string encryptedTemp = EncryptTemperature(temperature); // Implementera denna funktion
			await hubConnection.SendAsync("SendTemperature", "YourDeviceId", encryptedTemp);
			await Task.Delay(5000);
		}
	}
}
