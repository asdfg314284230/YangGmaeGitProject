using System;
using UnityEngine;
using System.Collections;
using NS_Datalite;
using System.Collections.Generic;

//单个文件节点，不可自行在外面自行卸载AB，一定要使用node的卸载函数
public class ResLoadNode
{
    //下载路径
    public string path;
    //相对路径
    public string relaPath;
    public bool success = false;
    public int useCount = 1;
    //
    public loadCallBack fn;
    public List<loadCallBack> exFn;
    public delegate void loadCallBack(ResLoadNode node);

    public bool isABFile = true;
    //对其他类型文件有效
#pragma warning disable 0618
    internal WWW www;
    /// <summary>
    /// 资源版本号，更新的资源与app中资源不一样
    /// </summary>
    public Hash128 version;
    //是否已经卸载过了
    public bool unloaded = false;
    //是否需要预加载
    public bool preLoadABRes = false;
    //如果有预加载的功能，则这里装着loadAllAssets的内容
    public UnityEngine.Object[] preLoadObjectArr;

    public T[] GetPreloadObject<T>() where T : UnityEngine.Object
    {
        List<T> result = new List<T>();
        for (int i = 0; i < preLoadObjectArr.Length; i++)
        {
            if (preLoadObjectArr[i] is T)
            {
                result.Add((T)preLoadObjectArr[i]);
            }
        }
        return result.ToArray();
    }

    //获取AB
    public AssetBundle AssetBundle
    {
        get
        {
            //先从注册器里拿
            AssetBundle ab = ResABManager.GetAB(path);
            if (ab == null)
            {
                if (success == false || www == null || www.error != null)
                {
                    Log.Error("*********  get AB fail  ->" + path);
                    return null;
                }
                ab = ResABManager.RegAB(path, www.assetBundle);
            }
            return ab;
        }
        set
        {
            if (ResABManager.GetAB(path) != null)
            {
                Log.Error("重复加载了AB ->" + path);
                return;
            }
            ResABManager.RegAB(path, value);
        }
    }

    //合并一个下载
    public void Join(ResLoadNode node)
    {
        if (exFn == null) { exFn = new List<loadCallBack>(); }
        exFn.Add(node.fn);
        useCount++;
    }

    public void Unload(bool unloadUsedObjects)
    {
        useCount--;
        if (useCount > 0)
        {
            return;
        }
        //反注册一个ab，如果这个ab还有其他node用，则不会销毁
        ResABManager.UnRegAB(path, unloadUsedObjects);
        //www的就不计数了，直接删除
        if (www != null)
        {
            www.Dispose();
        }
        this.unloaded = true;
    }
}
