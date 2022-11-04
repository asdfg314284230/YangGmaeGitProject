using UnityEngine;

/// <summary>
/// UI事件监听基类
/// 1.保证事件监听和移除匹配调用
/// </summary>
public abstract class BaseEventListener : MonoBehaviour
{
    #region INIT

    protected bool IsAddListenersSuccess = false;

    // 初始化(如果子类没有调用Awake,则调用基类的Awake,保证事件监听执行)
    private void Awake()
    {
        AddListenersWhenAwake();
    }

    //创建界面时调用
    protected void AddListenersWhenAwake()
    {
        if (!IsAddListenersSuccess)
        {
            AddListeners();
            IsAddListenersSuccess = true;
        }
    }

    //销毁界面时调用
    protected virtual void OnDestroy()
    {
        if (IsAddListenersSuccess)
        {
            RemoveListeners();
            IsAddListenersSuccess = false;
        }
    }

    #endregion

    #region 事件监听

    //事件监听,必须保证移除
    protected virtual void AddListeners() { }

    //事件监听,必须保证移除
    protected virtual void RemoveListeners() { }

    #endregion
}