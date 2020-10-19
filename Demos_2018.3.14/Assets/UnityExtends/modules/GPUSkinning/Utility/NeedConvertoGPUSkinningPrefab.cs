using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.GPUSkinning
{
    /// <summary>
    /// （编辑器功能支持脚本）
    /// </summary>
    public class NeedConvertoGPUSkinningPrefab :MonoBehaviour, IEditorOnlyScript
    {
        private void Awake()
        {
            if(Application.isPlaying)
                GameObject.Destroy(this);
        }

        public string reName;
        public bool checkRoot = true;
        public bool replaceOrignPrefab = false;
        public bool useStandaloneMesh = false;
        public bool mLighting = false;

        /*
        public bool AddLevel

        public bool AddName

         */

    }
}
