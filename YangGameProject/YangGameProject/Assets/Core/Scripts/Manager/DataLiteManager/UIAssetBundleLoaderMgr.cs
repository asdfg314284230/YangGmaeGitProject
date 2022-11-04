using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class UIAssetBundleLoaderMgr
{
    public void ClearAB()
    {
        foreach (var item in m_abDic.Values)
        {
            item.Unload(true);
        }
        m_abDic.Clear();
    }
    /// <summary>
    /// 缓存加载的AssetBundle，防止多次加载
    /// </summary>
    private Dictionary<string, AssetBundle> m_abDic = new Dictionary<string, AssetBundle>();

    private AssetBundleManifest m_manifest;

    private string[] m_AllAssetBundles;
    public void Init()
    {
        // AssetBundle streamingAssetsAb = AssetBundle.LoadFromFile(Path.Combine(PathManager.ABFilePath(), "uiab", "uiab"));
        // m_manifest = streamingAssetsAb.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        // streamingAssetsAb.Unload(false);
        // streamingAssetsAb = null;
        // //所有AB列表
        // m_AllAssetBundles = m_manifest.GetAllAssetBundles();

        /* for (int i = 0; i < m_AllAssetBundles.Length; i++)
        {
            Log.Error(m_AllAssetBundles[i]);
        } */
    }

    private string GetRealAssetBundlesName(string uiName, string suffix)
    {
        string realName = string.Format("_{0}.{1}.assetbundle", uiName.ToLower(), suffix);
        for (int i = 0; i < m_AllAssetBundles.Length; i++)
        {
            if (m_AllAssetBundles[i].IndexOf(realName) >= 0)
            {
                return m_AllAssetBundles[i];
            }
        }
        Log.Error("UIName = " + uiName + "找不到AB");
        return "";
    }

    /// <summary>
    /// 单例
    /// </summary>
    private static UIAssetBundleLoaderMgr s_instance;
    public static UIAssetBundleLoaderMgr instance
    {
        get
        {
            if (null == s_instance)
                s_instance = new UIAssetBundleLoaderMgr();
            return s_instance;
        }
    }
    /// <summary>
    /// 加载AssetBundle
    /// </summary>
    /// <param name="abName">AssetBundle名称</param>
    /// <returns></returns>
    private AssetBundle LoadAssetBundle(string abName)
    {
        AssetBundle ab = null;
        // if (!m_abDic.ContainsKey(abName))
        // {
        //     string abResPath = Path.Combine(PathManager.ABFilePath(), "uiab", abName);
        //     ab = AssetBundle.LoadFromFile(abResPath);
        //     m_abDic[abName] = ab;
        // }
        // else
        // {
        //     ab = m_abDic[abName];
        // }
        // //加载依赖
        // string[] dependences = m_manifest.GetAllDependencies(abName);
        // int dependenceLen = dependences.Length;
        // if (dependenceLen > 0)
        // {
        //     for (int i = 0; i < dependenceLen; i++)
        //     {
        //         string dependenceAbName = dependences[i];
        //         //Log.Error(dependenceAbName);
        //         if (!m_abDic.ContainsKey(dependenceAbName))
        //         {
        //             AssetBundle dependenceAb = LoadAssetBundle(dependenceAbName);
        //             m_abDic[dependenceAbName] = dependenceAb;
        //         }
        //     }
        // }

        return ab;
    }

    /// <summary>
    /// 从AssetBundle中加载Asset
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="abName">AssetBundle名</param>
    /// <param name="assetName">Asset名</param>
    /// <returns></returns>
    public T LoadAsset<T>(string assetName, string suffix) where T : Object
    {
        string abName = GetRealAssetBundlesName(assetName, suffix);
        AssetBundle ab = LoadAssetBundle(abName);
        T t = ab.LoadAsset<T>(assetName);
        return t;
    }

    public AssetBundle GetAssetBundle(string assetName, string suffix)
    {
        string abName = GetRealAssetBundlesName(assetName, suffix);
        AssetBundle ab = LoadAssetBundle(abName);
        return ab;
    }

}
