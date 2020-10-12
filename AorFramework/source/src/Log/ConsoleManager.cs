using System;
using UnityEngine;

namespace Framework.Console
{

    public class ConsoleLogHandler : ILogHandler
    {

        #region Events
        //当Log发生改变时触发，返回新的Log内容
        public Action<LogType, string> OnLogChanged;
        #endregion

        private ILogHandler _backup;
        public ConsoleLogHandler(ILogHandler baseHandler)
        {
            _backup = baseHandler;
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            _dispatchEvent(LogType.Exception, "*** Exception > " + exception.Message + " \n" + exception.ToString());
            _backup.LogException(exception, context);
        }

        //private string _hr;
        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {

            //switch (logType)
            //{
            //    case LogType.Assert:
            //        _hr = "<color=\"glay\">A > {0}</color>";
            //        break;
            //    case LogType.Log:
            //        _hr = "<color=\"white\">L > {0}</color>";
            //        break;
            //    case LogType.Warning:
            //        _hr = "<color=\"yellow\">W > {0}</color>";
            //        break;
            //    case LogType.Error:
            //        _hr = "<color=\"red\">E > {0}</color>";
            //        break;
            //    case LogType.Exception:
            //        _hr = "<color=\"red\">** E > {0}</color>";
            //        break;
            //}

#if UNITY_2018
            if (Debug.unityLogger.IsLogTypeAllowed(logType))
#elif UNITY_5
            if (Debug.logger.IsLogTypeAllowed(logType))
#endif
            {
                //_dispatchEvent(string.Format(_hr, string.Format(format, args)));
                _dispatchEvent(logType, string.Format(format, args));
            }
            _backup.LogFormat(logType, context, format, args);
        }

        private void _dispatchEvent(LogType logType,string message) {
            if (OnLogChanged != null) OnLogChanged(logType,message);
        }

        //销毁并返回原始的ILogHandler
        public ILogHandler Dispose()
        {
            ILogHandler r = _backup;
            _backup = null;
            OnLogChanged = null;
            return r;
        }

    }

    public class ConsoleManager : ManagerBase
    {

        //@@@ 静态方法实现区

        private static ConsoleManager _instance;
        public static ConsoleManager Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// 创建带有独立GameObject的Instance
        /// </summary>
        public static ConsoleManager CreateInstance(Transform parenTransform = null)
        {
            return ManagerBase.CreateInstance<ConsoleManager>(ref _instance, parenTransform);
        }

        /// <summary>
        /// 在目标GameObject上的创建Instance
        /// </summary>
        public static ConsoleManager CreateInstanceOnGameObject(GameObject target)
        {
            return ManagerBase.CreateInstanceOnGameObject<ConsoleManager>(ref _instance, target);
        }

        public static void Request(Action ConsoleManagerIniteDoSh)
        {
            CreateInstance();
            ManagerBase.Request(ref _instance, ConsoleManagerIniteDoSh);
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

        public LogType FilterLogType {
#if UNITY_2018
            get { return Debug.unityLogger.filterLogType; }
            set { Debug.unityLogger.filterLogType = value; }
#elif UNITY_5
            get { return Debug.logger.filterLogType; }
            set { Debug.logger.filterLogType = value; }
#endif

        }

        private void OnEnable()
        {
            if (_isInit) init();
        }

        private ConsoleLogHandler m_handler;

        protected override void init()
        {
#if UNITY_2018
            m_handler = new Framework.Console.ConsoleLogHandler(Debug.unityLogger.logHandler);
            m_handler.OnLogChanged += m_onLogChengd;
            Debug.unityLogger.logHandler = m_handler;
#elif UNITY_5
            m_handler = new Framework.Console.ConsoleLogHandler(Debug.logger.logHandler);
            m_handler.OnLogChanged += m_onLogChengd;
            Debug.logger.logHandler = m_handler;
#endif

        }

        private void OnDisable()
        {
            if (m_handler != null)
            {
#if UNITY_2018
                Debug.unityLogger.logHandler = m_handler.Dispose();
#elif UNITY_5
                Debug.logger.logHandler = m_handler.Dispose();
#endif

                m_handler = null;
            }
        }

#region Events and handler
        public Action<LogType,string> OnLogChanged;
        private void m_onLogChengd(LogType logType, string message)
        {
            if (OnLogChanged != null) OnLogChanged(logType, message);
        }
#endregion
    }
}
