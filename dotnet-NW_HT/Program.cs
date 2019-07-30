using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace dotnet_NW_HT
{
	class Program
	{
		static void Main(string[] args)
		{
			IPAddress ip = IPAddress.Parse("127.0.0.1");
			IPEndPoint ep = new IPEndPoint(ip, 3000);
			Socket s = new Socket(
				AddressFamily.InterNetwork,
				SocketType.Stream,
				ProtocolType.Tcp
			);
			try
			{
				s.Connect(ep);
				if (s.Connected)
				{
					String strSend;

					for (int i = 0; i < 5; i++)
					{
						strSend = (i+1).ToString() + ". Hello!<EOF>";
						s.Send(System.Text.Encoding.ASCII.GetBytes(strSend));
						Console.WriteLine("Send: " + strSend);
					}

					byte[] buffer = new byte[1024];
					string result = "";

					do
					{
						try
						{
							result += System.Text.Encoding.ASCII.GetString(buffer, 0, s.Receive(buffer));
						}
						catch (Exception)
						{
							break;
						}

					} while (s.Receive(buffer) > 0);
					Console.WriteLine(result);
				}
			}
			catch (SocketException ex)
			{
				Console.WriteLine(ex.Message);
				foreach (var item in ex.StackTrace)
				{
					Console.WriteLine();
				}
				throw;
			}

			Console.Read();
		}
	}
}
