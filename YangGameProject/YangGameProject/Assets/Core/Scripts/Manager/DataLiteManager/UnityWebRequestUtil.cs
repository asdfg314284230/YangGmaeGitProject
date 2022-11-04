using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;
using WebP;

public class UnityWebRequestUtil : MonoBehaviour
{

    string[] UsList =
    {
        "Mozilla/5.0 (Windows NT 10; Win64; x64; rv:83.0) Gecko/20100101 Firefox/83.0",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.93 Safari/537.36",
        "Mozilla/5.0 (Linux; Android 10; ELS-AN00) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.92 Mobile Safari/537.36",
        "Mozilla/5.0 (Linux; Android 9; Mi 10 Pro) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.92 Mobile Safari/537.36"
    };

    private static UnityWebRequestUtil instance;
    public static UnityWebRequestUtil Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject mounter = new GameObject("UnityWebRequestUtil");
                instance = mounter.AddComponent<UnityWebRequestUtil>();
                // mounter.transform.SetParent(GameInit.gameRoot);
            }
            return instance;
        }
    }

    public Action<UnityWebRequest, Action<UnityWebRequest>, Action<UnityWebRequest>> commonUWRBack = (UnityWebRequest uwr, Action<UnityWebRequest> deal, Action<UnityWebRequest> failDeal) =>
    {
        if (!ReportUWRException(uwr))
        {
            if (uwr.responseCode == 200)//200表示接受成功
            {
                deal(uwr);
                return;
            }
        }
        failDeal?.Invoke(uwr);
    };

    private static bool ReportUWRException(UnityWebRequest uwr)
    {
        if (!string.IsNullOrEmpty(uwr.error) || uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.LogError("commonUWRBack error " + uwr.error);
            Debug.LogError(uwr.error);
            Debug.LogError(uwr.url);
            Debug.LogError("uwr.isNetworkError " + uwr.isNetworkError);
            Debug.LogError("uwr.isHttpError " + uwr.isHttpError);
            Debug.LogError("uwr.responseCode " + uwr.responseCode);
            return true;
        }
        return false;
    }



    public void GetAudioClip(string url, Action<AudioClip> handle)
    {
        StartCoroutine(DoVioceClip(url, handle));
    }




    IEnumerator DoVioceClip(string url, Action<AudioClip> handle)
    {
        UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
        {
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError)
                Debug.LogError(uwr.error);
            else
            {
                // source.clip = DownloadHandlerAudioClip.GetContent(uwr);
                // source.Play();

                AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                handle(clip);
            }
        }
    }

    /// <summary>
    /// GET请求
    /// </summary>
    /// <param name="url">请求地址</param>
    /// <param name="actionResult">报错对象回调处理</param>
    /// <param name="a">请求发起后处理回调结果的委托,处理请求对象</param>
    public void Get(string url, Action<UnityWebRequest, Action<UnityWebRequest>, Action<UnityWebRequest>> actionResult = null, Action<UnityWebRequest> a = null, Action<UnityWebRequest> failAction = null)
    {
        StartCoroutine(_Get(url, actionResult, a, failAction));
    }


    /// <summary>
    /// GET请求
    /// </summary>
    /// <param name="url">请求地址,like 'http://www.my-server.com/ '</param>
    /// <param name="actionResult">请求发起后 报错对象回调处理</param>
    /// <param name="a">请求发起后处理回调结果的委托</param>
    /// <returns></returns>
    IEnumerator _Get(string url, Action<UnityWebRequest, Action<UnityWebRequest>, Action<UnityWebRequest>> actionResult = null, Action<UnityWebRequest> a = null, Action<UnityWebRequest> failAction = null)
    {
        using (UnityWebRequest uwr = UnityWebRequest.Get(url))
        {
            yield return uwr.SendWebRequest();
            actionResult?.Invoke(uwr, a, failAction);
        }
    }


    /// <summary>
    /// 请求图片
    /// </summary>
    /// <param name="url">图片地址,like 'http://www.my-server.com/image.png '</param>
    /// <param name="action">请求发起后处理回调结果的委托,处理请求结果的图片</param>
    /// <returns></returns>
    public void GetTexture(string url, Action<Texture2D> actionResult = null)
    {
        StartCoroutine(_GetTexture(url, actionResult));
    }


    /// <summary>
    /// 请求图片
    /// </summary>
    /// <param name="url">图片地址,like 'http://www.my-server.com/image.png '</param>
    /// <param name="action">请求发起后处理回调结果的委托,处理请求结果的图片</param>
    /// <returns></returns>
    IEnumerator _GetTexture(string url, Action<Texture2D> actionResult = null, Func<Texture2D, Image> funcResult = null)
    {
        int UAId = UnityEngine.Random.Range(0, UsList.Length);

        //if (EventConstants.isDouYin)
        //{
        //    url = url + "png";
        //}

        UnityWebRequest uwr = new UnityWebRequest(url);
        uwr.SetRequestHeader("User-Agent", UsList[UAId]);
        DownloadHandlerTexture downloadTexture = new DownloadHandlerTexture(true);
        uwr.downloadHandler = downloadTexture;


        yield return uwr.SendWebRequest();

        if (checkImage(downloadTexture.data))
        {
            Texture2D t = null;
            if (!ReportUWRException(uwr))
            {
                t = downloadTexture.texture;
                actionResult?.Invoke(t);
            }
        }
        else
        {
            StartCoroutine(LoadWebp(url, actionResult));
        }

    }

    IEnumerator LoadWebp(string url, Action<Texture2D> actionResult = null)
    {
        int UAId = UnityEngine.Random.Range(0, UsList.Length);
        UnityWebRequest uwr = new UnityWebRequest(url);
        uwr.SetRequestHeader("User-Agent", UsList[UAId]);
        DownloadHandlerTexture downloadTexture = new DownloadHandlerTexture(true);
        uwr.downloadHandler = downloadTexture;
        yield return uwr.SendWebRequest();
        var bytes = uwr.downloadHandler.data;

        Texture2D texture = Texture2DExt.CreateTexture2DFromWebP(bytes, lMipmaps: true, lLinear: true, lError: out Error lError);

        if (lError == Error.Success)
        {
            actionResult?.Invoke(texture);
        }
        else
        {
            Debug.LogError("Webp Load Error : " + lError.ToString());
        }
    }


    private bool checkImage(byte[] pngData)
    {

        if (pngData == null)
            return false;
        if (pngData.Length > 4)
        {
            string fileHead = pngData[0].ToString() + pngData[1].ToString();
            string flieTail = pngData[pngData.Length - 2].ToString() + pngData[pngData.Length - 1].ToString();

            return checkImageFileFormat(fileHead, flieTail);
        }
        else
        {
            return false;
        }

    }
    private bool checkImageFileFormat(string fileHead, string fileTail)
    {
        if ((fileHead == "255216" && fileTail == "255217") ||
            (fileHead == "13780" && fileTail == "96130"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
