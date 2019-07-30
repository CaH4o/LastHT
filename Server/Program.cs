using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			Socket s = new Socket(
				AddressFamily.InterNetwork,
				SocketType.Stream,
				ProtocolType.Tcp
			);
			IPAddress ip = IPAddress.Parse("127.0.0.1");
			IPEndPoint ep = new IPEndPoint(ip, 3000);

			try
			{
				s.Bind(ep);
				s.Listen(10);

				while (true)
				{
					Socket ns = s.Accept();
					Console.WriteLine("Recive from: " + ns.RemoteEndPoint.ToString());
					byte[] buffer = new byte[1024];
					string result = "";

					while (true)
					{
						result += Encoding.ASCII.GetString(buffer, 0, ns.Receive(buffer));
						Console.WriteLine($"Recive: {result}");
						if (result.IndexOf("<EOF>") > -1)
						{
							break;
						}
					}

					result = result.Replace("<EOF>", "");
					Console.WriteLine("Send to: " + ns.RemoteEndPoint.ToString());

					result += " frog!" + "<EOF>";
					Console.WriteLine("Send: " + result + "\n");

					byte[] msg = Encoding.ASCII.GetBytes(result);
					ns.Send(msg);
					ns.Shutdown(SocketShutdown.Both);
					ns.Close();
				}
			}
			catch (SocketException ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
