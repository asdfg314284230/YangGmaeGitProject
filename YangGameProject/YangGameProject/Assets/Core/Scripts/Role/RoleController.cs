using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RoleController : MonoBehaviour
{
    // Start is called before the first frame update

    RoleData _roleData;
    Text _RoleName;
    Text _Info;
    Image _Bg;
    Animator _Animator;


    private void Awake()
    {
        _RoleName = transform.Find("Canvas/RoleName").GetComponent<Text>();
        _Info = transform.Find("Canvas/Bg/Info").GetComponent<Text>();
        _Bg = transform.Find("Canvas/Bg").GetComponent<Image>();
        _Animator = GetComponent<Animator>();


        _Animator.speed = 0.5f;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 随机取一个点移动
    }

    public void Bind(RoleData data)
    {
        _roleData = data;
        _RoleName.text = data.RoleName;
        //_Info.text = data.Info;

        //if (data.Info != string.Empty)
        //{
        //    ShowInfo(data.Info);
        //}
    }

    /// <summary>
    /// 放大
    /// </summary>
    public void RoleMax()
    {
        transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        ToolKit.DelayCall(3f, () =>
        {
            transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        });
    }


    /// <summary>
    /// 切换动画
    /// </summary>
    /// <param name="aniNmae"></param>
    public void SwitchAni(string aniNmae)
    {
        _Animator.Play(aniNmae);
    }


    /// <summary>
    /// 播放弹幕
    /// </summary>
    /// <param name="str"></param>
    public void ShowInfo(string str)
    {
        _Bg.gameObject.SetActive(true);
        _Info.text = str;

        ToolKit.DelayCall(3, () =>
        {
            _Bg.gameObject.SetActive(false);
        });
    }


    /// <summary>
    /// 随机移动
    /// </summary>
    public void RandomMove()
    {
        Vector3 v3 = new Vector3(Random.Range(DataAccess.gameModel._MinX, DataAccess.gameModel._MaxX), Random.Range(DataAccess.gameModel._MinY, DataAccess.gameModel._MaxY));
        transform.DOMove(v3, 3);
    }

}
