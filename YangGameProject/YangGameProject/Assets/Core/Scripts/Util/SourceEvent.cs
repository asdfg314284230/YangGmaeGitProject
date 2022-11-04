using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceEvent : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator ani;
    void Start()
    {

    }



    public void PlaySource()
    {
        this.gameObject.GetComponent<AudioSource>().Play();
    }


    public void ResultAni()
    {
        ani.SetInteger("Animation", 0);
    }


}
