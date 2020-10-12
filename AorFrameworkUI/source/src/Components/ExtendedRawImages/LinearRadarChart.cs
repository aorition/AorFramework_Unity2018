using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UILinearRadarChart : MaskableGraphic
{
    //半径
    public float Radius = 50f;
    //边数
    public int Sides = 3;

    [SerializeField]
    private Texture m_Texture;

    public Color InnerColor = Color.white;

    /// <summary>
    /// Chart图数值列表
    /// (值范围：0 - 1) 
    /// </summary>
    public List<float> ValueOfSides;
    
    public Texture texture
    {
        get { return m_Texture; }
        set
        {
            if (m_Texture == value)
                return;

            m_Texture = value;
            SetVerticesDirty();
            SetMaterialDirty();
        }
    }

    public override Texture mainTexture
    {
        get { return m_Texture == null ? s_WhiteTexture : m_Texture; }
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        SetVerticesDirty();
        SetMaterialDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {

        if (Sides < 3) Sides = 3;

        float bAg = 360f / Sides;

        vh.Clear();

        //this.SetClipRect(new Rect(transform.position, new Vector2(Radius * 2, Radius * 2)), true);
        
        UIVertex ct = new UIVertex();
        ct.position = Vector2.zero;
        ct.color = InnerColor;
        ct.uv0 = new Vector2(0.5f, 0.5f);
        vh.AddVert(ct); //idx 0;

        for (int i = 0; i < Sides; i++)
        {

            int idx = vh.currentVertCount;

            //p0
            UIVertex p0 = new UIVertex();
            p0.position = calcuPoint(i * bAg) * getVos(i);
            p0.color = this.color;
            p0.uv0 = new Vector2(
                                    (p0.position.x + rectTransform.rect.width * 0.5f)  / rectTransform.rect.width,
                                    (p0.position.y + rectTransform.rect.height * 0.5f) / rectTransform.rect.height
                                ) ;
            vh.AddVert(p0); // if (i == 0) idx 1;

            if ( (i + 1) < Sides)
            {
                //p1
                UIVertex p1 = new UIVertex();
                p1.position = calcuPoint((i + 1) * bAg) * getVos(i + 1);
                p1.color = this.color;
                p1.uv0 = new Vector2(
                                    (p1.position.x + rectTransform.rect.width * 0.5f) / rectTransform.rect.width,
                                    (p1.position.y + rectTransform.rect.height * 0.5f) / rectTransform.rect.height
                    );
                vh.AddVert(p1);
                
                vh.AddTriangle(0, idx , idx + 1);
            }
            else
            {
                vh.AddTriangle(0, idx, 1);
            }

        }
    }



    private float getVos(int index)
    {
        if(ValueOfSides != null && ValueOfSides.Count > index)
        {
            return ValueOfSides[index];
        }
        return 1f;
    }

    private Vector2 calcuPoint(float angles)
    {
        return new Vector2(
                            Mathf.Sin(angles * Mathf.PI / 180f) * Radius,
                            Mathf.Cos(angles * Mathf.PI / 180f) * Radius
                        );
    }

}
