
public static class DelegateEnums
{
    public delegate void NoneParam();
    public delegate void DataParam(object data);
    /// <summary>
    /// 泛型参数 嘿嘿嘿
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    public delegate void DataParamT<T>(T data);

}
