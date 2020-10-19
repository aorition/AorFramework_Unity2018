using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Framework.Extends;

namespace Framework.Utility.Editor
{

    [PBPTagLabel("批量移除MissingComponent")]
    public class ClearMissingComponent : PrefabBatchActionBase
    {

        public override string DesInfo
        {
            get {
                return "批量移除MissingComponent";
            }
        }

        protected override void _foreachTransformProcess(Transform transform, int indent, ref bool dirty)
        {
            var components = transform.GetComponents<Component>();
            if(components != null && components.Length > 0)
            {
                var serializedObject = new SerializedObject(transform.gameObject);
                var prop = serializedObject.FindProperty("m_Component");
                int r = 0;
                for(int j = 0; j < components.Length; j++)
                {
                    if(components[j] == null)
                    {
                        prop.DeleteArrayElementAtIndex(j - r);
                        r++;
                    }
                }
                serializedObject.ApplyModifiedProperties();

                if(r > 0)
                {
                    _infoAppendLine(indent, "Node>" + transform.GetHierarchyPath() + " ::  finded MissingComponents -> " + r);
                    _trySetDirty(ref dirty);
                }
            }
        }

    }

}


