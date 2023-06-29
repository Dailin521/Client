using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Net
{
    internal class TCPServer
    {
        TcpListener tcpListener;
        public Client tempClient;
        /// <summary>
        /// Start the server ,Create TcpListener
        /// </summary> 
        public void Start()
        {
            try
            {
                tcpListener = TcpListener.Create(7788);
                tcpListener.Start(500);
                Console.WriteLine("TCP Server Start");
                Accept();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        /// <summary>
        /// Listen for server connections
        /// </summary>
        public async void Accept()
        {
            try
            {
                TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();//
                Console.WriteLine("客户端已连接：" + tcpClient.Client.RemoteEndPoint);
                Client client = new Client(tcpClient);
                tempClient = client;
                Accept();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Accept:{e.Message}");
                tcpListener.Stop();//停止客户端监听的连接：
            }

        }
    }
}
