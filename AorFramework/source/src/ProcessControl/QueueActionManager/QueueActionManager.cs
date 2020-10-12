using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{

    /// <summary>
    /// 队列行为管理器
    /// 
    /// 提示: 原则上添加一个队列行为必须要保证在行为代码内有地方会调用到Next方法用以标识该行为完成,否则可能造成队列逻辑错误.
    /// 
    /// </summary>
   // [AddComponentMenu("")] //在菜单中隐藏此组件
    public class QueueActionManager : ManagerBase
    {

        public static int QueueLimit = 128; //最大队列数,超过此队列数则会触发队列溢出错误机制

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

        private static QueueActionManager _instance;
        public static QueueActionManager Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// 创建带有独立GameObject的Instance
        /// </summary>
        public static QueueActionManager CreateInstance(Transform parenTransform = null)
        {
            return ManagerBase.CreateInstance<QueueActionManager>(ref _instance, parenTransform);
        }

        /// <summary>
        /// 在目标GameObject上的创建Instance
        /// </summary>
        public static QueueActionManager CreateInstanceOnGameObject(GameObject target)
        {
            return ManagerBase.CreateInstanceOnGameObject<QueueActionManager>(ref _instance, target);
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
            Debug.Log("~QueueActionManager was destroyed");
            ManagerBase.VerifyUniqueOnDispose(ref _instance, this);
        }

        //=======================================================================

        //-----------------------------------------------------------------

        /// <summary>
        /// 当前队列行为为空或者不能执行时触发
        /// </summary>
        public Action<QueueAction> OnNullAction;

        /// <summary>
        /// 队列超过最大上限时触发
        /// </summary>
        public Action OnQueueOverLimit;

        //-----------------------------------------------------------------

        private readonly List<QueueAction> m_queueActions = new List<QueueAction>();

        private bool m_sortDirty = false;
        private bool m_locked = false;
        public void AddAction(string name, Action action)
        {
            AddAction(new QueueAction(name, action));
        }
        public void AddAction(string name, Action action, float timeout)
        {
            AddAction(new QueueAction(name, action, timeout, null));
        }
        public void AddAction(string name, Action action, float timeout, Action timeoutCallback)
        {
            AddAction(new QueueAction(name, action, timeout, timeoutCallback));
        }
        public void AddAction(QueueAction qaction)
        {
            if (m_locked)
            {

                if (m_queueActions.Count > QueueLimit)
                {
                    Debug.LogWarning("** QueueActionManager.QueueOverLimit !! :: " + m_queueActions.Count + " | " + QueueLimit);
                    if (OnQueueOverLimit != null) OnQueueOverLimit();
                }

                lock (m_queueActions)
                {
                    m_queueActions.Add(qaction);
                    if (!m_sortDirty)
                    {
                        m_sortDirty = true;
                        StartCoroutine(SortProcess());
                    }
                }
                //}
            }
            else
            {
                m_locked = true;
                _doCurrentAction(qaction);
            }
        }

        private IEnumerator SortProcess()
        {
            yield return new WaitForEndOfFrame();
            lock (m_queueActions)
            {
                m_queueActions.Sort((a, b) => {
                    if (a.priority < b.priority)
                        return -1;
                    else
                        return 1;
                });
                m_sortDirty = false;
            }
        }

        /// <summary>
        /// 移除指定行为
        /// </summary>
        /// <param name="qaction"></param>
        public void RemoveAction(QueueAction qaction)
        {
            lock (m_queueActions) {
                if(m_queueActions.Contains(qaction))
                    m_queueActions.Remove(qaction);
            }
        }
        
        /// <summary>
        /// 移除所有指定名称的队列行为
        /// </summary>
        /// <param name="name"></param>
        public void RemoveAction(string name)
        {
            lock (m_queueActions)
            {
                List<QueueAction> findeds = m_queueActions.FindAll(qa => qa.name == name);
                if(findeds != null && findeds.Count > 0)
                {
                    for (int i = 0; i < findeds.Count; i++)
                    {
                        m_queueActions.Remove(findeds[i]);
                    }
                }
            }
        }

        public List<string> GetQueueNames()
        {
            List<string> names = new List<string>();
            if (_currentAction != null)
            {
                names.Add(_currentAction.name);
            }
            foreach (QueueAction item in m_queueActions)
            {
                names.Add(item.name);
            }
            return names;
        }

        /// <summary>
        /// 执行下一条队列行为
        /// </summary>
        public void Next()
        {
            if (m_queueActions.Count > 0)
            {
                lock (m_queueActions)
                {
                    //_doCurrentAction(_queueActions.Dequeue());
                    QueueAction cur = m_queueActions[0];
                    m_queueActions.RemoveAt(0);
                    _doCurrentAction(cur);
                }
            }
            else
                _resetQueueActions();
        }

        private void _resetQueueActions()
        {
            m_queueActions.Clear();
            _currentAction = null;
            m_locked = false;
        }

        private Coroutine _currentTimeOut;
        private QueueAction _currentAction;
        private void _doCurrentAction(QueueAction queueAction)
        {
            //清理工作
            if(_currentTimeOut != null)
            {
                Coroutine stop = _currentTimeOut;
                StopCoroutine(stop);
                _currentTimeOut = null;
            }

            //容错
            if(queueAction == null || queueAction.action == null)
            {
                //事件:当前队列行为不可执行
                if (OnNullAction != null) OnNullAction(queueAction);
                _currentAction = null;
                //自动执行下一条
                Next();
                return;
            }

            _currentAction = queueAction;
            _currentAction.action();

            if ( _currentAction.timeout > 0)
                _currentTimeOut = StartCoroutine(CheckCurrentTimeout(_currentAction.timeout));

        }

        private IEnumerator CheckCurrentTimeout(float sec)
        {
            yield return new WaitForSecondsRealtime(sec);

            if (_currentAction.timeoutCallback != null)
                _currentAction.timeoutCallback();
            else
                Next();//预防队列断裂,自动转入下一个队列行为 

        }

    }

    //--------

    public class QueueAction
    {
        /// <summary>
        /// 优先级
        /// </summary>
        public int priority = 0;

        public Action action;

        public Action timeoutCallback;
        private float m_timeout = 0;

        public float timeout { get { return m_timeout; } }
        
        private string m_name;
        public string name { get { return m_name; } }
        
        public QueueAction(string name, Action action)
        {
            this.m_name = name;
            this.action = action;
        }

        public QueueAction(string name, Action action, float timeout, Action timeoutCallback)
        {
            this.m_name = name;
            this.action = action;
            this.m_timeout = timeout;
            this.timeoutCallback = timeoutCallback;
        }

        ~QueueAction()
        {
            this.m_name = null;
            this.action = null;
            this.timeoutCallback = null; 
        }

    }


}