using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleInfoItem : MonoBehaviour
{
    // Start is called before the first frame update
    public Text roleName;
    public Image Icon;
    public Image Hp;

    void Start()
    {
        
    }


    public void SetData(RoleData data)
    {
        roleName.text = data.RoleName;

        UnityWebRequestUtil.Instance.GetTexture(data.Img, (tex) =>
        {
            //´´½¨sprite
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            Icon.sprite = sprite;
            Resources.UnloadUnusedAssets();
        });
    }



    public void UpdateHp(float i)
    {
        Hp.fillAmount = i;
    }
}
