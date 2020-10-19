using System;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    [RequireComponent(typeof(RawImage))]
    public class Texture2DCacheForRawImage : MonoBehaviour
    {

        protected RawImage m_rawImage;
        protected RawImage rawImage
        {
            get {
                if(!m_rawImage)
                    m_rawImage = GetComponent<RawImage>();
                return m_rawImage;
            }
        }

        [SerializeField]
        private byte[] m_texCache;
        
        public long CacheLens
        {
            get {
                if(m_texCache != null)
                    return m_texCache.LongLength;
                return 0;
            }
        }

        public Texture texture
        {
            get {
                if(rawImage)
                    return rawImage.texture;
                return null;
            }
            set {
                if(rawImage)
                    m_rawImage.texture = value;
            }
        }

        public int GenTexSizeWidth = 512;
        public int GenTexSizeHeight = 512;

        protected virtual void Awake()
        {
            TryRecoverTexFormCache();
        }

        public void TryRecoverTexFormCache()
        {
            if(!rawImage.texture && CacheLens > 0)
            {
                TryBuildingTexture((tex) => {
                    tex.LoadRawTextureData(m_texCache);
                    tex.Apply();
                });
            }
        }

        public void SaveTexToCache(Texture2D texture2D)
        {
            GenTexSizeWidth = texture2D.width;
            GenTexSizeHeight = texture2D.height;
            byte[] bytes = texture2D.GetRawTextureData();
            m_texCache = bytes;
        }

        public void TryBuildingTexture(Action<Texture2D> finishCallBack)
        {
            Texture2D texture2D = rawImage.texture as Texture2D;
            if(texture2D)
                finishCallBack(texture2D);
            else
            {
                texture2D = new Texture2D(GenTexSizeWidth, GenTexSizeHeight, TextureFormat.RGBA32, false);
                rawImage.texture = texture2D;
                finishCallBack(texture2D);
            }
        }

        public void ClearCache()
        {
            m_texCache = null;
        }

    }
}
