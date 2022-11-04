using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomPanel : MonoBehaviour
{
    // Start is called before the first frame update
    Transform content;
    GameObject plantsCustomPrefab;
    GameObject zombiesCustomPrefab2;
    GameObject[] allPlantCard;
    Button sure, close;
    void Start()
    {
        transform.Find("close").GetComponent<Button>().onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
        transform.Find("sure").GetComponent<Button>().onClick.AddListener(() =>
        {
            ClickSure();
        });
        content = transform.Find("Scroll/Viewport/Content");
        plantsCustomPrefab = Resources.Load<GameObject>("Prefabs/custom/customItem");
        
        for (int i = 0; i < allPlantCard.Length; i++)
        {
            GameObject item = Instantiate(plantsCustomPrefab);
            CustomItem c = item.GetComponent<CustomItem>();
            c.SetData(allPlantCard[i]);
            item.transform.parent = content;
        }
    }


    public void GetData(GameObject[]all)
    {
        allPlantCard = all;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickSure()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            CustomItem c = content.GetChild(i).GetComponent<CustomItem>();
            c.SaveData(); 
        }
        gameObject.SetActive(false);
    }
}
