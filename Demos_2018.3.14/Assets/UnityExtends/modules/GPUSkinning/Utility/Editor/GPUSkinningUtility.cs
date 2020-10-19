using Framework.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Framework.GPUSkinning.Editor
{
    public static class GPUSkinningUtility
    {

        /// <summary>
        /// 本代码只采样一个顶点上的两个bone权重，超过2个就被废弃了
        /// </summary>
        public static void BakeGPUSkinningPrefab(GameObject instObj, string name, bool checkRoot, bool instance, bool mLighting, bool useStandaloneMesh)
        {

            string instObjPath = AssetDatabase.GetAssetPath(instObj);
            if(string.IsNullOrEmpty(instObjPath))
            {
                //不是预制体
                return;
            }

            EditorAssetInfo insAssetInfo = new EditorAssetInfo(instObjPath);
            string saveDir_prefab = insAssetInfo.dirPath;

            GPUSkinningResCacheAsset resCache = GPUSkinningResCacheAsset.GetOrCreate();

            if(string.IsNullOrEmpty(name))
                name = instObj.name;

            name = name.Trim();
            GameObject target = GameObject.Instantiate<GameObject>(instObj);
            target.name = instance ? name + "_GPUSkinning" : name;

            SkinnedMeshRenderer[] skinnedMeshRenderers = target.GetComponentsInChildren<SkinnedMeshRenderer>();
            if(skinnedMeshRenderers != null && skinnedMeshRenderers.Length > 0)
            {
                for(int i = 0; i < skinnedMeshRenderers.Length; i++)
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = skinnedMeshRenderers[i];
                    Animator animator = skinnedMeshRenderer.gameObject.GetComponentInParent<Animator>();
                    if(!animator)
                    {
                        //没有发现Animator
                        continue;
                    }
                    GameObject animRoot = animator.gameObject;

                    //create skinning components
                    var texturen = animRoot.AddComponent<GPUSkinningTexturen>();
                    var instancing = animRoot.AddComponent<GPUSkinAndInstancing>();
                    var meshFilter = animRoot.AddComponent<MeshFilter>();
                    var meshRenderer = animRoot.AddComponent<MeshRenderer>();
                    //Todo ？？ 还有用否？
                    if(checkRoot)
                        CheckRootBoneValid(skinnedMeshRenderer);

                    Mesh originMesh = skinnedMeshRenderer.sharedMesh;

                    //增加判断Mesh的唯一性
                    string fbxTag = string.Empty;
                    string fbxPath = AssetDatabase.GetAssetPath(originMesh);
                    if(!string.IsNullOrEmpty(fbxPath) && fbxPath.Contains("@"))
                    {
                        fbxTag = "_" + fbxPath.Split('@')[1].ToLower().Replace(".fbx", "");
                    }
                    string MeshName = originMesh.name + fbxTag;

                    string mPath = AssetDatabase.GetAssetPath(originMesh);
                    //string mResDirName = "GPUSkinRes_" + MeshName;
                    string mResDirName = "GPUSkinRes";
                    EditorAssetInfo mInfo = new EditorAssetInfo(mPath);
                    if(!AssetDatabase.IsValidFolder(mInfo.parentDirPath + "/" + mResDirName))
                    {
                        AssetDatabase.CreateFolder(mInfo.parentDirPath, mResDirName);
                    }
                    string saveDir_GPUSkin = mInfo.parentDirPath + "/" + mResDirName;

                    string cache_key = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(originMesh));
                    bool resCacheDirty = false;
                    GPUSkinningResCacheItem item = resCache.GetValue(cache_key);
                    if(item == null)
                    {
                        item = resCache.Add(cache_key, new GPUSkinningResCacheItem());
                        resCacheDirty = true;
                    }

                    //创建GPUSkinning资源文件夹
                    // string saveDir_GPUSkin = GPUSkinningResRootPath + "/" + name;
                    // if(!AssetDatabase.IsValidFolder(GPUSkinningResRootPath + "/" + name))
                    // {
                    //     AssetDatabase.CreateFolder(GPUSkinningResRootPath, name);
                    //     AssetDatabase.SaveAssets();
                    //     AssetDatabase.Refresh();
                    // }

                    if(useStandaloneMesh)
                    {
                        string meshGUID = item.GUID_mesh;
                        Mesh mesh = null;
                        if(!string.IsNullOrEmpty(meshGUID))
                            mesh = AssetDatabase.LoadAssetAtPath<Mesh>(AssetDatabase.GUIDToAssetPath(meshGUID));
                        if(mesh == null)
                        {
                            mesh = GameObject.Instantiate<Mesh>(originMesh);
                            mesh.name = originMesh.name + "_GPUSkin";

                            AssetDatabase.CreateAsset(mesh, saveDir_GPUSkin + "/" + MeshName + "_GPUSkinMesh.asset");
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();

                            item.GUID_mesh = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(mesh));
                            resCacheDirty = true;
                        }
                        else
                        {
                            //更新

                            Mesh newMesh = GameObject.Instantiate<Mesh>(originMesh);
                            newMesh.name = originMesh.name + "_GPUSkin";

                            mesh.vertices = newMesh.vertices;
                            mesh.triangles = newMesh.triangles;
                            mesh.colors = newMesh.colors;
                            mesh.tangents = newMesh.tangents;
                            mesh.normals = newMesh.normals;
                            mesh.bounds = newMesh.bounds;
                            mesh.bindposes = newMesh.bindposes;
                            mesh.boneWeights = newMesh.boneWeights;
                            mesh.uv = newMesh.uv;
                            mesh.uv2 = newMesh.uv2;

                            EditorUtility.SetDirty(mesh);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                        meshFilter.sharedMesh = mesh;
                    }
                    else
                    {

                        //检查引用的Mesh是否属于FBX,并且保证其可读属性是打开的
                        if(!string.IsNullOrEmpty(fbxPath))
                        {
                            EditorAssetInfo eaInfo = new EditorAssetInfo(fbxPath);
                            if(eaInfo.suffix == ".fbx")
                            {
                                ModelImporter importer = (ModelImporter)ModelImporter.GetAtPath(eaInfo.path);
                                if(importer && !importer.isReadable)
                                {
                                    importer.isReadable = true;
                                    importer.SaveAndReimport();
                                    AssetDatabase.SaveAssets();
                                    AssetDatabase.Refresh();
                                }
                            }
                        }

                        meshFilter.sharedMesh = originMesh;
                    }

                    //创建Material
                    string matGUID = mLighting ? item.GUID_lightMat : item.GUID_mat;
                    Material mat = null;
                    if(!string.IsNullOrEmpty(matGUID))
                        mat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(matGUID));
                    if(mat == null)
                    {
                        mat = mLighting ? new Material(Shader.Find(GPU_SKIN_LIGHT_SHADER)) : new Material(Shader.Find(GPU_SKIN_SHADER));
                        AssetDatabase.CreateAsset(mat, saveDir_GPUSkin + "/" + MeshName + (mLighting ? "_Lt" : "") + "_mat.mat");

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        if(mLighting)
                            item.GUID_lightMat = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(mat));
                        else
                            item.GUID_mat = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(mat));

                        resCacheDirty = true;
                    }

                    Texture mainTex = skinnedMeshRenderer.sharedMaterial.GetTexture("_MainTex");
                    meshRenderer.sharedMaterial = mat;
                    mat.enableInstancing = true;
                    mat.SetTexture("_MainTex", mainTex);
                    EditorUtility.SetDirty(mat);

                    //创建animMap
                    //验证AnimatiorController
                    bool verfiyAnimMap = VerifyAnimControllerByPrefab(animator, ref item);
                    string animMapGUID = item.GUID_animMap;
                    Texture2D animMap = null;
                    if(!string.IsNullOrEmpty(animMapGUID) && verfiyAnimMap)
                    {
                        animMap = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(animMapGUID));
                        //恢复texturen数据 
                        //（暂时废弃）
                        // if(animMap && item.Cache_animMap_frameNames != null && item.Cache_animMap_frames != null)
                        // {
                        //     texturen.frames = item.Cache_animMap_frames;
                        //     texturen.frameNames = item.Cache_animMap_frameNames;
                        // }
                    }

                    if(animMap == null)
                    {

                        AnalysisMesh(originMesh, skinnedMeshRenderer, out List<GPUSkinning_Bone> bonelist, out GPUSkinning_Bone[] originBones, out int rootBoneIndex);
                        AnalysisAnimation(animRoot, animator, originBones, ref item, out GPUSkinning_BoneAnimation[] boneAnimations);
                        animMap = SaveTextures(originBones, boneAnimations, bonelist, texturen, rootBoneIndex, ref item);
                        animMap.name = MeshName + "_animMap";

                        AssetDatabase.CreateAsset(animMap, saveDir_GPUSkin + "/" + MeshName + "_animMap.asset");
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        item.GUID_animMap = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(animMap));
                        resCacheDirty = true;

                    }
                    else
                    {
                        //更新
                        AnalysisMesh(originMesh, skinnedMeshRenderer, out List<GPUSkinning_Bone> bonelist, out GPUSkinning_Bone[] originBones, out int rootBoneIndex);
                        AnalysisAnimation(animRoot, animator, originBones, ref item, out GPUSkinning_BoneAnimation[] boneAnimations);

                        Texture2D newAnimMap = SaveTextures(originBones, boneAnimations, bonelist, texturen, rootBoneIndex, ref item);
                        newAnimMap.name = MeshName + "_animMap";

                        if(newAnimMap.width.Equals(animMap.width) && newAnimMap.height.Equals(animMap.height))
                        {
                            animMap.SetPixels(newAnimMap.GetPixels(0, 0, newAnimMap.width, newAnimMap.height));
                            animMap.Apply();
                        }
                        else
                        {
                            animMap.Resize(newAnimMap.width, newAnimMap.height);
                            animMap.SetPixels(newAnimMap.GetPixels(0, 0, newAnimMap.width, newAnimMap.height));
                            animMap.Apply();
                        }

                        EditorUtility.SetDirty(animMap);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                    }

                    mat.SetTexture("_AnimMap", animMap);

                    //移除无用Component
                    if(animator)
                        GameObject.DestroyImmediate(animator);

                    Body body = animRoot.GetComponent<Body>();
                    if(body)
                        GameObject.DestroyImmediate(body);


                    for(int j = animRoot.transform.childCount - 1; j >= 0; j--)
                    {
                        GameObject child = animRoot.transform.GetChild(j).gameObject;

                        if(child.GetComponent<MeshRenderer>() != null)
                            continue;

                        GameObject.DestroyImmediate(child);
                    }

                    if(resCacheDirty)
                        EditorUtility.SetDirty(resCache);

                }

                //移除无用Component
                GameObject.DestroyImmediate(target.GetComponent<NeedConvertoGPUSkinningPrefab>());

                //保存GPUSkin Prefab （不需要将Prefab保存在GPUSkin文件夹下，因此废弃此段）
                // string savePath = saveDir_GPUSkin + "/" + name + ".prefab";
                // if(AssetDatabase.DeleteAsset(savePath))
                // {
                //     AssetDatabase.SaveAssets();
                //     AssetDatabase.Refresh();
                // }
                // PrefabUtility.SaveAsPrefabAsset(target, saveDir_GPUSkin + "/" + name + ".prefab");

                //保存目标 Prefab
                string savePath = saveDir_prefab + "/" + (instance ? name + "_GPUSkinning" : name) + ".prefab";
                PrefabUtility.SaveAsPrefabAsset(target, savePath);

                GameObject.DestroyImmediate(target);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

            }

        }

        public static bool VerifyAnimControllerByPrefab(Animator animator, ref GPUSkinningResCacheItem item)
        {
            if(animator.runtimeAnimatorController)
            {
                string p = AssetDatabase.GetAssetPath(animator.runtimeAnimatorController);
                if(!string.IsNullOrEmpty(p) && !string.IsNullOrEmpty(item.MD5_animController))
                {
                    FileInfo inf = new FileInfo(Application.dataPath.Replace("Assets", "") + p);
                    string md5 = AorBaseUtility.MD5Util.GetMD5(inf);
                    if(!string.IsNullOrEmpty(md5))
                    {
                        return md5.Equals(item.MD5_animController);
                    }
                }
            }
            return false;
        }

        #region BakeGPUSkinningPrefab 实现

        //private const string GPUSkinningResRootPath = "Assets/Resources/Model/GUPSkin";

        private const string GPU_SKIN_SHADER = "Xcore/Role/LWGPUSkinningTexturen";
        private const string GPU_SKIN_LIGHT_SHADER = "Xcore/Role/LWGPUSkinningTexturenLight";

        private static void NormalizeQuaternion(ref Quaternion q)
        {
            float sum = 0;
            for(int i = 0; i < 4; ++i)
                sum += q[i] * q[i];
            float magnitudeInverse = 1 / Mathf.Sqrt(sum);
            for(int i = 0; i < 4; ++i)
                q[i] *= magnitudeInverse;
        }

        private static int GetBoneId(GPUSkinning_Bone[] bones, string n)
        {
            string[] narr = n.Split('/');
            string nn = narr[narr.Length - 1];
            for(int i = 0; i < bones.Length; i++)
            {
                if(nn == bones[i].name)
                {
                    return i;
                }
            }
            return -1;
        }

        private static string GetBoneName(string n)
        {
            string[] narr = n.Split('/');
            string nn = narr[narr.Length - 1];
            return nn;
        }

        private static GPUSkinning_Bone GetParentBone(List<GPUSkinning_Bone> bonelist, Transform t, SkinnedMeshRenderer skinnedMeshRenderer)
        {
            if(t == skinnedMeshRenderer.rootBone)
            { return null; }

            Transform parent = t.parent;
            GPUSkinning_Bone parentBone = null;

            foreach(GPUSkinning_Bone bone in bonelist)
            {
                if(bone.transform == parent)
                {
                    parentBone = bone;
                    break;
                }
            }

            if(parentBone == null)
            {
                parentBone = new GPUSkinning_Bone();
                parentBone.transform = parent;
                bonelist.Add(parentBone);
                parentBone.parent = GetParentBone(bonelist, parent, skinnedMeshRenderer);
            }

            return parentBone;
        }

        private static GPUSkinning_Bone[] GetChildBone(List<GPUSkinning_Bone> bonelist, GPUSkinning_Bone bone)
        {
            List<GPUSkinning_Bone> childs = new List<GPUSkinning_Bone>();
            int numBones = bonelist.Count;
            for(int i = 0; i < numBones; ++i)
            {
                if(bonelist[i].parent != null && bonelist[i].parent == bone)
                { childs.Add(bonelist[i]); }
            }
            return childs.ToArray();
        }

        private static void AnalysisMesh(Mesh originMesh, SkinnedMeshRenderer skinnedMeshRenderer,
            out List<GPUSkinning_Bone> bonelist
            , out GPUSkinning_Bone[] originBones,
            out int rootBoneIndex)
        {

            rootBoneIndex = -1;

            bonelist = new List<GPUSkinning_Bone>();

            // BoneWeight[] boneWs = originMesh.boneWeights;
            // Vector4[] tangents = new Vector4[originMesh.vertexCount];
            // for(int i = 0; i < originMesh.vertexCount; i++)
            // {
            //     BoneWeight boneWeight = boneWs[i];
            //     tangents[i].x = boneWeight.boneIndex0;
            //     tangents[i].y = boneWeight.weight0;
            //     tangents[i].z = boneWeight.boneIndex1;
            //     tangents[i].w = boneWeight.weight1;
            // 
            //     if(boneWeight.boneIndex2 > 0 || boneWeight.boneIndex3 > 0)
            //     { throw new Exception("Only support less than 2 bones!!!!"); }
            // }
            // 
            // originMesh.tangents = tangents;

            int meshBoneNum = skinnedMeshRenderer.bones.Length;
            originBones = new GPUSkinning_Bone[meshBoneNum];
            for(int i = 0; i < meshBoneNum; ++i)
            {
                GPUSkinning_Bone bone = new GPUSkinning_Bone();
                originBones[i] = bone;
                bone.id = i;
                bone.transform = skinnedMeshRenderer.bones[i];
                bone.bindpose = originMesh.bindposes[i] /*smr to bone*/;
                bonelist.Add(bone);
            }

            for(int i = 0; i < meshBoneNum; ++i)
            { originBones[i].parent = GetParentBone(bonelist, originBones[i].transform, skinnedMeshRenderer); }

            for(int i = 0; i < bonelist.Count; ++i)
            { bonelist[i].children = GetChildBone(bonelist, bonelist[i]); }

            for(int i = 0; i < bonelist.Count; ++i)
            {
                if(bonelist[i].transform == skinnedMeshRenderer.rootBone)
                {
                    rootBoneIndex = i;
                    break;
                }
            }
        }

        private static void AnalysisAnimation(GameObject gameObject, Animator animator, GPUSkinning_Bone[] originBones, ref GPUSkinningResCacheItem item, out GPUSkinning_BoneAnimation[] boneAnimations)
        {
            if(!animator.runtimeAnimatorController)
            {
                boneAnimations = null;
                return;
            }

            //获取animController的MD5
            string p = AssetDatabase.GetAssetPath(animator.runtimeAnimatorController);
            if(!string.IsNullOrEmpty(p))
            {
                FileInfo inf = new FileInfo(Application.dataPath.Replace("Assets", "") + p);
                item.MD5_animController = AorBaseUtility.MD5Util.GetMD5(inf);
            }

            AnimatorController animController = (AnimatorController)animator.runtimeAnimatorController;
            List<GPUSkinning_BoneAnimation> boneAnimList = new List<GPUSkinning_BoneAnimation>();
            foreach(var animState in animController.layers[0].stateMachine.states)
            {
                AnimatorState state = animState.state;
                AnimationClip clip = (AnimationClip)state.motion;
                if(clip)
                {
                    GPUSkinning_BoneAnimation boneAnimation = ScriptableObject.CreateInstance<GPUSkinning_BoneAnimation>();
                    boneAnimation.fps = (int)clip.frameRate;
                    //boneAnimation.animName = clip.name;
                    boneAnimation.animName = state.name;
                    boneAnimation.frames = new GPUSkinning_BoneAnimationFrame[(int)(clip.length * boneAnimation.fps) + 1];
                    boneAnimation.length = clip.length;

                    for(int frameIndex = 0; frameIndex < boneAnimation.frames.Length; ++frameIndex)
                    {
                        GPUSkinning_BoneAnimationFrame frame = new GPUSkinning_BoneAnimationFrame();
                        boneAnimation.frames[frameIndex] = frame;
                        float second = (float)(frameIndex) / (float)boneAnimation.fps;

                        List<string> bonesPaths = new List<string>();
                        List<Matrix4x4> matrices = new List<Matrix4x4>();
                        List<int> bonesHierarchyIds = new List<int>();
                        List<string> bonesHierarchyNames = new List<string>();
                        List<Matrix4x4> matrices2 = new List<Matrix4x4>();
                        EditorCurveBinding[] curvesBinding = AnimationUtility.GetCurveBindings(clip);
                        foreach(EditorCurveBinding curveBinding in curvesBinding)
                        {
                            string path = curveBinding.path;
                            if(bonesPaths.Contains(path))
                            { continue; }

                            bonesPaths.Add(path);
                            Transform trans = gameObject.transform.Find(path);

                            if(trans == null)
                                continue;

#if UNITY_2018
                            AnimationCurve curveRX = AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, curveBinding.type, "m_LocalRotation.x"));
                            AnimationCurve curveRY = AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, curveBinding.type, "m_LocalRotation.y"));
                            AnimationCurve curveRZ = AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, curveBinding.type, "m_LocalRotation.z"));
                            AnimationCurve curveRW = AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, curveBinding.type, "m_LocalRotation.w"));

                            AnimationCurve curvePX = AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, curveBinding.type, "m_LocalPosition.x"));
                            AnimationCurve curvePY = AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, curveBinding.type, "m_LocalPosition.y"));
                            AnimationCurve curvePZ = AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, curveBinding.type, "m_LocalPosition.z"));

                            AnimationCurve curveSX = AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, curveBinding.type, "m_LocalScale.x"));
                            AnimationCurve curveSY = AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, curveBinding.type, "m_LocalScale.y"));
                            AnimationCurve curveSZ = AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, curveBinding.type, "m_LocalScale.z"));
#elif UNITY_5

                            AnimationCurve curveRX = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalRotation.x");
                            AnimationCurve curveRY = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalRotation.y");
                            AnimationCurve curveRZ = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalRotation.z");
                            AnimationCurve curveRW = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalRotation.w");

                            AnimationCurve curvePX = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalPosition.x");
                            AnimationCurve curvePY = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalPosition.y");
                            AnimationCurve curvePZ = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalPosition.z");

                            AnimationCurve curveSX = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalScale.x");
                            AnimationCurve curveSY = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalScale.y");
                            AnimationCurve curveSZ = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalScale.z");
#endif
                            Vector3 scale = trans.localScale;

                            if(curveSX != null || curveSY != null || curveSZ != null)
                            {
                                scale = new Vector3(
                                    curveSX != null ? curveSX.Evaluate(second) : 0,
                                    curveSY != null ? curveSY.Evaluate(second) : 0,
                                    curveSZ != null ? curveSZ.Evaluate(second) : 0);
                            }

                            Vector3 translation = trans.localPosition;
                            if(curvePX != null || curvePY != null || curvePZ != null)
                            {
                                translation = new Vector3(
                                    curvePX != null ? curvePX.Evaluate(second) : 0,
                                    curvePY != null ? curvePY.Evaluate(second) : 0,
                                    curvePZ != null ? curvePZ.Evaluate(second) : 0);
                            }

                            Quaternion rotation = trans.localRotation;
                            ;

                            if(curveRX != null || curveRY != null || curveRZ != null || curveRW != null)
                            {
                                rotation = new Quaternion(
                                    curveRX != null ? curveRX.Evaluate(second) : 0,
                                    curveRY != null ? curveRY.Evaluate(second) : 0,
                                    curveRZ != null ? curveRZ.Evaluate(second) : 0,
                                    curveRW != null ? curveRW.Evaluate(second) : 0);
                            }

                            NormalizeQuaternion(ref rotation);
                            int boneId = GetBoneId(originBones, path);
                            if(boneId != -1)
                            {
                                bonesHierarchyIds.Add(boneId);
                                matrices.Add(Matrix4x4.TRS(translation, rotation, scale));
                            }
                            else
                            {
                                bonesHierarchyNames.Add(GetBoneName(path));
                                matrices2.Add(Matrix4x4.TRS(translation, rotation, scale));
                            }
                        }

                        frame.matrices = matrices.ToArray();
                        frame.bonesId = bonesHierarchyIds.ToArray();
                        frame.bonesName = bonesHierarchyNames.ToArray();
                        frame.matrices2 = matrices2.ToArray();
                    }

                    boneAnimList.Add(boneAnimation);
                }
            }
            boneAnimations = boneAnimList.ToArray();

#region 之前的算法
            //AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            //boneAnimations = new GPUSkinning_BoneAnimation[clips.Length];
            //for(int i = 0; i < clips.Length; i++)
            //{
            //    var clip = clips[i];
            //    GPUSkinning_BoneAnimation boneAnimation = ScriptableObject.CreateInstance<GPUSkinning_BoneAnimation>();
            //    boneAnimation.fps = (int)clip.frameRate;
            //    boneAnimation.animName = clip.name;
            //    boneAnimation.frames = new GPUSkinning_BoneAnimationFrame[(int)(clip.length * boneAnimation.fps) + 1];
            //    boneAnimation.length = clip.length;
            //    boneAnimations[i] = boneAnimation;

            //    for(int frameIndex = 0; frameIndex < boneAnimation.frames.Length; ++frameIndex)
            //    {
            //        GPUSkinning_BoneAnimationFrame frame = new GPUSkinning_BoneAnimationFrame();
            //        boneAnimation.frames[frameIndex] = frame;
            //        float second = (float)(frameIndex) / (float)boneAnimation.fps;

            //        List<string> bonesPaths = new List<string>();
            //        List<Matrix4x4> matrices = new List<Matrix4x4>();
            //        List<int> bonesHierarchyIds = new List<int>();
            //        List<string> bonesHierarchyNames = new List<string>();
            //        List<Matrix4x4> matrices2 = new List<Matrix4x4>();
            //        EditorCurveBinding[] curvesBinding = AnimationUtility.GetCurveBindings(clip);
            //        foreach(EditorCurveBinding curveBinding in curvesBinding)
            //        {
            //            string path = curveBinding.path;
            //            if(bonesPaths.Contains(path))
            //            { continue; }

            //            bonesPaths.Add(path);
            //            Transform trans = gameObject.transform.Find(path);

            //            if(trans == null)
            //                continue;

            //            AnimationCurve curveRX = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalRotation.x");
            //            AnimationCurve curveRY = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalRotation.y");
            //            AnimationCurve curveRZ = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalRotation.z");
            //            AnimationCurve curveRW = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalRotation.w");

            //            AnimationCurve curvePX = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalPosition.x");
            //            AnimationCurve curvePY = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalPosition.y");
            //            AnimationCurve curvePZ = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalPosition.z");

            //            AnimationCurve curveSX = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalScale.x");
            //            AnimationCurve curveSY = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalScale.y");
            //            AnimationCurve curveSZ = AnimationUtility.GetEditorCurve(clip, path, curveBinding.type, "m_LocalScale.z");

            //            Vector3 scale = trans.localScale;

            //            if(curveSX != null || curveSY != null || curveSZ != null)
            //            {
            //                scale = new Vector3(
            //                    curveSX != null ? curveSX.Evaluate(second) : 0,
            //                    curveSY != null ? curveSY.Evaluate(second) : 0,
            //                    curveSZ != null ? curveSZ.Evaluate(second) : 0);
            //            }

            //            Vector3 translation = trans.localPosition;
            //            if(curvePX != null || curvePY != null || curvePZ != null)
            //            {
            //                translation = new Vector3(
            //                    curvePX != null ? curvePX.Evaluate(second) : 0,
            //                    curvePY != null ? curvePY.Evaluate(second) : 0,
            //                    curvePZ != null ? curvePZ.Evaluate(second) : 0);
            //            }

            //            Quaternion rotation = trans.localRotation;
            //            ;

            //            if(curveRX != null || curveRY != null || curveRZ != null || curveRW != null)
            //            {
            //                rotation = new Quaternion(
            //                    curveRX != null ? curveRX.Evaluate(second) : 0,
            //                    curveRY != null ? curveRY.Evaluate(second) : 0,
            //                    curveRZ != null ? curveRZ.Evaluate(second) : 0,
            //                    curveRW != null ? curveRW.Evaluate(second) : 0);
            //            }

            //            NormalizeQuaternion(ref rotation);
            //            int boneId = GetBoneId(originBones, path);
            //            if(boneId != -1)
            //            {
            //                bonesHierarchyIds.Add(boneId);
            //                matrices.Add(Matrix4x4.TRS(translation, rotation, scale));
            //            }
            //            else
            //            {
            //                bonesHierarchyNames.Add(GetBoneName(path));
            //                matrices2.Add(Matrix4x4.TRS(translation, rotation, scale));
            //            }
            //        }

            //        frame.matrices = matrices.ToArray();
            //        frame.bonesId = bonesHierarchyIds.ToArray();
            //        frame.bonesName = bonesHierarchyNames.ToArray();
            //        frame.matrices2 = matrices2.ToArray();
            //    }
            //}
#endregion
        }

        private static void CheckRootBoneValid(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            var rootBone = skinnedMeshRenderer.rootBone;
            var rootParent = rootBone.parent;
            if(rootParent != null && rootParent.name.ToLower().Contains("bip"))
            { throw new Exception("根节点设置不正确"); }
        }

        private static Texture2D SaveTextures(GPUSkinning_Bone[] originBones, GPUSkinning_BoneAnimation[] boneAnimations, List<GPUSkinning_Bone> bonelist, GPUSkinningTexturen texturen, int rootBoneIndex, ref GPUSkinningResCacheItem item)
        {
            int width = 0;
            int height = originBones.Length * 4;

            texturen.frames = new Vector2[boneAnimations.Length];
            texturen.frameNames = new string[boneAnimations.Length];
            for(int i = 0; i < boneAnimations.Length; i++)
            {
                //Todo 生成frames信息在某动画小于2帧时，会出Bug。
                Vector2 v2 = new Vector2(width + 1, width + boneAnimations[i].frames.Length - 1);
                //Vector2 v2 = new Vector2(width, width + boneAnimations[i].frames.Length - 1);
                width += boneAnimations[i].frames.Length;
                texturen.frames[i] = v2;
                texturen.frameNames[i] = boneAnimations[i].animName;
            }

            item.Cache_animMap_frames = texturen.frames;
            item.Cache_animMap_frameNames = texturen.frameNames;

            Texture2D animMap = new Texture2D(width, height, TextureFormat.RGBAHalf, false);
            animMap.filterMode = FilterMode.Point;
            animMap.wrapMode = TextureWrapMode.Clamp;
            int x = 0;
            int y = 0;
            for(int i = 0; i < boneAnimations.Length; i++)
            { FillTexture(bonelist, originBones, rootBoneIndex, boneAnimations[i], animMap, height, ref x, ref y); }

            animMap.Apply();
            return animMap;
        }

        private static void FillTexture(List<GPUSkinning_Bone> bonelist, GPUSkinning_Bone[] originBones, int rootBoneIndex, GPUSkinning_BoneAnimation ba, Texture2D animMap, int height, ref int x, ref int y)
        {
            for(int frameIndex = 0; frameIndex < ba.frames.Length; ++frameIndex)
            {
                UpdateBoneTransformMatrix(bonelist[rootBoneIndex], Matrix4x4.identity, ba.frames[frameIndex]);

                for(int i = 0; i < originBones.Length; ++i)
                {
                    Matrix4x4 mx = originBones[i].animationMatrix;
                    int ii = i * 4 + y * height;
                    animMap.SetPixel(x, ii++, new Color(mx.m00, mx.m01, mx.m02, mx.m03));
                    animMap.SetPixel(x, ii++, new Color(mx.m10, mx.m11, mx.m12, mx.m13));
                    animMap.SetPixel(x, ii++, new Color(mx.m20, mx.m21, mx.m22, mx.m23));
                    animMap.SetPixel(x, ii++, new Color(mx.m30, mx.m31, mx.m32, mx.m33));
                }

                x++;
            }
        }

        private static void UpdateBoneTransformMatrix(GPUSkinning_Bone bone, Matrix4x4 parentMatrix, GPUSkinning_BoneAnimationFrame frame)
        {
            int index = BoneAnimationFrameIndexOf(frame, bone);
            Matrix4x4 mat;
            if(index == -1)
            {
                index = BoneAnimationFrameIndexOf2(frame, bone);
                if(index == -1)
                { mat = parentMatrix * Matrix4x4.TRS(bone.transform.localPosition, bone.transform.localRotation, bone.transform.localScale); }
                else
                { mat = parentMatrix * frame.matrices2[index]; }
            }
            else
            { mat = parentMatrix * frame.matrices[index]; }

            bone.animationMatrix = mat * bone.bindpose;
            GPUSkinning_Bone[] children = bone.children;
            int numChildren = children.Length;
            for(int i = 0; i < numChildren; ++i)
            { UpdateBoneTransformMatrix(children[i], mat, frame); }
        }

        private static int BoneAnimationFrameIndexOf2(GPUSkinning_BoneAnimationFrame frame, GPUSkinning_Bone bone)
        {
            string[] bs = frame.bonesName;
            for(int i = 0; i < bs.Length; i++)
            {
                if(bs[i] == bone.name)
                { return i; }
            }

            return -1;
        }

        private static int BoneAnimationFrameIndexOf(GPUSkinning_BoneAnimationFrame frame, GPUSkinning_Bone bone)
        {
            int[] bs = frame.bonesId;
            for(int i = 0; i < bs.Length; i++)
            {
                if(bs[i] == bone.id)
                { return i; }
            }

            return -1;
        }

#endregion

    }

}

