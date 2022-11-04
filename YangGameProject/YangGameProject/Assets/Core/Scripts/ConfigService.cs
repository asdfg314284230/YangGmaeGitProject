using System;
using System.Reflection;
using UnityEngine;
using Stardom.Core.Model;

namespace Stardom.Core.XProto
{
    public class ConfigService
    {
        public static readonly ConfigService Instance = new ConfigService();
        /*
         * 所有的格式开始-----------------------------------------------------
         */
        //消息机制-文本系统
        //public ConfigList<TextCfg> TextCfgList;
        //public ConfigList<ErrorCodeCfg> ErrorCodeCfgList;
        //public ConfigList<WhiteProtoIDCfg> WhiteProtoIDCfgList;

        // 弹窗排斥文本
        public ConfigList<GameConfCfg> GameConfCfgList;
        public ConfigList<PlantsConfCfg> PlantsConfList;

        /*
        * 所有的格式结束------------------------------------------------------
        */


        #region  初始化和自动解析
        public void Initialize()
        {
            Type type = this.GetType();
            MethodInfo method = type.GetMethod("ReadConfig", BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in typeof(ConfigService).GetFields())
            {
                if (field.FieldType.IsGenericType)
                {
                    Type[] ts = field.FieldType.GetGenericArguments();

                    if (ts != null && ts.Length == 1 && ts[0].BaseType == typeof(ConfigBase))
                    {
                        MethodInfo m = method.MakeGenericMethod(ts);
                        if (m == null) continue;
                        field.SetValue(this, m.Invoke(this, null));
                    }
                }
            }
        }

        private ConfigList<T> ReadConfig<T>() where T : ConfigBase
        {
            Type type = typeof(T);
            string path = string.Format("{0}GameConfig/{1}.dat", Application.dataPath + "/StreamingAssets/", type.Name);
            return ConfigReader.Parse<T>(path);
        }

#if UNITY_EDITOR
        public void InitializeInEditor()
        {
            Type type = this.GetType();
            MethodInfo method = type.GetMethod("ReadConfigInEditor", BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in typeof(ConfigService).GetFields())
            {
                if (field.FieldType.IsGenericType)
                {
                    Type[] ts = field.FieldType.GetGenericArguments();

                    if (ts != null && ts.Length == 1 && ts[0].BaseType == typeof(ConfigBase))
                    {
                        MethodInfo m = method.MakeGenericMethod(ts);
                        if (m == null) continue;
                        field.SetValue(this, m.Invoke(this, null));
                    }
                }
            }
        }

        private ConfigList<T> ReadConfigInEditor<T>() where T : ConfigBase
        {
            Type type = typeof(T);
            string path = string.Empty;
            path = string.Format("{0}GameConfig/{1}.dat", Application.dataPath + "/StreamingAssets/", type.Name);
            return ConfigReader.ParseInEditor<T>(path);
        }
#endif
        #endregion
    }
}

