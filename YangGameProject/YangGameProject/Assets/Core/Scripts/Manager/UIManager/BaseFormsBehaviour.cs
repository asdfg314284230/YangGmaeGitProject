using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
// using CUI.UI;

namespace CUI.View
{

    public class BaseFormsBehaviour : UIBaseBehaviour, IDisposable, ICanvasRaycastFilter
    {
        //确保调用一次OnDisplay
        private bool m_IsOnDisplay = false;

        private void Awake()
        {
            AwakeBase();
        }


        // protected UIAnimElement _animElement;
        // public UIAnimElement AnimElement
        // {
        //     get
        //     {
        //         if (_animElement == null)
        //             _animElement = GetComponent<UIAnimElement>();
        //         return _animElement;
        //     }
        //     protected set
        //     {
        //         _animElement = value;
        //     }
        // }

        public GameObject GoForms
        {
            get
            {
                return gameObject;
            }
        }

        //不与active关联
        protected bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            protected set
            {
                _isVisible = value;
                if (value)
                {
                    if (m_IsOnDisplay) return;
                    m_IsOnDisplay = true;
                    OnDisplay();
                }
                else
                {
                    if (!m_IsOnDisplay) return;
                    m_IsOnDisplay = false;
                    OnHide();
                }
            }
        }
        public bool UILock { set; get; } = false;
        public bool UIAnimLock { private set; get; } = false;


        public virtual Transform ParentNode { get; protected set; }
        public virtual string PrefabName { get; protected set; }


        #region 核心  
        // private UISingleAnimHanlder _uiAnimHandler;
        // public UISingleAnimHanlder UIAnimHandler
        // {
        //     get
        //     {
        //         if (_uiAnimHandler == null)
        //             _uiAnimHandler = new UISingleAnimHanlder();
        //         return _uiAnimHandler;
        //     }
        // }
        //用于在某些情况下临时关闭开窗动画 
        private bool _isAnimOpen = true;
        public bool IsAnimOpen
        {
            get
            {
                return _isAnimOpen;
            }
            protected set
            {
                _isAnimOpen = value;
            }
        }

        /// <summary>
        /// 显示隐藏界面唯一接口[放弃SetActive]
        /// </summary>
        /// <param name="isShow"></param>
        public void Show(bool isShow, bool isAnim = false)
        {
            if (isShow == IsVisible) return;
            IsAnimOpen = isAnim;
            if (isShow)
                ExecuteDisplay();
            else
                ExecuteHide();
        }


        private void ExecuteDisplay()
        {
            //动画开始之前显示
            Display();
            IsVisible = true;//动画之前更新数据
            UIAnimLock = true;
            DisplayAnim(() =>
            {
                UIAnimLock = false;
            });
        }

        private void ExecuteHide()
        {
            UIAnimLock = true;
            HideAnim(() =>
            {
                //动画结束之后隐藏
                Hide();
                IsVisible = false;
                UIAnimLock = false;
            });
        }
        #endregion

        #region 内部调用 
        protected bool m_IsInitUI = false;
        //需要在Awake中调用
        protected void AwakeBase()
        {
            if (m_IsInitUI) return;
            AddListenersWhenAwake();
            InitData();
            InitUI();
            if (!m_IsInitUI) m_IsInitUI = true;
        }

        //[1.1][界面激活前预处理]
        protected virtual void Preprocessing() { }
        //[1.2]初始化变量数据
        protected virtual void InitData() { }
        //[1.3]初始化界面
        protected virtual void InitUI() { }


        //显示方法(动画效果开始前触发)
        protected virtual void Display()
        {
            gameObject.SetActive(true);
        }

        //隐藏方法(动画效果结束后触发)
        protected virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        //替代OnEnable(动画效果开始前触发)
        protected virtual void OnDisplay()
        {
        }

        //替代OnDisable(动画效果结束后触发)
        protected virtual void OnHide()
        {
        }

        public virtual void Dispose()
        {
            // _uiAnimHandler = null;
        }
        #endregion


        #region Anim
        //界面开启动画回调
        protected virtual void OnDisplayAnimCallBack() { }
        //界面关闭动画回调
        protected virtual void OnHideAnimCallBack() { }
        //界面开启动画名
        protected string DisplayAnimName = "";
        //界面关闭动画名
        protected string HideAnimName = "";
        /// <summary>
        /// 显示动画
        /// </summary>
        /// <param name="callback"></param>
        protected virtual void DisplayAnim(Action callback)
        {
            //if (IsAnimOpen && AnimElement != null && !string.IsNullOrEmpty(DisplayAnimName) && !UILock)
            //{ 
            //    UIAnimHandler.PlayAnim(DisplayAnimName, () =>
            //    {
            //        OnDisplayAnimCallBack();
            //        if (callback != null)
            //            callback();
            //    }, AnimElement, "DisplayAnim");
            //    return;
            //}
            //OnDisplayAnimCallBack();
            if (callback != null)
                callback();
        }

        /// <summary>
        /// 隐藏动画
        /// </summary>
        /// <param name="callback"></param>
        protected virtual void HideAnim(Action callback)
        {
            //if (IsAnimOpen && AnimElement != null && !string.IsNullOrEmpty(HideAnimName) && !UILock)
            //{ 
            //    UIAnimHandler.PlayAnim(HideAnimName, () =>
            //    {
            //        OnHideAnimCallBack();
            //        if (callback != null)
            //            callback();
            //    }, AnimElement, "HideAnim");
            //    return;
            //}
            //OnHideAnimCallBack();
            if (callback != null)
                callback();
        }

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return !UIAnimLock;
        }
        #endregion



    }

}

public class UIData
{

}

