

public class GEvent
{
    public string type = "";
    public object data;
	public object[] args;

    public GEvent(string _type, object _data = null)
    {
        type = _type;
        data = _data;
    }

    public GEvent(string _type, params object[] _args)
    {
        type = _type;
        args = _args;
        if (_args != null && _args.Length > 0)
        {
			data = _args[0];
        }
    }

    public T GetData<T>(int idx = 0)
    {
        if (args != null && args.Length > idx && args[idx] is T)
            return (T)args[idx];

        return default(T);
    }
}