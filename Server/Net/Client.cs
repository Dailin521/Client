using Server.Helper;
using System;
using System.Net.Sockets;
using System.Text;

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
        byte[] data = new byte[4096];
        int msgLength = 0;
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
                        Array.Copy(buffer, 0, data, msgLength, length);
                        msgLength += length;
                        Handle();
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

        private void Handle()
        {
            if (msgLength >= 8)
            {
                //包体（4）+ID(4)+包体数据
                byte[] _size = new byte[4];
                Array.Copy(data, 0, _size, 0, 4);
                int size = BitConverter.ToInt32(_size, 0);
                //本次要拿的长度
                var _length = size + 8;

                if (msgLength >= _length)
                {
                    //拿ID
                    byte[] _id = new byte[4];
                    Array.Copy(data, 4, _id, 0, 4);
                    int id = BitConverter.ToInt32(_id, 0);
                    //包体
                    byte[] _body = new byte[size];
                    Array.Copy(data, 8, _body, 0, size);
                    if (msgLength > _length)
                    {
                        for (int i = 0; i < msgLength - _length; i++)
                        {
                            data[i] = data[i + _length];
                        }
                    }
                    msgLength -= _length;
                    Console.WriteLine($"收到客户端请求：{id}");
                    switch (id)
                    {
                        case 1001://注册请求
                            RisterMsgHandle(_body);
                            break;
                        case 1002://登录业务
                            LoginMsgHandle(_body);
                            break;
                        case 1003://聊天业务
                            ChatMsgHandle(_body);
                            break;
                        case 1004:
                            break;

                        default:
                            break;
                    }


                }

            }
        }
        /// <summary>
        /// 注册请求
        /// </summary>
        /// <param name="msg"></param>
        private void RisterMsgHandle(byte[] msg)
        {
            var obj = JsonHelper.ToObject<RegisterMsgC2S>(msg);
            RegisterMsgS2C response = null;
            //判断是否已经存在
            if (PlayerData.Instance.Contain(obj.account))
            {
                response = new RegisterMsgS2C();
                response.result = 1;
            }
            else
            {
                response = PlayerData.Instance.Add(obj);
            }
            SendToClient(1001, JsonHelper.ToJson(response));
        }
        /// <summary>
        /// 登录请求
        /// </summary>
        /// <param name="msg"></param>
        private void LoginMsgHandle(byte[] msg)
        {
            //判断账号和密码是否跟缓存的一致
            var obj = JsonHelper.ToObject<LoginMsgC2S>(msg);
            LoginMsgS2C response = new LoginMsgS2C();
            if (PlayerData.Instance.Contain(obj.account))
            {
                response.result = 0;//已经存在
                response.account = obj.account;
                response.password = obj.password;
                PlayerData.Instance.AddLoginUser(obj.account, this);
            }
            else
            {
                response.result = 1;//账号或者密码错误
            }
            SendToClient(1002, JsonHelper.ToJson(response));
        }
        /// <summary>
        /// 聊天请求
        /// </summary>
        /// <param name="msg"></param>
        private void ChatMsgHandle(byte[] obj)
        {
            //转发给所有在线用户
            Console.WriteLine("转发聊天消息");
            var msg = JsonHelper.ToObject<ChatMsgC2S>(obj);
            ChatMsgS2C sendMsg = new ChatMsgS2C();
            sendMsg.msg = msg.msg;
            sendMsg.player = msg.player;
            sendMsg.type = msg.type;
            var dct = PlayerData.Instance.GetALLLoginUser();
            var json = JsonHelper.ToJson(sendMsg);
            foreach (var item in dct)
            {
                item.Value.SendToClient(1003, json);
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
        //按格式封装后 发送消息
        public void SendToClient(int id, string str)
        {
            //转换成byte[]
            var body = Encoding.UTF8.GetBytes(str);
            //包体大小（4） 包体ID（4）+包体内容
            byte[] send_buff = new byte[body.Length + 8];
            int size = body.Length;
            var _size = BitConverter.GetBytes(size);
            var _id = BitConverter.GetBytes(id);
            Array.Copy(_size, 0, send_buff, 0, 4);
            Array.Copy(_id, 0, send_buff, 4, 4);
            Array.Copy(body, 0, send_buff, 8, body.Length);
            Send(send_buff);
        }
    }
}
