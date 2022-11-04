using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowItemManager : SingletonClass<ShowItemManager>
{
    public List<DanMuItem> itemListPool = new List<DanMuItem>();
    RectTransform danmuPanel;


    public void Init()
    {
        danmuPanel = GameObject.Find("DanMuPanel").GetComponent<RectTransform>();
    }

    public void Clear()
    {
        itemListPool.Clear();
    }


    public void SetItemPos(Transform pos, RoleData data)
    {
        DanMuItem o = GetItem();
        o.Bind(data, pos);
    }


    // 从池子里面获取一个Item
    private DanMuItem GetItem()
    {

        bool IsGet = false;

        foreach (var item in itemListPool)
        {
            if (item.gameObject.activeInHierarchy == false)
            {
                IsGet = true;
                item.gameObject.SetActive(true);
                return item;
            }
        }

        // 没获取到
        if (!IsGet)
        {
            GameObject m_item = UGUITool.InstantiateObject("DanMuItem");
            DanMuItem danMuItem = m_item.GetComponent<DanMuItem>();
            m_item.gameObject.SetActive(true);
            m_item.transform.SetParent(danmuPanel);
            itemListPool.Add(danMuItem);
            return danMuItem;
        }

        return null;
    }

}
