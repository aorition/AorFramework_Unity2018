using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace Framework.Editor {


    public class ImportPicturePostprocessor : AssetPostprocessor
    {

        void OnPreprocessTexture()
        {
            TextureImporter importer = assetImporter as TextureImporter;
            if (importer != null)
            {
                if (IsFirstImport(importer))
                {

                    #region 附加图片导入规则示例
                    //importer.textureType = TextureImporterType.Sprite;
                    //                    TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings("iPhone");
                    //                    bool isPowerOfTwo = IsPowerOfTwo(importer);
                    //                    TextureImporterFormat defaultAlpha = isPowerOfTwo ? TextureImporterFormat.PVRTC_RGBA4 : TextureImporterFormat.ASTC_RGBA_4x4;
                    //                    TextureImporterFormat defaultNotAlpha = isPowerOfTwo ? TextureImporterFormat.PVRTC_RGB4 : TextureImporterFormat.ASTC_RGB_6x6;
                    //                    settings.overridden = true;
                    //                    settings.format = importer.DoesSourceTextureHaveAlpha() ? defaultAlpha : defaultNotAlpha;
                    //                    importer.SetPlatformTextureSettings(settings);

                    //                    settings = importer.GetPlatformTextureSettings("Android");
                    //                    settings.overridden = true;
                    //                    settings.allowsAlphaSplitting = false;
                    //                    bool divisible4 = IsDivisibleOf4(importer);
                    //#if UNITY_5
                    //                    defaultAlpha = divisible4 ? TextureImporterFormat.ETC2_RGBA8 : TextureImporterFormat.ASTC_RGBA_4x4;
                    //                    defaultNotAlpha = divisible4 ? TextureImporterFormat.ETC_RGB4 : TextureImporterFormat.ASTC_RGB_6x6;
                    //#elif UNITY_4
                    //                    defaultAlpha = divisible4 ? TextureImporterFormat.ETC2_RGBA8Crunched : TextureImporterFormat.ASTC_RGBA_4x4;
                    //                    defaultNotAlpha = divisible4 ? TextureImporterFormat.ETC_RGB4Crunched : TextureImporterFormat.ASTC_RGB_6x6;
                    //#endif

                    //                    settings.format = importer.DoesSourceTextureHaveAlpha() ? defaultAlpha : defaultNotAlpha;
                    //                    importer.SetPlatformTextureSettings(settings);
                    #endregion

                    //首次导入时为Meta文件添加图片原始尺寸数据
                    int orginalWidth, orginalHegith;
                    GetTextureImporterSize(importer, out orginalWidth, out orginalHegith);

                    Dictionary<string, string> dic = MetaUserDataUtility.GetUserDataDic(importer);
                    if (dic == null) dic = new Dictionary<string, string>();
                    string value = orginalWidth + "," + orginalHegith;
                    if (dic.ContainsKey(MetaUserDataUtility.USERDATA_ORGINALSIZE))
                        dic[MetaUserDataUtility.USERDATA_ORGINALSIZE] = value;
                    else
                        dic.Add(MetaUserDataUtility.USERDATA_ORGINALSIZE, value);

                    MetaUserDataUtility.SetUserDataDic(importer, dic, false);
                }
            }
        }
        //被4整除
        bool IsDivisibleOf4(TextureImporter importer)
        {
            int width;
            int height;
            GetTextureImporterSize(importer, out width, out height);
            return (width % 4 == 0 && height % 4 == 0);
        }

        //2的整数次幂
        bool IsPowerOfTwo(TextureImporter importer)
        {
            int width;
            int height;
            GetTextureImporterSize(importer, out width, out height);
            return (width == height) && (width > 0) && ((width & (width - 1)) == 0);
        }

        //贴图不存在、meta文件不存在、图片尺寸发生修改需要重新导入
        bool IsFirstImport(TextureImporter importer)
        {
            int width;
            int height;
            GetTextureImporterSize(importer, out width, out height);
            Texture tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            bool hasMeta = File.Exists(AssetDatabase.GetAssetPathFromTextMetaFilePath(assetPath));
            return tex == null || !hasMeta || (tex.width != width && tex.height != height);
        }

        //获取导入图片的宽高
        void GetTextureImporterSize(TextureImporter importer, out int width, out int height)
        {
            if (importer != null)
            {
                object[] args = new object[2];
                MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);
                width = (int)args[0];
                height = (int)args[1];
            }
            else
            {
                width = 0;
                height = 0;
            }
        }
    }

}


