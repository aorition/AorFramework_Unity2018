#if UNITY_2018_3_OR_NEWER
    //2018.3.x 以上Unity版本提供原生Prefab嵌套功能，此功能就用不上了
#else


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utility
{
    public class PrefabLinker : MonoBehaviour, IEditorOnlyScript
    {

        public bool IsSource;
        public string SourceGUID;

    }
}

#endif