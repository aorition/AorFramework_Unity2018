﻿#pragma warning disable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 基于MonoBehaviour的Manager的基类
    /// </summary>
    public abstract class ManagerBase : MonoBehaviour
    {

        //--------------

        private static readonly Dictionary<Type, ManagerBase> _regDic = new Dictionary<Type, ManagerBase>();

        private static void _regManager(Type type, ManagerBase ins)
        {
            if (!_regDic.ContainsKey(type))
                _regDic.Add(type, ins);
        }

        private static void _unregManager(Type type)
        {
            if (_regDic.ContainsKey(type))
                _regDic.Remove(type);
        }

        //--------------

        private static readonly Dictionary<Type, Action> _AfterInitDo = new Dictionary<Type, Action>();

        private static T FindOrCreateManager<T>(string GameObjectName, Transform parenTransform = null)
            where T : ManagerBase
        {
            GameObject go = null;
            if (parenTransform)
            {
                Transform t = parenTransform.Find(GameObjectName);
                if (t) go = t.gameObject;
            }

            if (!go) go = GameObject.Find(GameObjectName);

            if (go)
            {

                if (parenTransform) go.transform.SetParent(parenTransform, false);
                T gm = go.GetComponent<T>();
                if (gm)
                {
                    return gm;
                }
                else
                {
                    return go.AddComponent<T>();
                }
            }
            else
            {
                go = new GameObject(GameObjectName);
                if (parenTransform) go.transform.SetParent(parenTransform, false);
                //                if (Application.isPlaying && _dontDestroyOnLoad && !_parentTransform) GameObject.DontDestroyOnLoad(go);
                return go.AddComponent<T>();
            }
        }

        /// <summary>
        /// 创建带有独立GameObject的Instance
        /// </summary>
        protected static T CreateInstance<T>(ref T uniqueInstance, Transform parenTransform = null)
            where T : ManagerBase
        {

            string GameObjectName = typeof(T).Name;

            if (uniqueInstance == null)
            {

                uniqueInstance = FindOrCreateManager<T>(GameObjectName, parenTransform);
            }
            else if (parenTransform)
            {
                uniqueInstance.transform.SetParent(parenTransform, false);
            }

            return uniqueInstance as T;
        }

        /// <summary>
        /// 在目标GameObject上的创建Instance
        /// </summary>
        protected static T CreateInstanceOnGameObject<T>(ref T uniqueInstance, GameObject target)
            where T : ManagerBase
        {
            if (!target) return null;

            string GameObjectName = typeof(T).Name;

            if (uniqueInstance == null)
            {
                T instance = target.GetComponent<T>();
                if(!instance) instance = target.AddComponent<T>();
                uniqueInstance = instance;
            }

            return uniqueInstance as T;
        }

        protected static void Request<T>(ref T uniqueInstance, Action GraphicsManagerIniteDoSh)
            where T : ManagerBase
        {
            if (VerifyIsInit<T>(ref uniqueInstance))
            {
                GraphicsManagerIniteDoSh();
            }
            else
            {
                Type t = typeof(T);
                if (!_AfterInitDo.ContainsKey(t))
                {
                    _AfterInitDo.Add(t, GraphicsManagerIniteDoSh);
                }
                else
                {
                    _AfterInitDo[t] += GraphicsManagerIniteDoSh;
                }

            }
        }

        protected static bool VerifyIsInit<T>(ref T uniqueInstance)
            where T : ManagerBase
        {
            return uniqueInstance && uniqueInstance._isInit;
        }

        protected static void VerifyUniqueOnAwake<T>(ref T uniqueInstance, T instance)
            where T : ManagerBase
        {
            if (uniqueInstance != null && uniqueInstance != instance)
            {
                GameObject.Destroy(instance);
            }
            else if (uniqueInstance == null)
            {
                uniqueInstance = instance;
                _regManager(uniqueInstance.GetType(), uniqueInstance);
            }
        }

        protected static void VerifyUniqueOnDispose<T>(ref T uniqueInstance, T instance)
            where T : ManagerBase
        {
            if (uniqueInstance != null && uniqueInstance == instance)
            {
                Type t = uniqueInstance.GetType();
                if (_AfterInitDo.ContainsKey(t))
                    _AfterInitDo.Remove(t);
                uniqueInstance = null;
                _unregManager(t);
            }
        }

        //=====================================================

        /// <summary>
        /// 标识Manager是否完成参数装载
        /// </summary>
        protected bool _isSetuped = false;
        /// <summary>
        /// 标识Mananger是否初始化
        /// </summary>
        protected bool _isInit = false;

        /// <summary>
        /// Awake阶段: 推荐Manager调用ManagerBase.VerifyUniqueOnAwake完成单例唯一验证
        /// </summary>
        protected virtual void Awake()
        {
            //Debug.Log("... " + this.GetType().Name);

            //Do -> ManagerBase.VerifyUniqueOnAwake
        }

        /// <summary>
        /// Start方法固定判断Mananger是否已经Setup,如果是则自定调用Init经行初始化,否则调用OnUnSetupedStart方法
        /// </summary>
        protected virtual void Start()
        {
            if (!_isSetuped && !_isInit)
            {
                OnUnSetupedStart();
            }
            else if (_isSetuped && !_isInit)
            {
                __init();
            }

            if (_isInit)
            {
                __afterInitDo();
            }
            else
            {
                StartCoroutine(_afterInitRun());
            }

        }

        private void __afterInitDo()
        {
            if (_AfterInitDo != null)
            {
                Type t = this.GetType();
                if (_AfterInitDo.ContainsKey(t))
                {
                    Action tmpDo = _AfterInitDo[t];
                    tmpDo();
                    _AfterInitDo.Remove(t);
                }
            }
            OnAfterInit();
        }

        IEnumerator _afterInitRun()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                if (_isInit)
                {
                    __afterInitDo();
                    break;
                }
            }
        }

        /// <summary>
        /// 如果在Start阶段Manager还没有Setup会调用该方法
        /// 
        /// 提示: 使用该方法后,需要设置_isInit为true表示以通过该方法初始化,
        /// 
        /// </summary>
        protected virtual void OnUnSetupedStart()
        {
            //默认行为在Start阶段跳过了Setup行为,直接进入init行为
            setupComplete();
        }

        /// <summary>
        /// 初始化(壳)
        /// </summary>
        protected void __init()
        {
            if (_isInit) return;
            init();
            _isInit = true;
        }

        /// <summary>
        /// 初始化(实现)
        /// </summary>
        protected virtual void init()
        {
            //do sh
        }

        /// <summary>
        /// OnDestroy阶段: 推荐Manager调用ManagerBase.VerifyUniqueOnDispose完成单例唯一卸载判断
        /// </summary>
        protected virtual void OnDestroy()
        {
            //Do -> ManagerBase.VerifyUniqueOnDispose
        }

        /// <summary>
        /// 标识Setup过程完成
        /// </summary>
        protected void setupComplete()
        {
            _isSetuped = true;
            __init();
        }

        /// <summary>
        /// 子类如需传入数据，请实现Setup方法并在结尾调用setupComplete方法
        /// </summary>
        // public void Setup()
        // {
        //     setupComplete();
        // }

        /// <summary>
        /// 初始化完成之后调用
        /// </summary>
        protected virtual void OnAfterInit()
        {
            //do sh
        }

    }
}
