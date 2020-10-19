using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Framework.Utility.Editor
{

    [Serializable]
    public class TOS_Prefab_CacheItem
    {
        public TOS_Prefab_CacheItem(string name, List<string> sp_guidAddNameDependencies, List<string> sp_hierarchies, List<string> tx_guidAddNameDependencies, List<string> tx_hierarchies)
        {
            this.name = name;
            this.sp_guidAddNameDependencies = sp_guidAddNameDependencies;
            this.sp_hierarchies = sp_hierarchies;
            this.tx_guidAddNameDependencies = tx_guidAddNameDependencies;
            this.tx_hierarchies = tx_hierarchies;
        }

        public string name;
        public List<string> sp_guidAddNameDependencies;
        public List<string> sp_hierarchies;

        public List<string> tx_guidAddNameDependencies;
        public List<string> tx_hierarchies;

    }

    [Serializable]
    public class TOS_Sprite_CacheItem {

        public TOS_Sprite_CacheItem(string name)
        {
            this.name = name;
            RefCounts = 0;
        }

        public string name;

        /// <summary>
        /// 被引用次数
        /// </summary>
        public int RefCounts;
    }

    [Serializable]
    public class TOS_Texture_CacheItem
    {
        public TOS_Texture_CacheItem(string name)
        {
            this.name = name;
            RefCounts = 0;
        }

        public string name;

        /// <summary>
        /// 被引用次数
        /// </summary>
        public int RefCounts;
    }


    public class TOSRefCacheAsset : ScriptableObject
    {

        private const string m_savePath = "Assets/TOSCache.asset";

        public static TOSRefCacheAsset GetAsset()
        {
            TOSRefCacheAsset asset = AssetDatabase.LoadAssetAtPath<TOSRefCacheAsset>(m_savePath);
            if( !asset ) {
                asset = ScriptableObject.CreateInstance<TOSRefCacheAsset>( );
                //保存
                if( AssetDatabase.DeleteAsset(m_savePath))
                    AssetDatabase.SaveAssets( );

                AssetDatabase.CreateAsset(asset, m_savePath);
                AssetDatabase.SaveAssets( );
                AssetDatabase.Refresh( );
            }
            return asset;
        }

        public bool CacheIsReady = false;

        public string Date;

        public List<string> Prefab_GUIDs = new List<string>();
        public List<string> Sprite_GUIDs = new List<string>();
        public List<TOS_Prefab_CacheItem> PrefabItems = new List<TOS_Prefab_CacheItem>();
        public List<TOS_Sprite_CacheItem> SpriteItems = new List<TOS_Sprite_CacheItem>();

        public List<string> Texture_GUIDs = new List<string>();
        public List<TOS_Texture_CacheItem> TextureItems = new List<TOS_Texture_CacheItem>();

        public void Reset()
        {
            Date = string.Empty;
            Prefab_GUIDs.Clear();
            Sprite_GUIDs.Clear();
            PrefabItems.Clear();
            SpriteItems.Clear();
            CacheIsReady = false;
        }

        #region Prefab

        public void AddPrefabData ( string guid, TOS_Prefab_CacheItem item ) {
            if( !Prefab_GUIDs.Contains( guid ) ) {
                Prefab_GUIDs.Add( guid );
                PrefabItems.Add( item );
            }
        }

        public void RemovePrefabData (string guid) {
            int idx = Prefab_GUIDs.IndexOf( guid );
            if( !idx.Equals( -1 ) ) {
                Prefab_GUIDs.RemoveAt( idx );
                PrefabItems.RemoveAt( idx );
            }
        }

        public TOS_Prefab_CacheItem GetPrefabItemData (string guid) {
            int idx = Prefab_GUIDs.IndexOf( guid );
            if( !idx.Equals( -1 ) )
            {
                return PrefabItems[idx];
            }
            return null;
        }


        #endregion

        #region Sprite

        public void AddSpriteData ( string guid, TOS_Sprite_CacheItem item ) {
            if( !Sprite_GUIDs.Contains( guid ) ) {
                Sprite_GUIDs.Add( guid );
                SpriteItems.Add(item);
            }
        }

        public void RemoveSpriteData ( string guid ) {
            int idx = Sprite_GUIDs.IndexOf( guid );
            if( !idx.Equals( -1 ) ) {
                Sprite_GUIDs.RemoveAt( idx );
                SpriteItems.RemoveAt( idx );
            }
        }

        public TOS_Sprite_CacheItem GetSpriteItemData ( string guid ) {
            int idx = Sprite_GUIDs.IndexOf( guid );
            if( !idx.Equals( -1 ) ) {
                return SpriteItems [idx];
            }
            return null;
        }


        #endregion

        #region Texture2D

        public void AddTextureData(string guid, TOS_Texture_CacheItem item)
        {
            if(!Texture_GUIDs.Contains(guid))
            {
                Texture_GUIDs.Add(guid);
                TextureItems.Add(item);
            }
        }

        public void RemoveTextureData(string guid)
        {
            int idx = Texture_GUIDs.IndexOf(guid);
            if(!idx.Equals(-1))
            {
                Texture_GUIDs.RemoveAt(idx);
                TextureItems.RemoveAt(idx);
            }
        }

        public TOS_Texture_CacheItem GetTextureItemData(string guid)
        {
            int idx = Texture_GUIDs.IndexOf(guid);
            if(!idx.Equals(-1))
            {
                return TextureItems[idx];
            }
            return null;
        }

        #endregion

        public int PrefabCounts
        {
            get { return Prefab_GUIDs.Count; }
        }

        public int SpriteCounts
        {
            get { return SpriteItems.Count; }
        }

        public int ItemCounts
        {
            get { return Prefab_GUIDs.Count + SpriteItems.Count; }
        }

        //public void SetDirty()
        //{
        //    EditorUtility.SetDirty(this);
        //}

    }

}


