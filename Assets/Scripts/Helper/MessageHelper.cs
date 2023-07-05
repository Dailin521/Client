using System;
using System.Text;

public class MessageHelper
{
    private static MessageHelper instance;
    public static MessageHelper Instance()
    {
        instance ??= new MessageHelper();
        return instance;

    }
    byte[] data = new byte[4096];
    int msgLength = 0;
    public void CopyToDate(byte[] buffer, int length)
    {
        Array.Copy(buffer, 0, data, msgLength, length);
        msgLength += length;
        Handle();
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
    public Action<RegisterMsgS2C> registerHandle;
    /// <summary>
    /// 注册(结果)业务
    /// </summary>
    /// <param name="msg"></param>
    private void RisterMsgHandle(byte[] msg)
    {
        var str = Encoding.UTF8.GetString(msg);
        RegisterMsgS2C msgS2C = JsonHelper.ToObject<RegisterMsgS2C>(str);
        registerHandle?.Invoke(msgS2C);
    }

    public Action<LoginMsgsS2C> loginHandle;
    /// <summary>
    /// 登录（结果）请求
    /// </summary>
    /// <param name="msg"></param>
    private void LoginMsgHandle(byte[] msg)
    {
        var str = Encoding.UTF8.GetString(msg);
        LoginMsgsS2C msgS2C = JsonHelper.ToObject<LoginMsgsS2C>(str);
        loginHandle?.Invoke(msgS2C);
    }
    /// <summary>
    /// 聊天（转发）请求
    /// </summary>
    /// <param name="msg"></param>
    private void ChatMsgHandle(byte[] msg)
    {

    }

    public void SendToServer(int id, string str)
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
        Client.Instance().Send(send_buff);
    }
    /// <summary>
    /// 发送注册消息给服务器
    /// </summary>
    public void SendRegisterMsg(string account, string email, string pwd)
    {
        RegisterMsgC2S msg = new RegisterMsgC2S();
        msg.account = account; msg.email = email; msg.password = pwd;
        var str = JsonHelper.ToJson(msg);
        SendToServer(1001, str);
    }
    /// <summary>
    /// 发送登录消息
    /// </summary>
    /// <param name="account"></param>
    /// <param name="pwd"></param>
    public void SendLoginMsg(string account, string pwd)
    {
        LoginMsgC2S msg = new LoginMsgC2S();
        msg.account = account;
        msg.password = pwd;
        var str = JsonHelper.ToJson(msg);
        SendToServer(1002, str);
    }
}

/// <summary>
/// 1001 注册业务
/// </summary> 
public class RegisterMsgC2S
{
    public string account;
    public string password;
    public string email;
}
public class RegisterMsgS2C
{
    public string account;
    public string password;
    public string email;
    public int result;//1成功，0失败
}
/// <summary>
/// 102 登录业务
/// </summary> 
public class LoginMsgC2S
{
    public string account;
    public string password;
}
public class LoginMsgsS2C
{
    public string account;
    public string password;
    public int result;//1成功，0失败
}

/// <summary>
/// 1003 聊天业务
/// </summary> 
public class ChatMsgC2S
{
    public string player;
    public string msg;
    public int type;//0世界聊天
}
//服务器转发给所有在线的客户端
public class ChatMsgS2C
{
    public string player;
    public string msg;
    public int type;//0世界聊天
}
