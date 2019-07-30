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
	class AsyncServer
	{
		IPEndPoint endP;
		Socket socket;

		public AsyncServer(string strAddr, int port)
		{
			endP = new IPEndPoint(IPAddress.Parse(strAddr), port);
		}

		static int cnt = 0;

		void MyAcceptCallbakFunction(IAsyncResult ia)
		{
			Socket socket = (Socket)ia.AsyncState;

			Thread t1 = new Thread(() => {
				int curr = ++cnt;
				Socket ns = socket.EndAccept(ia);

				byte[] buffer = new byte[1024];
				string msg = "";
				int iKey;

				msg += Encoding.ASCII.GetString(buffer, 0, ns.Receive(buffer));
				Console.WriteLine($"Request No: {curr}");
				Console.WriteLine("Recive from: " + ns.RemoteEndPoint.ToString());
				Console.WriteLine($"Recive msg: {msg}\n");

				iKey = int.Parse(msg.Substring(0, 1));
				if (iKey == 1) Thread.Sleep(1000);
				if (iKey == 2) Thread.Sleep(5000);
				if (iKey == 3) Thread.Sleep(10000);

				msg = msg.Replace("<EOF>", "");
				msg += " Frog!" + "<EOF>";

				Console.WriteLine($"Request No: {curr}"); 
				Console.WriteLine("Send to: " + ns.RemoteEndPoint.ToString());
				Console.WriteLine("Send msg: " + msg + "\n");

				byte[] sendBufer = System.Text.Encoding.ASCII.GetBytes(msg);
				ns.BeginSend(sendBufer, 0, sendBufer.Length, SocketFlags.None, new AsyncCallback(MySendCallbackFunction), ns);
			});

			t1.IsBackground = true;
			t1.Start();

			socket.BeginAccept(new AsyncCallback(MyAcceptCallbakFunction), socket);
		}

		void MySendCallbackFunction(IAsyncResult ia)
		{
			Socket ns = (Socket)ia.AsyncState;
			int n = ((Socket)ia.AsyncState).EndSend(ia);
			ns.Shutdown(SocketShutdown.Send);
			ns.Close();
		}

		public void StartServer()
		{
			if (socket != null) return;
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Bind(endP);
			socket.Listen(10);
			socket.BeginAccept(new AsyncCallback(MyAcceptCallbakFunction), socket);
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			AsyncServer server = new AsyncServer("127.0.0.1", 1024);
			server.StartServer();

			Console.Read();
		}
	}
}