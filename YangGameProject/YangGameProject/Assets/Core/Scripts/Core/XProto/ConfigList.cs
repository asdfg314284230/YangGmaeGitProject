
using System;
using System.Collections.Generic;

namespace Stardom.Core.XProto
{
    public class ConfigList<T> : List<T> where T : ConfigBase
    {
        public T GetOne(int id, bool islog = true)
        {
            T cfg = Find(x => x.Id == id);
#if UNITY_EDITOR
            if (cfg == null && islog) UnityEngine.Log.Error("配置数据错误：" + GetType() + " --->Id: == " + id);
#endif
            return cfg;
        }

        public List<T> GetList(params int[] ids)
        {
            return FindAll(x => Array.Exists(ids, id => id == x.Id));
        }

        public T GetLastOne()
        {
            for (int i = 0; i < Count; i++)
            {
                if (i == Count-1) return this[i];
            }
            throw new Exception("无配置数据");
        }

        #region 暂时不需
        //public Dictionary<int, T> GetDict(params int[] ids)
        //{
        //    Dictionary<int, T> list = new Dictionary<int, T>();

        //    foreach (var item in this)
        //    {
        //        if (Array.Exists(ids, id => id == item.Id))
        //            list.Add(item.Id, item);

        //        if (ids.Length == list.Count)
        //            break;
        //    }

        //    return list;
        //}

        //public Dictionary<int, T> GetDict(List<int> ids)
        //{
        //    var list = new Dictionary<int, T>();

        //    foreach (var item in this)
        //    {
        //        if (ids.Exists(x => x.Equals(item.Id)))
        //            list.Add(item.Id, item);

        //        if (ids.Count == list.Count)
        //            break;
        //    }

        //    return list;
        //}
        #endregion
    }
}
