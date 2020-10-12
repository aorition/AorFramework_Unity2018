using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System;

public class Tools
{

    //暂时禁用
    //public static object[] CallMethod(string module, string func, params object[] args)
    //{
    //    LuaManager luaMgr = AppFacade.Instance.GetManager<LuaManager>();
    //    if (luaMgr == null) return null;
    //    return luaMgr.CallFunction(module + "." + func, args);
    //}

    public static string GetOS()
    {
#if UNITY_STANDALONE_WIN
        if (IntPtr.Size == 4) return "Win"; //32 bit
        else if (IntPtr.Size == 8) return "Win64"; //64 bit
#elif UNITY_ANDROID
        return "Android";
#elif UNITY_IPHONE
        return "iOS";
#endif
        return string.Empty;
    }

    public static string UpdateviewResPath
    {
        get
        {
            return DataPath + "UpdateviewRes/";
        }
    }

    public static string CarouselResPath {
        get {
            return DataPath + "CarouselRes/";
        }
    }

    public static string LuaResPath {
        get {
            return DataPath + "Lua/";
        }
    }

    public static string CommonResPath {
        get {
            return ABResPath + "Common/";
        }
    }

    public static string ModulesResPath
    {
        get
        {
            return ABResPath + "Modules/";
        }
    }

    /// <summary>
    /// 取得数据存放目录
    /// </summary>
    public static string ABResPath
    {
        get 
        {

   //         string game = GameSetting.AppName.ToLower();
   //         if (Application.isMobilePlatform)
   //         {
   //             return DataPath + game + "/";
   //         }
   //         if (Application.platform == RuntimePlatform.OSXEditor)
   //         {
   //             return DataPath + game + "/";
   //         }
			//return DataPath + GetOS() + "/";

            return DataPath + "AssetBundles/";

        }
    }

    public static string DataPath {
        get
        {
            if (Application.isMobilePlatform)
            {
                if(Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    if(!Directory.Exists(Application.persistentDataPath + "/caches"))
                        Directory.CreateDirectory(Application.persistentDataPath + "/caches");
                    return Application.persistentDataPath + "/caches/";
                }
                return Application.persistentDataPath + "/";
            }
            return LocalStreamingAssets;
        }
    }

    private static string m_localStreamingAssets;
    private static string LocalStreamingAssets
    {
        get {
            if (string.IsNullOrEmpty(m_localStreamingAssets))
            {
                m_localStreamingAssets = FullPathHeader + "LocalStreamingAssets/";
            }
            return m_localStreamingAssets;
        }
    }

    private static string m_FullPathHeader;
    public static string FullPathHeader
    {
        get
        {
            if (string.IsNullOrEmpty(m_FullPathHeader))
                m_FullPathHeader = Application.dataPath.Replace("Assets", "");
            return m_FullPathHeader;
        }
    }

    /// <summary>
    /// 应用程序内容路径
    /// </summary>
    public static string AppContentPath
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }

    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    public static string md5file(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }

    public static void ChangeChildLayer(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        for (int i = 0; i < t.childCount; ++i)
        {
            Transform child = t.GetChild(i);
            child.gameObject.layer = layer;
            ChangeChildLayer(child, layer);
        }
    }

    public static void AddChildToTarget(Transform target, Transform child)
    {
        child.SetParent(target, false);
        child.localScale = Vector3.one;
        child.localPosition = Vector3.zero;
        child.localRotation = Quaternion.identity;
        ChangeChildLayer(child, target.gameObject.layer);
    }

}
