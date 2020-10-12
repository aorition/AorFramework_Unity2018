using System;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 此类必须配合 UI - AlphaMask##.shader 系列使用，实现alphaMask的UV旋转，平移，缩放做动画
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class EffectScript_UIImageAlphaMask : MonoBehaviour
{
    [Tooltip("Mask UV 的缩放")]
    public Vector2 Tiling = Vector2.one;
    [Tooltip("Mask UV 的偏移")]
    public Vector2 Offset = Vector2.zero;
    [Tooltip("Mask UV 的旋转")] 
    public float Rotation = 0;

    private Image _image;
    private Material _targetMaterial;
    private CanvasRenderer _canvasRenderer;
    void Awake()
    {
        _image = GetComponent<Image>();
        //_canvasRenderer = GetComponent<CanvasRenderer>();
        _targetMaterial = _image.materialForRendering;
        //_targetMaterial = _image.material;

    }

    void LateUpdate()
    {
        if (_targetMaterial)
        {
            _targetMaterial.SetTextureScale("_MaskTex", Tiling);
            _targetMaterial.SetTextureOffset("_MaskTex", Offset);
            _targetMaterial.SetFloat("_Rotation", Rotation);

            _image.enabled = false;
            _image.enabled = true;
        }
    }
}