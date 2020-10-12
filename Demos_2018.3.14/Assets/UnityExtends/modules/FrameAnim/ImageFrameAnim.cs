using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Framework.FrameAnim
{

    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class ImageFrameAnim : FrameAnimBase
    {

        public bool EnableSetNativeSizeByFrame = true;

        [SerializeField]
        private Sprite[] m_Frames;
        private Image m_image;

        public int TotalFrames
        {
            get {
                if (this.m_FrameLens == 0 && m_Frames != null) this.m_FrameLens = m_Frames.Length;
                return this.m_FrameLens;
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
            m_image = GetComponent<Image>();
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
            if (m_Frames != null && m_Frames.Length > 0)
            {
                this.m_FrameLens = m_Frames.Length;
                m_image.sprite = m_Frames[0];
                m_index = 0;
                m_index_cache = 0;
            }
        }

        public void Setup(Sprite[] SpriteList)
        {
            m_Frames = SpriteList;
            init();
            if (this.AutoPlayOnEnabled) Play();
        }

        public override void Play(float startTime = 0)
        {
            if (this.m_FrameLens > 0)
            {
                base.Play(startTime);
            }
        }

        protected override void onIndexChanged()
        {
            if (m_Frames != null && m_Frames.Length > 0)
            {
                m_image.sprite = m_Frames[m_index];
                if (EnableSetNativeSizeByFrame) m_image.SetNativeSize();
            }
        }
    }
}
