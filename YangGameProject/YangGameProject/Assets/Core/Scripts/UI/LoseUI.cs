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
        //    // ��ȡ��˭���ɵ����ɵ�Ƶ�
        //    mInfo.text = "����һ��:" + DataAccess.gameModel.KillData.RoleName;
        //}

    }

    protected override void OnDisplay()
    {
        base.OnDisplay();

        ToolKit.DelayCall(5, () =>
        {
            // ���￪ʼִ�����¿�ʼ��ص��߼�
            var r = GameObject.Find("RoleNode");
            // ����ԭ�����
            for (int i = 0; i < r.transform.childCount; i++)
            {
                Destroy(r.transform.GetChild(i).gameObject);
            }

            // ��������
            DataAccess.gameModel.roleDic.Clear();

            // �����������
            var p = UGUITool.InstantiateObject("Player");
            p.name = "Player";
            p.transform.position = new Vector3(-9, 1, 10);

            UINodesManager.UIRoot.gameObject.SetActive(false);

            OnClickCloseBtn(null);

        });
    }

}
