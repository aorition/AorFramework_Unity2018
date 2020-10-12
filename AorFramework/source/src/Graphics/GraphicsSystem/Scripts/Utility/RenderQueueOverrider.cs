using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic
{
    public class RenderQueueOverrider : MonoBehaviour
    {

        public int OverrideRenderQueue = 5000;

        private readonly Dictionary<int, int> m_rqCache = new Dictionary<int, int>();
        private Renderer[] m_renderers;
        private void OnEnable()
        {
            m_renderers = GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < m_renderers.Length; i++)
            {
                Renderer renderer = m_renderers[i];
                for (int m = 0; m < renderer.materials.Length; m++)
                {
                    Material mt = renderer.materials[m];
                    int hs = mt.GetHashCode();
                    m_rqCache.Add(hs, mt.renderQueue);
                    mt.renderQueue += mt.renderQueue;
                }
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < m_renderers.Length; i++)
            {
                Renderer renderer = m_renderers[i];
                for (int m = 0; m < renderer.materials.Length; m++)
                {
                    Material mt = renderer.materials[m];
                    int hs = mt.GetHashCode();
                    if (m_rqCache.ContainsKey(hs))
                    {
                        mt.renderQueue = m_rqCache[hs];
                    }
                }
            }
            m_rqCache.Clear();
            m_renderers = null;
        }

    }
}