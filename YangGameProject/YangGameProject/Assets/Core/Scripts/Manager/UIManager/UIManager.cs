using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CUI.View;

public class UIManager : SingletonClass<UIManager>
{

    public UIManager()
    {
        EventManager.AddListener(EventConstants.UI_OnFormsOpen, E_OnFormsOpen);
        EventManager.AddListener(EventConstants.UI_OnFormsClosed, E_OnFormsClosed);
    }

    //窗体容器
    private List<BaseForms> m_listBaseUI = new List<BaseForms>();//UIBase表 
    public string GetListStr()
    {
        return string.Join("\n", m_listBaseUI.ConvertAll(t => t.PrefabName));
    }

    #region 外部调用
    //跳转UI界面功能
    public T GotoUI<T>(string prefabName, int tabId, Action<BaseForms> callback) where T : BaseForms
    {
        T uiforms = OpenUI(prefabName) as T;
        return uiforms;
    }

    public void BackToOpenUI(string formName)
    {
        BaseForms forms = OpenUI(formName);
        // forms.Comeback();
    }


    #region 同步打开UI 
    public BaseForms OpenUI(string formName)
    {
        BaseForms baseform = GetForms(formName);
        if (baseform == null)
        {
            Debug.LogError("界面加载失败:" + formName);
            return null;
        }
        //初始化[初始化之后才确定窗体类型]
        InitUI(baseform, formName);
        //[1]外部特性处理
        SetUI(baseform);
        //[2]已开启窗体处理 
        CheckOpenList(baseform);
        //[3]打开界面
        baseform.Show(true);
        //baseform.gameObject.SetActive(true);
        //[4]入栈
        PushUI(baseform.UIType, baseform.PrefabName);
        return baseform;
    }

    public void PushUI(UIType uitype, string prefabName)
    {
        if (uitype == UIType.NonPush) return;
        FormsTree form = new FormsTree(prefabName, uitype);
        UIStackMgr.Instance.PushUI(form);
    }

    public void MapOpenList(Action<BaseForms> map)
    {
        for (int i = m_listBaseUI.Count - 1; i >= 0; i--)
        {
            if (m_listBaseUI[i].IsVisible)
            {
                map(m_listBaseUI[i]);
            }
        }
    }


    /// <summary>
    /// 获取UI界面
    /// </summary>
    /// <param name="formName"></param>
    /// <returns></returns>
    public BaseForms GetUIIfExist(string formName)
    {
        BaseForms baseforms = m_listBaseUI.Find((form) => form.PrefabName == formName);
        if (baseforms != null && baseforms.GoForms != null)
        {
            return baseforms;
        }
        else
        {
            m_listBaseUI.Remove(baseforms);
            return null;
        }
    }

    /// <summary>
    /// 获取UI界面
    /// </summary>
    /// <param name="formName"></param>
    /// <returns></returns>
    public bool IsUIShow(string formName)
    {
        BaseForms baseforms = GetUIIfExist(formName);
        if (baseforms != null && baseforms.IsVisible)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 释放单个界面
    /// </summary>
    /// <param name="formName"></param>
    public void ReleaseUI(string formName)
    {
        BaseForms baseforms = GetUIIfExist(formName);
        if (baseforms != null)
        {
            if (m_listBaseUI.Remove(baseforms))
            {
                GameObject.Destroy(baseforms.GoForms);
                GameObject.Destroy(baseforms);
                baseforms = null;
            }
        }
        else
        {
            Log.RedInfo("未找到窗体:" + formName);
        }
    }

    public void ReleaseAllHideUI()
    {
        for (int i = m_listBaseUI.Count - 1; i >= 0; i--)
        {
            if (!m_listBaseUI[i].IsVisible)
            {
                m_listBaseUI[i].ReleaseSelf();
                i++;//队列长度减少，索引不变
            }
        }
    }

    public void ReleaseAllExcept()
    {
        for (int i = m_listBaseUI.Count - 1; i >= 0; i--)
        {
            if (i >= m_listBaseUI.Count) continue;
            string formName = m_listBaseUI[i].PrefabName;
            if (!UIConst.WhiteList.Contains(formName))
            {
                m_listBaseUI[i].ReleaseSelf();
                i++;//队列长度减少，索引不变
            }
        }
        // if (HeaderUI.Instance != null)
        //     HeaderUI.Instance.Show(false);
        // MsgCenter.ClearToast();
    }


    public static void OpenMainUI()
    {
        UIStackMgr.Instance.Release();
        Instance.OpenUI(UIConst.MainUI);
    }
    #endregion

    #endregion

    #region 事件
    void E_OnFormsOpen(GEvent e)
    {
        BaseForms forms = e.GetData<BaseForms>();
        CheckOpenList(forms);
    }

    void E_OnFormsClosed(GEvent e)
    {

    }
    #endregion

    /// <summary>
    /// 处理已开启全屏界面
    /// </summary>
    /// <param name="forms"></param>
    private void CheckOpenList(BaseForms forms)
    {
        //打开的界面是全屏界面
        if (forms.UIType == UIType.FullScreen)
        {
            var tempList = new List<BaseForms>(m_listBaseUI);
            for (int i = 0; i < tempList.Count - 1; i++)
            {
                var tempForms = tempList[i];
                if (tempForms == null)
                    continue;
                if (!(forms as BaseFullScreen).IsHideMainUI && tempForms.PrefabName == "MainUI")
                    continue;
                if (tempForms.IsVisible && tempForms.IsHideWhenNewOpen)
                {
                    if (tempForms is BaseFullScreen)
                    {
                        tempForms.Show(false);
                    }
                    else
                    {
                        tempForms.ReleaseSelf();
                    }
                }
            }
        }
    }


    #region Tool  

    //这里只能同一时间存在一个窗体实例
    private BaseForms GetForms(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        BaseForms baseforms = GetUIIfExist(name);
        if (baseforms == null)
        {
            baseforms = LoadUI(name);
        }
        return baseforms;
    }



    #region 加载界面并初始化处理

    /// <summary>
    /// 同步加载界面,未找到缓存时会进行加载
    /// </summary>
    /// <param name="prefabName"></param>
    /// <returns></returns>
    private BaseForms LoadUI(string prefabName)
    {
        GameObject uiprefab = UGUITool.InstantiateObject(prefabName);
        if (uiprefab == null) return null;
        if (uiprefab.activeSelf)
        {
            Debug.LogError("预制体未设置为非激活状态" + uiprefab.name);
        }
        BaseForms uiBase = uiprefab.GetComponent<BaseForms>();
        if (uiBase == null)
        {
#if UNITY_EDITOR
            Debug.LogError("未绑定窗体数据--->" + uiprefab.name);
#endif
            GameObject.Destroy(uiprefab);
            return null;
        }
        uiprefab.name = prefabName;
        return uiBase;
    }

    /// <summary>
    /// 每次开启窗体时会进行初始化操作
    /// </summary>
    /// <param name="uiBase"></param>
    /// <param name="prefabName"></param>
    private void InitUI(BaseForms uiBase, string prefabName)
    {
        if (uiBase == null) return;
        //[1]初始化(每次开启窗体时会进行初始化操作) 
        uiBase.Init();
        if (uiBase.PrefabName == "" || uiBase.ParentNode == null || uiBase.gameObject == null)
        {
#if UNITY_EDITOR
            throw new Exception("窗体未成功绑定预设名或挂载节点！---" + prefabName);
#endif
        }
        //初次加载必须使用此方法
        if (!m_listBaseUI.Contains(uiBase))
        {
            uiBase.transform.SetParent(uiBase.ParentNode, false);
        }
        else
        {
            uiBase.transform.SetParent(uiBase.ParentNode);
        }
        uiBase.transform.localPosition = Vector3.zero;
        uiBase.transform.localRotation = Quaternion.identity;
        uiBase.transform.localScale = Vector3.one;

        m_listBaseUI.Remove(uiBase);
        m_listBaseUI.Add(uiBase);
    }

    /// <summary>
    /// 窗体特性处理(开启窗体时对外部影响)
    /// </summary>
    /// <param name="uiBase"></param>
    private void SetUI(BaseForms uiBase)
    {
        if (uiBase == null || uiBase.UIType != UIType.FullScreen) return;
    }

    #endregion 


    #endregion

    public void PrintVisible()
    {
        LogList((form) => form.IsVisible);
    }

    public void LogList(System.Predicate<BaseForms> filter)
    {
        for (int i = 0; i < m_listBaseUI.Count; i++)
        {
            if (filter != null && filter(m_listBaseUI[i]))
                Log.Error(i + "~~~~" + m_listBaseUI[i].PrefabName);
        }
    }
}

public enum UIType
{
    FullScreen,
    Pop,
    NonPush
}