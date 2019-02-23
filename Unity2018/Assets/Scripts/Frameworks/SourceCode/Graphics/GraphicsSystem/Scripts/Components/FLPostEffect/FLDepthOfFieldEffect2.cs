#pragma warning disable
using Framework.Graphic.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic.Effect
{
    [ExecuteInEditMode]
    public class FLDepthOfFieldEffect2 : FLEffectBase
    {

        public string ScriptName
        {
            get { return "DepthOfField"; }
        }

        public float FocalDistance01 = 0.0f;
        [Range(0.0001f, 0.02f)]
        public float OffsetDistance = 0.01f;

        public bool HighQuality = false;

        private bool m_hq_cache;
        protected override void init()
        {
            base.init();
            //填充后处理用Shader&Material
            if (renderMat == null)
            {
                renderMat = new Material(ShaderBridge.Find("Hidden/PostEffect/DepthOfField - Simple"));
                m_hq_cache = HighQuality;
                if (HighQuality) renderMat.EnableKeyword("_HIGHQUALITY_ON");
            }
            //填充默认渲染等级值
            if (m_RenderLevel == 0) m_RenderLevel = 125;
        }

        protected override void OnDisable()
        {
            if (GraphicsManager.IsInit())
                GraphicsManager.Instance.RemovePostEffectComponent(this);
        }

        protected override void render(RenderTexture src, RenderTexture dst)
        {
            base.render(src, dst);

            if (renderMat == null || renderMat.shader == null)
            {
                Debug.Log("this is a null material");
                return;
            }

            if(m_hq_cache != HighQuality)
            {
                if (HighQuality)
                {
                    renderMat.EnableKeyword("_HIGHQUALITY_ON");
                }
                else
                {
                    renderMat.DisableKeyword("_HIGHQUALITY_ON");
                }
                m_hq_cache = HighQuality;
            }

            renderMat.SetFloat("focalDistance01", FocalDistance01); // 0--1 , or near plane to far plane.
            renderMat.SetFloat("_OffsetDistance", OffsetDistance);
            Graphics.Blit(src, dst, renderMat);

        }
        
    }
}


