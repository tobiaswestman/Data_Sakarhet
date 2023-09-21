using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace UdpServer
{
	class Program
	{
		static void Main(string[] args)
		{
			//Create UdpClient and listen to port 4000
			int port = 4000;
			UdpClient udpServer = new UdpClient(port);
			Console.WriteLine($"Server started, listening on port {port}...");

			while (true)
			{
				// Define an endpoint and receive data from client
				IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, port);
				byte[] bytesReceived = udpServer.Receive(ref clientEndPoint);

				// Convert byte-data to JSON-string
				string jsonString = Encoding.UTF8.GetString(bytesReceived);

				// Deserialize JSON to object and write out result
				var data = JsonConvert.DeserializeObject<Person>(jsonString);
				Console.WriteLine($"Received message from {clientEndPoint} - Name: {data.Name}, Age: {data.Age}");
			}
		}
	}

	public class Person
	{
		public string Name { get; set; }
		public int Age { get; set; }
	}
}
