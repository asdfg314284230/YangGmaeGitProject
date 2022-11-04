using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
using Ilyfairy.LoliSocket;
using WebSocketSharp;
using System.Threading;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Stardom.Core.XProto;

public class MessageHandle : MonoBehaviour
{
    int mId = 0;
    WebSocket socketIo;
    List<ActionMessage> dic = new List<ActionMessage>();



    /// <summary>
    /// 监听弹幕回调函数
    /// </summary>
    public event Action<MessageData> OnDanmuCallBack;

    /// <summary>
    /// 监听礼物回调函数
    /// </summary>
    public event Action<MessageData> OnGiftCallBack;


    List<MessageData> messageDatas = new List<MessageData>();
    List<MessageData> giftDatas = new List<MessageData>();


    void Awake()
    {
        OpenSocket(null);
    }


    private void OnDestroy()
    {
        CloseSocket(null);
    }





    void Update()
    {
        if (messageDatas.Count != 0)
        {
            for (int i = 0; i < messageDatas.Count; i++)
            {
                OnDanmuCallBack?.Invoke(messageDatas[i]);
            }

            messageDatas.Clear();
        }


        if (giftDatas.Count != 0)
        {
            for (int i = 0; i < giftDatas.Count; i++)
            {
                OnGiftCallBack?.Invoke(giftDatas[i]);
            }

            giftDatas.Clear();
        }
    }




    void OpenSocket(GEvent e)
    {
        socketIo = new WebSocket("ws://127.0.0.1:8328");
        socketIo.OnMessage += ((object o, MessageEventArgs e) =>
        {
            //Debug.Log("OnMessage :" + e.Data);

            try
            {
                MessageData json1 = JsonConvert.DeserializeObject<MessageData>(e.Data);
                //ActionMessage a = new ActionMessage();
                switch (json1.method)
                {
                    case "MemberMessage":
                        //a.sendType = MessageTypeLive.Join;
                        //a.str = string.Format("0\0{0}\0{1}\0{2}", json1.nickName, json1.content, json1.imgURL);
                        //Debug.Log(json1.nickName + "加入:" + json1.content);
                        break;
                    case "GiftMessage":
                        giftDatas.Add(json1);
                        break;
                    case "LikeMessage":
                        break;
                    case "ChatMessage":
                        messageDatas.Add(json1);
                        break;
                }

                //dic.Add(a);

            }
            catch (Exception es)
            {
                Debug.Log(es.Message);
                throw;
            }



        });
        socketIo.OnError += (sender, e) =>
        {
            Debug.Log("socket Error: " + e.Message.ToString());
        };
        socketIo.Connect();



    }

    public bool IsNumberic(string str)
    {
        double vsNum;
        bool isNum;
        isNum = double.TryParse(str, System.Globalization.NumberStyles.Float,
            System.Globalization.NumberFormatInfo.InvariantInfo, out vsNum);

        return isNum;

    }

    void CloseSocket(GEvent e)
    {
        if (socketIo != null)
        {
            socketIo.Dispose();
            socketIo.Close();
        }
    }


    public (MessageTypeLive type, string str) GetMessageType(string input)
    {
        //Debug.Log("================"+input);
        if (input.StartsWith("msg"))
        {
            return (MessageTypeLive.Message, input.Remove(0, 4));
        }
        else if (input.StartsWith("gift"))
        {
            return (MessageTypeLive.Gift, input.Remove(0, 5));
        }
        else if (input.StartsWith("join"))
        {
            return (MessageTypeLive.Join, input.Remove(0, 5));
        }
        //else if (input.StartsWith("rep"))
        //{
        //    //return (MessageTypeLive.GiftRep, input.Remove(0, 4));
        //}
        else
        {
            return (MessageTypeLive.None, null);
        }
    }


}


public class ActionMessage
{
    public MessageTypeLive sendType;
    // public DanMuData data;
    public string str;
}

//如果好用，请收藏地址，帮忙分享。
public class MessageData
{
    /// <summary>
    /// 
    /// </summary>
    public string method { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string imgURL { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string nickName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string secUid { get; set; }
    /// <summary>
    /// 进入直播间
    /// </summary>
    public string content { get; set; }
    public string Count { get; set; }
}



