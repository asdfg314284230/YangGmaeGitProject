using UnityEngine;
using System.Collections.Generic;
using System;
using Proto;
using System.IO;
using Google.Protobuf;
using Stardom.Core.XProto;

namespace Net
{
    public class NetManager : MonoBehaviour
    {
        //public static NetManager Instance { get; private set; }

        //private void Awake()
        //{
        //    if (Instance != null)
        //    {
        //        Log.Error("不能重复创建NetManager");
        //        return;
        //    }
        //    Instance = this;
        //    Init();
        //}

        //private Dictionary<Type, TocHandler> _handlerDic;
        //private static SocketClient _socketClient;
        //static SocketClient SocketClient
        //{
        //    get
        //    {
        //        if (_socketClient == null)
        //        {
        //            _socketClient = new SocketClient();
        //        }
        //        return _socketClient;
        //    }
        //}

        //private void Init()
        //{
        //    _handlerDic = new Dictionary<Type, TocHandler>();
        //    SocketClient.OnRegister();
        //}

        //public bool IsConnect()
        //{
        //    return SocketClient.IsConnect();
        //}

        ///// <summary>
        ///// 发送链接请求
        ///// </summary>
        //public void SendConnect()
        //{
        //    if (!IsConnect())
        //    {
        //        SocketClient.SendConnect();
        //    }
        //    else
        //    {
        //        Log.Error("已经处于连接状态");
        //    }
        //}

        ///// <summary>
        ///// 关闭网络 退出游戏的时候调用避免异常
        ///// </summary>
        //public void OnRemove()
        //{
        //    SocketClient.OnRemove();
        //    SocketClient.OnRegister();
        //}

        ///// <summary>
        ///// 发送SOCKET消息
        ///// </summary>
        //void SendMessage(ByteBuffer buffer)
        //{
        //    SocketClient.SendMessage(buffer);
        //}

        //private int SessionID;
        ///// <summary>
        ///// 发送SOCKET消息
        ///// </summary>
        //public void SendMessage(IMessage obj)
        //{
        //    if (!ProtoDic.ContainProtoType(obj.GetType()))
        //    {
        //        Log.Error(string.Format("不存在的协议类型 {0}", obj.GetType().ToString()));
        //        return;
        //    }
        //    ByteBuffer buff = new ByteBuffer();
        //    int protoId = ProtoDic.GetProtoIdByProtoType(obj.GetType());

        //    byte[] result;
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        obj.WriteTo(ms);
        //        result = ms.ToArray();
        //    }

        //    //重要：2个short 一个是length本身一个是protoId 等于4 另外一个4是SessionID
        //    UInt16 length = (UInt16)(result.Length + 4 + 4);

        //    Log.ShowPackageInfo(Log.LogPackageType.ClientToServer, string.Format("Send--->>>长度=={0}，{1}，ID == ", length.ToString(), obj.GetType().ToString()), protoId);
        //    buff.WriteShort((UInt16)length);
        //    buff.WriteShort((UInt16)protoId);

        //    //协议白名单
        //    if (ContainsProtoID(protoId))
        //    {
        //        buff.WriteInt(0);
        //        //Log.RedInfo(string.Format("++++++++++SessionID == {0},protoId == {1}", 0, protoId));
        //    }
        //    else
        //    {
        //        SessionID++;
        //        buff.WriteInt(SessionID);
        //        //Log.RedInfo(string.Format("++++++++++SessionID == {0},protoId == {1}", SessionID, protoId));
        //        MsgCenter.ShowLoadingTips(string.Format("请求协议{0}", protoId));
        //    }
        //    buff.WriteBytes(result);
        //    SendMessage(buff);
        //}

        //private bool ContainsProtoID(int protoId)
        //{
        //    foreach (var item in ConfigService.Instance.WhiteProtoIDCfgList)
        //    {
        //        if (item.Id == protoId)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        ///// <summary>
        ///// 连接 
        ///// </summary>
        //public void OnConnect()
        //{
        //    Log.Info("======连接成功======");
        //    EventManager.SendEventAsync(new GEvent(EventConstants.DATA_NetOnConnect));
        //}

        ///// <summary>
        ///// 断开连接
        ///// </summary>
        //public void OnDisConnect()
        //{
        //    Log.Info("======断开连接======");
        //    EventManager.SendEventAsync(new GEvent(EventConstants.DATA_NetOnDisConnect));
        //}

        ///// <summary>
        ///// 派发协议
        ///// </summary>
        ///// <param name="protoId"></param>
        ///// <param name="buff"></param>
        //public void DispatchProto(int sID, int protoId, byte[] buff)
        //{
        //    if (!IsConnect())
        //    {
        //        Log.Error("未处于连接中");
        //        return;
        //    }
        //    if (!ProtoDic.ContainProtoId(protoId))
        //    {
        //        Log.Error(string.Format("未知协议号,protoId == {0}", protoId.ToString()));
        //        return;
        //    }
        //    //Log.RedInfo(string.Format("----------SessionID == {0},protoId == {1}", sID, protoId));
        //    if (sID != 0)
        //    {
        //        if (sID == SessionID)
        //        {
        //            EventManager.SendEventAsync(new GEvent(EventConstants.HideLoadingTips));
        //        }
        //        else
        //        {
        //            Log.RedInfo(string.Format("当前SessionID == {0},回了一个{1},protoId == {2}", SessionID, sID, protoId));
        //        }
        //    }

        //    Type protoType = ProtoDic.GetProtoTypeByProtoId(protoId);
        //    try
        //    {
        //        Log.ShowPackageInfo(Log.LogPackageType.ServerToClient, string.Format("Receive<<<---长度=={0}，{1}，ID == ", buff.Length.ToString(), protoType.ToString()), protoId);

        //        MessageParser messageParser = ProtoDic.GetMessageParser(protoType.TypeHandle);
        //        object toc = null;
        //        //单独针对战斗包体过大的情况下用LZ4处理一下。
        //        if (protoType == typeof(Stardom.NTF_WAR_INFO))
        //        {
        //            //Log.Error("buff.Length " + buff.Length);
        //            //var newBuff = new byte[LZ4.LZ4Codec.MaximumOutputLength(buff.Length)];
        //            var newBuff = new byte[buff.Length * 6];
        //            //Log.Error("MaximumOutputLength " + newBuff.Length);
        //            var newBuffLength = LZ4.LZ4Codec.Decode(buff, 0, buff.Length, newBuff, 0, newBuff.Length);
        //            //Log.Error("newBuffLength.Length " + newBuffLength);

        //            try
        //            {
        //                Stream stream = new MemoryStream(newBuff, 0, newBuffLength);
        //                toc = messageParser.ParseFrom(stream);
        //                sEvents.Enqueue(new KeyValuePair<Type, object>(protoType, toc));
        //            }
        //            catch (Exception e)
        //            {
        //                Log.Error(e.ToString());
        //                throw;
        //            }
        //        }
        //        else
        //        {

        //            toc = messageParser.ParseFrom(buff);
        //            sEvents.Enqueue(new KeyValuePair<Type, object>(protoType, toc));
        //        }
        //    }
        //    catch
        //    {
        //        Log.Error(string.Format("协议解包错误:{0}"), protoType.ToString());
        //    }
        //}

        //static Queue<KeyValuePair<Type, object>> sEvents = new Queue<KeyValuePair<Type, object>>();
        ///// <summary>
        ///// 交给Command，这里不想关心发给谁。
        ///// </summary>
        //void FixedUpdate()
        //{
        //    if (sEvents.Count > 0)
        //    {
        //        while (sEvents.Count > 0)
        //        {
        //            KeyValuePair<Type, object> _event = sEvents.Dequeue();
        //            if (_handlerDic.ContainsKey(_event.Key))
        //            {
        //                _handlerDic[_event.Key](_event.Value);
        //            }
        //        }
        //    }
        //}

        //public void AddHandler(Type type, TocHandler handler)
        //{
        //    if (_handlerDic.ContainsKey(type))
        //    {
        //        _handlerDic[type] += handler;
        //    }
        //    else
        //    {
        //        _handlerDic.Add(type, handler);
        //    }
        //}
    }

}
