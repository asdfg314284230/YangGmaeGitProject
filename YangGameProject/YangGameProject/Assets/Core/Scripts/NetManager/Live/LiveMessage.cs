using System;
public class LiveMessage : MessageBase
{
    public string Message { get; set; }
    public string ImgUrl;
    public LiveMessage(string text)
    {
        var split = text.Split('\0');
        try
        {
            this.UID = split[0];
            this.Name = split[1];
            this.Message = split[2];
            this.ImgUrl = split[3];
        }
        catch (Exception ex)
        {
            throw new Exception($"消息解析失败: {ex.Message}");
        }
    }
}
