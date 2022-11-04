using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CUI.UI;
using UnityEngine.UI;

public class UIBaseBehaviour : BaseEventListener
{
    private RectTransform _rectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    #region 控件相关  
    Dictionary<string, CustomButton> m_Btns = new Dictionary<string, CustomButton>();
    protected CustomButton GetBtn(string widget)
    {
        if (m_Btns.ContainsKey(widget))
        {
            return m_Btns[widget];
        }
        else
        {
            CustomButton btn = null;
            if (TryGetCompRef(widget, ref btn))
            {
                m_Btns.Add(widget, btn);
                return btn;
            }
            else
            {
                throw new System.Exception("未找到组件引用" + widget);
            }
        }
    }


    protected bool TryGetCompRef<T>(string path, ref T comp) where T : Component
    {
        ValidateWidget();
        return m_Widget.TryGetCompRef(path, ref comp);
    }

    //通过控件名查找组件
    protected T FindComponent<T>(string transformName) where T : Component
    {
        ValidateWidget();
        return m_Widget.GetComp<T>(transformName);
    }


    protected WidgetData m_Widget;
    protected Transform FindTransform(string transformName)
    {
        ValidateWidget();
        return m_Widget.FindTransform(transformName);
    }

    private void ValidateWidget()
    {
        if (m_Widget == null)
        {
            m_Widget = new WidgetData(transform, false);
        }

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_Btns.Clear();
        if (m_Widget != null)
        {
            m_Widget.Dispose();
            m_Widget = null;
        }
        RawImage[] raws = GetComponentsInChildren<RawImage>(true);
        for (int i = 0; i < raws.Length; i++)
        {
            raws[i].texture = null;
        }
    }
    #endregion

}


public class WidgetData : System.IDisposable
{
    private Transform m_myTransform;
    private Dictionary<string, string> m_widgetToFullName;


    public WidgetData(Transform trans, bool isNameUni)
    {
        m_myTransform = trans;
        m_widgetToFullName = WidgetPathManager.FillFullNameData(m_myTransform.name, m_myTransform, isNameUni);
    }

    public bool TryGetCompRef<T>(string path, ref T comp) where T : Component
    {
        if (comp == null)
        {
            comp = GetComp<T>(path);
        }
        bool isSuccess = comp != null;
        #region UNITY_EDITOR
        if (!isSuccess)
        {
            Log.Error("未找到组件引用" + path);
        }
        #endregion
        return isSuccess;
    }


    //通过控件名查找组件
    public T GetComp<T>(string transformName) where T : Component
    {
        Transform _transform = FindTransform(transformName);
        if (_transform != null)
        {
            return _transform.GetComponent<T>();
        }
        return null;
    }

    //通过控件名查找控件
    public Transform FindTransform(string transformName)
    {
        if (string.IsNullOrEmpty(transformName)) return null;
        Transform find = null;
        if (m_widgetToFullName.ContainsKey(transformName))
        {
            find = m_myTransform.Find(m_widgetToFullName[transformName]);
        }

        if (find == null)
        {
            if (m_myTransform.name == transformName)
                find = m_myTransform;
        }

        return find;
    }

    public void Release()
    {
        WidgetPathManager.RemoveWigetDic(m_myTransform.name);
        m_widgetToFullName.Clear();
        m_widgetToFullName = null;
    }



    public void Print()
    {
        foreach (var item in m_widgetToFullName.Keys)
        {
            Log.Error(string.Format("Key:{0}---Value:{1}", item, m_widgetToFullName[item]));
        }
    }

    public void Dispose()
    {
        Release();
    }
}