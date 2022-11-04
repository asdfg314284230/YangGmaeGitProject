using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIStackMgr : SingletonClass<UIStackMgr>
{


    private List<FormsTree> m_listUIStack = new List<FormsTree>();

    #region 外部调用
    public void Release()
    {
        m_listUIStack.Clear();
    }

    public void PushUI(FormsTree baseform)
    {
        if (baseform.UIType == UIType.NonPush) return;
        if (baseform.UIType == UIType.Pop)
        {
            if (m_listUIStack.Count > 0)
                m_listUIStack[m_listUIStack.Count - 1].AddChildren(baseform);
        }
        else if (baseform.UIType == UIType.FullScreen)
        {
            for (int i = 0; i < m_listUIStack.Count; i++)
            {
                if (m_listUIStack[i].PrefabName == baseform.PrefabName)
                    m_listUIStack.Remove(m_listUIStack[i]);
            }
            m_listUIStack.Add(baseform);
        }

    }

    public void PopUIPrevious(UIType uitype)
    {
        RemoveLastOne(uitype);
        PopUILastFullScreen();
    }


    public void RemoveLastOne(UIType uitype)
    {
        if (m_listUIStack.Count > 0)
        {
            if (uitype == UIType.Pop)
            {
                m_listUIStack[m_listUIStack.Count - 1].RemoveLastOneChild();
            }
            else if (uitype == UIType.FullScreen)
            {
                m_listUIStack.RemoveAt(m_listUIStack.Count - 1);
            }
        }
    }


    /// <summary>
    /// 移除栈顶界面, 弹出栈顶界面
    /// </summary>
    public void PopUILastFullScreen()
    {
        if (m_listUIStack.Count <= 0)
        {
            UIManager.OpenMainUI();
            return;
        }
        FormsTree formsTree = m_listUIStack[m_listUIStack.Count - 1];
        UIManager.Instance.BackToOpenUI(formsTree.PrefabName);
        formsTree.MapChildrenWithClear((ft) =>
        {
            UIManager.Instance.BackToOpenUI(ft.PrefabName);
        });
    }

    public void RemoveFromStack(string prefname)
    {
        m_listUIStack.RemoveAll((node) => node.PrefabName == prefname);
    }


    #endregion


    #region Tool
    public void Print()
    {
        for (int i = 0; i < m_listUIStack.Count; i++)
        {
            Log.Error("~~~" + m_listUIStack[i].PrefabName);
        }
    }
    #endregion
}

public class FormsTree
{

    public FormsTree ParentNode;
    public string PrefabName;
    public UIType UIType;
    public List<FormsTree> Trees = new List<FormsTree>();
    public FormsTree Node;
    public FormsTree(string prefname, UIType uitype)
    {
        PrefabName = prefname;
        UIType = uitype;
        Node = this;
    }

    public void AddChildren(FormsTree treeChild)
    {
        //如果子节点窗体类型小于父节点,则无法添加(数值越小等级越高)
        if (treeChild.UIType <= UIType) return;
        if (!Trees.Contains(treeChild))
            Trees.Add(treeChild);
        treeChild.ParentNode = Node;
    }

    public void MapChildrenWithClear(System.Action<FormsTree> map)
    {
        for (int i = 0; i < Trees.Count; i++)
        {
            map(Trees[i]);
        }
        Trees.Clear();
    }

    public void RemoveLastOneChild()
    {
        if (Trees.Count >= 1)
            Trees.RemoveAt(Trees.Count - 1);
    }

    public int GetChildCount()
    {
        return Trees.Count;
    }
}

public interface IStack : IForms
{
    UIType UIType { get; set; }
}

public interface IForms
{
    GameObject GoForms { get; }
    string PrefabName { get; }
}