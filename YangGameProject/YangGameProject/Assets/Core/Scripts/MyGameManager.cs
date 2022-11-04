using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CUI.View;
using UnityEngine.UI;
using System.Text;
using DG.Tweening;
using System.IO;
using Newtonsoft.Json;
using Liluo.BiliBiliLive;
using Stardom.Core.XProto;

public class MyGameManager : UIBaseBehaviour
{


    Transform RolePanel;


    string[] nameList = { "����1", "����2", "����3", "����4", "����5" };

    List<string> HeadIconTest = new List<string>();


    string[] AniNameList = { "MeiShaoNv", "WoNiu", "MeiGuoDuiZhang", "Lang", "Yang" };

    protected override void AddListeners()
    {
        base.AddListeners();
        HeadIconTest.Add("https://p3.douyinpic.com/aweme/100x100/aweme-avatar/tos-cn-i-0813_2b95d7b6faea4cd09cd872ca826930bc.");
        HeadIconTest.Add("https://p3.douyinpic.com/aweme/100x100/aweme-avatar/tos-cn-i-0813_2b95d7b6faea4cd09cd872ca826930bc.");
        HeadIconTest.Add("https://p3.douyinpic.com/aweme/100x100/aweme-avatar/tos-cn-i-0813_2b95d7b6faea4cd09cd872ca826930bc.");
        HeadIconTest.Add("https://p3.douyinpic.com/aweme/100x100/aweme-avatar/tos-cn-i-0813_2b95d7b6faea4cd09cd872ca826930bc.");
        HeadIconTest.Add("https://p3.douyinpic.com/aweme/100x100/aweme-avatar/tos-cn-i-0813_2b95d7b6faea4cd09cd872ca826930bc.");


        ShowItemManager.Instance.Init();
    }

    async void Start()
    {

        // ������ȡ
        RolePanel = GameObject.Find("RolePanel").transform;


        MessageHandle mes = this.gameObject.AddComponent<MessageHandle>();
        mes.OnDanmuCallBack += GetDanmu;
        mes.OnGiftCallBack += GetGift;

        if (UINodesManager.UIRoot != null)
        {
            UINodesManager.UIRoot.gameObject.SetActive(false);
        }
    }


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F5))
        {
            int r = Random.Range(0, nameList.Length);
            RoleData roleData = new RoleData();
            roleData.RoleName = nameList[r];
            roleData.Info = nameList[Random.Range(0, 5)];
            roleData.Img = HeadIconTest[0];
            CreatePlay(roleData);
        }


        if (Input.GetKeyDown(KeyCode.F4))
        {
            int r = Random.Range(0, nameList.Length);
            RoleData roleData = new RoleData();
            roleData.RoleName = nameList[r];
            roleData.Info = "����ƶ�";
            roleData.Img = HeadIconTest[0];
            CreatePlay(roleData);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            int r = Random.Range(0, nameList.Length);
            MessageData data = new MessageData();
            data.imgURL = HeadIconTest[0];
            data.nickName = nameList[r];
            data.content = "õ��";
            data.Count = "1";
            GetGift(data);
        }

    }

    #region �������

    void CreatePlay(RoleData data)
    {
        if (DataAccess.gameModel.GetPlayerIsGet(data.RoleName))
        {
            AnalyMessage(data);
        }
        else
        {
            RoleController roleController = UGUITool.InstantiateObject("Role").GetComponent<RoleController>();
            roleController.transform.SetParent(RolePanel);
            roleController.transform.position = new Vector3(Random.Range(DataAccess.gameModel._MinX, DataAccess.gameModel._MaxX), Random.Range(DataAccess.gameModel._MinY, DataAccess.gameModel._MaxY));

            roleController.Bind(data);
            DataAccess.gameModel.roleDic.Add(data, roleController.gameObject);
        }
    }


    void ShowPlayDanMu(RoleData data, Transform pos)
    {
        ShowItemManager.Instance.SetItemPos(pos, data);
    }


    void AnalyMessage(RoleData data)
    {
        RoleController roleController = null;

        foreach (var item in DataAccess.gameModel.roleDic)
        {
            if (item.Key.RoleName == data.RoleName)
            {
                roleController = item.Value.GetComponent<RoleController>();
                //roleController.ShowInfo(data.Info);

                ShowPlayDanMu(data, item.Value.transform);
                break;
            }
        }


        switch (data.Info)
        {
            case "������":
                string aniName = AniNameList[Random.Range(0, AniNameList.Length)];
                if (roleController != null)
                {
                    roleController.SwitchAni(aniName);
                }
                break;
            case "����ƶ�":
                if (roleController != null)
                {
                    roleController.RandomMove();
                }
                break;
            default:
                break;
        }




    }




    #endregion


    #region ����Э��

    public void GetGift(MessageData data)
    {
        Debug.Log($"<color=#FEA356>����</color> ���: {data.nickName}, ��������: {data.content}, ����: {data.Count}");

        RoleData roleData = null;

        foreach (var item in DataAccess.gameModel.roleDic)
        {
            if (item.Key.RoleName == data.nickName)
            {
                roleData = item.Key;
                break;
            }
        }


        if (roleData == null)
        {
            roleData = new RoleData();
            roleData.RoleName = data.nickName;
            roleData.Info = data.content;
            roleData.Img = data.imgURL;
            CreatePlay(roleData);
            //return;
        }

        roleData.AllContribute = int.Parse(data.Count);

        RoleController roleController = null;

        // ��������������Ŵ�
        foreach (var item in DataAccess.gameModel.roleDic)
        {
            if (item.Key.RoleName == roleData.RoleName)
            {
                roleController = item.Value.GetComponent<RoleController>();
                roleController.RoleMax();
                break;
            }
        }


    }

    public void GetDanmu(MessageData data)
    {
        RoleData roleData = new RoleData();
        roleData.RoleName = data.nickName;
        roleData.Info = data.content;
        roleData.Img = data.imgURL;

        CreatePlay(roleData);
    }





    #endregion




}
