using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameInit : MonoBehaviour
{
    public static Transform gameRoot;

    private static bool gamePause = false;
    public static bool GamePause { get => gamePause; set => gamePause = value; }

    void Start()
    {
        gameRoot = transform;
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 120; //游戏30帧
        CorePlugins.Instance.Init(PlayGameLoading);
    }


    // 游戏登录
    private void PlayGameLoading()
    {
        UIManager.Instance.OpenUI(UIConst.LoginUI);
    }


    // 暂停接口
    private void OnApplicationPause(bool pause)
    {
        GamePause = pause;
    }

    private void Update()
    {
        TimerHeap.Tick();
    }


    private void OnApplicationQuit()
    {
        EventManager.Send(EventConstants.OnApplicationQuit);
        EventManager.Send(EventConstants.CloseSocket);
    }


}


