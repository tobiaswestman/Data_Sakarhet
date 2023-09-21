using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace UdpClientApp
{
	class Program
	{
		static void Main(string[] args)
		{
			// Create UdpClient and connect to set IP and port
			int port = 4000;
			UdpClient udpClient = new UdpClient();
			udpClient.Connect("127.0.0.1", port);

			// Let user input name and age, parse age to int
			Console.Write("Enter name: ");
			string name = Console.ReadLine();

			Console.Write("Enter age: ");
			int age;
			Int32.TryParse(Console.ReadLine(), out age);

			// Create person object with user input
			var dataToSend = new Person() { Name = name, Age = age };

			// Serialize object to JSON-string
			string jsonString = JsonConvert.SerializeObject(dataToSend);

			//Convert JSON-string to byte-data
			byte[] bytesToSend = Encoding.UTF8.GetBytes(jsonString);

			// Send byte-data and notify user
			udpClient.Send(bytesToSend, bytesToSend.Length);
			Console.WriteLine("Data sent to server!");
		}
	}

	public class Person
	{
		public string Name { get; set; }
		public int Age { get; set; }
	}
}
