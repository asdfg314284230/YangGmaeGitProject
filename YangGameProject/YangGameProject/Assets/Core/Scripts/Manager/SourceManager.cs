using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json.Linq;

public class SourceManager : SingletonClass<SourceManager>
{

    private string MiuseNameText;
    private List<FileInfo> MiusePathList = new List<FileInfo>();
    private int MiuseIndex;
    private AudioSource source;


    //public void Init()
    //{
    //    // 监听事件
    //    EventManager.AddListener(EventConstants.PlayGetMusic, PlayGetMusic);




    //    string path = string.Format("{0}", Application.streamingAssetsPath + "/Music/");


    //    if (Directory.Exists(path))
    //    {
    //        DirectoryInfo direction = new DirectoryInfo(path);
    //        FileInfo[] files = direction.GetFiles("*");
    //        for (int i = 0; i < files.Length; i++)
    //        {
    //            // 忽略.meta文件和当前播放相同的音乐文件
    //            if (files[i].Name.EndsWith(".meta"))
    //            {
    //                continue;
    //            }
    //            MiusePathList.Add(files[i]);
    //        }
    //    }

    //}

    //public void Update()
    //{
    //    //if (!source.isPlaying && MiusePathList.Count != 0)
    //    //{
    //    //    GetMiuse();
    //    //}
    //}


    //public void Clear()
    //{
    //    EventManager.RemoveListener(EventConstants.PlayGetMusic, PlayGetMusic);
    //}


    //private void PlayGetMusic(GEvent e)
    //{
    //    string musicName = e.GetData<string>(0).ToString();

    //    if (source == null)
    //    {
    //        // 获取音频
    //        source = Camera.main.GetComponent<AudioSource>();
    //    }

    //    foreach (var item in MiusePathList)
    //    {
    //        if (item.Name.Substring(0, item.Name.Length - 4) == musicName)
    //        {

    //            UnityWebRequestUtil.Instance.GetAudioClip(item.FullName, (clip) =>
    //            {
    //                source.clip = clip;
    //                source.Play();
    //                Debug.Log("当前歌曲播放:" + MiuseNameText);
    //            });

    //            break;
    //        }
    //    }
    //}

}
