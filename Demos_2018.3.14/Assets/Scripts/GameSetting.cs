using UnityEngine;
using System;
using System.Collections.Generic;

public class GameSetting
{

	//应用程序名称
	public static string AppName;

    //帧率限制
    public static int GameFrameRate;
	//渠道(长度32之内)
	public static string Agent;

    public static string SignCode;

    public static string Account;
    public static int SocketPort; //Socket服务器端口

    public static bool ErrorReport; //发生错误时弹窗报告

    public static string IPaddress; //主服务器上的ip地址 
	//public static bool ShowFPS = true; //是否显示FPS    
	public static bool DevelopMode; // 开发模式 

    public static bool UpdateMode;
    //public static bool LuaByteMode; //Lua字节码模式-默认关闭

    public static bool LuaBundleMode; //Lua代码AssetBundle模式
	// //public static bool Logdebug; //输出日志文件
    // public static string WebUrlConfig;
	// public static string DownloadURL; //游戏下载地址
	// public static string ServerBundleVersion; //服务器上最新版本号
	// public static string ClientIp; //客户端IP
    // 
    public static bool EnableLuaDebug;
    // 
    // 
    // public static string H5URL;
    // public static string[] H5Options;
    // 
    // public static string OtherParamsJSON;
    // //public static Dictionary<string, object> OtherParams;
    // 
    // public static ResourcelistAsset Resourcelist; //资源清单
    // public static Dictionary<string, int> ResSearchDic; //资源路径查找字典
}
