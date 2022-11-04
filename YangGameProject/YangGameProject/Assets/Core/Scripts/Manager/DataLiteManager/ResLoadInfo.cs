using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResLoadInfo
{
    public delegate void callback(ResLoadInfo res, object param);

    //原始url列表
    public string[] pathArr;
    //载入完毕触发的回调
    private callback fn;
    //回调参数
    private object fnPara;
    //文件数
    private int fileCount;
    //根目录
    public string dataRoot = "";
    //public bool success = false;
    public Dictionary<string, ResLoadNode> nodeDict = new Dictionary<string, ResLoadNode>();
    //针对单个下载
    public ResLoadNode content;
    //是否单文件
    private bool singleFile;
    // 载入完成时，延迟删除专用（因为fn是复数的，如果第一个fn里就把res 卸载了，后面的……）
    private bool delayUnload = false;
    private bool delayUnload_deepMode = false;
    // 是否载入完成
    public bool loadFinished = false;
    // 是否执行过unload
    private bool unloaded = false;
    //是否需要预加载
    public bool preLoadABRes = false;
    public void Start(string[] pathArr, callback fn = null, object fnPara = null, string dataRoot = "", bool preLoadABRes = false)
    {
        //  this.success = false;
        this.dataRoot = dataRoot;
        this.pathArr = pathArr;
        this.fn = fn;
        this.fnPara = fnPara;
        this.preLoadABRes = preLoadABRes;

        LoadAll();
    }
    //开始下载
    private void LoadAll()
    {
        this.fileCount = this.pathArr.Length;
        this.singleFile = fileCount == 1;
        for (int i = 0; i < fileCount; i++)
        {
            ResLoadNode node = new ResLoadNode();
            node.relaPath = pathArr[i];
            node.fn = OnNodeLoaded;
            node.preLoadABRes = this.preLoadABRes;
            node.isABFile = node.relaPath.IndexOf(".assetbundle") != -1;

            // 这是以前的方法，不用处理hash
            node.path = node.relaPath;//ResLib.SetRootPathAndVersion(node.relaPath, node.path, node.isABFile, dataRoot);//配置节点路径与hash等关键信息
            DownLoadManager.instance.AddNode(node);
        }
    }
    private void OnNodeLoaded(ResLoadNode node)
    {
        nodeDict[node.relaPath] = node;
        fileCount--;

        //如果只有一个文件，则main等于
        if (singleFile)
        {
            content = node;
        }

        if (fileCount <= 0)
        {
            //this.success = true;
            //Log.Info("### DataGroup Loaded ###", "ResLoadInfo");
            fn?.Invoke(this, fnPara);
            fn = null;
        }
        //完全载入完毕
        loadFinished = true;

        //是否在上面的fn里，执行过unload，有的话统一清理
        if (delayUnload)
        {
            ExecUnload(delayUnload_deepMode);
        }
    }

    //释放assets object
    public void Unload(bool unloadUsedObjects = false)
    {
        //Debug.Log("尝试unload resloadinfo  ->" + pathArr[0]);
        //还在处于载入结束的那瞬间
        if (loadFinished == false)
        {
            this.delayUnload = true;
            this.delayUnload_deepMode = unloadUsedObjects;
        }
        else
        {
            ExecUnload(unloadUsedObjects);
        }
    }

    //实际去执行
    private void ExecUnload(bool unloadUsedObjects = false)
    {
        if (this.unloaded) { return; }
        foreach (ResLoadNode node in this.nodeDict.Values)
        {
            node.Unload(unloadUsedObjects);
        }
        this.unloaded = true;
    }

}
