using Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TCPServer tCPServer = new TCPServer();
            tCPServer.Start();
            while (true)
            {
                string str = Console.ReadLine();
                tCPServer.tempClient.Send(Encoding.UTF8.GetBytes($"主动发送数据测试:{str}"));
            }
        }
    }
}
