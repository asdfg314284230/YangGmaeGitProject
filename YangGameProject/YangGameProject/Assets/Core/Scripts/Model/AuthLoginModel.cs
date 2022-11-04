using UnityEngine;
using Stardom;
using Stardom.Core.Model;
using Stardom.Core.XProto;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using System.IO;
using System.Text;
using System;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

public partial class AuthLoginModel : BaseModel<AuthLoginModel>
{

    public string AppId
    {
        get { return "20220919"; }
    }

    public string M
    {
        get { return "29790badca2f958f08ba560b61991d73"; }
    }

    public string Key
    {
        get { return "bc3ed860b5d8fe27392a5e634562a6a9"; }
    }


    protected override void InitAddTocHandler()
    {

    }


    public override void InitData()
    {

    }


    public override void Clear()
    {

    }

    public void ClearData()
    {

    }

    /// <summary>
    /// 返回MD5序列
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public string GetMD5Hash(string input)
    {
        MD5 md5Hasher = MD5.Create();
        byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }


    /// <summary>
    /// 获取服务器时间轴
    /// </summary>
    /// <returns></returns>
    public string GetServerTime(string pass)
    {
        //获取当前系统时间
        System.DateTime dt = System.DateTime.Now;
        //将系统时间转换成字符串
        string strTime = dt.ToString("yyyy-MM-dd HH:mm:ss");
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("api", "date.in"); // 固定调用登录API
        dic.Add("appid", AppId);// APPID
        dic.Add("m", M); // 软件特征码
        dic.Add("BSphpSeSsl", "6666"); // 固定
        dic.Add("date", strTime);
        dic.Add("md5", "");
        dic.Add("mutualkey", Key);// 通信Key
        dic.Add("appsafecode", "");
        dic.Add("sgin", "");
        dic.Add("icid", pass); // 激活码
        dic.Add("icpwd", "");
        var json = Get("http://116.205.162.8/AppEn.php", dic);
        var mj = JObject.Parse(json);
        return mj["response"]["date"].ToString();
    }

    /// <summary>
    /// 返回机器特征码
    /// </summary>
    /// <returns></returns>
    public string GetMachineID() { return SystemInfo.deviceUniqueIdentifier; }

    /// <summary>
    /// 解绑
    /// </summary>
    /// <param name="pass"></param>
    /// <returns></returns>
    public bool isUnlock(string pass)
    {
        //获取当前系统时间
        System.DateTime dt = System.DateTime.Now;
        //将系统时间转换成字符串
        string strTime = dt.ToString("yyyy-MM-dd HH:mm:ss");
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("api", "setcarnot.ic"); // 固定调用登录API
        dic.Add("appid", AppId);// APPID
        dic.Add("m", M); // 软件特征码
        dic.Add("BSphpSeSsl", "6666"); // 固定
        dic.Add("date", strTime);
        dic.Add("md5", "");
        dic.Add("mutualkey", Key);// 通信Key
        dic.Add("appsafecode", "");
        dic.Add("sgin", "");
        dic.Add("icid", pass); // 激活码
        dic.Add("icpwd", "");
        var json = Get("http://116.205.162.8/AppEn.php", dic);
        var mj = JObject.Parse(json);

        string str = mj["response"]["data"].ToString();
        if (str.Split('|')[0] == "01")
        {
            //Info.text = str;
            return true;
        }
        else
        {
            //Info.text = str;
            return false;
        }
    }



    /// <summary>
    /// 获取公告信息
    /// </summary>
    /// <returns></returns>
    public string isGetGongGao()
    {
        //获取当前系统时间
        System.DateTime dt = System.DateTime.Now;
        //将系统时间转换成字符串
        string strTime = dt.ToString("yyyy-MM-dd HH:mm:ss");
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("api", "gg.in"); // 固定调用登录API
        dic.Add("appid", AppId);// APPID
        dic.Add("m", M); // 软件特征码
        dic.Add("BSphpSeSsl", "6666"); // 固定
        dic.Add("date", strTime);
        dic.Add("md5", "");
        dic.Add("mutualkey", Key);// 通信Key
        dic.Add("appsafecode", "");
        dic.Add("sgin", "");
        dic.Add("icid", ""); // 激活码
        dic.Add("icpwd", "");
        var json = Get("http://116.205.162.8/AppEn.php", dic);
        var mj = JObject.Parse(json);

        string str = mj["response"]["data"].ToString();
        return str;

    }


    /// <summary>
    /// 登录方法
    /// </summary>
    public bool GetIsLogin(string pass)
    {
        string strTime1 = GetServerTime(pass);
        string md = GetMD5Hash(UnityEngine.Random.Range(0, 10000).ToString());

        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("api", "login.ic"); // 固定调用登录API
        dic.Add("appid", AppId);// APPID
        dic.Add("m", M); // 软件特征码
        dic.Add("BSphpSeSsl", "6666"); // 固定
        dic.Add("date", strTime1);
        dic.Add("md5", "");
        dic.Add("appsafecode", md);
        dic.Add("sgin", "");
        dic.Add("mutualkey", Key);// 通信Key
        dic.Add("icid", pass); // 激活码
        dic.Add("icpwd", "");
        dic.Add("key", GetMachineID());
        dic.Add("maxoror", GetMachineID());
        var json = Get("http://116.205.162.8/AppEn.php", dic);
        var mj = JObject.Parse(json);

        string str = mj["response"]["data"].ToString();
        if (str.Split('|')[0] == "01")
        {
            // 验证时间戳
            if (mj["response"]["date"].ToString() != strTime1)
            {
                EventManager.Send(EventConstants.InfoMsg, "时间异常");
                return false;
            }

            if (mj["response"]["appsafecode"].ToString() != md)
            {
                EventManager.Send(EventConstants.InfoMsg, "绑定机器码异常");
                return false;
            }

            PlayerPrefs.SetString("PassWord", pass);
            EventManager.Send(EventConstants.InfoMsg, "登录成功");
            return true;
        }
        else
        {
            EventManager.Send(EventConstants.InfoMsg, str);
            return false;
        }
    }




    public string Get(string url, Dictionary<string, string> dic)
    {
        string result = "";
        StringBuilder builder = new StringBuilder();
        builder.Append(url);
        if (dic.Count > 0)
        {
            builder.Append("?");
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
        }
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(builder.ToString());
        //添加参数
        HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
        Stream stream = resp.GetResponseStream();
        try
        {
            //获取内容
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
        }
        finally
        {
            stream.Close();
        }

        return result;

    }

}



