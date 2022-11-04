using System;

//单例类
public class SingletonClass<T> where T : new()
{
    private static readonly object _lock;
    private static T _instance;

    static SingletonClass()
    {
        SingletonClass<T>._lock = new object();
    }

    protected SingletonClass()
    {
    }

    public static bool Exists
    {
        get
        {
            return (SingletonClass<T>._instance != null);
        }
    }

    public static T Instance
    {
        get
        {
            if (_instance == null) {
                object obj2 = _lock;
                lock (obj2) {
                    if (_instance == null) {
                        T local = default(T);
                        _instance = (local == null) ? Activator.CreateInstance<T>() : default(T);
                    }
                }
            }
            return _instance;
        }
    }
}