using System;
using System.Collections.Generic;
using System.IO;
using AorBaseUtility.Extends;
using Framework.Extends;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor.Utility
{
    public class BMFontCreateToolWindow : UnityEditor.EditorWindow
    {
        private static GUIStyle _titleStyle;
        protected static GUIStyle titleStyle
        {
            get
            {
                if (_titleStyle == null)
                {
                    _titleStyle = EditorStyles.largeLabel.Clone();
                    _titleStyle.fontSize = 16;
                }
                return _titleStyle;
            }
        }

        private static GUIStyle _sTitleStyle;
        protected static GUIStyle sTitleStyle
        {
            get
            {
                if (_sTitleStyle == null)
                {
                    _sTitleStyle = EditorStyles.largeLabel.Clone();
                    _sTitleStyle.fontSize = 14;
                    _sTitleStyle.fontStyle = FontStyle.Bold;
                }
                return _sTitleStyle;
            }
        }

        private static GUIContent _GUITitleContent;
        protected static GUIContent guiTitleContent
        {
            get
            {
                if(_GUITitleContent == null)
                {
                    _GUITitleContent = new GUIContent("BMFontTool");
                }
                return _GUITitleContent;
            }
        }

        //--------------------------------------------------------------

        private static BMFontCreateToolWindow _instance;

        [MenuItem("AssetsManagedTools/BMFont/BMFont生成工具")]
        public static BMFontCreateToolWindow init()
        {

            _instance = UnityEditor.EditorWindow.GetWindow<BMFontCreateToolWindow>();
            _instance.minSize = new Vector2(497, 280);
            _instance.titleContent = guiTitleContent;
            return _instance;
        }

        private Vector2 _scrollPos = new Vector2();
        private void OnGUI()
        {

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            GUILayout.Space(15);

            _draw_toolTitle_UI();

            GUILayout.Space(15);

            _draw_create_UI();

            GUILayout.Space(5);

            GUILayout.EndScrollView();

        }

        //--------------------------------------

        private void _draw_toolTitle_UI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("      BMFont字体生成工具      ", titleStyle);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("提示: 支持BMFont生成工具生成的.Fnt文件以及字体图集");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(10);
            }
            GUILayout.EndVertical();
        }

        private Texture2D _fotTex;
        private TextAsset _fntFile;

        private void _draw_create_UI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                EditorPlusMethods.Draw_TitleLabelUI("------ BMFont数据导入 ------", sTitleStyle);
                GUILayout.Space(5);
            }
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical(GUILayout.Width(Screen.width * 0.7f));
                {
                    _fotTex = (Texture2D)EditorGUILayout.ObjectField(_fntFile, typeof(Texture2D), false);
                    _fntFile = (TextAsset)EditorGUILayout.ObjectField(_fntFile, typeof(TextAsset), false);
                }
                GUILayout.EndVertical();

                if (GUILayout.Button("SetFromSelection", GUILayout.Height(28)))
                {
                    foreach (UnityEngine.Object o in Selection.objects)
                    {

                        if (o == null) continue;
                        if (o is Texture2D)
                        {
                            _fotTex = (Texture2D)o;
                        }
                        else if(o is TextAsset)
                        {
                            _fntFile = (TextAsset)o;
                            break;
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            if (_fntFile)
            {
                if (GUILayout.Button("Start", GUILayout.Height(36)))
                {
                    createBMFont(_fntFile, _fotTex);
                }
            }
            else
            {
                GUI.color = Color.gray;
                if (GUILayout.Button("Start", GUILayout.Height(36)))
                {
                    //do nothing;
                }
                GUI.color = Color.white;
            }


            GUILayout.Space(5);

            GUILayout.EndVertical();
        }

        private static void createBMFont(TextAsset import, Texture2D tex)
        {
            Material mat = null;
            Font m_myFont = null;
            TextAsset m_data = import;

            if (m_data == null || string.IsNullOrEmpty(m_data.text))
            {
                EditorUtility.DisplayDialog("提示", "没有选中fnt文件", "OK");
                return;
            }

            EditorAssetInfo baInfo = new EditorAssetInfo(AssetDatabase.GetAssetPath(m_data));
            string filePath = baInfo.dirPath + "/" + baInfo.name;

            string matPathName = filePath + ".mat";
            string fontPathName = filePath + ".fontsettings";

            //获取材质，如果没有则创建对应名字的材质  
            if (mat == null)
            {
                mat = (Material)AssetDatabase.LoadAssetAtPath(matPathName, typeof(Material));
            }
            if (mat == null)
            {
                mat = new Material(Shader.Find("GUI/Text Shader"));
                AssetDatabase.CreateAsset(mat, matPathName);
            }
            else
            {
                mat.shader = Shader.Find("GUI/Text Shader");
            }
            mat.SetTexture("_MainTex", tex);

            //获取font文件，如果没有则创建对应名字的font文件  
            if (m_myFont == null)
            {
                m_myFont = (Font)AssetDatabase.LoadAssetAtPath(fontPathName, typeof(Font));
            }
            if (m_myFont == null)
            {
                m_myFont = new Font();
                AssetDatabase.CreateAsset(m_myFont, fontPathName);
            }
            m_myFont.material = mat;

            BMFont mbFont = new BMFont();
            //借助NGUI的类，读取字体fnt文件信息，可以不用自己去解析了  
            BMFontReader.Load(mbFont, m_data.name, m_data.bytes);
            CharacterInfo[] characterInfo = new CharacterInfo[mbFont.glyphs.Count];
            for (int i = 0; i < mbFont.glyphs.Count; i++)
            {
                BMGlyph bmInfo = mbFont.glyphs[i];
                CharacterInfo info = new CharacterInfo();
                //设置ascii码  
                info.index = bmInfo.index;
                //设置字符映射到材质上的坐标  
                info.uvBottomLeft = new Vector2((float)bmInfo.x / mbFont.texWidth, 1f - (float)(bmInfo.y + bmInfo.height) / mbFont.texHeight);
                info.uvBottomRight = new Vector2((float)(bmInfo.x + bmInfo.width) / mbFont.texWidth, 1f - (float)(bmInfo.y + bmInfo.height) / mbFont.texHeight);
                info.uvTopLeft = new Vector2((float)bmInfo.x / mbFont.texWidth, 1f - (float)(bmInfo.y) / mbFont.texHeight);
                info.uvTopRight = new Vector2((float)(bmInfo.x + bmInfo.width) / mbFont.texWidth, 1f - (float)(bmInfo.y) / mbFont.texHeight);
                //设置字符顶点的偏移位置和宽高  
                info.minX = bmInfo.offsetX;
                info.minY = -bmInfo.offsetY - bmInfo.height;
                info.maxX = bmInfo.offsetX + bmInfo.width;
                info.maxY = -bmInfo.offsetY;
                //设置字符的宽度  
                info.advance = bmInfo.advance;
                characterInfo[i] = info;
            }
            m_myFont.characterInfo = characterInfo;
            EditorUtility.SetDirty(m_myFont);//设置变更过的资源  
            EditorUtility.SetDirty(mat);//设置变更过的资源  
            AssetDatabase.SaveAssets();//保存变更的资源  
            AssetDatabase.Refresh();//刷新资源，貌似在Mac上不起作用  

            //由于上面fresh之后在编辑器中依然没有刷新，所以暂时想到这个方法，  
            //先把生成的字体导出成一个包，然后再重新导入进来，这样就可以直接刷新了  
            //这是在Mac上遇到的，不知道Windows下面会不会出现，如果不出现可以把下面这一步注释掉  
            //AssetDatabase.ExportPackage(fontPathName, "temp.unitypackage");
            //AssetDatabase.DeleteAsset(fontPathName);
            //AssetDatabase.ImportPackage("temp.unitypackage", true);
            //AssetDatabase.Refresh();

            Debug.Log("创建字体成功");
        }

    }
}
