using UnityEngine;
using System.Collections.Generic;

namespace Framework.GPUSkinning
{
    public class GPUSkinningTexturen :GPUSkinningBase
    {
        //Renderer render;
        //private int shaderPropID_Matrices = 0;

        public Vector2[] frames;
        public string[] frameNames;
        //Vector2 currentFrame;
        //protected override void Play(string aname)
        //{
        //    int _aid = getAid(aname);
        //    if (_aid != -1)
        //    {
        //        currentAname = aname;
        //        currentAid = _aid;
        //        currentFrame = frames[currentAid];
        //        second = currentFrame[0];
        //    }
        //}

        //protected override int getAid(string aname)
        //{
        //    for (int i = 0; i < frameNames.Length; i++)
        //    {
        //        if (frameNames[i] == aname)
        //        {
        //            return i;
        //        }
        //    }
        //    return -1;
        //}
        //MaterialPropertyBlock _propertyBlock;
        void Awake()
        {
            //Vector4 projdir = new Vector3(2, 6, 5);       


            Material m = GetComponent<MeshRenderer>().material;

            //m.SetVector("_ShadowProjDir", projdir);
            //m.SetVector("_ShadowPlane", new Vector4(0.0f, 1.0f, 0.0f, 0.0f));
            ////m.SetVector("_ShadowFadeParams", new Vector4(0.0f, 1.5f, 0.7f, 0.0f));
            ////m.SetFloat("_ShadowFalloff", 1.35f);

            m.EnableKeyword("_GPUAnimation");
            //GPUSkinningGroup g = new GPUSkinningGroup();
            //g.addObject(this.gameObject);
            MeshFilter mf = GetComponent<MeshFilter>();
            if(mf == null)
            {
                mf = GetComponentInChildren<MeshFilter>();
            }
            Mesh mesh = mf.sharedMesh;
            BoneWeight[] boneWs = mesh.boneWeights;
            Vector4[] tangents = new Vector4[mesh.vertexCount];
            for(int i = 0; i < mesh.vertexCount; i++)
            {
                BoneWeight boneWeight = boneWs[i];
                tangents[i].x = boneWeight.boneIndex0;//1顶点id
                tangents[i].y = boneWeight.weight0;//1顶点权重
                tangents[i].z = boneWeight.boneIndex1;//2顶点id
                tangents[i].w = boneWeight.weight1;//2顶点权重    
            }
            mesh.tangents = tangents;
            //shaderPropID_Matrices = Shader.PropertyToID("_Frame");
            //_propertyBlock = new MaterialPropertyBlock();
            //if (render == null) render = transform.GetComponent<Renderer>();
            //Play(frameNames[0]);
        }
        //const float f = 1f / 60f * 2f;
        //int _frame;
        //float jiange = 0;
        //float _second = 0;
        //void Update()
        //{

        //    if (!isStop && !isPlayEnd)
        //    {
        //        _frame++;
        //        jiange += Time.deltaTime;
        //        second += Speed * jiange / f;
        //        jiange = 0;
        //        if (second > currentFrame[1])
        //        {
        //            //一个动画播放到结尾
        //            PlayEnd();
        //        }
        //        if(Mathf.Floor(second)!= _second)           
        //        {
        //            _second = Mathf.Floor(second);

        //            //Vector4 worldpos = transform.position;
        //            //_propertyBlock.SetVector("_WorldPos", worldpos);

        //            UpdateBoneAnimationMatrix();

        //        }
        //    }
        //}
        //public override void PlayEnd()
        //{
        //    base.PlayEnd();
        //    if(_loopType == WrapMode.Loop)
        //        second = currentFrame[0];
        //}
        ////************* 更新骨骼的动画信息 *******************
        //protected override void UpdateBoneAnimationMatrix()
        //{
        //    _propertyBlock.SetFloat(shaderPropID_Matrices, second);
        //    render.SetPropertyBlock(_propertyBlock);
        //}

        //public override float GetAnimLength()
        //{
        //    return (float)((currentFrame.y - currentFrame.x) * BattleDefine.FRAME_DELTA);
        //}
    }

}

