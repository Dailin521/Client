using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client
{
    // Start is called before the first frame update
    private static Client _instance;
    public static Client Instance()
    {
        if (_instance == null)
        {
            _instance = new();
        }
        return _instance;
    }
    TcpClient client;
    public void Start()
    {
        client = new TcpClient();
        Connect();
    }
    public async void Connect()
    {
        try
        {
            await client.ConnectAsync("127.0.0.1", 7788);
            Debug.Log("连接成功");
            Receive();
        }
        catch (System.Exception e)
        {
            Debug.Log($"Connect Error:{e.Message}");
        }
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
                    Debug.Log($"数据接收长度:{length}");
                    Debug.Log($"数据接收内容:{Encoding.UTF8.GetString(buffer, 0, buffer.Length)}");
                }
                else
                {
                    client.Close();
                }

            }
            catch (System.Exception e)
            {
                Debug.Log($"Receive Error:{e.Message}");
            }
        }
    }
    public async void Send(byte[] data)
    {
        try
        {
            await client.GetStream().WriteAsync(data, 0, data.Length);
            Debug.Log($"发送数据内容:{Encoding.UTF8.GetString(data, 0, data.Length)}");
            Debug.Log("发送成功");
        }
        catch (System.Exception e)
        {
            Debug.Log($"Send Error:{e.Message}");

        }
    }


}
