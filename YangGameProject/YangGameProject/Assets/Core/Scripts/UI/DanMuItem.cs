using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DanMuItem : MonoBehaviour
{
    // Start is called before the first frame update

    public Text info;
    public Image Bg;

    Transform mPos;

    public void Bind(RoleData data, Transform pos)
    {

        info.text = string.Empty;

        info.text = data.Info;


        if (pos == null)
        {
            return;
        }

        mPos = pos;

        Invoke("HandleClose", 5f);
    }


    void Update()
    {
        if (this.gameObject.activeInHierarchy && mPos != null)
        {
            Vector3 newPos = WorldToUIPos(mPos.position);
            gameObject.transform.position = new Vector3(newPos.x, newPos.y + 50, newPos.z);
        }
    }


    public Vector3 WorldToUIPos(Vector3 position)
    {
        Debug.Log(position);
        Vector3 tempMain2Screenpos = Camera.main.WorldToScreenPoint(position);
        return tempMain2Screenpos;
    }


    void HandleClose()
    {
        this.gameObject.SetActive(false);
    }

}
