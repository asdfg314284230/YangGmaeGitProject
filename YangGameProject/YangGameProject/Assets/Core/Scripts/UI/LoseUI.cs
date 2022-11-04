using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CUI.UI;
using CUI.View;
using UnityEngine.UI;

public class LoseUI : BasePop
{
    Button mBtn;
    Text mInfo;

    protected override void InitUI()
    {
        base.InitUI();
        TryGetCompRef("Button", ref mBtn);
        TryGetCompRef("mInfo", ref mInfo);

        //if (DataAccess.gameModel.KillData != null)
        //{
        //    // 获取是谁最后干掉这个傻逼的
        //    mInfo.text = "最终一击:" + DataAccess.gameModel.KillData.RoleName;
        //}

    }

    protected override void OnDisplay()
    {
        base.OnDisplay();

        ToolKit.DelayCall(5, () =>
        {
            // 这里开始执行重新开始相关的逻辑
            var r = GameObject.Find("RoleNode");
            // 清理原先玩家
            for (int i = 0; i < r.transform.childCount; i++)
            {
                Destroy(r.transform.GetChild(i).gameObject);
            }

            // 清理数据
            DataAccess.gameModel.roleDic.Clear();

            // 重新生成玩家
            var p = UGUITool.InstantiateObject("Player");
            p.name = "Player";
            p.transform.position = new Vector3(-9, 1, 10);

            UINodesManager.UIRoot.gameObject.SetActive(false);

            OnClickCloseBtn(null);

        });
    }

}
