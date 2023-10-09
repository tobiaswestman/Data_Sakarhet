using IoTUnit_Console.Encryption;
using IoTUnit_Console.Models;
using Newtonsoft.Json;
using System.Text;

class Program
{
    private static readonly HttpClient httpClient = new HttpClient();

    static async Task Main(string[] args)
    {
        var apiUrl = "https://localhost:7087/api/temperatureData/PostTemperature";

        while (true)
        {
            var temperature = GetRandomTemperature();

            var schema = new TemperatureDataSchema
            {
                Value = temperature,
                UnitId = Guid.NewGuid()
            };

            var encryptedPayload = new
            {
                Data = EncryptionModule.Encrypt(JsonConvert.SerializeObject(schema))
            };

            var content = new StringContent(JsonConvert.SerializeObject(encryptedPayload), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Sent temperature: {temperature}");
            }
            else
            {
                Console.WriteLine($"Error sending temperature: {response.StatusCode}");
            }

            await Task.Delay(TimeSpan.FromSeconds(5)); // Send every 5 seconds for this example
        }
    }

    static double GetRandomTemperature()
    {
        Random random = new Random();
        return 20 + random.NextDouble() * 10;  // Random temperature between 20 and 30
    }
}
