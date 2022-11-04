using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using NS_Datalite;

public class DataLite : MonoBehaviour
{
    private static bool inited = false;
    public static void Init()
    {
        if (!inited)
        {
            // 临时注释
            //DownLoadManager.Init();
            inited = true;
        }
    }
    public static void LoadList(string[] path, ResLoadInfo.callback fn = null, object fnPara = null, string dataRoot = "", bool preLoadABRes = false)
    {
        ResLoadInfo res = new ResLoadInfo();
        res.Start(path, fn, fnPara, dataRoot, preLoadABRes);
    }

    public static void LoadOne(string path, ResLoadInfo.callback fn = null, object fnPara = null, string dataRoot = "", bool preLoadABRes = false)
    {
        LoadList(new string[] { path }, fn, fnPara, dataRoot, preLoadABRes);
    }

}
