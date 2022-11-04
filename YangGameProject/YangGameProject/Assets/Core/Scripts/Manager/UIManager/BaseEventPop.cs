using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CUI.View;
using UnityEngine.UI;

public class BaseEventPop : BasePop
{
    protected override void Preprocessing()
    {
        base.Preprocessing();
        IsHideWhenNewOpen = false;//不能受其它界面[如新手引导的强制开启界面]影响导致mask混乱
    }
    public virtual void StartEventPlay(GEvent e)
    {

    }


    protected override void OnDisplay()
    {
        base.OnDisplay();
        // UIPopEventManager.Instance.IsPlaying = true;
    }

    protected override void OnHide()
    {
        base.OnHide();
        // UIPopEventManager.Instance.IsPlaying = false;
        // UIPopEventManager.Instance.Next();
    }


    public override void ReleaseSelf()
    {
        base.ReleaseSelf();
        // UIPopEventManager.Instance.IsPlaying = false;
        // UIPopEventManager.Instance.Next();
    }

}
