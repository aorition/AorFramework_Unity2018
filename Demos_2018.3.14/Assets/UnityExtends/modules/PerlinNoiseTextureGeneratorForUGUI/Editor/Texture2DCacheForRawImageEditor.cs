using UnityEngine;
using UnityEditor;

namespace Framework.Editor
{
    [CustomEditor(typeof(Texture2DCacheForRawImage))]
    public class Texture2DCacheForRawImageEditor : UnityEditor.Editor
    {

        [MenuItem("Component/UI/RawImagePlus/Texture2DCacheForRawImage")]
        private static void AddComponent()
        {
            GameObject select = Selection.activeGameObject;
            if(select)
            {
                select.AddComponent<Texture2DCacheForRawImage>();
            }
        }

        //-------------------------------------------------

        private Texture2DCacheForRawImage m_target;

        private void Awake()
        {
            m_target = target as Texture2DCacheForRawImage;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            GUILayout.Space(24);

            serializedObject.Update();

            GUILayout.Space(12);

            if(m_target.texture)
            {
                GUILayout.BeginVertical("box");
                {
                    GUILayout.Space(2);

                    GUILayout.BeginHorizontal();
                    {
                        if(GUILayout.Button("SaveTexture2DToCache"))
                        {
                            Texture2D texture2D = m_target.texture as Texture2D;
                            if(texture2D)
                            {
                                if(texture2D.format != TextureFormat.RGBA32)
                                {
                                    string path = AssetDatabase.GetAssetPath(texture2D);
                                    if(!string.IsNullOrEmpty(path))
                                    {
                                        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
                                        if(importer)
                                        {

                                            TextureImporterPlatformSettings defSettings = importer.GetDefaultPlatformTextureSettings();
                                            defSettings.textureCompression = TextureImporterCompression.Uncompressed;
                                            defSettings.format = TextureImporterFormat.RGBA32;
                                            importer.SetPlatformTextureSettings(defSettings);

                                            importer.alphaIsTransparency = true;
                                            importer.alphaSource = TextureImporterAlphaSource.FromInput;
                                            importer.mipmapEnabled = false;

                                            importer.SaveAndReimport();
                                            AssetDatabase.Refresh();

                                            m_target.SaveTexToCache(texture2D);

                                        }
                                    }
                                }
                                else
                                {
                                    m_target.SaveTexToCache(texture2D);
                                }
                            }
                        }
                        if(GUILayout.Button("ClearTexture"))
                        {
                            m_target.texture = null;
                        }
                    }
                    GUILayout.EndHorizontal();


                    GUILayout.Space(2);
                }
                GUILayout.EndVertical();
            }

            if(m_target.CacheLens > 0)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("CacheLens[" + m_target.CacheLens + "]");
                    if(GUILayout.Button("ClearCache"))
                    {
                        m_target.ClearCache();
                    }
                    if(GUILayout.Button("TryRecoverTexFormCache"))
                    {
                        m_target.TryRecoverTexFormCache();
                    }
                }
                GUILayout.EndHorizontal();

            }

            serializedObject.ApplyModifiedProperties();

        }

    }
}
