using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
// using CUI.UI;
using Stardom.Core.Model;
using Stardom.Core.XProto;

namespace CUI.View
{
    /// <summary>
    /// 主窗体
    /// </summary>
    public class BaseForms : BaseFormsBehaviour, IStack
    {
        private bool m_isInit = false;


        public int FormsID { protected set; get; } = 0;

        protected Transform _parentNode;
        public override Transform ParentNode
        {
            get
            {
                return _parentNode;
            }
            protected set
            {
                _parentNode = value;
            }
        }

        protected string _prefabName;
        public override string PrefabName
        {
            get
            {
                return _prefabName;
            }
            protected set
            {
                _prefabName = value;
            }
        }

        #region 重要属性
        ////窗体类型
        protected UIType _uiType = UIType.NonPush;
        public virtual UIType UIType
        {
            get
            {
                return _uiType;
            }
            set
            {
                _uiType = value;
            }
        }



        protected bool _isHideMainCamera = false;
        public bool IsHideMainCamera
        {
            get
            {
                return _isHideMainCamera;
            }
            protected set
            {
                _isHideMainCamera = value;
            }
        }

        protected bool _isHideWhenNewOpen = false;
        public bool IsHideWhenNewOpen
        {
            get
            {
                return _isHideWhenNewOpen;
            }
            protected set
            {
                _isHideWhenNewOpen = value;
            }
        }


        #endregion

        public void Init()
        {
            if (m_isInit) return;
            Preprocessing();
            m_isInit = true;
        }

        protected override void Display()
        {
            base.Display();
            if (ParentNode == null)
            {
                //throw new Exception("未设置父节点，查看窗体类型是否有误");
                Debug.LogError("未设置父节点，查看窗体类型是否有误");
            }
            
             UGUITool.SetParent(ParentNode.gameObject, gameObject, true);
            transform.SetAsLastSibling();
        }

        protected override void Hide()
        {
            if (gameObject != null)
            {
                UGUITool.SetParent(UINodesManager.HideUIRoot.gameObject, gameObject, false);
            }
        }

        protected override void OnDisplay()
        {
            base.OnDisplay();
        }


        public virtual void ReleaseSelf()
        {
            UIManager.Instance.ReleaseUI(PrefabName);
        }

        protected override void OnDisplayAnimCallBack()
        {
            //界面显示通知
            //EventManager.Send(EventConstants.UI_OnFormsOpen, this);
        }



    }


}
