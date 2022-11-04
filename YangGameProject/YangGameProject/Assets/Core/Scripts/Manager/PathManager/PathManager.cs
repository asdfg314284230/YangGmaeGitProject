using System.IO;
using UnityEngine;

public class PathManager
{

    public static string AB_BattleScene = "resab/battlescene_";
    public static string AB_FightRoleSpine = "resab/spine_";
    public static string AB_UIRoleSpine = "resab/uispine_";
    public static string AB_SpineRes = "resab/spineres_";
    public static string AB_FightSkillEffect = "resab/effect_prefab_skill_";
    public static string AB_SpineBuff = "resab/spinebuff_";
    public static string AB_PlotTextureBg = "resab/plottexturebg_";
    public static string AB_PlotProp = "resab/plotprops_prefabs_";
    public static string AB_UI_Effect = "resab/effect_prefab_ui_";
    public static string AB_SpecialUI_Effect = "resab/effect_prefab_specialui_";
    public static string AB_PlotRole = "resab/plotrole_prefabs_";
    public static string AB_RoleCard = "resab/rolecards_";
    public static string AB_RoleCardPose = "resab/rolecardspose_";
    public static string AB_FightRoleChange = "resab/fightrolechange_";
    public static string AB_UIMainBg = "resab/mainbg_";
//     public static string FileBasePath()
//     {
// #if UNITY_ANDROID && !UNITY_EDITOR
//         string filePath = "jar:file://" + Application.dataPath + "!";
// #elif UNITY_IOS && !UNITY_EDITOR
//         string filePath = Application.dataPath;
// #elif UNITY_EDITOR
//         string filePath = Application.dataPath;
// #endif
//         return "";
//     }

    //从这里拷贝数据到Application.persistentDataPath，用来对比更新
//     public static string ResUpdateCopyFromFile()
//     {
// #if UNITY_ANDROID && !UNITY_EDITOR 
//         string filePath = StreamingAssets() + "Android/";
// #elif UNITY_IOS && !UNITY_EDITOR
//         string filePath = StreamingAssets() + "iOS/";
// #elif UNITY_EDITOR_WIN
//         string filePath = Application.dataPath + "/../AssetBundles/Android/";
// #elif UNITY_EDITOR_OSX
//         string filePath = Application.dataPath + "/../AssetBundles/iOS/";
// #endif
//         return filePath;
//     }
//     public static string StreamingAssets() //各平台StreamingAssets路径
//     {
// #if UNITY_ANDROID && !UNITY_EDITOR
//         string filePath = "jar:file://" + Application.dataPath + "!/assets/";
// #elif UNITY_IOS && !UNITY_EDITOR
//         string filePath = Application.dataPath + "/Raw/";
// #elif UNITY_EDITOR
//         string filePath = Application.dataPath + "/StreamingAssets/";
// #endif
//         return filePath;
//     }

//     public static string ABFilePath()
//     {
// #if UNITY_ANDROID && !UNITY_EDITOR
//         //string filePath = StreamingAssets() + "Android/";
//         string filePath = Application.persistentDataPath + "/Android/";
// #elif UNITY_IOS && !UNITY_EDITOR
//         string filePath = StreamingAssets() + "iOS/";
//         //string filePath = Application.persistentDataPath + "/iOS/";
// #elif UNITY_EDITOR_WIN
//         string filePath = Application.dataPath + "/../AssetBundles/Android/";
// #elif UNITY_EDITOR_OSX
//         string filePath = Application.dataPath + "/../AssetBundles/iOS/";
// #endif
//         return filePath;
//     }


//     public static string PlatFileName()
//     {
// #if UNITY_ANDROID 
//         string fileName = "Android";
// #elif UNITY_IOS
//         string fileName = "iOS";
// #elif UNITY_EDITOR
//         string fileName = "Android";
// #endif
//         return fileName;
//     }



}
