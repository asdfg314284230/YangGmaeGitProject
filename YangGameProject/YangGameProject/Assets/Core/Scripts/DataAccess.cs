public class DataAccess
{

    public static GameModel gameModel;
    public static AuthLoginModel authLoginModel;

    public static void Init()
    {
        gameModel = new GameModel();
        authLoginModel = new AuthLoginModel();
    }

    public static void InitData()
    {
        gameModel.InitData();
        authLoginModel.InitData();
    }

    public static void Clear()
    {
        gameModel.Clear();
        authLoginModel.Clear();
    }

}
