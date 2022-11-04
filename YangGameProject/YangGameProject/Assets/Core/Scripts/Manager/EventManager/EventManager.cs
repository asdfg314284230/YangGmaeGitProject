using System.Collections.Generic;
using UnityEngine;

// 
public class EventManager
{
    public delegate void callback(GEvent e);
    private static Dictionary<string, List<callback>> dict = new Dictionary<string, List<callback>>();
    //加入一个侦听
    public static void AddListener(string type, callback fn)
    {
        if (!dict.ContainsKey(type))
        {
            dict.Add(type, new List<callback>());
        }

        foreach (callback cb in dict[type])
        {
            if (cb.Target is UnityEngine.Object && cb.Target.Equals(null))
            {
                
#if UNITY_EDITOR
                Log.Error(string.Format("出现对象已销毁，但事件回调未被remove的情况：{0} {1}", type, cb.Method.Name));
#endif

                dict[type].Remove(cb);
                return;
            }
        }

        if (dict[type].Contains(fn))
        {
#if UNITY_EDITOR
            Log.Error(string.Format("重复加入了侦听{0},{1}", type, fn.Method.ToString()));
#endif
            return;
        }
        dict[type].Add(fn);
    }

    public static void Print()
    {
        foreach (var item in dict)
        {
            Log.Error(string.Format("事件名 ：{0}，事件函数个数：{1}", item.Key, item.Value.Count));
        }
    }


    //删除一个类型的，一个指定回调
    public static void RemoveListener(string ntype, callback fn)
    {
        List<callback> fblist = null;
        if (dict.TryGetValue(ntype, out fblist))
        {
            if (fblist.Contains(fn))
            {
                fblist.Remove(fn);

                if (fblist.Count <= 0)
                {
                    dict.Remove(ntype);
                }
            }

        }

    }
    //将一个类型的事件都删除
    public static void RemoveListenerByType(string type)
    {
        if (dict.ContainsKey(type))
        {
            dict.Remove(type);
        }
    }

    /// <summary>
    /// 发出带参数数组事件，读取时要用GetData add by leon
    /// </summary>
    /// <param name="type"></param>
    /// <param name="args"></param>
    public static void Send(string type, params object[] args)
    {
        GEvent e = new GEvent(type, args);
        SendEvent(e);
        //if (LuaMain.Instance != null) LuaMain.Instance.LuaEventHandler(type, args);
    }

    //回调数组物件池，每次从其中抽取一个，用于本次循环使用
    public static List<List<callback>> tmpCache = new List<List<callback>>();

    //发出一个事件
    public static void SendEvent(GEvent e)
    {
        //如果存在这个事件
        if (dict.ContainsKey(e.type))
        {
            List<callback> tmp;
            if (tmpCache.Count == 0)
            {
                tmp = new List<callback>();
            }
            else
            {
                tmp = tmpCache[0];
                tmpCache.RemoveAt(0);
            }
            //无gc复制,copy一份出来防止fn触发事件又改变list，导致乱序
            for (int i = 0; i < dict[e.type].Count; i++)
            {
                tmp.Add(dict[e.type][i]);
            }
            //执行
            for (int i = 0; i < tmp.Count; i++)
            {
                tmp[i](e);
            }
            //清理回收
            tmp.Clear();
            tmpCache.Add(tmp);
        }
    }

    //异步发送事件，主要给多线程用的
    static Queue<GEvent> qEvent = new Queue<GEvent>();
    public static void SendEventAsync(GEvent e)
    {
        lock (qEvent)
        {
            qEvent.Enqueue(e);
        }
    }

    public static void Update()
    {
        while (true)
        {
            GEvent e = null;
            lock (qEvent)
            {
                if (qEvent.Count > 0)
                {
                    e = qEvent.Dequeue();
                }
            }

            if (e != null)
            {
                SendEvent(e);
            }
            else
            {
                break;
            }
        }
    }

    public static bool IsExistsListener(string type, callback fn)
    {
        if (dict.ContainsKey(type) && dict[type].Contains(fn))
        {
            return true;
        }

        return false;
    }

}