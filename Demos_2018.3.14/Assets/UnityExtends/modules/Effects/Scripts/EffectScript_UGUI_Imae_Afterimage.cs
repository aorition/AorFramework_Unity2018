using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UGUI Image组件 简单残像效果实现
/// </summary>
[RequireComponent(typeof(UnityEngine.UI.Image))]
public class EffectScript_UGUI_Imae_Afterimage : BaseMeshEffect
{
    protected EffectScript_UGUI_Imae_Afterimage()
    { }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }

#endif

    private Image m_image;

    public bool IgnoreTimeScale = false;

    public int AfterimageNum = 12;

    public float Interval = 0.3f;

    private List<Vector3> m_sdPosCache = new List<Vector3>();

    private float m_now = 0;

    protected override void OnDisable()
    {
        base.OnDisable();
        m_sdPosCache.Clear();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_baseVerts = null;
        m_image = null;
        m_sdPosCache = null;
    }

    protected void Update()
    {
        if(!m_image) m_image = GetComponent<Image>();
        m_image.SetVerticesDirty();
    }

    protected void LateUpdate()
    {
        m_now += IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
        if(m_now >= Interval)
        {
            m_now = 0;
            m_sdPosCache.Add(transform.position);
            if (m_sdPosCache.Count > AfterimageNum)
            {
                int g = Mathf.Abs(m_sdPosCache.Count - AfterimageNum);
                if (g > 1)
                    m_sdPosCache.RemoveRange(0, g);
                else
                    m_sdPosCache.RemoveAt(0);
            }
        }
    }

    private List<UIVertex> m_baseVerts = new List<UIVertex>();
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        m_baseVerts.Clear();
        vh.GetUIVertexStream(m_baseVerts);
        vh.Clear();

        List<UIVertex> newUIVertexs = new List<UIVertex>();
        
        Vector3 nowPos = transform.position;
        float ap = 1f / m_sdPosCache.Count;
        for (int i = 0; i < m_sdPosCache.Count; i++)
        {
            Vector3 offset = m_sdPosCache[i] - nowPos;
            for (int j = 0; j < m_baseVerts.Count; j++)
            {
                UIVertex tmp2 = UIVertexClone(m_baseVerts[j]);
                tmp2.position += offset;
                byte r = (byte)Mathf.RoundToInt(tmp2.color.r * 0.75f);
                byte g = (byte)Mathf.RoundToInt(tmp2.color.g * 0.75f);
                byte b = (byte)Mathf.RoundToInt(tmp2.color.b * 0.82f);
                byte a = (byte)Mathf.RoundToInt(tmp2.color.a * (ap * i));
                tmp2.color = new Color32(r, g, b, a);
                newUIVertexs.Add(tmp2);
            }
        }
        
        for (int i = 0; i < m_baseVerts.Count; i++)
        {
            newUIVertexs.Add(m_baseVerts[i]);
        }

        vh.AddUIVertexTriangleStream(newUIVertexs);
    }

    private UIVertex UIVertexClone (UIVertex s)
    {
        UIVertex t = new UIVertex();
        t.color = s.color;
        t.normal = s.normal;
        t.position = s.position;
        t.tangent = s.tangent;
        t.uv0 = s.uv0;
        t.uv1 = s.uv1;
        t.uv2 = s.uv2;
        t.uv3 = s.uv3;
        return t;
    }

}
