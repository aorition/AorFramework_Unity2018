using UnityEngine.UI;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Framework.UI
{
    public class BaseRawImage : MaskableGraphic
    {
        [FormerlySerializedAs("m_Tex")]
        //[FormerlySerializedAs("m_Frame")]
        [SerializeField] protected Texture m_Texture;
        [SerializeField] protected Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

        protected BaseRawImage()
        {
            useLegacyMeshGeneration = false;
        }

        /// <summary>
        /// Returns the texture used to draw this Graphic.
        /// </summary>
        public override Texture mainTexture
        {
            get
            {
                if (m_Texture == null)
                {
                    if (material != null && material.mainTexture != null)
                    {
                        return material.mainTexture;
                    }
                    return s_WhiteTexture;
                }

                return m_Texture;
            }
        }

        /// <summary>
        /// Texture to be used.
        /// </summary>
        public Texture texture
        {
            get
            {
                return m_Texture;
            }
            set
            {
                if (m_Texture == value)
                    return;

                m_Texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        /// <summary>
        /// UV rectangle used by the texture.
        /// </summary>
        public Rect uvRect
        {
            get
            {
                return m_UVRect;
            }
            set
            {
                if (m_UVRect == value)
                    return;
                m_UVRect = value;
                SetVerticesDirty();
            }
        }

        /// <summary>
        /// Adjust the scale of the Graphic to make it pixel-perfect.
        /// </summary>

        public override void SetNativeSize()
        {
            Texture tex = mainTexture;
            if (tex != null)
            {
                int w = Mathf.RoundToInt(tex.width * uvRect.width);
                int h = Mathf.RoundToInt(tex.height * uvRect.height);
                rectTransform.anchorMax = rectTransform.anchorMin;
                rectTransform.sizeDelta = new Vector2(w, h);
            }
        }

        /// <summary>
        /// 子类需要重写该方法来自定义Image形状
        /// </summary>
        /// <param name="vh"></param>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
        }
    }
}
