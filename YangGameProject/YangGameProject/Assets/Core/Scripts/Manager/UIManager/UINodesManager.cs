using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//using XEngine.AssetLoader;

/// <summary>
/// UI关键节点管理
/// </summary>
public class UINodesManager
{
    public const int NORMAL_SORTING_ORDER = 3000;
    public const int MIDDLE_SORTING_ORDER = 3200;
    public const int TOP_SORTING_ORDER = 12000;
    public const int TOP2_SORTING_ORDER = 16000;
    public const int TOP3_SORTING_ORDER = 20000;
    public const int TOP4_SORTING_ORDER = 24000;
    #region 2D关键节点设计

    #region Transform

    /// <0>无事件层(第0层)
    /// <1>普通层(第1层)
    /// <2>中间层(第2层)
    /// <3>T1(第3层)
    /// <4>T2(第4层)
    /// <5>T3(第5层)



    //代码创建UIRoot
    public static void InitRoot()
    {
        //存在就移除
        GameObject root = GameObject.Find("UIRoot");
        if (root != null)
        {
            Object.DestroyImmediate(root);
        }

        //Root

        GameObject go = new GameObject("UIRoot");
        Object.DontDestroyOnLoad(go);
        UIRoot = go.transform;

        //EventSystem

        go = new GameObject("EventSystem");
        go.transform.SetParent(UIRoot);
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();

        //UICamera

        go = new GameObject("UICamera");
        go.transform.SetParent(UIRoot);
        go.tag = "MainCamera";
        go.layer = LayerMask.NameToLayer("UI");
        UICamera = go.AddComponent<Camera>();
        // go.AddMissComponent<AudioListener>();
        UICamera.clearFlags = CameraClearFlags.SolidColor;
        UICamera.backgroundColor = Color.black;
        UICamera.cullingMask = 1 << LayerMask.NameToLayer("UI");
        UICamera.orthographic = true;
        UICamera.orthographicSize = 5;
        UICamera.nearClipPlane = -22f;
        UICamera.farClipPlane = 1601f;
        UICamera.depth = 90;
        UICamera.renderingPath = RenderingPath.Forward;
        UICamera.useOcclusionCulling = false;
        UICamera.allowHDR = false;
        UICamera.allowMSAA = true;
        UICamera.allowDynamicResolution = false;

        //HideCanvas
        Vector2 resolutionV2 = new Vector2(750, 1334);

        go = new GameObject("HideCanvas");
        go.transform.SetParent(UIRoot);
        HideUIRoot = go.transform;
        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.pixelPerfect = false;
        canvas.worldCamera = UICamera;
        canvas.planeDistance = -1000;
        canvas.sortingOrder = 0;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
        CanvasScaler canvasScaler = go.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = resolutionV2;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        canvasScaler.referencePixelsPerUnit = 100f;

        //UICanvas

        go = new GameObject(UINodeName.NormalUIRootName);
        go.transform.SetParent(UIRoot);
        go.layer = LayerMask.NameToLayer("UI");
        NormalUIRoot = go.transform;
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.pixelPerfect = false;
        canvas.worldCamera = UICamera;
        canvas.planeDistance = 100;
        canvas.sortingOrder = NORMAL_SORTING_ORDER;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
        canvasScaler = go.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = resolutionV2;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        canvasScaler.referencePixelsPerUnit = 100f;
        GraphicRaycaster graphicRaycaster = go.AddComponent<GraphicRaycaster>();
        graphicRaycaster.ignoreReversedGraphics = true;
        graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.TwoD;

        //UICanvasMiddle

        go = new GameObject(UINodeName.MiddleUIRootName);
        go.transform.SetParent(UIRoot);
        go.layer = LayerMask.NameToLayer("UI");
        MiddleUIRoot = go.transform;
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.pixelPerfect = false;
        canvas.worldCamera = UICamera;
        canvas.planeDistance = 300;
        canvas.sortingOrder = MIDDLE_SORTING_ORDER;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
        canvasScaler = go.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = resolutionV2;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        canvasScaler.referencePixelsPerUnit = 100f;
        graphicRaycaster = go.AddComponent<GraphicRaycaster>();
        graphicRaycaster.ignoreReversedGraphics = true;
        graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.TwoD;

        //UICanvasTop

        go = new GameObject(UINodeName.TopUIRootName);
        go.transform.SetParent(UIRoot);
        go.layer = LayerMask.NameToLayer("UI");
        TopUIRoot = go.transform;
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.pixelPerfect = false;
        canvas.worldCamera = UICamera;
        canvas.planeDistance = 800;
        canvas.sortingOrder = TOP_SORTING_ORDER;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
        canvasScaler = go.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = resolutionV2;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        canvasScaler.referencePixelsPerUnit = 100f;
        graphicRaycaster = go.AddComponent<GraphicRaycaster>();
        graphicRaycaster.ignoreReversedGraphics = true;
        graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.TwoD;

        //UICanvasTop2 
        go = new GameObject(UINodeName.T2RootName);
        go.transform.SetParent(UIRoot);
        go.layer = LayerMask.NameToLayer("UI");
        T2UIRoot = go.transform;
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.pixelPerfect = false;
        canvas.worldCamera = UICamera;
        canvas.planeDistance = 1000;
        canvas.sortingOrder = TOP2_SORTING_ORDER;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
        canvasScaler = go.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = resolutionV2;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        canvasScaler.referencePixelsPerUnit = 100f;
        graphicRaycaster = go.AddComponent<GraphicRaycaster>();
        graphicRaycaster.ignoreReversedGraphics = true;
        graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.TwoD;

        //UICanvasTop3 
        go = new GameObject(UINodeName.T3RootName);
        go.transform.SetParent(UIRoot);
        go.layer = LayerMask.NameToLayer("UI");
        T3UIRoot = go.transform;
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.pixelPerfect = false;
        canvas.worldCamera = UICamera;
        canvas.planeDistance = 1200;
        canvas.sortingOrder = TOP3_SORTING_ORDER;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
        canvasScaler = go.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = resolutionV2;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        canvasScaler.referencePixelsPerUnit = 100f;
        graphicRaycaster = go.AddComponent<GraphicRaycaster>();
        graphicRaycaster.ignoreReversedGraphics = true;
        graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.TwoD;

        //UICanvasTop4 
        go = new GameObject(UINodeName.T4RootName);
        go.transform.SetParent(UIRoot);
        go.layer = LayerMask.NameToLayer("UI");
        T4UIRoot = go.transform;
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.pixelPerfect = false;
        canvas.worldCamera = UICamera;
        canvas.planeDistance = 1400;
        canvas.sortingOrder = TOP4_SORTING_ORDER;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
        canvasScaler = go.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = resolutionV2;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        canvasScaler.referencePixelsPerUnit = 100f;
        graphicRaycaster = go.AddComponent<GraphicRaycaster>();
        graphicRaycaster.ignoreReversedGraphics = true;
        graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.TwoD;

    }


    /// <summary>
    /// [普通层(第1层)]
    /// (1)普通界面,会被其他界面顶掉
    /// </summary>
    static private Transform m_NormalUIRoot;
    static public Transform NormalUIRoot
    {
        get
        {
            if (m_NormalUIRoot == null && UIRoot != null)
                m_NormalUIRoot = UIRoot.Find(UINodeName.NormalUIRootName);
            return m_NormalUIRoot;
        }
        private set
        {
            m_NormalUIRoot = value;
        }
    }

    /// <summary>
    /// [中间层(第2层)]
    /// (1)弹出界面,不会被其他界面顶掉
    /// (2)指引界面(在普通层和中间层之上),T1/T2/T3等T节点下的界面不能进行指引
    /// </summary>
    static private Transform m_MiddleUIRoot;
    static public Transform MiddleUIRoot
    {
        get
        {
            if (m_MiddleUIRoot == null && UIRoot != null)
                m_MiddleUIRoot = UIRoot.Find(UINodeName.MiddleUIRootName);
            return m_MiddleUIRoot;
        }
        private set
        {
            m_MiddleUIRoot = value;
        }
    }

    /// <summary>
    /// [T1(第3层)]
    /// 任务对话框界面 
    /// 奖励面板
    /// 升级面板
    /// </summary>
    static private Transform m_TopUIRoot;
    static public Transform TopUIRoot
    {
        get
        {
            if (m_TopUIRoot == null && UIRoot != null)
                m_TopUIRoot = UIRoot.Find(UINodeName.TopUIRootName);
            return m_TopUIRoot;
        }
        private set
        {
            m_TopUIRoot = value;
        }
    }

    /// <summary>
    /// [T2(第4层)]非常规UI层
    /// 物品tips  
    /// </summary>
    static private Transform m_T2UIRoot;
    static public Transform T2UIRoot
    {
        get
        {
            if (m_T2UIRoot == null && UIRoot != null)
                m_T2UIRoot = UIRoot.Find(UINodeName.T2RootName);
            return m_T2UIRoot;
        }
        private set
        {
            m_T2UIRoot = value;
        }
    }

    /// <summary>
    /// [T3(第5层)]非常规UI层
    /// 转菊花 
    /// </summary>
    static private Transform m_T3UIRoot;
    static public Transform T3UIRoot
    {
        get
        {
            if (m_T3UIRoot == null && UIRoot != null)
                m_T3UIRoot = UIRoot.Find(UINodeName.T3RootName);
            return m_T3UIRoot;
        }
        private set
        {
            m_T3UIRoot = value;
        }
    }

    /// <summary>
    /// [T4(第6层)]非常规UI层
    /// 系统管理信息弹窗  
    /// </summary>
    static private Transform m_T4UIRoot;
    static public Transform T4UIRoot
    {
        get
        {
            if (m_T4UIRoot == null && UIRoot != null)
                m_T4UIRoot = UIRoot.Find(UINodeName.T4RootName);
            return m_T4UIRoot;
        }
        private set
        {
            m_T4UIRoot = value;
        }
    }


    /// <summary>
    /// [隐藏层]
    /// 处于非激活状态，用于临时隐藏UI窗体
    /// </summary>
    static private Transform m_HideUIRoot;
    static public Transform HideUIRoot
    {
        get
        {
            if (m_HideUIRoot == null && UIRoot != null)
                m_HideUIRoot = UIRoot.Find(UINodeName.HideUIRootName);
            return m_HideUIRoot;
        }
        private set
        {
            m_HideUIRoot = value;
        }
    }


    static private Camera m_UICamera;
    static public Camera UICamera
    {
        get
        {
            if (m_UICamera == null && UIRoot != null)
                m_UICamera = UIRoot.Find("UICamera").GetComponent<Camera>();
            return m_UICamera;
        }
        private set
        {
            m_UICamera = value;
        }
    }

    #endregion


    #endregion

    #region 节点Transform

    //UI根节点
    static private Transform m_UIRoot;
    static public Transform UIRoot
    {
        get
        {
            if (m_UIRoot == null)
            {
                GameObject go = GameObject.Find(UINodeName.UIRootName);
                if (go != null)
                {
                    Object.DontDestroyOnLoad(go);
                    m_UIRoot = go.transform;
                }
            }
            return m_UIRoot;
        }
        set
        {
            m_UIRoot = value;
        }
    }

    //UI事件系统
    static private EventSystem m_UIEventSystem;
    static public EventSystem UIEventSystem
    {
        get
        {
            if (m_UIEventSystem == null && UIRoot != null)
                m_UIEventSystem = UIRoot.Find(UINodeName.UIEventSystem).GetComponent<EventSystem>();

            return m_UIEventSystem;
        }
    }

    #endregion
}

#region UINodeBit

public class UINodeBit
{
    public const int NoEventsUIRoot = 1;
    public const int NormalUIRoot = 2;
    public const int MiddleUIRoot = 4;
    public const int TopUIRoot = 8;
    public const int T2Root = 16;
    public const int T3Root = 32;
    public const int BitDefault = NoEventsUIRoot | NormalUIRoot | MiddleUIRoot;

    //目标结点s是否包含该结点    
    static public bool IsContainNode(int dst_nodes, int node)
    {
        return ((dst_nodes & node) > 0);
    }
}

#endregion

#region 节点名称

public class UINodeName
{
    public const string UIRootName = "UIRoot";
    public const string UIEventSystem = "EventSystem";
    public const string NormalUIRootName = "UICanvas";
    public const string MiddleUIRootName = "UICanvasMiddle";
    public const string TopUIRootName = "UICanvasTop";
    public const string T2RootName = "UICanvasT2OfSpeical";
    public const string T3RootName = "UICanvasT3OfSpecial";
    public const string T4RootName = "UICanvasT4OfSpecial";
    public const string HideUIRootName = "HideCanvas";

}

#endregion