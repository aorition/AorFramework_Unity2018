using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;

namespace Framework.Utility.Editor
{
    
    public class PBPTagLabelAttribute :Attribute
    {

        public string label;
        public PBPTagLabelAttribute(string label)
        {
            this.label = label;
        }

    }
}
