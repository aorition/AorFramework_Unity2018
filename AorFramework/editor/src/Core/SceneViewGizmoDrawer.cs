using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{

    [InitializeOnLoad]
    public class SceneViewGizmoDrawer
    {
        static SceneViewGizmoDrawer()
        {
            ClearAllDrawMethod();
        }

        private static readonly List<Action<Transform, GizmoType>> m_actionList = new List<Action<Transform, GizmoType>>();

        public static void AddDrawMethod(Action<Transform, GizmoType> method)
        {
            if (!m_actionList.Contains(method)) m_actionList.Add(method);
        }

        public static void RemoveDrawMethod(Action<Transform, GizmoType> method)
        {
            if (m_actionList.Contains(method)) m_actionList.Remove(method);
        }

        public static void ClearAllDrawMethod()
        {
            m_actionList.Clear();
        }

        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
        private static void DrawCinemaGizmo(Transform transform, GizmoType gizmoType)
        {
            for (int i = 0; i < m_actionList.Count; i++)
            {
                if (m_actionList[i] != null) m_actionList[i](transform, gizmoType);
            }
        }

    }

}
