using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// (BMFontGenerator 暂时保留代码备用)
/// </summary>
public class BMFontGeneratorWindow : EditorWindow
{

    //[MenuItem("FrameworkTools/BMFont/BMFontTools")]
    public static BMFontGeneratorWindow Init()
    {
        BMFontGeneratorWindow window = EditorWindow.GetWindow<BMFontGeneratorWindow>();
        return window;
    }

    private struct ChrRect
    {
        public int id;
        public int x;
        public int y;
        public int w;
        public int h;
        public int xofs;
        public int yofs;

        public int index;
        public float uvX;
        public float uvY;
        public float uvW;
        public float uvH;
        public float vertX;
        public float vertY;
        public float vertW;
        public float vertH;
        public float width;
    }

    private TextAsset fontPosTbl;
    private Texture fontTexture;

    // layout window
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Use BMFont Texture File", EditorStyles.boldLabel);
        // use .fnt(.txt)
        fontTexture = (Texture)EditorGUILayout.ObjectField(fontTexture, typeof(Texture), false);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Use BMFont fnt File", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("(must Font Descriptor select Text)", EditorStyles.boldLabel);

        fontPosTbl = (TextAsset)EditorGUILayout.ObjectField("BMFont fnt (.fnt)", fontPosTbl, typeof(TextAsset), false);
        if (GUILayout.Button("Create .fontSetting"))
        {

            if (fontTexture == null) this.ShowNotification(new GUIContent("No Font Texture selected"));
            else if (fontPosTbl == null) this.ShowNotification(new GUIContent("No Font Position Table file selected"));
            else
            {
                CalcChrRect(fontPosTbl, fontTexture);
            }
        }
        EditorGUILayout.EndVertical();
    }

    void OnInspectorUpdate()
    {
        this.Repaint();
    }

    #region 实现方法

    private void CalcChrRect(TextAsset posTbl, Texture tex)
    {
        string fileName = AssetDatabase.GetAssetPath(fontPosTbl);
        string texName = AssetDatabase.GetAssetPath(tex);
        string fontName = System.IO.Path.GetFileNameWithoutExtension(fileName);
        string fontPath = fileName.Replace(".fnt", ".fontsettings");
        string matPath = fileName.Replace(".fnt", ".mat");
        float imgw = (float)tex.width;
        float imgh = (float)tex.height;
        string txt = posTbl.text;
        
        List<ChrRect> tblList = new List<ChrRect>();
        foreach (string line in txt.Split('\n'))
        {
            
            if (line.IndexOf("char id=") == 0)
            {
                ChrRect d = GetChrRect(line, imgw, imgh);
                tblList.Add(d);
            }
        }
        if (tblList.Count == 0)
        {
            new GUIContent("Failed");
            return;
        }

        ChrRect[] tbls = tblList.ToArray();
        Font font = new Font();
        font.name = fontName;
        SetCharacterInfo(tbls, font);


        Material mat = new Material(Shader.Find("GUI/Text Shader"));
        mat.mainTexture = tex;
        mat.name = fontName;
        font.material = mat;


        Debug.Log(System.IO.Path.GetFileNameWithoutExtension(fileName));
        Debug.Log(fileName);

        AssetDatabase.CreateAsset(mat, matPath);
        AssetDatabase.CreateAsset(font, fontPath);
        AssetDatabase.SaveAssets();
        this.ShowNotification(new GUIContent("Complete"));
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    // over write custom font by new CharacterInfo
    private void SetCharacterInfo(ChrRect[] tbls, Font fontObj)
    {
        CharacterInfo[] nci = new CharacterInfo[tbls.Length];
        for (int i = 0; i < tbls.Length; i++)
        {
            nci[i].index = tbls[i].index;
            nci[i].advance = (int)tbls[i].width;
            nci[i].uv.x = tbls[i].uvX;
            nci[i].uv.y = tbls[i].uvY;
            nci[i].uv.width = tbls[i].uvW;
            nci[i].uv.height = tbls[i].uvH;
            nci[i].vert.x = tbls[i].vertX;
            nci[i].vert.y = tbls[i].vertY;
            nci[i].vert.width = tbls[i].vertW;
            nci[i].vert.height = tbls[i].vertH;
        }
        fontObj.characterInfo = nci;
    }

    private ChrRect GetChrRect(string line, float imgw, float imgh)
    {
        ChrRect d = new ChrRect();

        foreach (string s in line.Split(' '))
        {
            if (s.IndexOf("id=") >= 0) d.id = GetParamInt(s, "id=");
            else if (s.IndexOf("x=") >= 0) d.x = GetParamInt(s, "x=");
            else if (s.IndexOf("y=") >= 0) d.y = GetParamInt(s, "y=");
            else if (s.IndexOf("width=") >= 0) d.w = GetParamInt(s, "width=");
            else if (s.IndexOf("height=") >= 0) d.h = GetParamInt(s, "height=");
            else if (s.IndexOf("xoffset=") >= 0) d.xofs = GetParamInt(s, "xoffset=");
            else if (s.IndexOf("yoffset=") >= 0) d.yofs = GetParamInt(s, "yoffset=");
            else if (s.IndexOf("xadvance=") >= 0) d.width = GetParamInt(s, "xadvance=");
        }
        d.index = d.id;
        d.uvX = (float)d.x / imgw;
        d.uvY = (float)(imgh - (d.y)) / imgh;
        d.uvW = (float)d.w / imgw;
        d.uvH = (float)-d.h / imgh;

        d.vertX = (float)d.xofs;
        d.vertY = (float)-d.yofs;
        d.vertW = d.w;
        d.vertH = d.h;

        return d;
    }

    // "wd=int" to int
    private int GetParamInt(string s, string wd)
    {
        if (s.IndexOf(wd) >= 0)
        {
            int v;
            if (int.TryParse(s.Substring(wd.Length), out v)) return v;
        }
        return int.MaxValue;
    }

    #endregion
}