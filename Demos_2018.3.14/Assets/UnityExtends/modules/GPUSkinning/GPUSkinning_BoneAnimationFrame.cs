using UnityEngine;
using System.Collections;

namespace Framework.GPUSkinning
{
    [System.Serializable]
    public class GPUSkinning_BoneAnimationFrame
    {
        //public GPUSkinning_Bone[] bones = null;

        public int[] bonesId = null;
        public Matrix4x4[] matrices = null;
        public string[] bonesName = null;
        public Matrix4x4[] matrices2 = null;
    }
}
