using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using AorBaseUtility;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;


public static class GameUtility
{

#if UNITY_IPHONE
        /* Interface to native implementation */
        [DllImport("__Internal")]
        private static extern void _copyTextToClipboard(string text);
#endif

    /// <summary>
    /// 字符串复制到剪贴板
    /// </summary>
    /// <param name="copyText"></param>
    public static void CopyToClippad(string copyText)
    {
#if UNITY_EDITOR_WIN
        TextEditor t = new TextEditor();
        t.text = copyText;
        t.OnFocus();
        t.Copy();
#elif UNITY_STANDALONE_WIN
		//Window剪切板处理
		GUIUtility.systemCopyBuffer = copyText;
#elif UNITY_ANDROID
		//Android剪切板处理
		AndroidJavaObject androidObject = new AndroidJavaObject("com.apowo.clipboard.ClipboardTools");
		AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
		// 复制到剪贴板
		androidObject.CallStatic("copyTextToClipboard", activity, copyText);
#elif UNITY_IPHONE
		_copyTextToClipboard(copyText);
#endif
    }

    private static string m_LocalCacheTagHead;
    public static string LocalCacheTagHead
    {
        get
        {
            if (string.IsNullOrEmpty(m_LocalCacheTagHead))
            {
                m_LocalCacheTagHead = GameSetting.AppName + "." + GameSetting.Agent + "." + GameSetting.SignCode + ".";
            }
            return m_LocalCacheTagHead;
        }
    }

    /// <summary>
    /// 获取用户本地上次使用的URL
    /// </summary>
    /// <returns></returns>
    public static string GetLocalCache_URL()
    {
        return PlayerPrefs.GetString(LocalCacheTagHead + GlobalConst.PlayerLocalUrl);
    }
    public static void SetLocalCache_URL(string url)
    {
        PlayerPrefs.SetString(LocalCacheTagHead + GlobalConst.PlayerLocalUrl, url);
    }
    /// <summary>
    /// 获取用户本地上次使用的账户名
    /// </summary>
    /// <returns></returns>
    public static string GetLocalCache_Account()
    {
        return PlayerPrefs.GetString(LocalCacheTagHead + GlobalConst.PlayerLocalAccount);
    }
    public static void SetLocalCache_Account(string account)
    {
        PlayerPrefs.SetString(LocalCacheTagHead + GlobalConst.PlayerLocalAccount, account);
    }

    /// <summary>
    /// 获取本地保存是否启用错误报告窗口缓存数据
    /// </summary>
    public static int GetLocalCache_ErrorReport()
    {
        return PlayerPrefs.GetInt(LocalCacheTagHead + GlobalConst.EnableErrorReportWindow);
    }

    /// <summary>
    /// 设置本地保存是否启用错误报告窗口缓存数据
    /// </summary>
    public static void SetLocalCache_ErrorReport(bool enabled)
    {
        PlayerPrefs.SetInt(LocalCacheTagHead + GlobalConst.EnableErrorReportWindow, (enabled ? 1 : 0));
    }

    public static string GetLocalCache_DomainList()
    {
        return PlayerPrefs.GetString(LocalCacheTagHead + GlobalConst.DomainList);
    }

    public static void SetLocalCache_DomainList(string domainList)
    {
        PlayerPrefs.SetString(LocalCacheTagHead + GlobalConst.DomainList, domainList);
    }

    public static string GetLocalCache_DUIdentifier()
    {
        return PlayerPrefs.GetString(LocalCacheTagHead + GlobalConst.DUIdentifier);
    }

    public static void SetLocalCache_DUIdentifier(string DUIdentifier)
    {
        PlayerPrefs.SetString(LocalCacheTagHead + GlobalConst.DUIdentifier, DUIdentifier);
    }

    /// <summary>
    /// 清理PlayerPrefs所有缓存值
    /// </summary>
    public static void ClearPlayerPrefs()
    {
        //PlayerPrefs.SetString(GlobalConst.PlayerLocalAccount, "");
        //PlayerPrefs.SetString(GlobalConst.PlayerLocalUrl, "");
        PlayerPrefs.DeleteAll();
    }

    // Game IO --- --- ---

    public static string ReadText(string path)
    {
#if UNITY_IOS
        try
        {
            StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8);
            string ct = sr.ReadToEnd();
            sr.Close();
            return ct;
        }catch(Exception ex)
        {
            Debug.LogError("*** GameUtilty.ReadText Error :: " + ex.Message);
            return null;
        }
#else
        try
        {
            return AorIO.ReadStringFormFile(path);
        }catch(Exception ex)
        {
            Debug.LogError("*** GameUtilty.ReadText Error :: " + ex.Message);
            return null;
        }
#endif
    }

    public static bool SaveText(string path, string data)
    {

#if UNITY_IOS
        try
        {
            StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8);
            sw.Write(data);
            sw.Close();
            return true;
        }catch(Exception ex)
        {
            Debug.LogError("*** GameUtilty.SaveText Error :: " + ex.Message);
            return false;
        }
#else
        try
        {
            return AorIO.SaveStringToFile(path, data);
        }catch(Exception ex)
        {
            Debug.LogError("*** GameUtilty.SaveText Error :: " + ex.Message);
            return false;
        }
#endif
    }

    public static string[] ReadAllLines(string path)
    {
        return File.ReadAllLines(path);
    }

    public static byte[] ReadBytes(string path)
    {
        try
        {
            return AorIO.ReadBytesFormFile(path);
        }catch(Exception ex)
        {
            Debug.LogError("*** GameUtilty.ReadBytes Error :: " + ex.Message);
            return null;
        }
    }

    public static bool SaveBytes(string path, byte[] bytes)
    {
        try
        {
            return AorIO.SaveBytesToFile(path, bytes);
        }catch(Exception ex)
        {
            Debug.LogError("*** GameUtilty.SaveBytes Error :: " + ex.Message);
            return false;
        }
    }
    
    public static void SaveAllBytes(string path, byte[] bytes)
    {
        try
        {
            File.WriteAllBytes(path, bytes);
        }
        catch (Exception ex)
        {
            Debug.LogError("*** GameUtilty.SaveAllBytes Error :: " + ex.Message);
        }
    }

    public static void CopyFile(string path, string newPath, bool overwirte = true)
    {
        try
        {
            File.Copy(path, newPath, overwirte);
        }catch(Exception ex)
        {
            Debug.LogError("*** GameUtilty.CopyFile Error :: " + ex.Message);
        }
    }

    public static bool ExistsFile(string path)
    {
        return File.Exists(path);
    }

    public static void DeleteFile(string path)
    {
        try
        {
            File.Delete(path);
        }catch(Exception ex)
        {
            Debug.LogError("*** GameUtility.DeleteFile Error :: " + ex.Message);
        }
    }

    public static bool ExistsDir(string path)
    {
        return Directory.Exists(path);
    }

    public static void DeleteDir(string path, bool deleteInnerFiles = true)
    {
        try
        {
            Directory.Delete(path, deleteInnerFiles);
        }catch(Exception ex)
        {
            Debug.LogError("*** GameUtility.DeleteDir Error :: " + ex.Message);
        }
    }

    public static DirectoryInfo CreateDir(string path)
    {
        return Directory.CreateDirectory(path);
    }

    public static string GetDirNameUsePath(string path)
    {
        return Path.GetDirectoryName(path);
    }

    public static string[] GetDirectories(string path)
    {
        return Directory.GetDirectories(path);
    }

    public static string[] GetFilesInTopDir(string dirPath, string searchPattern)
    {
        return Directory.GetFiles(dirPath, searchPattern, SearchOption.TopDirectoryOnly);
    }

    public static string[] GetFilesInAllDir(string dirPath, string searchPattern)
    {
        return Directory.GetFiles(dirPath, searchPattern, SearchOption.AllDirectories);
    }

    //----------------------------

    public static string SaveScreenshot(string screenshotName = null)
    {
        string fileName = string.IsNullOrEmpty(screenshotName) ? "noname_screen_shot_image.png" : screenshotName;
        string path = Path.Combine(Application.persistentDataPath, fileName);
#if UNITY_5
        Application.CaptureScreenshot(fileName);
#elif UNITY_2018
        ScreenCapture.CaptureScreenshot(fileName);
#endif
        return path;
    }
    
	private static string _deviceUniqueIdentifier;
	public static string deviceUniqueIdentifier{
		get{
#if UNITY_IOS && !UNITY_EDITOR
			return _deviceUniqueIdentifier;
#else
			return SystemInfo.deviceUniqueIdentifier;
#endif
		}
		set{
#if UNITY_IOS && !UNITY_EDITOR
			_deviceUniqueIdentifier = value;
#endif
		}
	}

}
