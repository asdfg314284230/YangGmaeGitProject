using Net;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum DisType
{
    Exception,
    Disconnect,
}

public class SocketClient
{
    //private TcpClient client = null;
    //private NetworkStream outStream = null;
    //private MemoryStream memStream;
    //private BinaryReader reader;

    //private const int MAX_READ = 65535;
    //private byte[] byteBuffer = new byte[MAX_READ];

    //// Use this for initialization
    //public SocketClient()
    //{
    //}

    ///// <summary>
    ///// 注册代理
    ///// </summary>
    //public void OnRegister()
    //{
    //    memStream = new MemoryStream();
    //    reader = new BinaryReader(memStream);
    //}

    ///// <summary>
    ///// 移除代理
    ///// </summary>
    //public void OnRemove()
    //{
    //    Close();
    //    reader.Close();
    //    memStream.Close();
    //}

    ///// <summary>
    ///// 连接服务器
    ///// </summary>
    //void ConnectServer(string host, int port)
    //{
    //    if (client != null)
    //    {
    //        Close();
    //        return;
    //    }
    //    IPAddress[] address = Dns.GetHostAddresses(host);
    //    if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
    //    {
    //        Log.RedInfo("IPV6网络环境");
    //        client = new TcpClient(AddressFamily.InterNetworkV6);
    //    }
    //    else
    //    {
    //        Log.RedInfo("IPV4网络环境");
    //        //再做一次判定
    //        string ipv6host = UnityPlayerMessage.GetIPV6Host(host, port.ToString());
    //        if (!string.IsNullOrEmpty(ipv6host))
    //        {
    //            client = new TcpClient(AddressFamily.InterNetworkV6);
    //            host = ipv6host;
    //            Log.RedInfo("iOS底层判定IPV6网络环境");
    //        }
    //        else
    //        {
    //            client = new TcpClient(AddressFamily.InterNetwork);
    //        }
    //    }
    //    client.SendTimeout = 5000;
    //    client.ReceiveTimeout = 5000;
    //    client.NoDelay = true;

    //    IAsyncResult iar = client.BeginConnect(host, port, new AsyncCallback(OnConnect), null);
    //    bool success = iar.AsyncWaitHandle.WaitOne(2000);
    //    if (!success)
    //    {
    //        MsgCenter.ShowAlert("连接服务器无响应", autoCloseTime: 0);
    //        Close();
    //    }
    //}

    ///// <summary>
    ///// 连接上服务器
    ///// </summary>
    //void OnConnect(IAsyncResult asr)
    //{
    //    if (client != null)
    //    {
    //        outStream = client.GetStream();
    //        client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
    //        NetManager.Instance.OnConnect();
    //    }
    //}

    ///// <summary>
    ///// 写数据
    ///// </summary>
    //void WriteMessage(byte[] message)
    //{
    //    MemoryStream ms = null;
    //    using (ms = new MemoryStream())
    //    {
    //        ms.Position = 0;
    //        BinaryWriter writer = new BinaryWriter(ms);
    //        writer.Write(message);
    //        writer.Flush();
    //        if (client != null && client.Connected)
    //        {
    //            byte[] payload = ms.ToArray();
    //            outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
    //        }
    //        else
    //        {
    //            Log.RedInfo("client.connected----->>false");
    //        }
    //    }
    //}

    ///// <summary>
    ///// 读取消息
    ///// </summary>
    //void OnRead(IAsyncResult asr)
    //{
    //    int bytesRead = 0;
    //    try
    //    {
    //        lock (client.GetStream())
    //        {
    //            //读取字节流到缓冲区
    //            bytesRead = client.GetStream().EndRead(asr);
    //        }
    //        if (bytesRead < 1)
    //        {
    //            // bytesRead=0正常关闭连接，bytesRead<0连接错误，断线处理                
    //            OnDisconnected(DisType.Disconnect, "bytesRead < 1");
    //            return;
    //        }
    //        OnReceive(byteBuffer, bytesRead);   //分析数据包内容，抛给逻辑层
    //        lock (client.GetStream())
    //        {
    //            //分析完，再次监听服务器发过来的新消息
    //            Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
    //            client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        //PrintBytes();
    //        OnDisconnected(DisType.Exception, ex.Message);
    //    }
    //}

    ///// <summary>
    ///// 丢失链接
    ///// </summary>
    //void OnDisconnected(DisType dis, string msg)
    //{
    //    Log.Info("OnDisconnected -> " + dis.ToString() + " " + msg);
    //    Close();   //关掉客户端链接
    //    if (dis == DisType.Exception)
    //        NetManager.Instance.OnDisConnect();
    //}


    ///// <summary>
    ///// 打印字节
    ///// </summary>
    ///// <param name="bytes"></param>
    //void PrintBytes()
    //{
    //    string returnStr = string.Empty;
    //    for (int i = 0; i < byteBuffer.Length; i++)
    //    {
    //        returnStr += byteBuffer[i].ToString("X2");
    //    }
    //    Log.Error(returnStr);
    //}

    ///// <summary>
    ///// 向链接写入数据流
    ///// </summary>
    //void OnWrite(IAsyncResult r)
    //{
    //    try
    //    {
    //        outStream.EndWrite(r);
    //    }
    //    catch (Exception ex)
    //    {
    //        Log.Error("OnWrite--->>>" + ex.Message);
    //    }
    //}

    ///// <summary>
    ///// 接收到消息
    ///// </summary>
    //void OnReceive(byte[] bytes, int length)
    //{
    //    memStream.Seek(0, SeekOrigin.End);
    //    memStream.Write(bytes, 0, length);
    //    //Reset to beginning
    //    memStream.Seek(0, SeekOrigin.Begin);
    //    while (RemainingBytes() > 2)
    //    {
    //        ushort messageLen = reader.ReadUInt16();
    //        messageLen -= 2;
    //        if (RemainingBytes() >= messageLen)
    //        {
    //            MemoryStream ms = new MemoryStream();
    //            BinaryWriter writer = new BinaryWriter(ms);
    //            writer.Write(reader.ReadBytes(messageLen));
    //            ms.Seek(0, SeekOrigin.Begin);
    //            OnReceivedMessage(ms);
    //        }
    //        else
    //        {
    //            memStream.Position = memStream.Position - 2;
    //            break;
    //        }
    //    }
    //    byte[] leftover = reader.ReadBytes((int)RemainingBytes());
    //    memStream.SetLength(0);
    //    memStream.Write(leftover, 0, leftover.Length);
    //}

    ///// <summary>
    ///// 剩余的字节
    ///// </summary>
    //private long RemainingBytes()
    //{
    //    return memStream.Length - memStream.Position;
    //}

    ///// <summary>
    ///// 接收到消息
    ///// </summary>
    ///// <param name="ms"></param>
    //void OnReceivedMessage(MemoryStream ms)
    //{
    //    if (!IsConnect()) return;

    //    BinaryReader r = new BinaryReader(ms);
    //    byte[] message = r.ReadBytes((int)(ms.Length - ms.Position));
    //    ByteBuffer buffer = new ByteBuffer(message);

    //    int protoId = buffer.ReadShort();
    //    int sessionID = buffer.ReadInt();
    //    int pbDataLen = message.Length - 6;
    //    byte[] pbData = buffer.ReadBytes(pbDataLen);
    //    NetManager.Instance.DispatchProto(sessionID, protoId, pbData);
    //}


    ///// <summary>
    ///// 会话发送
    ///// </summary>
    //void SessionSend(byte[] bytes)
    //{
    //    WriteMessage(bytes);
    //}


    //public bool IsConnect()
    //{
    //    if (client != null)
    //    {
    //        return client.Connected;
    //    }
    //    return false;
    //}

    ///// <summary>
    ///// 关闭链接
    ///// </summary>
    //public void Close()
    //{
    //    if (client != null)
    //    {
    //        client.Close();
    //        client = null;
    //    }
    //}

    ///// <summary>
    ///// 发送连接请求
    ///// </summary>
    //public void SendConnect()
    //{
    //    string[] iphost = DataAccess.loginModel.DefaultServer.host.Split(':');
    //    if (iphost.Length < 2)
    //    {
    //        Log.Error(string.Format("是不是端口号没填？ {0}", DataAccess.loginModel.DefaultServer.host.ToString()));
    //        return;
    //    }
    //    ToolKit.DelayCall(0.1f, () => ConnectServer(iphost[0], int.Parse(iphost[1])));
    //}

    ///// <summary>
    ///// 发送消息
    ///// </summary>
    //public void SendMessage(ByteBuffer buffer)
    //{
    //    SessionSend(buffer.ToBytes());
    //    buffer.Close();
    //}
}
