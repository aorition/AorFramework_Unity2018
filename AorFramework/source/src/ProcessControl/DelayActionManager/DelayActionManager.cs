using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Framework
{
    /// <summary>
    /// 延迟执行管理器
    /// </summary>
    [AddComponentMenu("")] //在菜单中隐藏此组件
    public class DelayActionManager : ManagerBase
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

        private static DelayActionManager _instance;
        public static DelayActionManager Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// 创建带有独立GameObject的Instance
        /// </summary>
        public static DelayActionManager CreateInstance(Transform parenTransform = null)
        {
            return ManagerBase.CreateInstance<DelayActionManager>(ref _instance, parenTransform);  
        }

        /// <summary>
        /// 在目标GameObject上的创建Instance
        /// </summary>
        public static DelayActionManager CreateInstanceOnGameObject(GameObject target)
        {
            return ManagerBase.CreateInstanceOnGameObject<DelayActionManager>(ref _instance, target);
        }

        public static void Request(Action GraphicsManagerIniteDoSh)
        {
            //自启动
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
            Debug.Log("~GameManager was destroyed");
            ManagerBase.VerifyUniqueOnDispose(ref _instance, this);
        }

        //=======================================================================

        public void AddDelayAction(DelayActionBase action)
        {
            if (action == null) return;
            _delayActions.Add(action);
            action.Init();
        }

        public void RemoveDelayActionByHash(int hash)
        {
            DelayActionBase ac = _delayActions.Find(d => d.hash == hash);
            if (ac != null) _delayActions.Remove(ac);
        }

        public void AddLoopAction(DelayActionBase action)
        {
            if (action == null) return;
            _loopActions.Add(action);
            action.Init();
        }

        public void RemoveLoopActionByHash(int hash)
        {
            DelayActionBase ac = _loopActions.Find(d => d.hash == hash);
            if (ac != null)
            {
                _loopActions.Remove(ac);
                ac.Dispose();
            }
        }

        private readonly List<DelayActionBase> _delayActions = new List<DelayActionBase>();
        private readonly List<DelayActionBase> _delTmp = new List<DelayActionBase>();
        private readonly List<DelayActionBase> _loopActions = new List<DelayActionBase>();

        private void Update()
        {
            if (_delayActions.Count > 0)
            {
                for (int i = 0; i < _delayActions.Count; i++)
                {
                    if (_delayActions[i].dead)
                        _delTmp.Add(_delayActions[i]);
                    else
                        _delayActions[i].Update();

                }
                if (_delTmp.Count > 0)
                {
                    for (int j = 0; j < _delTmp.Count; j++)
                    {
                        _delayActions.Remove(_delTmp[j]);
                    }
                    _delTmp.Clear();
                }

            }

            if (_loopActions.Count > 0)
            {
                for (int i = 0; i < _loopActions.Count; i++)
                {
                    _loopActions[i].Update();
                }
            }
        }

    }
}
