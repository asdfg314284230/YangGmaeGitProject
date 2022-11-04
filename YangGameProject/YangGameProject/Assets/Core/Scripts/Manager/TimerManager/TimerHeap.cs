using System;
using System.Diagnostics;
using System.Threading;

/// <summary>
/// 定时触发器
/// </summary>
public class TimerHeap
{
    #region 变量

    private static uint m_nNextTimerId;
    private static uint m_unTick;
    private static KeyedPriorityQueue<uint, AbsTimerData, ulong> m_queue;
    private static Stopwatch m_stopWatch;
    private static readonly object m_queueLock = new object();


    private static float m_deltaSystemTimer = 1000f;
    private static float m_timeSystemTimer;
    private static float m_timeSystemTimerTmp;
    private static float m_checkPerTime = 10.0f;
    private static float m_checkTimeTmp;
    private static float m_invokeReaptingTime = 0.02f;

    private static Action m_cheatHandler;
    private static bool m_cheat;

    #endregion

    #region 构造函数

    /// <summary>
    /// 私有构造函数，封闭实例化。
    /// </summary>
    private TimerHeap() { }

    /// <summary>
    /// 默认构造函数
    /// </summary>
    static TimerHeap()
    {
        m_cheat = false;

        m_queue = new KeyedPriorityQueue<uint, AbsTimerData, ulong>();
        m_stopWatch = new Stopwatch();

        System.Timers.Timer t = new System.Timers.Timer(m_deltaSystemTimer);
        t.Elapsed += new System.Timers.ElapsedEventHandler(theout);//到达时间的时候执行事件； 
        t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)； 
        t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
    }

    #endregion

    #region 逻辑

    public static void theout(object source, System.Timers.ElapsedEventArgs e)
    {
        m_timeSystemTimer += m_deltaSystemTimer / 1000f;
        m_timeSystemTimerTmp = m_timeSystemTimer;
    }

    #region AddTimer

    /// <summary>
    /// 添加定时对象
    /// </summary>
    /// <param name="delay">延迟启动时间。（毫秒）</param>
    /// <param name="interval">重复间隔，为零不重复。（毫秒）</param>
    /// <param name="handler">定时处理方法</param>
    /// <returns>定时对象Id</returns>
    public static uint AddTimer(uint delay, int interval, Action handler)
    {
        //起始时间会有一个tick的误差,tick精度越高,误差越低
        var p = GetTimerData(new TimerData(), delay, interval);
        p.Action = handler;
        return AddTimer(p);
    }

    /// <summary>
    /// 添加定时对象
    /// </summary>
    /// <typeparam name="T">参数类型1</typeparam>
    /// <param name="delay">延迟启动时间。（毫秒）</param>
    /// <param name="interval">重复间隔，为零不重复。（毫秒）</param>
    /// <param name="handler">定时处理方法</param>
    /// <param name="arg1">参数1</param>
    /// <returns>定时对象Id</returns>
    public static uint AddTimer<T>(uint delay, int interval, Action<T> handler, T arg1)
    {
        var p = GetTimerData(new TimerData<T>(), delay, interval);
        p.Action = handler;
        p.Arg1 = arg1;
        return AddTimer(p);
    }

    /// <summary>
    /// 添加定时对象
    /// </summary>
    /// <typeparam name="T">参数类型1</typeparam>
    /// <typeparam name="U">参数类型2</typeparam>
    /// <param name="delay">延迟启动时间。（毫秒）</param>
    /// <param name="interval">重复间隔，为零不重复。（毫秒）</param>
    /// <param name="handler">定时处理方法</param>
    /// <param name="arg1">参数1</param>
    /// <param name="arg2">参数2</param>
    /// <returns>定时对象Id</returns>
    public static uint AddTimer<T, U>(uint delay, int interval, Action<T, U> handler, T arg1, U arg2)
    {
        var p = GetTimerData(new TimerData<T, U>(), delay, interval);
        p.Action = handler;
        p.Arg1 = arg1;
        p.Arg2 = arg2;
        return AddTimer(p);
    }

    /// <summary>
    /// 添加定时对象
    /// </summary>
    /// <typeparam name="T">参数类型1</typeparam>
    /// <typeparam name="U">参数类型2</typeparam>
    /// <typeparam name="V">参数类型3</typeparam>
    /// <param name="delay">延迟启动时间。（毫秒）</param>
    /// <param name="interval">重复间隔，为零不重复。（毫秒）</param>
    /// <param name="handler">定时处理方法</param>
    /// <param name="arg1">参数1</param>
    /// <param name="arg2">参数2</param>
    /// <param name="arg3">参数3</param>
    /// <returns>定时对象Id</returns>
    public static uint AddTimer<T, U, V>(uint delay, int interval, Action<T, U, V> handler, T arg1, U arg2, V arg3)
    {
        var p = GetTimerData(new TimerData<T, U, V>(), delay, interval);
        p.Action = handler;
        p.Arg1 = arg1;
        p.Arg2 = arg2;
        p.Arg3 = arg3;
        return AddTimer(p);
    }

    #endregion

    #region DelTimer

    /// <summary>
    /// 删除定时对象
    /// </summary>
    /// <param name="timerId">定时对象Id</param>
    public static void DelTimer(uint timerId)
    {
        lock (m_queueLock)
            m_queue.Remove(timerId);
    }

    #endregion

    /// <summary>
    /// 立即执行某个timer的回调
    /// </summary>
    /// <param name="timerId"></param>
    public static void ExecuteImmediately(uint timerId)
    {
        m_queue.Get(timerId).DoAction();
        DelTimer(timerId);
    }

    public static void AddCheatCheckHandler(Action handler)
    {
        m_cheatHandler = handler;
    }

    /// <summary>
    /// 检测是否用了加速器
    /// </summary>
    private static void CheckCheat()
    {
        if (m_timeSystemTimerTmp - m_timeSystemTimer > 5.0f)
        {
            m_cheat = true;
            m_cheatHandler();
        }
    }

    /// <summary>
    /// 定时器暂停功能
    /// </summary>
    private static bool isPause = false;
    public static void Pause(bool _isPause)
    {
        isPause = _isPause;
    }

    /// <summary>
    /// 周期调用触发任务
    /// </summary>
    public static void Tick()
    {
        if (isPause) return;
        if (!m_stopWatch.IsRunning)
            m_stopWatch.Start();

        m_unTick = (uint)(UnityEngine.Time.unscaledTime * 1000);

        m_checkTimeTmp += m_invokeReaptingTime;
        m_timeSystemTimerTmp += m_invokeReaptingTime;
        if (m_cheat == false)
        {
            if (m_checkTimeTmp > m_checkPerTime)
            {
                m_checkTimeTmp = 0;
                CheckCheat();
            }
        }

        bool profilerSample = false;// UnityEngine.Debug.isDebugBuild || UnityEngine.Application.isEditor;
        while (m_queue.Count != 0)
        {
            AbsTimerData p;
            lock (m_queueLock)
                p = m_queue.Peek();
            if (m_unTick < p.UnNextTick)
            {
                break;
            }
            lock (m_queueLock)
                m_queue.Dequeue();
            if (p.NInterval > 0)
            {
                p.UnNextTick += (ulong)p.NInterval;
                lock (m_queueLock)
                    m_queue.Enqueue(p.NTimerId, p, p.UnNextTick);
                if (profilerSample)
                {
                    var name = string.IsNullOrEmpty(p.StackTrack) ? p.Action.Method.Name : p.StackTrack;
                    UnityEngine.Profiling.Profiler.BeginSample(name);
                }
                p.DoAction();
                if (profilerSample)
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                }
            }
            else
            {
                if (profilerSample)
                {
                    var name = string.IsNullOrEmpty(p.StackTrack) ? p.Action.Method.Name : p.StackTrack;
                    UnityEngine.Profiling.Profiler.BeginSample(name);
                }
                p.DoAction();
                if (profilerSample)
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                }
            }
        }
    }

    /// <summary>
    /// 重置定时触发器
    /// </summary>
    public static void Reset()
    {
        m_unTick = 0;
        m_nNextTimerId = 0;
        lock (m_queueLock)
            while (m_queue.Count != 0)
                m_queue.Dequeue();
    }

    private static uint AddTimer(AbsTimerData p)
    {
        //if (UnityEngine.Debug.isDebugBuild)
        //{
        //    var frame = new StackFrame(2, true);
        //    var fileName = UnityEngine.Application.isMobilePlatform ? frame.GetFileName().Replace('\\', '/') : frame.GetFileName();
        //    p.StackTrack = string.Format("[{0}, {1}]", System.IO.Path.GetFileName(fileName), frame.GetFileLineNumber());
        //}

        lock (m_queueLock)
            m_queue.Enqueue(p.NTimerId, p, p.UnNextTick);
        return p.NTimerId;
    }

    private static T GetTimerData<T>(T p, uint delay, int interval) where T : AbsTimerData
    {
        p.NInterval = interval;
        p.NTimerId = ++m_nNextTimerId;
        p.UnNextTick = m_unTick + 1 + delay;
        return p;
    }

    #endregion
}