using UnityEngine;
using Stardom;
using Stardom.Core.Model;
using Stardom.Core.XProto;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using System.IO;
using System.Text;
using System;

public partial class GameModel : BaseModel<GameModel>
{

    // 玩家列表
    public Dictionary<RoleData, GameObject> roleDic = new Dictionary<RoleData, GameObject>();

    public Dictionary<RoleData, GameObject> redDic = new Dictionary<RoleData, GameObject>();
    public Dictionary<RoleData, GameObject> buleDic = new Dictionary<RoleData, GameObject>();



    public float _MaxX = 2;
    public float _MaxY = 2.5f;
    public float _MinX = -2f;
    public float _MinY = -3f;

    protected override void InitAddTocHandler()
    {

    }


    public override void InitData()
    {

    }


    public override void Clear()
    {

    }

    public void ClearData()
    {

    }


    public bool GetPlayerIsGet(string Name)
    {
        foreach (var item in roleDic)
        {
            if (item.Key.RoleName == Name && item.Value != null)
            {
                return true;
            }
        }
        return false;
    }


}

// 角色实例数据
public class RoleData
{
    public string RoleName;
    public string Info;
    public int AllContribute;
    public string Img;
    public bool isRed;

}

