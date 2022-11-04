using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CUI.UI;
/// <summary>
/// 二级界面基类
/// </summary>
namespace CUI.View
{
    public class BasePop : BaseForms
    {

        // private Mask2Common m_Mask2Common;
        protected CustomButton m_CloseBtn;

        protected bool isClick;
        protected bool isMask = true;
        protected float alpha = 1f;

        protected bool releaseWhenHide = false;

        public System.Action OnCloseCallback = null;

        protected override void Preprocessing()
        {
            isClick = true;
            UIType = UIType.NonPush;//大多数非跳转二级界面都不入栈
            PrefabName = gameObject.name;
            ParentNode = UINodesManager.MiddleUIRoot;
            IsHideWhenNewOpen = true;
        }

        protected override void InitUI()
        {
            base.InitUI();
            m_CloseBtn = FindComponent<CustomButton>("CloseBtn");
            if (m_CloseBtn)
            {
                m_CloseBtn.onClickCustom = null;
                // m_CloseBtn.onClickCustom += (cb) => { ToolKit.PlayUISound(4); };
                m_CloseBtn.onClickCustom += OnClickCloseBtn;
                m_CloseBtn.IsNormalSound = false;
            }
        }

        protected override void OnDisplay()
        {
            base.OnDisplay();
            OnCloseCallback = null;
            // SetMask();
            HideWeakUI();
            SendUIShow();
        }

        protected override void OnHide()
        {
            base.OnHide();
            // ResetMask();

            if (releaseWhenHide)
            {
                ReleaseSelf();
                return;
            }

            SendUIHide();
        }

        protected virtual void HideWeakUI()
        {
            // if (!IsInWhiteListForNewGuideWeakUI(PrefabName))
            // {
            //     NewGuideWeakUI.WeakUI().HideUI();
            // }
        }

        protected virtual void SendUIShow()
        {
            EventManager.Send(EventConstants.UI_Show_PrefabName, PrefabName);
        }

        protected virtual void SendUIHide()
        {
            EventManager.Send(EventConstants.UI_Hide_PrefabName, PrefabName);
        }


        public override void ReleaseSelf()
        {
            // ResetMask();
            base.ReleaseSelf();
            SendUIHide();
        }

        // protected void SetMask()
        // {
        //     if (!isMask) return;
        //     m_Mask2Common = Mask2Common.GetMask(transform);
        //     if (m_Mask2Common == null || m_Mask2Common.gameObject == null)
        //     {
        //         throw new System.Exception("获取Mask失败");
        //     }
        //     UGUITool.SetParent(gameObject, m_Mask2Common.GoForms, false);
        //     m_Mask2Common.SetMask(isClick, alpha, OnClickMask);
        // }

        // protected void ResetMask()
        // {
        //     if (!isMask) return;
        //     m_Mask2Common?.HideMask();
        // }

        protected virtual void OnClickMask(CustomButton btn)
        {
            // 播放音效
            // ToolKit.PlayUISound(4);
            OnClickCloseBtn(btn);
        }

        protected virtual void OnClickCloseBtn(CustomButton btn)
        {
            if (UILock) return;
            if (UIType == UIType.Pop)
                UIStackMgr.Instance.RemoveLastOne(UIType);
            ReleaseSelf();
            OnCloseCallback?.Invoke();
        }

    }
}