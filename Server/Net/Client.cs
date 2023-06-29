using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Net
{
    internal class Client
    {
        TcpClient client;
        public Client(TcpClient tcpClient)
        {
            client = tcpClient;
            Receive();
        }
        public async void Receive()
        {
            while (client.Connected)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int length = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                    if (length > 0)
                    {
                        Console.WriteLine($"接收数据的长度：{length}");
                        Console.WriteLine($"接收数据的内容：{Encoding.UTF8.GetString(buffer, 0, buffer.Length)}");
                    }
                    else
                    {
                        client.Close();//客户端关闭了
                    }
                }
                catch (Exception e)
                {
                    client.Close();
                    Console.WriteLine($"Receive Error:{e.Message}");
                }
            }
        }
        public async void Send(byte[] date)
        {
            try
            {
                await client.GetStream().WriteAsync(date, 0, date.Length);
                Console.WriteLine($"发送数据成功");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Send Error:{e.Message}");
            }
        }
    }
}
