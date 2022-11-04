using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

//不可改变枚举顺序
public enum ListDir
{
    Right,
    Down,
    Left,
    Up,
}

/// <summary>
/// 格子列表控制器
/// 1.可以设置动画
/// </summary>
/// <summary>
/// UI异步加载策略思考
/// 1.第一步.每帧加载数量限制
/// 2.第二步.如果之前加载未完成,则跳过改帧加载请求,todo.
/// 3.缓存中的格子已经在显示状态的就立刻返回
/// </summary>
public class ListPool : MonoBehaviour
{
    #region 变量

    [HideInInspector]
    private int MAX = 100;//一帧最大加载数量
    private int mLoadNumberFrame = 100;//一帧加载数量,默认为100
    public int LoadNumberFrame
    {
        get { return mLoadNumberFrame; }
        set
        {
            mLoadNumberFrame = Mathf.Clamp(value, 1, MAX);
        }
    }

    [HideInInspector]
    public float LoadInterval = 0;//限制加载时间间隔

    public GameObject temp;//不可改变, 防止Prefab上引用丢失
    private string temp_prefabname;
    [HideInInspector]
    public List<GameObject> Items = new List<GameObject>();

    [HideInInspector]
    public bool isAutoCalcalateLayout = false;//是否自己计算ContentSize(false表示外部自己计算, 这里不自动计算)

    #endregion

    #region INIT

    void Start()
    {
        if (temp != null)
            temp.SetActive(false);
    }

    #endregion

    #region 外部调用

    //设置Asset
    public void SetItem(string prefab_name)
    {
        temp_prefabname = prefab_name;
    }

    /// <summary>
    /// 创建指定数量格子列表
    /// 异步加载思路：
    /// (1)创建格子->(2)执行创建后的逻辑回调,执行逻辑->(3)执行动画播放
    /// </summary>
    /// <param name="num"></param>
    private bool IsContainLayoutWidget = false;//是否有Layout组件
    public void Create(int num, Action<int> loadSuccess)
    {
        if (GetComponent<LayoutGroup>() != null)
            IsContainLayoutWidget = true;

        LoadSuccess = loadSuccess;
        mNum = num;
        mIndex = 0;
        HideRedundance();//隐藏冗余格子
    }

    public void Create<T>(IList<T> datas, Action<GameObject> callback = null, Action<GameObject> finalCallback = null)
    {
        if (datas == null) return;
        if (GetComponent<LayoutGroup>() != null)
            IsContainLayoutWidget = true;

        LoadSuccess = (index) =>
        {
            if (index < Items.Count)
            {
                IPoolItem<T> cmp = Items[index].GetComponent<IPoolItem<T>>();
                if (cmp != null)
                    cmp.BindData(datas[index]);
                if (callback != null)
                    callback(Items[index]);
            }
            if (index == datas.Count - 1)
            {
                if (finalCallback != null)
                    finalCallback(Items[index]);
            }
        };
        mNum = datas.Count;
        mIndex = 0;
        HideRedundance();//隐藏冗余格子
    }

    public T GetItemComponent<T>(int index) where T : Component
    {
        if (index < Items.Count)
        {
            if (Items[index] == null) return null;
            T cmp = Items[index].GetComponent<T>();
            if (cmp != null) return cmp;
        }
        return null;
    }

    public void Clear()
    {
        mNum = 0;
        mIndex = 0;
        HideRedundance();//隐藏冗余格子
    }

    //清除对象池
    public void Release()
    {
        mNum = 0;
        for (int i = 0; i < Items.Count; i++)
            MonoBehaviour.DestroyImmediate(Items[i]);
        Items.Clear();
    }

    #endregion



    #region 同步创建实例对象

    private int mNum = 0;
    private int mIndex = 0;
    private float mDeltaTime;
    void Update()
    {
        if (LoadInterval <= 0)
        {
            //已经打开的执行加速
            while (mIndex < mNum && IsInPoolWithActive(mIndex))
                DoInstantiateJustOne();

            LoadOnce();
        }
        else
        {
            mDeltaTime += Time.deltaTime;
            if (mDeltaTime <= LoadInterval)
                return;

            mDeltaTime = 0;
            LoadOnce();
        }
    }

    //加载一次
    private void LoadOnce()
    {
        int load_num = Mathf.Min(LoadNumberFrame, mNum - mIndex);
        for (int index = 0; index < load_num; ++index)
            DoInstantiateJustOne();
    }

    //加载一个
    private void DoInstantiateJustOne()
    {
        try
        {
            GetInstantiate(mIndex);
            mIndex++;
            //SetContentSize(mIndex);
        }
        catch (System.Exception ex)
        {
            Log.Error(ex.Message + "\n" + ex.StackTrace);
            mIndex++;
            throw ex;
        }
    }

    private bool IsInPoolWithActive(int index)
    {
        if (index < Items.Count && Items[index].activeSelf)
            return true;
        else
            return false;
    }

    private void GetInstantiate(int index)
    {
        if (index < Items.Count)//[1.1]创建的数量[<=]当前数量, 使用原有实例对象
        {
            Items[index].SetActive(true);
        }
        else//[1.2]创建的数量[>]当前数量, 创建新实例对象
        {
            GameObject goInstantiate = DoInstantiate(index);
            if (goInstantiate != null)
            {
                goInstantiate.SetActive(true);
                Items.Add(goInstantiate);
            }
        }

        InstantiateSuccess(index);
    }

    private Action<int> LoadSuccess;
    private void InstantiateSuccess(int index)
    {
        //1.1动画播放
        DoAnimation(index);

        //1.2逻辑回调
        if (LoadSuccess != null)
            LoadSuccess(mIndex);
    }

    /// <summary>
    /// 隐藏冗余格子
    /// </summary>
    private void HideRedundance()
    {
        for (int index = mNum; index < Items.Count; index++)
        {
            GameObject item = Items[index];
            if (item != null)
                item.SetActive(false);
        }
    }

    /// <summary>
    /// 创建实例对象
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private GameObject DoInstantiate(int index)
    {
        GameObject goInstantiate = null;
        if (temp != null)
        {
            goInstantiate = UGUITool.AddChild(this.gameObject, temp, false);
            goInstantiate.name = temp.name + index.ToString();
        }
        else
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(temp_prefabname))
                Log.Error("temp_prefabname is null");
#endif
            goInstantiate = UGUITool.InstantiateObject(temp_prefabname);
            UGUITool.SetParent(this.gameObject, goInstantiate, false);
            goInstantiate.name = temp_prefabname + index.ToString();
        }

        //设置格子位置
        // if (isAnimation)
        // {
        //     //1.1动画控制
        //     goInstantiate.GetComponent<RectTransform>().anchoredPosition =
        //         index >= maxShow * lineContainNum
        //         ? GetPos(index)//格子不在动画范围
        //         : GetPos(index) + moveing;//格子在动画范围
        // }
        // else
        // {
        //     //非动画控制(由layout控件控制, 不需要设置位置, 影响效率)
        //     if (!IsContainLayoutWidget)
        //         goInstantiate.GetComponent<RectTransform>().anchoredPosition = GetPos(index);
        // }
        if (!IsContainLayoutWidget)
            goInstantiate.GetComponent<RectTransform>().anchoredPosition = GetPos(index);

        return goInstantiate;
    }

    #endregion

    #region 扩展

    public int lineContainNum = 1;//一排的数量
    public ListDir Dir = ListDir.Down;
    public Vector2 moveing = Vector2.right;
    public Vector2 offsets = Vector2.zero;
    public bool isAnimation;
    public int maxShow = 6;//显示范围内的排数
    public float spacing = 1;
    public float padding = 1;
    public float duration = 1f;


    /// <summary>
    /// 设置列表结点的内容范围
    /// </summary>
    /// <param name="num"></param>
    private void SetContentSize(int num)
    {
        if (!isAutoCalcalateLayout)
            return;

        RectTransform parent = GetComponent<RectTransform>();
        if (Dir == ListDir.Right || Dir == ListDir.Up)
            parent.sizeDelta = 1f * GetDir() * GetLines(num) * spacing;
        else
            parent.sizeDelta = -1f * GetDir() * GetLines(num) * spacing;
    }

    /// <summary>
    /// 获取列表数量需要的行数显示
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private int GetLines(int num)
    {
        return Mathf.CeilToInt((float)num / (float)lineContainNum);
    }

    /// <summary>
    /// 格子动画[只针对开始的显示数量的格子做格子动画, 后面一开始是不可见的, 不需要动画]
    /// </summary>
    /// <param name="num"></param>
    private void DoAnimation(int index)
    {
        // if (isAnimation)
        // {
        //     if (index >= maxShow * lineContainNum)
        //         return;

        //     RectTransform rt = Items[index].GetComponent<RectTransform>();
        //     rt.anchoredPosition = GetPos(index) + moveing;
        //     Vector3 target = rt.anchoredPosition - moveing;
        //     StartCoroutine(rt.MoveToAnchoredPosition(target, 0.3f + (float)(duration * duration * duration * index), EaseType.CubeOut));
        // }
        if (isAnimation)
        {
            CanvasGroup canvasGroup = UGUITool.AddMissComponent<CanvasGroup>(Items[index]);
            UGUITool.AddMissComponent<GraphicRaycaster>(Items[index]);
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, 0.5f);
        }
    }

    private Vector2 GetPos(int i)
    {
        Vector2 dir = GetDir();
        return (dir * spacing * (int)(i / lineContainNum)) + dir * padding + offsets;
    }

    private Vector2 GetDir()
    {
        Vector2 dir;
        switch (Dir)
        {
            case ListDir.Right:
                dir = Vector2.right;
                break;
            case ListDir.Down:
                dir = Vector2.down;
                break;
            case ListDir.Left:
                dir = Vector2.left;
                break;
            case ListDir.Up:
                dir = Vector2.up;
                break;

            default:
                dir = Vector2.down;
                break;
        }

        return dir;
    }

    #endregion
}

public interface IPoolItem<T>
{
    void BindData(T data);
}