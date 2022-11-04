using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CUI.UI;
using CUI.View;
using UnityEngine.UI;



public class LoginTestPop : BasePop
{
    private Button testBtn;

    protected override void InitUI()
    {
        base.InitUI();
        TryGetCompRef("btn", ref testBtn);

        testBtn.onClick.AddListener(() =>
        {
            OnClickCloseBtn(null);
        });
    }

    protected override void OnDisplay()
    {
        base.OnDisplay();
    }


}
