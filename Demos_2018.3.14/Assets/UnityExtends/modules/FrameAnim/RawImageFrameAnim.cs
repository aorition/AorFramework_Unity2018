using AorBaseUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Framework.FrameAnim
{

    [RequireComponent(typeof(UnityEngine.UI.RawImage))]
    public class RawImageFrameAnim : FrameAnimBase
    {
        [SerializeField]
        private int m_totalFrame;
        [SerializeField]
        private Texture2D m_FramesTexture;

        public int GridU;
        public int GridV;
        
        private RawImage m_rawImage;

        private float m_w, m_h;

        public int TotalFrames
        {
            get { return m_totalFrame; }
            set
            {
                m_totalFrame = value;
                this.m_FrameLens = m_totalFrame;
            }
        }

        public override float Length
        {
            get
            {
                return 1f / this.FPS * TotalFrames;
            }
        }

        private void Awake()
        {
            m_rawImage = GetComponent<RawImage>();
            init();
        }
        
        protected override void OnEnable()
        {
            Animator animator = GetComponent<Animator>();
            if (animator) animator.enabled = false;
            base.OnEnable();
        }

        private void init()
        {
            if (m_FramesTexture)
                m_rawImage.texture = m_FramesTexture;

            m_w = 1f / GridU;
            m_h = 1f / GridV;

            this.m_FrameLens = m_totalFrame;

            updateRawImageUVRect();
        }

        public void Setup(Texture2D framesTex, int gridU, int gridV, int frames)
        {
            this.GridU = gridU;
            this.GridV = gridV;
            this.m_totalFrame = frames;
            Setup(framesTex);
        }

        public void Setup(Texture2D framesTex)
        {
            m_FramesTexture = framesTex;
            init();
            if (this.AutoPlayOnEnabled) Play();
        }
        
        private void updateRawImageUVRect()
        {
            if (GridU == 0 || GridV == 0) return;
            m_rawImage.uvRect = new Rect(
                (m_index % GridU) * m_w,
                1f - Mathf.Floor(m_index / GridU) * m_h - m_h,
                m_w,
                m_h
                ); 
            m_rawImage.SetAllDirty();
        }

        public override void Play(float startTime = 0)
        {
            if (this.m_FrameLens > 0 && m_FramesTexture)
            {
                base.Play(startTime);
            }
        }

        protected override void onIndexChanged()
        {
            updateRawImageUVRect();
        }

        public Vector2 GetCullSize()
        {
            if (!m_FramesTexture || GridU == 0 || GridV == 0) return new Vector2();
            return new Vector2(Mathf.FloorToInt(m_FramesTexture.width / GridU), Mathf.FloorToInt(m_FramesTexture.height / GridV));
        }

        public void SetNativeSize()
        {
            if (!m_FramesTexture || GridU == 0 || GridV == 0) return;
            Vector2 callSize = GetCullSize();
            RectTransform rt = GetComponent<RectTransform>();
            rt.anchorMax = rt.anchorMin;
            rt.sizeDelta = callSize;
        }

    }
}
