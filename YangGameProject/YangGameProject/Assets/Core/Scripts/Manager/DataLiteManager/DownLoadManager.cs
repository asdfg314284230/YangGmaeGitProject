using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NS_Datalite;
using System;
public class DownLoadManager : MonoBehaviour
{
    // 载入最大并行链接
    public static int maxThread = 15;
    //是否开启模拟网络延迟
    public static bool netEmulation = false;
    //模拟的下载速度，单位为KB
    public static int netSpeed = 24;
    //是否开启日志
    public static bool showDebug = false;
    public static DownLoadManager instance = null;

    public static void Init()
    {
        if (instance == null)
        {
            // 临时注释
            //GameObject go = new GameObject("DownLoadManager");
            //go.transform.SetParent(GameInit.gameRoot);
            //instance = go.AddComponent<DownLoadManager>();
        }
    }

    // 当前队列
    private List<ResLoadNode> actArr = new List<ResLoadNode>();
    // 全部队列
    private List<ResLoadNode> nodeArr = new List<ResLoadNode>();
    //当前在载入的
    private static Dictionary<string, ResLoadNode> loadingDict = new Dictionary<string, ResLoadNode>();

    //添加一个下载节点
    public void AddNode(ResLoadNode node)
    {
        if (actArr.Count < maxThread)
        {
            ExecOne(node);
        }
        else
        {
            nodeArr.Add(node);
        }
    }
    //检查是否可以继续下载
    public void CheckQueue()
    {
        if (nodeArr.Count > 0 && actArr.Count < maxThread)
        {
            ResLoadNode node = nodeArr[0];
            ExecOne(node);
            nodeArr.RemoveAt(0);
        }
        if (showDebug && nodeArr.Count == 0 && actArr.Count == 0)
        {
            Log.Error("DownloadManager :: *** All File Loaded  , Dict Count {0}***", loadingDict.Values.Count);
        }
    }
    //开始下载一个
    private void ExecOne(ResLoadNode node)
    {
        //检查是否有重复
        if (loadingDict.ContainsKey(node.path))
        {
            //已经存在相同的下载，合并fn
            if (showDebug)
            {
                Log.Error("DownloadManager :: *** 合并重复下载 {0}***" + node.path);
            }
            loadingDict[node.path].Join(node);
            node = null;
            return;
        }
        //加到集合里
        loadingDict.Add(node.path, node);
        actArr.Add(node);
        StartCoroutine(DownLoad(node));
    }
    //
    public IEnumerator DownLoad(ResLoadNode node)
    {
        while (!Caching.ready)
        {
            yield return null;
        }
        if (showDebug)
        {
            Log.Error("DownloadManager load : {0} ", node.path);
        }
        //如果AB文件，并且开启了Cache加载模式
        if (node.isABFile)
        {
            //先从ab缓存里找找 
            if (ResABManager.CheckAB(node.path))
            {
                if (showDebug)
                {
                    Log.Error("*********  从缓存中获取AB ->{0}", node.path);
                }
                //ab管理器引用+1 ，node可以直接从里面获取，所以不用再注册了
                ResABManager.AddUseCount(node.path);
                node.success = true;
            }
            else
            {
                AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(node.path);
                yield return req;
                if (req.assetBundle != null)
                {
                    //设定ab的瞬间，也会注册到ab管理器里
                    node.AssetBundle = req.assetBundle;
                    node.success = true;
                }
                else
                {
                    Log.Error("error -> ab load fail  " + node.relaPath);
                }
            }
            //如果需要预加载assets
            if (node.preLoadABRes && node.success)
            {
                AssetBundleRequest req = node.AssetBundle.LoadAllAssetsAsync();
                yield return req;
                node.preLoadObjectArr = req.allAssets;
            }
        }
        else
        {
#pragma warning disable 0618
            node.www = new WWW(node.path);
            while (!node.www.isDone)
            {
                yield return null;
            }
            //报错
            if (node.www.error != null)
            {
                Log.Error("error -> " + node.www.error + "  " + node.relaPath + " :: " + node.path);
            }
            else
            {
                node.success = true;
            }
        }
        OnFinish(node);
    }
    //下载完成
    private void OnFinish(ResLoadNode node)
    {
        actArr.Remove(node);
        loadingDict.Remove(node.path);

        if (node.fn != null)
        {
            node.fn(node);
        }

        if (node.exFn != null)
        {
            for (int i = 0; i < node.exFn.Count; i++)
            {
                node.exFn[i](node);
            }
        }
        CheckQueue();
    }
    void OnDestroy()
    {
        StopAllCoroutines();
        actArr.Clear();
        nodeArr.Clear();
    }
}
