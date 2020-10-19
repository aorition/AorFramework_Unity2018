using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.GPUSkinning
{
    public class GPUSkinInstanceRenderer : MonoBehaviour
    {
        
        [System.Serializable]
        public class InstanceData
        {
            public Transform Trans;
            public int frameOffset;
            public float speed = 1f;
            public bool enableOverrideFrameRange;
            public int frameMin;
            public int frameMax;

            [NonSerialized]
            public int frame;
        }

        [SerializeField]
        protected List<InstanceData> m_List;

        [SerializeField]
        protected Mesh m_Mesh;

        [SerializeField]
        protected Material m_Material;
        
        [SerializeField]
        protected int m_FrameRangeMin;
        [SerializeField]
        protected int m_FrameRangeMax;

        protected MaterialPropertyBlock m_MaterialProperties;

        public float FPS = 30f;

        private Matrix4x4[] m_Matrices;
        private float[] m_Frames;

        protected void Start()
        {
            m_MaterialProperties = new MaterialPropertyBlock();
            foreach(InstanceData data in m_List) {
                data.frame = data.frameOffset + (data.enableOverrideFrameRange ? data.frameMin : m_FrameRangeMin);
            }
            m_Matrices = new Matrix4x4[m_List.Count];
            m_Frames = new float[m_List.Count];
            SetupMeshTangents();
        }

        protected void SetupMeshTangents()
        {
            Mesh mesh = m_Mesh;
            BoneWeight[] boneWs = mesh.boneWeights;
            Vector4[] tangents = new Vector4[mesh.vertexCount];
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                BoneWeight boneWeight = boneWs[i];
                tangents[i].x = boneWeight.boneIndex0; //1顶点id
                tangents[i].y = boneWeight.weight0;    //1顶点权重
                tangents[i].z = boneWeight.boneIndex1; //2顶点id
                tangents[i].w = boneWeight.weight1;    //2顶点权重    
            }
            mesh.tangents = tangents;
        }

        private float _addedFrame = 0;

        public void Draw(float deltaTime)
        {

            float _tVal = 1f / FPS;

            for(int i = 0; i < m_List.Count; i++) {
                InstanceData data = m_List[i];
                m_Matrices[i] = Matrix4x4.TRS(data.Trans.position, data.Trans.rotation, data.Trans.lossyScale);
                if(data.speed > 0)
                {
                    _addedFrame += deltaTime * data.speed / _tVal;
                    if(_addedFrame > 1f)
                    {
                        data.frame += Mathf.FloorToInt(_addedFrame);
                        _addedFrame = _addedFrame % 1f;
                        if(data.enableOverrideFrameRange)
                        {
                            if(data.frame > data.frameMax)
                                data.frame = data.frameMin;
                        }
                        else
                        {
                            if(data.frame > m_FrameRangeMax)
                                data.frame = m_FrameRangeMin;
                        }
                    }
                }
                m_Frames[i] = data.frame;
            }

            m_MaterialProperties.SetFloatArray("_Frame", m_Frames);
            Graphics.DrawMeshInstanced(m_Mesh, 0, m_Material, m_Matrices, m_List.Count, m_MaterialProperties);
        }

        public void Update()
        {
            if(m_List == null || m_List.Count == 0) {
                return;
            }
            Draw(Time.deltaTime);
        }
    }
}