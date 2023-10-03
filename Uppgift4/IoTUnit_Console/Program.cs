using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.SignalR.Client;

namespace IoTUnit_Console;

class Program
{
	private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
	private static byte[] _key;  // Enhetens krypteringsnyckel

	public Program(byte[] encryptionKey)
	{
		_key = encryptionKey;
	}
	
	public static double GenerateRandomTemperature()
	{
		byte[] randomNumber = new byte[1];
		_rng.GetBytes(randomNumber);

		double percentage = randomNumber[0] / 255.0;
		return -20 + (60 * percentage);  // 60 är intervallet mellan -20 och 40
	}

	// Kryptera temperaturen med en symmetrisk krypteringsalgoritm
	public static string EncryptTemperature(double temperature)
	{
		using (Aes aes = Aes.Create())
		{
			aes.Key = _key;
			aes.IV = new byte[16];  // Initialisera IV till noll för detta exempel, men i praktiken bör du generera en ny IV för varje kryptering.

			byte[] temperatureBytes = Encoding.UTF8.GetBytes(temperature.ToString("F2"));  // Konverterar temperaturen till en sträng med två decimaler och sedan till byte-array

			using (var encryptor = aes.CreateEncryptor())
			{
				byte[] encryptedTemperature = encryptor.TransformFinalBlock(temperatureBytes, 0, temperatureBytes.Length);
				return Convert.ToBase64String(encryptedTemperature);  // Returnera som Base64-sträng för enkel transport
			}
		}
	}
	
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
			double temperature = GenerateRandomTemperature(); // Implementera denna funktion
			string encryptedTemp = EncryptTemperature(temperature); // Implementera denna funktion
			await hubConnection.SendAsync("SendTemperature", "YourDeviceId", encryptedTemp);
			await Task.Delay(5000);
		}
	}
}