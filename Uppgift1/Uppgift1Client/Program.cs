using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Uppgift1Client
{
    // Vårt objekt
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }

    public class Client
    {
        public void Connect()
        {
            // Skapa en anslutning till servern på "localhost" och port 4000.
            TcpClient client = new TcpClient("localhost", 4000);
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);

            // Skapa en person och serialisera den till en JSON-sträng.
            Person person = new Person { Name = "Tobias", Age = 25, Address = "Stockholm, Sweden" };
            string request = JsonConvert.SerializeObject(person);

            // Skicka JSON-strängen till servern.
            writer.WriteLine(request);
            writer.Flush();

            // Läs svaret från servern.
            string response = reader.ReadLine();
            Console.WriteLine($"Received: {response}");

            client.Close();
        }

        // Starta klienten
        public static void Main()
        {
            Client client = new Client();
            client.Connect();
        }
    }
}