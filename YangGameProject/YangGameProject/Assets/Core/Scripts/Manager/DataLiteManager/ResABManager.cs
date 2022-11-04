using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//只对ResNode开放，不要自行using
namespace NS_Datalite
{
    public class ResABManager
    {
        private static Dictionary<string, AssetBundle> dict = new Dictionary<string, AssetBundle>();
        //引用计数器
        private static Dictionary<string, int> useDict = new Dictionary<string, int>();

        public static AssetBundle RegAB(string path, AssetBundle ab)
        {
            if (!dict.ContainsKey(path))
            {
                dict.Add(path, ab);
                useDict.Add(path, 1);
            }
            return ab;
        }
        //反注册，如果计数器为0，则销毁ab
        public static void UnRegAB(string path, bool unloadUsedObjects)
        {
            if (dict.ContainsKey(path))
            {
                useDict[path] -= 1;
                if (useDict[path] <= 0)
                {
                    AssetBundle ab = dict[path];
                    if (ab != null)
                    {
                        ab.Unload(unloadUsedObjects);
                        //Log.Error("卸载 " + path);
                    }
                    dict.Remove(path);
                    useDict.Remove(path);
                }
            }
        }

        public static void PrintInfo()
        {
            foreach (string k in dict.Keys)
            {
                Log.Info("{0}   ({1})", k, useDict[k]);
            }
        }

        public static bool AddUseCount(string path)
        {
            if (dict.ContainsKey(path))
            {
                useDict[path] += 1;
                return true;
            }
            return false;
        }

        public static AssetBundle GetAB(string path)
        {

            if (dict.ContainsKey(path))
            {
                //   Debug.Log("******************* " + path + "," + dict[path].GetInstanceID()); 
                return dict[path];
            }
            return null;
        }
        public static bool CheckAB(string path)
        {
            return dict.ContainsKey(path);
        }


        public static void UnloadAll()
        {
            foreach (string k in dict.Keys)
            {
                if (dict[k] == null)
                {
                    Log.Error("ab is null ->" + k);
                    continue;
                }
                dict[k].Unload(false);
            }
            dict.Clear();
            useDict.Clear();
        }
    }

}