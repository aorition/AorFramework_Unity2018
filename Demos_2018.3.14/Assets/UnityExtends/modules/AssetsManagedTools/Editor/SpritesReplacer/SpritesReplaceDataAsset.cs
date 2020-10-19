using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utility.Editor
{
    public class SpritesReplaceDataAsset : ScriptableObject
    {

        public bool useThumbnail;
        public bool removeItemComf;

        public string targetDirPath;
        public Sprite[] srcList;
        public Sprite[] tarList;
        public bool[] nsList;

    }
}
