using System;
using System.Collections.Generic;
using Framework;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 接驳Animation仅支持float值
/// </summary>
public class EffectScript_MaterialProperties : MonoBehaviour
{
    public bool Enable_1;
    public string PropertieName_1;
    public float PropertieValue_1;

    public bool Enable_2;
    public string PropertieName_2;
    public float PropertieValue_2;

    public bool Enable_3;
    public string PropertieName_3;
    public float PropertieValue_3;

    public bool Enable_4;
    public string PropertieName_4;
    public float PropertieValue_4;

    public bool Enable_5;
    public string PropertieName_5;
    public float PropertieValue_5;

    private Material m_material;
    
    protected void OnEnable()
    {
        if (!m_material)
        {
            Graphic cr = GetComponent<Graphic>();
            if (cr) m_material = cr.material;
        }
        if (!m_material)
        {
            Material b = GetComponent<Renderer>().material;
            Material copy = GameObject.Instantiate(b);
            GetComponent<Renderer>().material = copy;
            m_material = copy;
        }
    }

    protected void FixedUpdate()
    {
        if (!m_material) return;

        if (Enable_1 && !string.IsNullOrEmpty(PropertieName_1))
            m_material.SetFloat(PropertieName_1, PropertieValue_1);

        if (Enable_2 && !string.IsNullOrEmpty(PropertieName_2))
            m_material.SetFloat(PropertieName_2, PropertieValue_2);

        if (Enable_3 && !string.IsNullOrEmpty(PropertieName_3))
            m_material.SetFloat(PropertieName_3, PropertieValue_3);

        if (Enable_4 && !string.IsNullOrEmpty(PropertieName_4))
            m_material.SetFloat(PropertieName_4, PropertieValue_4);

        if (Enable_5 && !string.IsNullOrEmpty(PropertieName_5))
            m_material.SetFloat(PropertieName_5, PropertieValue_5);
    }

}
