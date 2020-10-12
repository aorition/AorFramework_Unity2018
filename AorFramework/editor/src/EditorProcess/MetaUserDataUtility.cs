using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace Framework.Editor
{
    /// <summary>
    /// 为Meta文件中UserData字段提供封装方法
    /// </summary>
    public static class MetaUserDataUtility
    {
        /// <summary>
        /// 定义 UserData 标签 ： ORGINALSIZE -> 记录原始图片的大小
        /// </summary>
        public const string USERDATA_ORGINALSIZE = "OrginalSize";

        public static Dictionary<string,string> GetUserDataDic(AssetImporter importer)
        {
            if (importer && !string.IsNullOrEmpty(importer.userData))
            {
                string innerStr = importer.userData.Trim();
                string[] sp = innerStr.Split('|');
                Dictionary<string, string> dic = new Dictionary<string, string>();
                for (int i = 0; i < sp.Length; i++)
                {
                    string[] subSp = sp[i].Split('=');
                    if (subSp != null && subSp.Length > 1)
                        dic.Add(subSp[0], subSp[1]);
                    else
                        dic.Add(subSp[0], null);
                }
                return dic;
            }
            return null;
        }

        public static void SetUserDataDic(AssetImporter importer, Dictionary<string, string> dic, bool resave = true)
        {
            if (importer)
            {
                int idx = 0;
                StringBuilder s = new StringBuilder();
                foreach (string tag in dic.Keys)
                {
                    if (idx > 0) s.Append("|");
                    s.Append(tag + "=" + dic[tag]);
                }
                importer.userData = s.ToString();
                if (resave) importer.SaveAndReimport();
            }
        }

        //------------------------------------------------------------

        /// <summary>
        /// 尝试从obj的Meta信息信息中获取指定tag的值
        /// </summary>
        public static string TrygetUserDataByTag(UnityEngine.Object obj, string tag)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath)) return null;
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (!importer || string.IsNullOrEmpty(importer.userData)) return null;
            Dictionary<string, string> result = GetUserDataDic(importer);
            if(result != null && result.ContainsKey(tag))
            {
                return result[tag];
            }
            return null;
        }

    }
}
