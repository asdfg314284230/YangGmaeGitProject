using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CUI.UI;
using CUI.View;
using UnityEngine.UI;



public class SettingPop : BasePop
{
    private Button closeBtn;

    protected override void InitUI()
    {
        base.InitUI();
        TryGetCompRef("CloseBtn", ref closeBtn);

        closeBtn.onClick.AddListener(() =>
        {
            OnClickCloseBtn(null);
        });
    }

    protected override void OnDisplay()
    {
        base.OnDisplay();
    }


}
