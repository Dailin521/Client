using Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class PlayerData
    {
        static PlayerData instance = new PlayerData();
        public static PlayerData Instance { get { return instance; } }
        Dictionary<string, RegisterMsgS2C> userMsg = new Dictionary<string, RegisterMsgS2C>();
        public RegisterMsgS2C Add(RegisterMsgC2S msg)
        {
            var item = new RegisterMsgS2C();
            userMsg[msg.account] = item;//直接指定Key值进行赋值
            item.account = msg.account;
            item.password = msg.password;
            item.email = msg.email;
            item.result = 0;
            return item;
        }
        //判断是否存在账号
        public bool Contain(string account)
        {
            if (userMsg.ContainsKey(account))
            {
                Console.WriteLine("已存在账号：" + account);
                return true;
            }
            else
            {
                Console.WriteLine("不存在账号：" + account);
                return false;
            }
        }
        //维护已经登录的用户
        Dictionary<string, Client> LoginUser = new Dictionary<string, Client>();
        public void AddLoginUser(string account, Client client)
        {
            LoginUser[account] = client;
        }
        public Dictionary<string, Client> GetALLLoginUser() { return LoginUser; }
    }
}
