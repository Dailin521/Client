using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class MessageHelper
{
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
    public int result;//0成功，1失败
}
/// <summary>
/// 102 登录业务
/// </summary> 
public class LoginMsgC2S
{
    public string account;
    public string password;
}
public class LoginMsgS2C
{
    public string account;
    public string password;
    public int result;//0成功，1失败
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


