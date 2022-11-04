using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
public class UGUITool
{

    static public GameObject AddChild(GameObject parent, GameObject asset, bool forceShow)
    {
        if (asset == null)
        {
#if UNITY_EDITOR
            Log.Error("prefab is null");
#endif
            return null;
        }

        GameObject goInstantiate = GameObject.Instantiate(asset) as GameObject;
        SetParent(parent, goInstantiate, forceShow);
        return goInstantiate;
    }

    static public void SetParent(GameObject parent, GameObject goInstantiate, bool forceShow, string name)
    {
        if (goInstantiate != null && !string.IsNullOrEmpty(name))
            goInstantiate.name = name;
        SetParent(parent, goInstantiate, forceShow);
    }

    static public void SetParent(GameObject parent, GameObject goInstantiate, bool forceShow)
    {
        if (goInstantiate != null && forceShow)
            goInstantiate.SetActive(true);

        if (goInstantiate != null && parent != null)
        {
            Transform t = goInstantiate.transform;
            t.SetParent(parent.transform);
            goInstantiate.layer = parent.layer;

            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;

            // RefreshNormalSafeAreas(parent.transform);
        }
        else if (parent == null)
        {
            Transform t = goInstantiate.transform;

            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }
    }

    // static void RefreshNormalSafeAreas(Transform parentTrans)
    // {
    //     var parentCanvas = parentTrans.GetComponent<Canvas>();
    //     if (parentCanvas == null)
    //         return;

    //     var normalSafeAreas = parentTrans.GetComponentsInChildren<NormalSafeArea>(true);
    //     foreach (var normalSafeArea in normalSafeAreas)
    //     {
    //         normalSafeArea.InputCanvas = parentCanvas;
    //     }
    // }

    #region RectTransform

    //重置Transform,并设置根节点
    static public void ResetTransform(RectTransform go, Transform parent)
    {
        if (go != null)
        {
            go.SetParent(parent);
            ResetTransform(go);
        }
        else
        {
            Log.Error("go is null");
        }
    }

    //重置Transform
    static public void ResetTransform(RectTransform go)
    {
        if (go != null)
        {
            go.sizeDelta = Vector3.zero;
            go.localPosition = Vector3.zero;
            go.anchoredPosition = Vector2.zero;
            go.localRotation = Quaternion.identity;
            go.localScale = Vector3.one;
        }
        else
        {
            Log.Error("go is null");
        }
    }

    #endregion





    public static Vector3 StrToVec3(string str)
    {
        if (string.IsNullOrEmpty(str)) return Vector3.zero;
        string[] posArr = str.Split(',');
        if (posArr.Length <= 1) return Vector3.zero;
        Vector3 vec = new Vector3(0, 0, 0);
        if (posArr.Length >= 2)
        {
            vec.x = float.Parse(posArr[0]);
            vec.y = float.Parse(posArr[1]);
        }
        if (posArr.Length >= 3)
        {
            vec.z = float.Parse(posArr[2]);
        }
        return vec;
    }

    public static bool StrToIntArr(string str, ref List<int> intArr, char separation = ';')
    {
        if (string.IsNullOrEmpty(str)) return false;
        string[] strArr = str.Split(separation);
        if (intArr == null)
            intArr = new List<int>(strArr.Length);
        else
            intArr.Clear();
        for (int i = 0; i < strArr.Length; i++)
        {
            intArr.Add(int.Parse(strArr[i]));
        }
        return true;
    }


    public static GameObject InstantiateObject(string resName)
    {
        Object obj = LoadPrefab(resName);
        GameObject go = GameObject.Instantiate(obj) as GameObject;
        obj = null;
        return go;
    }

    public static Object LoadPrefab(string resName)
    {
        Object prefab = Resources.Load<Object>("Prefabs/" + resName); ;
        if (prefab == null)
        {
            Log.Error("加载预制体失败:" + resName);
            throw new System.Exception("加载预制体失败:" + resName);
        }
        return prefab;
    }

    //后缀要加.[暂时废弃]
    /* public static string GetResPath(string resName, string suffix)
    {
        string path = "";
        string resDirPath = PathManager.FileBasePath() + "/_Project/Modules";
        string resFullPath = SearchAllAsset(resDirPath,
        (str) => str.EndsWith(resName + suffix.ToLower()));

        if (resFullPath == null)
            throw new System.Exception("未找到资源:" + resName);
        if (resFullPath.Contains("Resources"))
            path = StrClip(resFullPath, "Resources", suffix).Substring(1);
        return path;
    }

    public static string SearchAllAsset(string path, System.Predicate<string> filter)
    {
        string[] files = Directory.GetFiles(path);
        for (int i = 0; i < files.Length; i++)
        {
            if (filter(files[i]))
                return files[i];
        }
        string[] dirs = Directory.GetDirectories(path);
        for (int i = 0; i < dirs.Length; i++)
        {
            string resPath = SearchAllAsset(dirs[i], filter);
            if (resPath != null)
                return resPath;
        }

        return null;
    }

    public static string StrClip(string str, string s, string e)
    {
        Regex rg = new Regex("(?<=(" + s + "))[.\\s\\S]*?(?=(" + e + "))", RegexOptions.Multiline | RegexOptions.Singleline);
        return rg.Match(str).Value;
    } */

    //屏幕坐标,左下角(0,0),右上角(max,max) 
    public static TextAnchor GetScreenArea(Vector2 screenPos)
    {
        if (screenPos.x < Screen.width / 2 && screenPos.y < Screen.height / 2)
        {
            //左下
            return TextAnchor.LowerLeft;
        }
        else if (screenPos.x < Screen.width / 2 && screenPos.y > Screen.height / 2)
        {
            //左上
            return TextAnchor.UpperLeft;
        }
        else if (screenPos.x > Screen.width / 2 && screenPos.y < Screen.height / 2)
        {
            //右下
            return TextAnchor.LowerRight;
        }
        else if (screenPos.x > Screen.width / 2 && screenPos.y > Screen.height / 2)
        {
            //右上
            return TextAnchor.UpperRight;
        }
        return TextAnchor.UpperLeft;
    }


    //public static void World2UI(Vector3 wpos, RectTransform uiParent, RectTransform uiTarget)
    //{
    //    Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(UINodesManager.UIPerspectiveCamera, wpos);
    //    Vector2 localPos;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(uiParent, screenPos, UINodesManager.UICamera, out localPos);
    //    uiTarget.anchoredPosition = localPos;
    //}

    public static T AddMissComponent<T>(GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp != null) return comp;
        comp = go.AddComponent<T>();
        return comp;
    }

    public static void SetLayerForAll(Transform target, int layer)
    {
        target.gameObject.layer = layer;
        if (target.childCount == 0) return;
        foreach (Transform item in target)
        {
            SetLayerForAll(item, target.gameObject.layer);
        }
    }

    public static void NumberToSimpleStr(ref int num, ref string str)
    {
        str = NumberToSimpleStr(num);
    }

    public static string NumberToSimpleStr(int num)
    {
        if (num >= 100000000)
        {
            return num / 100000000 + "亿";
        }
        else if (num >= 100000)
        {
            return num / 10000 + "万";
        }
        else
        {
            return num.ToString();
        }
    }

    public static T GetMax<T>(IList<T> list, System.Func<T, int> getval)
    {
        int max = 0;
        int index = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (getval(list[i]) > max)
            {
                max = getval(list[i]);
                index = i;
            }
        }
        return list[index];
    }

    public static void ToSimpleStr(int maxNum, ref string content)
    {
        if (content.Length >= maxNum)
            content = string.Concat(content.Substring(0, maxNum), "...");
    }


    /* public static void SetButtonStyle(CUI.UI.CustomButton targetBtn, CustomBtnType btnType)
    {
        if (btnType == CustomBtnType.Btn_A_1)
        {
            Image icon = targetBtn.targetGraphic as Image;
            if (icon != null)
            {
                SpriteTool.SetSprite("ui_control", "Btn_A_1", icon);
                icon.SetNativeSize();
            }
            Text text = targetBtn.txtLabel;
            if (text != null)
            {
                text.fontSize = 32;
                text.color = new Color32(255, 255, 255, 255);
            }
            Outline outline = UGUITool.AddMissComponent<Outline>(text.gameObject);
            outline.effectColor = new Color32(201, 110, 160, 255);
        }
    }

    public static void SetTabStyle(CUI.UI.CustomToggle targetBtn, CustomTabType tabType)
    {
        if (tabType == CustomTabType.Tab_A)
        {
            Image icon = targetBtn.targetGraphic as Image;
            if (icon != null)
            {
                SpriteTool.SetSprite("ui_control", "Btn_A_1", icon);
                icon.SetNativeSize();
            }
            Text text = targetBtn.Label;
            if (text != null)
            {
                text.fontSize = 32;
                text.color = new Color32(255, 255, 255, 255);
            }
            Outline outline = UGUITool.AddMissComponent<Outline>(text.gameObject);
            outline.effectColor = new Color32(201, 110, 160, 255);
        }
    } */

    private static Texture2D _Tex2dEmpty;
    public static Texture2D Tex2dEmpty
    {
        get
        {
            if (_Tex2dEmpty == null)
            {
                _Tex2dEmpty = Resources.Load<Texture2D>("TexEmpty");
            }

            return _Tex2dEmpty;
        }
    }

}
