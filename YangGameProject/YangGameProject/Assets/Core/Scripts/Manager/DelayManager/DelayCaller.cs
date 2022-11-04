

using UnityEngine;
using System.Collections;

public class DelayCaller : MonoBehaviour
{
    //唯一ID
    public string ID;

    public float time;
    public float finishTime;
    public bool isFrameMode = false;
    private bool isFinished = true;
    //不受timeScale影响的延迟模式
    public bool isRealTimeMode = false;
    public DelayManager.DelayCallBack onFinishCallBack;
    public DelegateEnums.NoneParam fn_None;
    public DelegateEnums.DataParam fn_Data;
    public object data;
    //全局时间缩放，当设置后，所有延迟的caller也会被乘以系数
    public static float TimeScale = 1f;
#if UNITY_EDITOR
    //获取调用堆栈
    public string callFunc = "";
#endif

    //****  从依赖协程，变更为update检测，为了可以在过程中控制时间缩放，以及后台运行的crash 2018-4-30  by a5

    void Update()
    {
        if (isFinished) { return; }

        //按帧的模式
        if (isFrameMode)
        {
            time += 1f * TimeScale;
            if (time >= finishTime)
            {
                this.isFinished = true;
                exec();
            }
        }
        else
        {
            if (isRealTimeMode)
            {
                time += Time.unscaledDeltaTime;
            }
            else
            {
                time += Time.deltaTime * TimeScale;
            }

            if (time >= finishTime)
            {
                this.isFinished = true;
                exec();
            }
        }

    }

    private void exec()
    {
        if (fn_None != null)
        {
            DelegateEnums.NoneParam tmp = this.fn_None;
            onFinishCallBack(this);
            tmp();
        }
        else
        {
            DelegateEnums.DataParam tmp = this.fn_Data;
            onFinishCallBack(this);
            tmp(data);
        }
    }
    public void clear()
    {
        this.fn_None = null;
        this.fn_Data = null;
        this.data = null;
        this.time = this.finishTime = 0;
    }

    //不带参数的
    public void addDelay(float t, DelegateEnums.NoneParam fn, bool realTimeMode = false)
    {
#if UNITY_EDITOR
        //获取调用堆栈
        callFunc = new System.Diagnostics.StackTrace().ToString();
#endif

        this.isRealTimeMode = realTimeMode;
        this.isFrameMode = t < 0;
        time = 0;
        finishTime = isFrameMode ? -t : t;
        this.fn_None = fn;

        this.isFinished = false;
    }

    //带参数的
    public void addDelay(float t, DelegateEnums.DataParam fn, object data, bool realTimeMode = false)
    {
#if UNITY_EDITOR
        //获取调用堆栈
        callFunc = new System.Diagnostics.StackTrace().ToString();
#endif
        this.isRealTimeMode = realTimeMode;
        this.isFrameMode = t < 0;
        time = 0;
        finishTime = isFrameMode ? -t : t;
        this.fn_Data = fn;
        this.data = data;

        this.isFinished = false;
    }
}
