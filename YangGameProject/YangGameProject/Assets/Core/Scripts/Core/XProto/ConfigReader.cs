
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 0618
namespace Stardom.Core.XProto
{
    public class ConfigReader
    {
        public static ConfigList<T> Parse<T>(string path) where T : ConfigBase
        {
            var list = new ConfigList<T>();
            var data = File.ReadAllBytes(path);
            //Log.Error(path);
            var stream = new ProtoStream(data);
            int count = stream.ReadInt();
            for (var i = 0; i < count; i++)
            {
                var item = Activator.CreateInstance<T>();
                item.Decode(stream);
                list.Add(item);
            }
            // UpdateManager.Instance.ConfigStatistics();
            return list;

            /* 
            //! 早期的cdn加载方式
            UnityWebRequestUtil.Instance.Get(path, UnityWebRequestUtil.Instance.commonUWRBack, delegate (UnityWebRequest obj)
            {
                byte[] data = obj.downloadHandler.data;
                var stream = new ProtoStream(data);
                int count = stream.ReadInt();
                for (var i = 0; i < count; i++)
                {
                    var item = Activator.CreateInstance<T>();
                    item.Decode(stream);
                    list.Add(item);
                }
                UpdateManager.Instance.ConfigStatistics();
            }); 
            return list;
            */
        }

#if UNITY_EDITOR
        public static ConfigList<T> ParseInEditor<T>(string path) where T : ConfigBase
        {
            var list = new ConfigList<T>();
            var data = File.ReadAllBytes(path);
            var stream = new ProtoStream(data);
            int count = stream.ReadInt();
            for (var i = 0; i < count; i++)
            {
                var item = Activator.CreateInstance<T>();
                item.Decode(stream);
                list.Add(item);
            }
            return list;
        }
#endif
    }
}
