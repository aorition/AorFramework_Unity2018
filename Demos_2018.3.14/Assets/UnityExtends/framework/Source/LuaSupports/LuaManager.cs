using UnityEngine;
using System.Collections;
using LuaInterface;
using System.IO;
using System;

namespace Framework.Lua {

    [AddComponentMenu("")] //在菜单中隐藏此组件
    public class LuaManager : ManagerBase
    {

        //========= Manager 模版 =============================
        //
        // 基于MonoBehavior的Manager类 需要遵循的约定:
        //
        // *. 采用_instance字段保存静态单例.
        // *. 非自启动Manager必须提供CreateInstance静态方法.
        // *. 提供Request静态方法.
        // *. 提供IsInit静态方法判定改Manager是否初始化
        // *. 须Awake中调用ManagerBase.VerifyUniqueOnInit验证单例唯一
        // *. 须Awake中调用ManagerBase.VerifyUniqueOnInit验证单例唯一
        //
        //=============== 基于MonoBehavior的Manager====================

        //@@@ 静态方法实现区

        private static LuaManager _instance;
        public static LuaManager Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// 创建带有独立GameObject的Instance
        /// </summary>
        public static LuaManager CreateInstance(Transform parenTransform = null)
        {
            return ManagerBase.CreateInstance<LuaManager>(ref _instance, parenTransform);
        }

        /// <summary>
        /// 在目标GameObject上的创建Instance
        /// </summary>
        public static LuaManager CreateInstanceOnGameObject(GameObject target)
        {
            return ManagerBase.CreateInstanceOnGameObject<LuaManager>(ref _instance, target);
        }

        public static void Request(Action GraphicsManagerIniteDoSh)
        {
            ManagerBase.Request(ref _instance, GraphicsManagerIniteDoSh);
        }

        public static bool IsInit()
        {
            return ManagerBase.VerifyIsInit(ref _instance);
        }

        //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        //@@@ override方法实现区

        protected override void Awake()
        {
            base.Awake();
            ManagerBase.VerifyUniqueOnAwake(ref _instance, this);
        }

        protected override void OnUnSetupedStart()
        {
            throw new Exception("*** LuaManager has been Setuped yet.");
        }

        protected override void OnDestroy()
        {
            //
            base.OnDestroy();
            m_onAfterInitDo = null;
            if (m_luaClient != null)
            {
                m_luaClient.Destroy();
                m_luaClient = null;
            }
            ManagerBase.VerifyUniqueOnDispose(ref _instance, this);
        }

        protected override void init()
        {
            InitLuaPath();
            InitLuaBundle();
        }

        protected override void OnAfterInit()
        {
            if (m_onAfterInitDo != null)
            {
                Action d = m_onAfterInitDo;
                d();
                m_onAfterInitDo = null;
            }
        }

        //=======================================================================

        public void Setup(LuaClient luaClient,string luaDataPath, bool developMode = false, bool luaBundleMode = false, Action onAfterInitDo = null)
        {
            m_luaClient = luaClient;
            m_luaDataPath = luaDataPath.EndsWith("/") ? luaDataPath.Substring(0, luaDataPath.Length - 1) : luaDataPath;
            m_developMode = developMode;
            m_luaBundleMode = luaBundleMode;
            m_onAfterInitDo = onAfterInitDo;
            _isSetuped = true;
        }

        private LuaClient m_luaClient;

        private bool m_developMode;
        private bool m_luaBundleMode;

        private string m_luaDataPath;

        private Action m_onAfterInitDo;

        private void InitLuaPath()
        {
            //LuaClient.GetMainState().AddSearchPath(m_luaDataPath);
            AddLuaASearchPath(m_luaDataPath);
        }

        private void AddBundle(string bundleName)
        {
            //string url = Tools.DataPath + "/lua/" + bundleName;
            string url = m_luaDataPath + "/" + bundleName;
            if (File.Exists(url))
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(url);
                if (bundle != null)
                {
                    bundleName = bundleName.Replace("lua/", "");
                    bundleName = bundleName.Replace(".unity3d", "");
                    LuaFileUtils.Instance.AddSearchBundle(bundleName.ToLower(), bundle);
                }
            }
        }

        /// <summary>
        /// 初始化LuaBundle
        /// </summary>
        private void InitLuaBundle()
        {
            if (!m_developMode && m_luaBundleMode)
            {
                AddBundle("lua/lua.unity3d");
                AddBundle("lua/lua_math.unity3d");
                AddBundle("lua/lua_system.unity3d");
                AddBundle("lua/lua_u3d.unity3d");
                AddBundle("lua/lua_misc.unity3d");
                AddBundle("lua/lua_cjson.unity3d");
                AddBundle("lua/lua_lscripts.unity3d");
                AddBundle("lua/lua_socket.unity3d");
                AddBundle("lua/lua_protobuf.unity3d");
            }
        }

        public object[] CallFunction(string funcName, params object[] args)
        {
            LuaFunction func = LuaClient.GetMainState().GetFunction(funcName);
            if (func != null)
            {
                return func.Invoke<object[], object[]>(args);
            }
            return null;
        }

        public void AddLuaASearchPath(string fullPath)
        {
            Debug.Log("* LuaManager添加搜索路径:" + fullPath);
            LuaClient.GetMainState().AddSearchPath(fullPath);
        }

        public void RemoveLuaSearchPath(string fullPath)
        {
            Debug.Log("* LuaManager移除搜索路径:" + fullPath);
            LuaClient.GetMainState().RemoveSeachPath(fullPath);
        }

    }
}

