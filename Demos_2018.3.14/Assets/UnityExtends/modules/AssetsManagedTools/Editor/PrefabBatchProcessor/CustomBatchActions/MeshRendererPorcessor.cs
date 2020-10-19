using Framework.Extends;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utility.Editor
{
    [PBPTagLabel("批量关闭MeshRenderer的CastShadows")]
    public class MeshRendererPorcessor : PrefabBatchActionBase
    {

        public override string DesInfo
        {
            get {
                return "批量关闭MeshRenderer的CastShadows属性";
            }
        }

        public override void Init()
        {
            m_cacheHash.Clear();
        }

        protected override void _foreachTransformProcess(Transform transform, int indent, ref bool dirty)
        {
            MeshRenderer meshRenderer = transform.GetComponent<MeshRenderer>();
            if(meshRenderer)
            {
                if(meshRenderer.shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.Off)
                {
                    meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    _infoAppendLine(indent, "Node>" + transform.GetHierarchyPath() + " ::  shadowCastingMode -> Off");
                    _trySetDirty(ref dirty);
                }
            }
        }

    }
}
