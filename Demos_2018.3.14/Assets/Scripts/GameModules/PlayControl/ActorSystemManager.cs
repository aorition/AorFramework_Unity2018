using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Framework.PlayControl
{

    /// <summary>
    /// 前段管理在场景内所有角色的管理器
    /// </summary>
    public class ActorSystemManager : ManagerBase
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

        private static ActorSystemManager _instance;
        public static ActorSystemManager Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// 创建带有独立GameObject的Instance
        /// </summary>
        public static ActorSystemManager CreateInstance(Transform parenTransform = null)
        {
            ManagerBase.CreateInstance<ActorSystemManager>(ref _instance, parenTransform);
            return _instance;
        }

        /// <summary>
        /// 在目标GameObject上的创建Instance
        /// </summary>
        public static ActorSystemManager CreateInstanceOnGameObject(GameObject target)
        {
            //自动初始化
            ManagerBase.CreateInstanceOnGameObject<ActorSystemManager>(ref _instance, target);
            return _instance;
        }

        public static void Request(Action GraphicsManagerIniteDoSh)
        {
            CreateInstance();
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ManagerBase.VerifyUniqueOnDispose(ref _instance, this);
        }

        //=======================================================================

        private readonly Dictionary<int, ActorDataBase> m_ActorPool = new Dictionary<int, ActorDataBase>();

        public void AddActor(GameObject tar, ActorDataBase ActorData)
        {

        }

    }
}
