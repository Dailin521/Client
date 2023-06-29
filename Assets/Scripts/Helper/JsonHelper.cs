using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class JsonHelper
{
    public static string ToJson(object x)
    {
        return JsonMapper.ToJson(x);
    }
    public static T ToObject<T>(string x)
    {
        return JsonMapper.ToObject<T>(x);
    }
    public static T ToObject<T>(byte[] bytes)
    {
        return ToObject<T>(Encoding.UTF8.GetString(bytes, 0, bytes.Length));
    }
}

