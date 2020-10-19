using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Framework.GPUSkinning.Editor
{
    [Serializable]
    public class GPUSkinningResCacheItem
    {

        public string GUID_mat; //"LWGPUSkinningTexturen"
        public string GUID_lightMat; //"LWGPUSkinningTexturenLight"

        public string GUID_mesh;

        public string GUID_animMap;
        public Vector2[] Cache_animMap_frames;
        public string[] Cache_animMap_frameNames;

        public string MD5_animController;

    }

    /// <summary>
    ///
    /// ref_GUID -> 源文件
    /// target_GUID -> 转换后目标文件
    ///
    /// 思路:
    ///     Material: 根据预制体Mesh确定一个唯一的Mat
    ///     GPUSkinningAnim : 根据预制体绑定的AnimationController确定唯一的图
    /// 
    /// </summary>
    public class GPUSkinningResCacheAsset :ScriptableObject
    {

        private const string cacheSaveRoot = "Assets/Tools/GPUSkinningUtility";
        private const string cacheSaveFileName = "GPUSkinResUtil_Cache.asset";
        private static string cacheSavePath { get { return cacheSaveRoot + "/cfg/" + cacheSaveFileName; } }

        public static GPUSkinningResCacheAsset GetOrCreate()
        {
            GPUSkinningResCacheAsset asset = AssetDatabase.LoadAssetAtPath<GPUSkinningResCacheAsset>(cacheSavePath);
            if(!asset)
            {
                asset = ScriptableObject.CreateInstance<GPUSkinningResCacheAsset>();

                if(!AssetDatabase.IsValidFolder(cacheSaveRoot + "/cfg"))
                {
                    AssetDatabase.CreateFolder(cacheSaveRoot, "cfg");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                AssetDatabase.CreateAsset(asset, cacheSavePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            return asset;
        }

        /// <summary>
        /// Mesh所在文件的GUID，作为缓存的key
        /// </summary>
        public List<string> Keys = new List<string>();
        public List<GPUSkinningResCacheItem> Values = new List<GPUSkinningResCacheItem>();

        //----------------------------------------

        public void Clear()
        {
            Keys.Clear();
            Values.Clear();
        }

        public GPUSkinningResCacheItem Add(string refGUID, GPUSkinningResCacheItem cacheItem)
        {
            if(!Keys.Contains(refGUID))
            {
                Keys.Add(refGUID);
                Values.Add(cacheItem);
            }
            return cacheItem;
        }

        public void Remove(string guid)
        {
            int index = Keys.IndexOf(guid);
            if(index != -1)
            {
                Keys.RemoveAt(index);
                Values.RemoveAt(index);
            }
        }

        public GPUSkinningResCacheItem GetValue(string guid)
        {
            int index = Keys.IndexOf(guid);
            if(index != -1)
                return Values[index];
            return null;
        }



    }
}


