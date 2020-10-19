using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.GPUSkinning
{
    public class GPUSkinAnimDebugTool :MonoBehaviour
    {

        private GPUSkinningTexturen m_texturen;
        public GPUSkinningTexturen texturen
        {
            get { return m_texturen; }
        }

        private GPUSkinAndInstancing draw;
        private MeshRenderer meshRenderer;

        private bool _isInit = false;
        public bool isInit
        {
            get { return _isInit; }
        }

        public bool UseScenePanel;
        public float FPS = 60f;

        private void OnEnable()
        {

            if(!_isInit)
            {

                draw = gameObject.GetComponent<GPUSkinAndInstancing>();
                if(!draw)
                    draw = gameObject.GetComponentInChildren<GPUSkinAndInstancing>();

                if(draw)
                {

                    m_texturen = draw.GetComponent<GPUSkinningTexturen>();
                    meshRenderer = draw.GetComponent<MeshRenderer>();
                    var Mat = meshRenderer.material;
                    Mat.enableInstancing = true;
                    var mf = draw.GetComponent<MeshFilter>();
                    var mesh = mf.sharedMesh;

                    if(Mat)
                    {
                        draw.Init(1, mesh, Mat, draw.transform);
                        _isInit = true;
                    }

                }
            }

            if(meshRenderer && meshRenderer.enabled)
                meshRenderer.enabled = false;

            if(draw && !draw.enabled)
                draw.enabled = true;

        }

        private void OnDisable()
        {
            if(meshRenderer && !meshRenderer.enabled)
                meshRenderer.enabled = true;

            if(draw)
            {
                draw.Set(0,
                        0f, 0f, 0f,
                        0f, 0f, 0f,
                        (float)0);
                if(draw.enabled)
                    draw.enabled = false;
            }
        }

        [HideInInspector]
        public string m_currentAnimName;
        [HideInInspector]
        public int m_frame;
        private float m_playTime;
        private bool _isPlaying = false;
        public bool isPlaying
        {
            get { return _isPlaying; }
        }
        private Vector2Int m_frameSet;

        public void Play(string animName, int frame = 0)
        {
            int idx = -1;
            for(int i = 0; i < m_texturen.frameNames.Length; i++)
            {
                if(m_texturen.frameNames[i].ToLower() == animName.ToLower())
                {
                    idx = i;
                    break;
                }
            }

            if(idx > -1)
            {
                m_frameSet = new Vector2Int((int)m_texturen.frames[idx].x, (int)m_texturen.frames[idx].y);
                m_currentAnimName = m_texturen.frameNames[idx];
                m_frame = frame;
                m_playTime = 0;
                _isPaused = false;
                _isPlaying = true;
            }

        }

        private bool _isPaused = false;
        public bool isPaused
        {
            get { return _isPaused; }
            set { _isPaused = value; }
        }

        // Update is called once per frame
        void Update()
        {

            if(!_isInit || !_isPlaying || _isPaused)
                return;

            float fps = 1f / FPS;
            m_playTime += Time.deltaTime;
            int f = (int)(m_playTime / fps);
            m_frame = m_frameSet.x + (f % (m_frameSet.y - m_frameSet.x));
            draw.Set(0,
                0f, 0f, 0f,
                0f, 0f, 0f,
                (float)m_frame);

        }

    }

}

