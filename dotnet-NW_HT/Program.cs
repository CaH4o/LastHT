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
		class Request
		{
			IPEndPoint endP;
			Socket socket;

			public Request(string strAddr, int port)
			{
				endP = new IPEndPoint(IPAddress.Parse(strAddr), port);
			}

			public void Send(int iKey)
			{
				Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				try
				{
					s.Connect(endP);

					if (s.Connected)
					{
						String strSend;
						strSend = (iKey).ToString() + ". Hello!<EOF>";
						s.Send(System.Text.Encoding.ASCII.GetBytes(strSend));
						Console.WriteLine("Send: " + strSend);


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

						result = result.Replace("<EOF>", "");
						Console.WriteLine("Recived: " + result);
					}

					s.Shutdown(SocketShutdown.Both);
					s.Close();
				}
				catch (SocketException ex)
				{
					Console.WriteLine(ex.Message);
					foreach (var item in ex.StackTrace)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}

		}


		static void Main(string[] args)
		{

			Thread t1 = new Thread(() =>
			{
				Request rq = new Request("127.0.0.1", 1024);
				rq.Send(1);
			});

			Thread t2 = new Thread(() => 
			{
				Request rq = new Request("127.0.0.1", 1024);
				rq.Send(2);
			});

			Thread t3 = new Thread(() =>
			{
				Request rq = new Request("127.0.0.1", 1024);
				rq.Send(3);
			});

			t1.IsBackground = true;
			t2.IsBackground = true;
			t3.IsBackground = true;

			t1.Start();
			t2.Start();
			t3.Start();

			t1.Join();
			t2.Join();
			t3.Join();

			Console.Read();
		}
	}
}
