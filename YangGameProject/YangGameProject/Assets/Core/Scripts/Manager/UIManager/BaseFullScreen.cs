using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stardom.Core.Model;
using Stardom.Core.XProto;


/// <summary>
/// 全屏界面基类
/// </summary>
namespace CUI.View
{
    public class BaseFullScreen : BaseForms
    {
        public bool IsHideMainUI { protected set; get; }

        protected override void Preprocessing()
        {
            UIType = UIType.FullScreen;
            PrefabName = gameObject.name;
            ParentNode = UINodesManager.NormalUIRoot;
            IsHideMainUI = true;
            IsHideWhenNewOpen = true;
        }


        protected override void OnDisplay()
        {
            base.OnDisplay();
            SendUIShow();

            SetHeaderUI();
            ScreenAdapt();
        }

        protected override void OnHide()
        {
            base.OnHide();
            // if (HeaderUI.Instance != null)
            // {
            //     HeaderUI.Instance.Show(false);
            // }
            SendUIHide();
        }
        protected virtual void SendUIShow()
        {
            EventManager.Send(EventConstants.UI_Show_PrefabName, PrefabName);
        }

        protected virtual void SendUIHide()
        {
            EventManager.Send(EventConstants.UI_Hide_PrefabName, PrefabName);
        }


        Vector2 offset = new Vector2(40, 0);
        protected virtual void ScreenAdapt()
        {
            //rectTransform.anchoredPosition = offset; 
        }


        protected virtual void SetHeaderUI()
        {
            // if (ConfigService.Instance.UIFormsTableCfgList == null) return;
            // UIFormsTableCfg tableCfg = ConfigService.Instance.UIFormsTableCfgList.Find((cfg) => cfg.Name == PrefabName);
            // if (tableCfg != null)
            //     HeaderUI.OpenHeaderUI(tableCfg, OnClickHeaderBack);
            // else
            //     Log.Info(string.Format("界面未注册:<color=red>{0}</color>", PrefabName));
        }

        // protected virtual void OnClickHeaderBack(HeaderUI header)
        // {
        //     header.Show(false);
        //     UIStackMgr.Instance.PopUIPrevious(UIType);
        // }

    }

}

