using UnityEngine;
using System.Collections.Generic;

namespace Framework.GPUSkinning
{
    public class GPUSkinning :GPUSkinningBase
    {

        //private Material newMtrl = null;

        public GPUSkinning_BoneAnimation_config[] boneAnimations;

        //private int shaderPropID_Matrices = 0;

        //GPUSkinning_BoneAnimation_config boneAnimation;
        //protected override void Play(string aname)
        //{
        //    int _aid = getAid(aname);
        //    if (_aid != -1)
        //    {
        //        currentAname = aname;
        //        currentAid = _aid;
        //        boneAnimation = boneAnimations[_aid];
        //        totalFrame = (int)(boneAnimation.length * boneAnimation.fps);
        //        second = 0;
        //    }
        //}

        //protected override int getAid(string aname)
        //{
        //    for (int i = 0; i < boneAnimations.Length; i++)
        //    {
        //        if (boneAnimations[i].animName == aname)
        //        {
        //            return i;
        //        }
        //    }
        //    return -1;
        //}
        //private void Start()
        //{
        //    shaderPropID_Matrices = Shader.PropertyToID("_Matrices");
        //    newMtrl = GetComponent<MeshRenderer>().material;
        //    Play(boneAnimations[currentAid].animName);
        //}

        //private void Update()
        //{
        //    if (!isStop && !isPlayEnd)
        //    {
        //        second += (float)(BattleDefine.FRAME_DELTA * Speed);
        //        if (second >= boneAnimation.length)
        //        {
        //            PlayEnd();
        //        }
        //        UpdateBoneAnimationMatrix();
        //    }
        //}
        //public override void PlayEnd()
        //{
        //    base.PlayEnd();
        //    second = 0;
        //}
        //int totalFrame;
        ////************* 更新骨骼的动画信息 *******************
        //protected override void UpdateBoneAnimationMatrix()
        //{
        //    int frameIndex = (int)(second * boneAnimation.fps) % totalFrame;
        //    GPUSkinning_BoneAnimationFrame_config frame = boneAnimation.frames[frameIndex];
        //    newMtrl.SetMatrixArray(shaderPropID_Matrices, frame.matrices);
        //}

        //public override float GetAnimLength()
        //{
        //    return boneAnimation.length;
        //}
    }

}

