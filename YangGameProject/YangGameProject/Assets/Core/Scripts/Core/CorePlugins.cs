using UnityEngine;
using Net;
using Stardom.Core.XProto;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class CorePlugins : SingletonClass<CorePlugins>
{
    // Start is called before the first frame update
    public int defaultWidth;
    public int defaultHeight;

    private bool isInit = false;

    public void Init(Action _action)
    {
        if (isInit)
        {
            Log.Error("多次调用CorePlugins.Init,检查一下代码");
            return;
        }

        isInit = true;

        ConfigService.Instance.Initialize();

        // 初始化配置
        //UI root
        UINodesManager.InitRoot();
        //Sdk初始化

        //AB加载初始化
        DataLite.Init();

        ///协议
        DataAccess.Init();
        DataAccess.InitData();

        ////记录原始分辨率
        defaultWidth = Screen.currentResolution.width;
        defaultHeight = Screen.currentResolution.height;



        //Wsise插件初始化，以及把对应xml加载完毕，该文件较大，否则不用做异步处理了
        // WwiseManager.Instance.Init(_action);
        _action();
    }


    // 退出游戏
    public void BackGameStart(string desc)
    {

        //清理所有白名单以外窗口
        // UIManager.Instance.ReleaseAllExcept();

        //数据层面清理
        DataAccess.Clear();
    }
}
