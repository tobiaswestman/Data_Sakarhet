using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Uppgift1Server
{
    // Vårt objekt
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }

    public class Server
    {
        public void Start()
        {
            // Starta en TCP-listener på port 4000 för alla IP-adresser.
            TcpListener listener = new TcpListener(IPAddress.Any, 4000);
            listener.Start();
            Console.WriteLine("Server started...");

            // En loop som acceptera klientanslutningar.
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected...");
                HandleClient(client);
            }
        }

        // Hantera varje klientanslutning individuellt.
        private void HandleClient(TcpClient client)
        {
            // Skapa strömmar för att läsa och skriva till klienten.
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);

            // Läs en rad data från klienten.
            string request = reader.ReadLine();
            Console.WriteLine($"Received: {request}");

            // Bearbeta förfrågan och skapa ett svar.
            string response = ProcessRequest(request);

            // Skicka svaret tillbaka till klienten.
            writer.WriteLine(response);
            writer.Flush();
            client.Close();
        }

        // deserialisera JSON-strängen till ett personobjekt.
        private string ProcessRequest(string request)
        {
            var person = JsonConvert.DeserializeObject<Person>(request);
            return $"Hello, {person.Name}! You are {person.Age} years old and live at {person.Address}.";
        }

        //Starta servern
        public static void Main()
        {
            Server server = new Server();
            server.Start();
        }
    }
}

