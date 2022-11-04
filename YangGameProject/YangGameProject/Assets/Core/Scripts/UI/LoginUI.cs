using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CUI.UI;
using CUI.View;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;

public class LoginUI : BaseFullScreen
{
    private InputField PassInput;
    private InputField RoomId;
    private Button GoBtn;
    private Button UnLockBtn;
    private Text InfoError;


    protected override void AddListeners()
    {
        base.AddListeners();
        EventManager.AddListener(EventConstants.InfoMsg, InfoMsg);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        EventManager.RemoveListener(EventConstants.InfoMsg, InfoMsg);
    }

    protected override void InitUI()
    {
        base.InitUI();
        TryGetCompRef("GoBtn", ref GoBtn);
        TryGetCompRef("UnLockBtn", ref UnLockBtn);
        TryGetCompRef("RoomId", ref RoomId);
        TryGetCompRef("PassInput", ref PassInput);
        TryGetCompRef("InfoError", ref InfoError);


        UnLockBtn.onClick.AddListener(() =>
        {
            DataAccess.authLoginModel.isUnlock(PassInput.text);
        });

        GoBtn.onClick.AddListener(() =>
        {
            bool isGet = DataAccess.authLoginModel.GetIsLogin(PassInput.text);
            if (isGet)
            {
                //µÇÂ¼³É¹¦
                SceneManager.LoadScene("MainScene");
            }
        });


        PassInput.text = PlayerPrefs.GetString("PassWord");

    }

    protected override void OnDisplay()
    {
        base.OnDisplay();
    }



    void InfoMsg(GEvent e)
    {
        string info = e.GetData<string>(0);
        InfoError.text = info;
    }



}
