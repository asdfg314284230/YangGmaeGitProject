using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 控件路径管理器
/// 控件名->全路径
/// </summary>
public class WidgetPathManager
{
    #region 变量

    static private Dictionary<string, Dictionary<string, string>> mapWidgetPath
        = new Dictionary<string, Dictionary<string, string>>();

    #endregion

    #region 解析控件字典

    #region 临时变量

    static private Transform m_myTransform;
    static private Dictionary<string, string> mWidgetToFullName;
    static private bool mNameUnique = false;//控件名是否要求唯一

    #endregion

    #region 外部调用

    /// <summary>
    /// 递归遍历控件，填充控件字典
    /// </summary>
    /// <param name="rootTransform"></param>
    static public Dictionary<string, string> FillFullNameData(string name, Transform rootTransform, bool name_unique)
    {
        if (mapWidgetPath.ContainsKey(name))
        {
            if (mapWidgetPath[name].Count != 0)
            {
                return mapWidgetPath[name];
            }
            else
            {
                Log.Info("部件清理异常");
            }
        }


        m_myTransform = rootTransform;
        mNameUnique = name_unique;
        mWidgetToFullName = new Dictionary<string, string>();

        AddWigetToFullNameData(rootTransform.name, rootTransform.name);
        ToFillFullNameData(rootTransform);

        if (!string.IsNullOrEmpty(name))
            mapWidgetPath[name] = mWidgetToFullName;
        else
            Log.Error("填充路径错误");

        return mWidgetToFullName;
    }

    #endregion

    #region 内部方法

    /// <summary>
    /// 递归遍历控件，填充控件字典
    /// </summary>
    /// <param name="rootTransform"></param>
    static private void ToFillFullNameData(Transform rootTransform)
    {
        for (int i = 0; i < rootTransform.childCount; ++i)
        {
            AddWigetToFullNameData(rootTransform.GetChild(i).name, GetFullName(rootTransform.GetChild(i)));
            ToFillFullNameData(rootTransform.GetChild(i));
        }
    }

    /// <summary>
    /// 填充控件字典
    /// </summary>
    /// <param name="widgetName"></param>
    /// <param name="fullName"></param>
    static private void AddWigetToFullNameData(string widgetName, string fullName)
    {
        if (mNameUnique)
        {
            if (mWidgetToFullName.ContainsKey(widgetName))
                Log.Error(string.Format("控件名唯一冲突{0}", widgetName));
            else
                mWidgetToFullName.Add(widgetName, fullName);
        }
        else
        {
            mWidgetToFullName[widgetName] = fullName;
        }
    }

    /// <summary>
    /// 获取该控件的完整路径名
    /// </summary>
    /// <param name="currentTran"></param>
    /// <returns></returns>
    static private string GetFullName(Transform currentTran)
    {
        string fullName = string.Empty;
        while (currentTran != m_myTransform)
        {
            fullName = string.Concat(currentTran.name, fullName);
            if (currentTran.parent != m_myTransform)
                fullName = string.Concat("/", fullName);

            currentTran = currentTran.parent;
        }
        return fullName;
    }

    public static void RemoveWigetDic(string widgetName)
    {
        if (mapWidgetPath.ContainsKey(widgetName))
        {
            mapWidgetPath.Remove(widgetName);
        }

    }


    public static string Print()
    {
        string ret = "";

        foreach (var kv in mapWidgetPath)
        {
            ret += kv.Key + "\n";
            foreach (var kv2 in kv.Value)
            {
                ret += "    " + kv2.Key + " " + kv2.Value + "\n";
            }

            ret += "\n";
        }

        return ret;
    }
    #endregion

    #endregion
}