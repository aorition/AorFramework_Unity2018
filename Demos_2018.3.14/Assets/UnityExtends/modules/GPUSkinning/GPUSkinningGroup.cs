using UnityEngine;

namespace Framework.GPUSkinning
{
    public class GPUSkinningGroup
    {
        static GPUSkinningGroup _instens;
        public static GPUSkinningGroup instens
        {
            get {
                if(_instens == null)
                {
                    _instens = new GPUSkinningGroup();
                }
                return _instens;
            }
        }
        public AnimationData addObject(GameObject goo)
        {
            AnimationData data = new AnimationData(goo.GetComponent<GPUSkinningTexturen>());
            string name = goo.name;
            //ObjectGroup ag = GPUInstancingMananger.Instance.getGroup(name);
            //bool isFirst = ag == null;
            //data.transform = GPUInstancingMananger.Instance.Add(goo, null, Vector3.zero, Vector3.zero, Vector3.one, name, 0, false , false);
            //if(isFirst)
            //{
            //    ag = GPUInstancingMananger.Instance.getGroup(name);
            //    ag.Mat[0].EnableKeyword("_GPUAnimation");
            //    BoneWeight[] boneWs = ag.mesh.boneWeights;
            //    Vector4[] tangents = new Vector4[ag.mesh.vertexCount];
            //    for (int i = 0; i < ag.mesh.vertexCount; i++)
            //    {
            //        BoneWeight boneWeight = boneWs[i];
            //        tangents[i].x = boneWeight.boneIndex0;//1顶点id
            //        tangents[i].y = boneWeight.weight0;//1顶点权重
            //        tangents[i].z = boneWeight.boneIndex1;//2顶点id
            //        tangents[i].w = boneWeight.weight1;//2顶点权重    
            //    }
            //    ag.mesh.tangents = tangents;
            //}
            //Object.Destroy(goo);
            return data;
        }

        //void Update()
        //{
        //    for (int i = 0; i < _ags.Count; i++)
        //    {
        //        _ags[i].updata();
        //        if (_ags[i].len > 0)
        //        {
        //            Graphics.DrawMeshInstanced(_ags[i].mesh, 0, _ags[i].Mat, _ags[i].matrices, _ags[i].showLen, _ags[i]._propertyBlock, ShadowCastingMode.On,true, layer);
        //        }
        //    }
        //}
    }
    //public class _AnimationGroup
    //{
    //    public MaterialPropertyBlock _propertyBlock = new MaterialPropertyBlock();
    //    public void updata()
    //    {
    //        showLen = 0;
    //        for (int i = 0; i < len; i++)
    //        {
    //            if (roles[i].transform.stage == ObjectGroup.objStage.show)
    //            {
    //                roles[i].Update();
    //                matrices[showLen] = roles[i].transform.localToWorldMatrix;
    //                _AnimLen[showLen] = roles[i].second;
    //                showLen++;
    //        }
    //        }
    //        _propertyBlock.SetFloatArray("_Frame", _AnimLen);
    //    }
    //}
}
