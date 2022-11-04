using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DrawImageManager : SingletonClass<DrawImageManager>
{
    public Dictionary<string, Sprite[]> atlasManage = new Dictionary<string, Sprite[]>();

    public void ClearAtlas()
    {
        atlasManage.Clear();
    }
    /// <summary>
    /// 加载图集
    /// </summary>
    /// <param name="spriteName">图集名称</param>
    private Sprite[] LoadAtals(string atlasName)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("GUI/" + atlasName);
        atlasManage.Add(atlasName, sprites);
        return sprites;
    }


    //加载图集上的一个精灵  
    public Sprite LoadAtlasSprite(string atlasName, string spriteName)
    {
        if (string.IsNullOrEmpty(atlasName) || string.IsNullOrEmpty(spriteName))
        {
            Log.Error(string.Format("参数为空 atlasName == {0},spriteName == {1}", atlasName, spriteName));
            return null;
        }
        Sprite[] sprites = GetAtals(atlasName);
        foreach (var item in sprites)
        {
            if (item.name == spriteName)
            {
                return item;
            }
        }
        Log.Error(atlasName + " 不存在这个精灵 " + spriteName);
        return null;
    }

    private Sprite[] GetAtals(string atlasName)
    {
        Sprite[] sprites;
        if (!atlasManage.ContainsKey(atlasName))
        {
            sprites = LoadAtals(atlasName);
        }
        else
        {
            sprites = atlasManage[atlasName];
        }
        if (sprites == null)
        {
            Log.Error(string.Format("图集不存在, Atlas_Name = {0}", atlasName));
            return null;
        }
        return sprites;
    }
}