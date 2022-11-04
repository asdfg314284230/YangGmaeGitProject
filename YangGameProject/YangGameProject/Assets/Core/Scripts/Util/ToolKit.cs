using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Stardom.Core.Model;
using Stardom.Core.XProto;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Reflection;
public class ToolKit
{
    #region DelayCall

    private static bool inited = false;
    private static DelayManager _dm;
    private static DelayManager Dm
    {
        get
        {
            if (!inited) Init();
            return _dm;
        }
        set
        {
            _dm = value;
        }
    }

    public static void Init()
    {
        if (!inited)
        {
            GameObject dmObj = new GameObject("DelayManager");
            dmObj.transform.SetParent(GameInit.gameRoot);
            Dm = dmObj.AddComponent<DelayManager>();
            inited = true;

            startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 8, 0, 0), TimeZoneInfo.Local);
        }
    }

    public static string DelayCall(float t, DelegateEnums.NoneParam fn, string ID = "", bool realTimeMode = false)
    {
        return Dm.AddDelay(t, fn, ID, realTimeMode);
    }
    public static string DelayCall(float t, DelegateEnums.DataParam fn, object data, string ID = "", bool realTimeMode = false)
    {
        return Dm.AddDelay(t, fn, data, ID, realTimeMode);
    }
    public static void StopDelayCall(string id)
    {
        if (Dm != null && !string.IsNullOrEmpty(id))
        {
            Dm.StopDelay(id);
        }
    }
    public static void StopDelayCall(DelegateEnums.NoneParam fn)
    {
        if (Dm != null)
        {
            Dm.StopDelay(fn);
        }
    }
    public static void StopDelayCall(DelegateEnums.DataParam fn)
    {
        if (Dm != null)
        {
            Dm.StopDelay(fn);
        }
    }

    #endregion

    // 延迟n帧执行
    public static void DelayFrame(int frameNum, Action afterDelayAction)
    {
        Dm?.StartCoroutine(DelayFrameCorou(frameNum, afterDelayAction));
    }

    public static void DelayFrame(MonoBehaviour mono, int frameNum, Action afterDelayAction)
    {
        mono?.StartCoroutine(DelayFrameCorou(frameNum, afterDelayAction));
    }

    private static IEnumerator DelayFrameCorou(int frameNum, Action afterDelayAction)
    {
        for (int i = 0; i < frameNum; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        afterDelayAction();
    }


    public static GameObject AddChildIn(GameObject parent) { return AddChildIn(parent, true); }
    private static GameObject AddChildIn(GameObject parent, bool undo)
    {
        GameObject go = new GameObject();
#if UNITY_EDITOR
        if (undo) UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
        if (parent != null)
        {
            Transform t = go.transform;
            t.SetParent(parent.transform);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            go.layer = parent.layer;
        }
        return go;
    }
    public static GameObject AddChildIn(GameObject parent, GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;
#if UNITY_EDITOR
        UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
        if (go != null && parent != null)
        {
            go.SetActive(true);
            Transform t = go.transform;
            t.SetParent(parent.transform);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;

            go.layer = parent.layer;
        }
        return go;
    }

    //战斗场景和主城场景切换的时候调用  剧情播放了一个大回合也可以调用
    public static void ClearAllRes()
    {
        Resources.UnloadUnusedAssets();
        DelayCall(0.5f, delegate ()
        {
            GC.Collect();
            Log.Info("清理内存结束");
        });
    }

    //移出所有子对象
    public static void RemoveAllChild(Transform transform)
    {
        if (transform == null) return;
        foreach (Transform item in transform)
        {
            UnityEngine.Object.Destroy(item.gameObject);
        }
    }

    //获得一个组件，包括deactive的
    public static T GetComponentInChildren<T>(Transform parent) where T : Component
    {
        T component = GetComponent<T>(parent);
        return component;
    }
    //遍历所有子对象
    public static T GetComponent<T>(Transform transfrom) where T : Component
    {
        T component = transfrom.GetComponent<T>();
        if (null != component)
        {
            return component;
        }
        foreach (Transform child in transfrom)
        {
            return GetComponent<T>(child);
        }
        return null;
    }

    //获得所有组件，包括deactive的
    public static List<T> GetComponentsInChildren<T>(Transform parent) where T : Component
    {
        List<T> components = new List<T>();
        GetComponent<T>(parent, ref components);
        return components;
    }

    //遍历所有子对象
    private static void GetComponent<T>(Transform transfrom, ref List<T> list) where T : Component
    {
        T component = transfrom.GetComponent<T>();
        if (null != component)
        {
            list.Add(component);
        }
        foreach (Transform child in transfrom)
        {
            GetComponent<T>(child, ref list);
        }
    }

    // using System.Security.Cryptography;
    public static string GetMd5Hash(String input)
    {
        if (input == null)
        {
            return null;
        }
        MD5 md5Hash = MD5.Create();
        // 将输入字符串转换为字节数组并计算哈希数据
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        // 创建一个 Stringbuilder 来收集字节并创建字符串
        StringBuilder sBuilder = new StringBuilder();
        // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        // 返回十六进制字符串
        return sBuilder.ToString();
    }

    /// <summary>
    /// 获取字符串md5,因主要给路径取md5值，所以传入的路径全部传小写，以减少因大小写引起的问题
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string GetStringMD5(string str)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str.ToLower()));
        return System.BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    public static string GetColorStringByColor(Color tmpColor)
    {
        int rInt = (int)(tmpColor.r * 255.0f);
        int gInt = (int)(tmpColor.g * 255.0f);
        int bInt = (int)(tmpColor.b * 255.0f);

        string red = Convert.ToString(rInt, 16);
        string green = Convert.ToString(gInt, 16);
        string blue = Convert.ToString(bInt, 16);
        if (red.Length == 1)
        {
            red = "0" + red;
        }
        if (green.Length == 1)
        {
            green = "0" + green;
        }
        if (blue.Length == 1)
        {
            blue = "0" + blue;
        }

        string colorStr = "[" + red + green + blue + "ff]";

        return colorStr;
    }


    //移除 内存里的资源
    public static void UnloadAssetObj(GameObject clearObj)
    {
        if (null == clearObj)
        {
            return;
        }
        UnloadAsset(clearObj);
        for (int i = 0, len = clearObj.transform.childCount; i < len; i++)
        {
            Transform child = clearObj.transform.GetChild(i);
            UnloadAssetObj(child.gameObject);
        }
    }

    public static void UnloadAsset(GameObject obj)
    {
        SkinnedMeshRenderer[] list = obj.GetComponents<SkinnedMeshRenderer>();
        for (int i = 0, len1 = list.Length; i < len1; i++)
        {
            Resources.UnloadAsset(list[i].sharedMesh);

            Material[] mList = list[i].materials;
            for (int j = 0, len = mList.Length; j < len; ++j)
            {
                Resources.UnloadAsset(mList[j].mainTexture);
            }
        }
        MeshRenderer[] list2 = obj.GetComponents<MeshRenderer>();
        for (int i = 0; i < list2.Length; i++)
        {
            Material[] mList = list2[i].materials;
            for (int j = 0, len = mList.Length; j < len; ++j)
            {
                Resources.UnloadAsset(mList[j].mainTexture);
            }
        }
        MeshFilter[] meshFilterList = obj.GetComponents<MeshFilter>();

        for (int i = 0, len1 = meshFilterList.Length; i < len1; i++)
        {
            Resources.UnloadAsset(meshFilterList[i].sharedMesh);
        }
        //特效
        ParticleSystem[] particles = obj.GetComponents<ParticleSystem>();

        for (int i = 0, len1 = particles.Length; i < len1; i++)
        {
            Material[] mList = particles[i].GetComponent<Renderer>().materials;
            for (int j = 0, len = mList.Length; j < len; ++j)
            {
                Resources.UnloadAsset(mList[j].mainTexture);
                Resources.UnloadAsset(mList[j]);
            }
        }

        Animation[] animations = obj.GetComponents<Animation>();

        for (int i = 0, len1 = animations.Length; i < len1; i++)
        {
            foreach (AnimationState state in animations[i])
            {
                Resources.UnloadAsset(state.clip);
            }

        }
    }

    /// <summary>
    /// 遍历查找制定名称的子对象
    /// </summary>
    public static GameObject FindChildDeep(GameObject parent, string name)
    {
        Transform trans = parent.transform;
        GameObject result = null;

        for (int i = 0; i < trans.childCount; i++)
        {
            GameObject child = trans.GetChild(i).gameObject;

            if (child.name == name)
                return child;
        }
        for (int i = 0; i < trans.childCount; i++)
        {
            GameObject child = trans.GetChild(i).gameObject;
            result = FindChildDeep(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    //随机数
    public static long GetRandomValueBySeed(long seed)
    {
        return (seed * 1103515245L + 12345L) & 0x7fffffff;
    }

    //数组元素值等于索引值
    public static List<int> GetTotalIndices(int num)
    {
        int[] ret = new int[num];
        for (int i = 0; i < num; i++)
        {
            ret[i] = i;
        }
        return new List<int>(ret);
    }

    public static List<int> GetRandomIndices(List<int> arr)
    {
        List<int> retList = new List<int>();
        foreach (var item in arr)
        {
            retList.Insert(UnityEngine.Random.Range(0, retList.Count + 1), item);
        }
        return retList;
    }

    public static List<int> GetRandomIndicesOfNum(int total, int num)
    {
        if (num <= 0)
        {
            return new List<int> { };
        }

        if (num > total)
        {
            return ToolKit.GetTotalIndices(num);
        }

        List<int> ret = new List<int>();
        List<int> randomIndices = ToolKit.GetRandomIndices(ToolKit.GetTotalIndices(total));

        for (int i = 0; i < num; i++)
        {
            ret.Add(randomIndices[i]);
        }

        return ret;
    }

    // public static void ShowToastOfEverything(Transform trans, Vector3 originLocalPos, Vector3 moveUpPos, float idleDuration = 2.0f)
    // {
    //     Atween.RltToPosition(trans, moveUpPos, 0.25f, DG.Tweening.Ease.InOutQuad, 0, null, true);

    //     ToolKit.DelayCall(idleDuration, () =>
    //     {
    //         Atween.RltToPosition(trans, moveUpPos, 0.25f, DG.Tweening.Ease.InOutQuad, 0, null, true);
    //         Atween.ToAlpha(trans, 0f, 0.25f, () =>
    //         {
    //             trans.localPosition = originLocalPos;
    //             trans.gameObject.SetActive(false);
    //             trans.GetComponent<CanvasGroup>().alpha = 1f;
    //         });
    //     });
    // }




    private static string[] digits = new string[11] { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };
    public static string GetChineseDigit(int digit)
    {
        if (digit < 0 || digit >= digits.Length) return digit.ToString();
        return digits[digit];
    }

    public static int GetStringLength(string str)
    {
        string temp = str;
        int j = 0;
        for (int i = 0; i < temp.Length; i++)
        {
            if (Regex.IsMatch(temp.Substring(i, 1), @"[^\x00-\xff]"))
            {
                j += 2;
            }
            else
            {
                j += 1;
            }
        }
        return j;
    }

    //获得非符号字段长度
    public static int GetNoSymbolCharactersLength(string str)
    {
        string temp = str;
        int j = 0;
        for (int i = 0; i < temp.Length; i++)
        {
            if (Regex.IsMatch(temp.Substring(i, 1), @"[^\x00-\xff]"))
            {
                j += 2;
            }
        }
        return j;
    }

    #region  专门给登录模块使用
    //获取最后一个汉字
    public static string GetStringLastOneCharacters(string str)
    {
        string temp = str;
        string bac = "";
        for (int i = 0; i < temp.Length; i++)
        {
            if (Regex.IsMatch(temp.Substring(i, 1), @"[\u4e00-\u9fa5]"))
            {
                bac = temp.Substring(i, 1);
            }
        }
        return bac;
    }

    #endregion

    /// <summary>
    /// 获取当天是周几
    /// </summary>
    /// <returns></returns>
    public static int GetWeekByCurrentTime(DateTime dt)
    {
        int curweek = Convert.ToInt32(dt.DayOfWeek);
        if (curweek == 0)
        {
            curweek = 7;
        }
        return curweek;
    }

    /// <summary>
    /// 周一到周日
    /// </summary>
    /// <param name="dt1"></param>
    /// <param name="dt2"></param>
    /// <returns></returns>
    public static bool IsSameWeek(DateTime dt1, DateTime dt2)
    {
        DateTime temp1 = dt1.AddDays(-GetChainDayOfWeek((int)dt1.DayOfWeek)).Date;
        DateTime temp2 = dt2.AddDays(-GetChainDayOfWeek((int)dt2.DayOfWeek)).Date;
        bool result = temp1 == temp2;
        return result;
    }

    public static int GetChainDayOfWeek(int dayofWeek)
    {
        if (dayofWeek == 0)
            return 6;
        else
            return dayofWeek - 1;
    }

    private static DateTime startTime;
    public static DateTime GetTimestampTime(double Timestamp)
    {
        DateTime time = DateTime.MinValue;
        time = startTime.AddSeconds(Timestamp);
        return time;
    }

    /// <summary>
    /// 获取时间的00:00:00显示
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string GetTimeString(int second)
    {
        int hour = 0;
        int minute = 0;
        hour = second / 3600;
        second = second % 3600;
        minute = second / 60;
        second = second % 60;
        return (hour < 10 ? "0" + hour + ":" : hour + ":") + (minute < 10 ? "0" + minute + ":" : minute + ":") + (second < 10 ? "0" + second : "" + second);
    }

    /// <summary>
    /// 获取时间的00:00显示
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string GetTimeMSString(int second)
    {
        int hour = 0;
        int minute = 0;
        hour = second / 3600;
        second = second % 3600;
        minute = second / 60;
        second = second % 60;
        return (minute < 10 ? "0" + minute + ":" : minute + ":") + (second < 10 ? "0" + second : "" + second);
    }
    /// <summary>
    /// 获取几天几小时
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string GetTimeDayHourString(int second)
    {
        if (second < 3600)
        {
            return ToolKit.GetTimeMSString(second);
        }
        if (second < 86400)
        {
            return string.Format("{0}小时", (int)(second / 3600));
        }
        int day = second / 86400;
        int hour = (int)((second % 86400) / 3600);
        return string.Format("{0}天{1}小时", day, hour);
    }

    /// <summary>
    /// 获取几天几小时几分几秒
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string GetTimeDayHourMinuteSecondString(int second)
    {
        int day = 0;
        int hour = 0;
        int minute = 0;
        if (second < 60)
        {
            return string.Format("{0}秒", second);
        }
        if (second < 3600)
        {
            minute = (int)(second / 60);
            second = (int)(second % 60);
            return string.Format("{0}分{1}秒", minute, second);
        }
        if (second < 86400)
        {
            hour = (int)(second / 3600);
            second = (int)((second % 3600) / 60);
            return string.Format("{0}小时{1}分", hour, minute, second);
        }
        day = (int)(second / 86400);
        hour = (int)(day / 3600);
        return string.Format("{0}天{1}小时", day, hour);
    }

    /// <summary>
    /// 秒转时分
    /// </summary>
    /// <returns></returns>
    public static string SecondToSF(ulong second)
    {
        ulong hour = second / (60 * 60);
        ulong minute = (second % (60 * 60)) / 60;
        return string.Format("{0}时{1}分", hour, (int)minute);
    }

    /// <summary>
    /// 秒转时
    /// </summary>
    /// <returns></returns>
    public static ulong SecondToH(ulong second)
    {
        ulong hour = second / (60 * 60);
        return hour;
    }

    /// <summary>
    /// 秒转分
    /// </summary>
    /// <returns></returns>
    public static ulong SecondToM(ulong second)
    {
        ulong minute = second / 60;
        return minute;
    }

    //内存对照
    private static bool MemoryCompare(byte[] b1, byte[] b2)
    {
        int len = b1.Length;

        if (len != b2.Length)
        {
            byte[] s = new byte[len];
            Array.Copy(b2, b2.Length - len, s, 0, len);
            for (int i = 0; i < len; i++)
            {
                if (b1[i] != s[i]) return false;
            }
        }
        else
        {
            for (int i = 0; i < len; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
        }

        return true;
    }

    public static void SetTrueShaderToGameObject(GameObject go)
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return;
        }
        ParticleSystem[] pss = go.GetComponentsInChildren<ParticleSystem>(true);
        foreach (ParticleSystem ps in pss)
        {
            ResetShaders(ps.GetComponent<Renderer>().sharedMaterials);
        }
        Renderer[] mrs = go.GetComponentsInChildren<Renderer>(true);
        if (mrs != null && mrs.Length > 0)
        {
            foreach (Renderer mr in mrs)
            {
                ResetShaders(mr.GetComponent<Renderer>().sharedMaterials);
            }
            return;
        }
        //
        MaskableGraphic maskableGraphic = go.GetComponent<MaskableGraphic>();
        if (maskableGraphic != null)
        {
            Material material = maskableGraphic.material;
            material.shader = Shader.Find(material.shader.name);
        }
        //大图
        RawImage[] rawImages = go.GetComponentsInChildren<RawImage>(true);
        foreach (RawImage img in rawImages)
        {
            Material material = img.material;
            material.shader = Shader.Find(material.shader.name);
        }

    }
    public static void ResetShaders(Material[] materials, string shaderName = "")
    {
        if (materials != null)
        {
            for (int m = 0; m < materials.Length; m++)
            {
                Material material = materials[m];
                if (material != null)
                {
                    material.shader = Shader.Find(shaderName == "" ? material.shader.name : shaderName);
                }
            }
        }
    }

    //数据写文件
    public static void WriteFileToPhone(string str, string fileName, bool isOverride = true)
    {
        StreamWriter sw = null;
        string path = "";
#if UNITY_EDITOR
        path = "d:/UnityLog/" + fileName + ".txt";
#else
        path = Application.persistentDataPath + "/" + fileName + ".txt";
#endif

        try
        {
            FileInfo info = new FileInfo(path);
            if (!info.Exists || isOverride)
            {
                sw = info.CreateText();
            }
            else
            {
                sw = info.AppendText();
            }
            sw.WriteLine(str);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        catch (Exception)
        {
        }
    }

    //界面音效
    public static void PlayUISound(int soundId)
    {
        // if (ConfigService.Instance == null || ConfigService.Instance.SoundCfgList == null
        // || ConfigService.Instance.SoundCfgList.Count == 0)
        // {
        //     Log.Error("找不到音效表配置 soundId：" + soundId);
        //     return;
        // }
        // SoundCfg soundCfg = ConfigService.Instance.SoundCfgList.GetOne(soundId);
        // if (soundCfg != null)
        // {
        //     WwiseManager.Instance.PlaySESound((uint)soundCfg.SoundId);
        // }
    }

    //InputField增加点击
    public static void BindUIEvent(UnityEngine.Events.UnityAction<BaseEventData> uiEvent,
    UnityEngine.Events.UnityAction<BaseEventData> uiEventDe, Transform trans)
    {
        if (!trans.GetComponent<EventTrigger>())
        {
            trans.gameObject.AddComponent<EventTrigger>();
        }
        UnityAction<BaseEventData> selectEvent = new UnityAction<BaseEventData>(uiEvent);
        EventTrigger.Entry onSelect = new EventTrigger.Entry();
        onSelect.eventID = EventTriggerType.Select;
        onSelect.callback.AddListener(selectEvent);
        trans.GetComponent<EventTrigger>().triggers.Add(onSelect);

        UnityAction<BaseEventData> deselectEvent = new UnityAction<BaseEventData>(uiEventDe);
        EventTrigger.Entry onDeSelect = new EventTrigger.Entry();
        onDeSelect.eventID = EventTriggerType.Deselect;
        onDeSelect.callback.AddListener(deselectEvent);
        trans.GetComponent<EventTrigger>().triggers.Add(onDeSelect);
    }

    public static void BindUIEventPointerDown(UnityEngine.Events.UnityAction<BaseEventData> uiEvent, Transform trans)
    {
        if (!trans.GetComponent<EventTrigger>())
        {
            trans.gameObject.AddComponent<EventTrigger>();
        }
        UnityAction<BaseEventData> selectEvent = new UnityAction<BaseEventData>(uiEvent);
        EventTrigger.Entry onSelect = new EventTrigger.Entry();
        onSelect.eventID = EventTriggerType.PointerDown;
        onSelect.callback.AddListener(selectEvent);
        trans.GetComponent<EventTrigger>().triggers.Add(onSelect);

    }

    public static string MD5File(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }
}