//using IoTUnit_Console.Encryption;
//using IoTUnit_Console.Models;
//using Newtonsoft.Json;
//using System.Text;

//class Program
//{
//    private static readonly HttpClient httpClient = new HttpClient();

//    static async Task Main(string[] args)
//    {
//        var apiUrl = "https://localhost:7087/api/temperatureData/PostTemperature";

//        while (true)
//        {
//            var temperature = GetRandomTemperature();

//            var schema = new TemperatureDataSchema
//            {
//                Value = temperature,
//                UnitId = Guid.NewGuid() // TODO: Ideally, this should be the ID you get back from the server on registration, not a new Guid each time.
//            };

//            var encryptedPayload = new
//            {
//                Data = EncryptionModule.Encrypt(JsonConvert.SerializeObject(schema))
//            };

//            var content = new StringContent(JsonConvert.SerializeObject(encryptedPayload), Encoding.UTF8, "application/json");

//            var response = await httpClient.PostAsync(apiUrl, content);

//            if (response.IsSuccessStatusCode)
//            {
//                Console.WriteLine($"Sent temperature: {temperature}");
//            }
//            else
//            {
//                Console.WriteLine($"Error sending temperature: {response.StatusCode}");
//            }

//            await Task.Delay(TimeSpan.FromSeconds(5)); // Send every 5 seconds for this example
//        }
//    }

//    static double GetRandomTemperature()
//    {
//        Random random = new Random();
//        return 20 + random.NextDouble() * 10;  // Random temperature between 20 and 30
//    }
//}

using IoTUnit_Console.Encryption;
using IoTUnit_Console.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;


class Program
{
    private static readonly HttpClient httpClient = new HttpClient();
    private static string _jwtToken;

    static async Task Main(string[] args)
    {
        await Task.Delay(TimeSpan.FromSeconds(3));  // Give server time to start
        await RegisterUnit();

        var apiUrl = "https://localhost:7087/api/temperatureData/PostTemperature";

        while (true)
        {
            // Generate and prepare the temperature payload for sending.
            var temperature = GetRandomTemperature();

            var schema = new TemperaturePayloadSchema
            {
                Value = temperature
            };

            var jsonPayload = JsonConvert.SerializeObject(schema);
            var encryptedPayload = EncryptionModule.Encrypt(jsonPayload);
            
            var content = new StringContent($"\"{encryptedPayload}\"", Encoding.UTF8, "application/json");

            // Add JWT to the request header for authentication.
            if (!string.IsNullOrEmpty(_jwtToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwtToken);
            }

            // Send the temperature data to the server.
            var response = await httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Sent temperature: {temperature}");
            }
            else
            {
                Console.WriteLine($"Error sending temperature: {response.StatusCode}");
            }

            await Task.Delay(TimeSpan.FromSeconds(5));  // Send every 5 seconds
        }
    }

    // Registers the unit with the server. 
    static async Task RegisterUnit()
    {
        var registrationUrl = "https://localhost:7087/api/unit/register";

        // sends empty request and recieves jwt token in response
        var content = new StringContent("", Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(registrationUrl, content);

        if (response.IsSuccessStatusCode)
        {
            _jwtToken = await response.Content.ReadAsStringAsync();
        }
        else
        {
            Console.WriteLine($"Error registering unit: {response.StatusCode}");
        }
    }

    // Generates a random temperature between 20 and 30.
    static double GetRandomTemperature()
    {
        Random random = new Random();
        return 20 + random.NextDouble() * 10;  // Random temperature between 20 and 30
    }
}

