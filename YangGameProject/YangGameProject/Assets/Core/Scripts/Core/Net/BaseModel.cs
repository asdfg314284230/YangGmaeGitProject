using System;
using Net;
using Google.Protobuf;

public delegate void TocHandler(object data);
public abstract class BaseModel<T>  where T : BaseModel<T>
{
    
    public BaseModel()
    {
        InitAddTocHandler();
    }
    //监听协议函数
    protected abstract void InitAddTocHandler();
    //角色成功登录以后的初始化
    public abstract void InitData();
    //角色切换需要清理数据
    public abstract void Clear();


    // 不存在网络回调
    //protected void AddTocHandler(Type type, TocHandler handler)
    //{
    //    NetManager.Instance.AddHandler(type, handler);
    //}

    //protected void SendTos(IMessage obj)
    //{
    //    NetManager.Instance.SendMessage(obj);
    //}
}
