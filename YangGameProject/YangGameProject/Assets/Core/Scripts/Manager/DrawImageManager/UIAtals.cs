using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIAtals : ScriptableObject
{
    public Texture2D mainText;
    public List<Sprite> spriteLists = new List<Sprite>();
    /// <summary>
    /// 根据图片名称获取sprite
    /// </summary>
    /// <param name="spritename">图片名称</param>
    /// <returns></returns>
    public Sprite GetSprite(string spritename)
    {
        Sprite ret = spriteLists.Find((Sprite s) => { return s.name == spritename; });
        if (ret != null)
        {
            return ret;
        }
        Log.Error(string.Format("mainText name = {0} 图集中不存在spritename == {1}", mainText.name, spritename));
        return null;
    }

    /// <summary>
    /// 设置Image的Sprite
    /// </summary>
    /// <param name="im">Image</param>
    /// <param name="spritename">图片名称</param>
    /* public void SetSprite(ref Image im, string spritename)
    {
        if (im == null)
        {
            return;
        }
        Sprite sp = GetSprite(spritename);
        if (sp != null)
        {
            im.overrideSprite = sp;
        }
        else
        {
            Log.Info(string.Format("图集没有对应的图片:{0}", spritename));
        }
    } */
}