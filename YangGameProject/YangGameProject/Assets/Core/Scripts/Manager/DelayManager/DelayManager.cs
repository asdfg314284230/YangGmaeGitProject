using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DelayManager : MonoBehaviour
{
    private static int autoID = 10000;
    //当前延迟字典
    private Dictionary<string, DelayCaller> dict = new Dictionary<string, DelayCaller>();
    //对象池
    private List<DelayCaller> pool = new List<DelayCaller>();
    //回调委托
    public delegate void DelayCallBack(DelayCaller caller);

    //从对象池中获取
    public DelayCaller GetCaller()
    {
        DelayCaller caller;
        if (pool.Count == 0)
        {
            caller = gameObject.AddComponent<DelayCaller>();
            caller.onFinishCallBack = OnFinish;
        }
        else
        {
            caller = pool[0];
            caller.enabled = true;
            pool.RemoveAt(0);
        }
        return caller;
    }
    //放回池中
    public void AddToPool(DelayCaller caller)
    {
        caller.fn_None = null;
        caller.fn_Data = null;
        caller.enabled = false;
#if UNITY_EDITOR
        caller.callFunc = "";
#endif
        caller.ID = "0";
        pool.Add(caller);
    }

    public string AddDelay(float t, DelegateEnums.DataParam fn_Data, object data, string ID = "", bool realTimeMode = false)
    {
        if (ID == "")
        {
            autoID++;
            ID = autoID.ToString();
        }
        else
        {
            if (dict.ContainsKey(ID))
            {
                StopDelay(ID);
            }
        }
        //
        DelayCaller caller = GetCaller();
        caller.ID = ID;


        caller.addDelay(t, fn_Data, data, realTimeMode);

        dict.Add(ID, caller);

        UpdateTitle();
        return ID;
    }

    public string AddDelay(float t, DelegateEnums.NoneParam fn_None, string ID = "", bool realTimeMode = false)
    {
        if (ID == "")
        {
            autoID++;
            ID = autoID.ToString();
        }
        else
        {
            if (dict.ContainsKey(ID))
            {
                StopDelay(ID);
            }
        }

        DelayCaller caller = GetCaller();
        caller.ID = ID;
        caller.addDelay(t, fn_None, realTimeMode);

        dict.Add(ID, caller);

        UpdateTitle();
        return ID;
    }

    //停止一个延迟
    public void StopDelay(string ID)
    {
        if (dict.ContainsKey(ID))
        {
            AddToPool(dict[ID]);
            dict.Remove(ID);
        }

        UpdateTitle();
    }
    //停止一个延迟(无参数
    public void StopDelay(DelegateEnums.NoneParam fn)
    {

        DelayCaller[] delayArr = new DelayCaller[dict.Count];
        dict.Values.CopyTo(delayArr, 0);
        //清理
        for (int i = 0; i < delayArr.Length; i++)
        {
            if (delayArr[i].fn_None == fn)
            {
                StopDelay(delayArr[i].ID);
            }
        }
    }
    //停止一个延迟(有参数
    public void StopDelay(DelegateEnums.DataParam fn)
    {

        DelayCaller[] delayArr = new DelayCaller[dict.Count];
        dict.Values.CopyTo(delayArr, 0);
        //清理
        for (int i = 0; i < delayArr.Length; i++)
        {
            if (delayArr[i].fn_Data == fn)
            {
                StopDelay(delayArr[i].ID);
            }
        }
    }


    //当一个延迟自然结束
    public void OnFinish(DelayCaller caller)
    {
        if (dict.ContainsKey(caller.ID))
        {
            dict.Remove(caller.ID);
        }
        AddToPool(caller);

        UpdateTitle();
    }
    private void UpdateTitle()
    {
#if UNITY_EDITOR
        //仅为了调试
        gameObject.name = string.Format("DelayManager {0} / {1}", dict.Count.ToString(), pool.Count.ToString());
#else
#endif
    }



    void OnDestroy()
    {
        dict.Clear();
        pool.Clear();
    }


}
