using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Extends;

namespace Framework.Console
{
    /// <summary>
    /// Console GUI View 
    /// 配合ConsoleManager将Log信息显示在GUI上的工具组件
    /// </summary>
    public class ConsoleGUIView : MonoBehaviour
    {

        private static GUIStyle m_logStyle;
        protected static GUIStyle logStyle {
            get {
                if(m_logStyle == null)
                {
                    m_logStyle = GUI.skin.GetStyle("Label").Clone();
                    m_logStyle.wordWrap = true;
                   // m_logStyle.richText = true;
                }
                return m_logStyle;
            }
        }

       // [Space(5)]

        public bool DrawGUI = true;

       // [Space(12)]

        public bool DrawBackground = true;

        public bool DrawControlButtons = true;

        //[Space(12)]

        public bool DisplayLogID = true;

        public bool AutoSrollLog = true;

        //[Space(12)]

        public bool UseConsoleManager = true;

        //[Space(12)]

#if UNITY_EDITOR
        [SerializeField]
        private LogType m_FilterLogType = LogType.Log;
#endif
        public LogType FilterLogType
        {

#if UNITY_5
            get { return Debug.logger.filterLogType; }
            set { Debug.logger.filterLogType = value; }
#elif UNITY_2018
            get { return Debug.unityLogger.filterLogType; }
            set { Debug.unityLogger.filterLogType = value; }
#endif
        }

        public int LogNumsLimit = 120;
        public int LogBufferNum = 30;

        [Space(12)]

        public Vector2 ViewOffest = Vector2.zero;
        public Vector2 ViewScale = Vector2.one;
        public float ViewPadding = 10f;

        [Space(5)]

        private bool m_isInit;
        private void OnEnable()
        {
            //if (UseConsoleManager)
            //{
            //    if (!ConsoleManager.IsInit())
            //    {
            //        ConsoleManager.CreateInstanceOnGameObject(gameObject);
            //    }
            //    ConsoleManager.Request(() =>
            //    {
            //        ConsoleManager.Instance.OnLogChanged += m_onNewMessage;
            //        m_isInit = true;
            //    });
            //}
            //else
            //{
            Application.logMessageReceived += m_onNewMessage2;
            m_isInit = true;
            //}
        }

        private void OnDisable()
        {
            //if (UseConsoleManager)
            //{
            //    if (ConsoleManager.IsInit())
            //    {
            //        ConsoleManager.Instance.OnLogChanged -= m_onNewMessage;
            //    }
            //}
            //else
            //{
            Application.logMessageReceived -= m_onNewMessage2;
            //}
            m_isInit = false;
        }

        private struct LogStruct {
            public LogStruct(int id, LogType type, string message, string stackTrace) {
                this.id = id;
                this.type = type;
                this.message = message;
                this.stackTrace = stackTrace;
                string[] rows = message.Split('\n');
                if(rows == null || rows.Length == 0)
                    this.rowsNum = 1;
                else
                    this.rowsNum = rows.Length;
            }
            public int id;
            public LogType type;
            public string message;
            public string stackTrace;
            public int rowsNum;
        }

        private int m_logID = 0;
        private readonly List<LogStruct> m_logs = new List<LogStruct>();
        private void m_onNewMessage(LogType type, string message)
        {
            m_logID ++;
            m_logs.Add(new LogStruct(m_logID, type, message, string.Empty));
            if (m_logs.Count > LogNumsLimit)
            {
                m_logs.RemoveRange(0, LogBufferNum);
            }
            _sviewPosDirty = true;
        }

        private void m_onNewMessage2(string condition, string stackTrace, LogType type) {
            m_logID++;
            m_logs.Add(new LogStruct(m_logID, type, condition, stackTrace));
            if (m_logs.Count > LogNumsLimit)
            {
                m_logs.RemoveRange(0, LogBufferNum);
            }
            _sviewPosDirty = true;
        }

        public void ClearLogData() {
            m_logs.Clear();
            m_logID = 0;
        }

        private const float AreaPadding = 10f;
        private const float ButtonHeight = 32f;
        private const float ControlAreaWidth = 136f;
        private const float LogLabelInnerSpacing = 1f;
        private const float InnerSpacing = 5f;

        private float IdLabelWidth(string id) {
            return id.Length * 6 + 20;
        }

        private float LogLabelHeight {
            get {
                return logStyle.lineHeight + 6;
            }
        }

        private Color getLabelColor(LogType type)
        {
            switch (type)
            {
                case LogType.Assert:
                case LogType.Log:
                    return Color.white;
                case LogType.Warning:
                    return Color.yellow;
                case LogType.Exception:
                case LogType.Error:
                    return Color.red;
                default: return Color.white;
            }
        }

        private float calculateSrollviewRectHeight(float ckHeight) {

            float h = AreaPadding;
            for (int i = 0; i < m_logs.Count; i++)
            {
                if(i > 0)
                {
                    h += LogLabelInnerSpacing;
                }
                h += m_logs[i].rowsNum * LogLabelHeight;
            }

            return h > ckHeight ? h : ckHeight;
        }
        
        private Rect _srollviewRect;
        private Rect _srollRect;
        private Vector2 _sviewPos;

        private Rect _controllAreaRect;

        private bool _sviewPosDirty = false;

#if UNITY_EDITOR
        private void Update()
        {
            //同步Log

#if UNITY_5
            if (Debug.logger.filterLogType != m_FilterLogType)
                Debug.logger.filterLogType = m_FilterLogType;
#elif UNITY_2018
            if (Debug.unityLogger.filterLogType != m_FilterLogType)
                Debug.unityLogger.filterLogType = m_FilterLogType;
#endif

        }
#endif

            private void OnGUI()
        {
            if (!m_isInit || !DrawGUI) return;
            
            //获取整个View的Rect
            Rect screenRect = new Rect(
                ViewOffest.x + ViewPadding * 0.5f,
                ViewOffest.y + ViewPadding * 0.5f,
                Screen.width * ViewScale.x - ViewPadding,
                Screen.height * ViewScale.y - ViewPadding
                );

            if (DrawControlButtons)
            {
                screenRect = new Rect(screenRect.x, screenRect.y,
                    screenRect.width - ControlAreaWidth - InnerSpacing,
                    screenRect.height
                    );
            }

            if (DrawBackground)
            {
                GUI.Box(screenRect, "");

                if (DrawControlButtons)
                {
                    _controllAreaRect = new Rect(
                        screenRect.max.x + InnerSpacing,
                        screenRect.y,
                        ControlAreaWidth,
                        screenRect.height
                        );
                    GUI.Box(_controllAreaRect, "");
                }
            }

            _srollRect = new Rect(
                screenRect.x + AreaPadding * 0.5f, 
                screenRect.y + AreaPadding * 0.5f, 
                screenRect.width - AreaPadding, 
                screenRect.height - AreaPadding
                );

            //计算srollviewRect
            //Rect viewRect = new Rect(0, 0, Screen.width, m_logs.Count * 18);
            _srollviewRect = new Rect(0, 0, _srollRect.width - 35, calculateSrollviewRectHeight(_srollRect.height));

            //float hs = SrollViewPadding * 0.5f;
            float hs = 0;
            _sviewPos = GUI.BeginScrollView(_srollRect, _sviewPos, _srollviewRect);
            {
                for (int i = 0; i < m_logs.Count; i++)
                {
                    if(i > 0)
                    {
                        hs += LogLabelInnerSpacing;
                    }
                    LogStruct e = m_logs[i];
                    float h = e.rowsNum * LogLabelHeight;

                    float x = AreaPadding * 0.5f;
                    float w = _srollviewRect.width;
                    if (DisplayLogID)
                    {
                        GUI.color = Color.gray;
                        string id = e.id.ToString();
                        w = IdLabelWidth(id);
                        GUI.Label(new Rect(x, hs, w, h), id);

                        x += w;
                        w = _srollviewRect.width - w;
                    }

                    GUI.color = getLabelColor(e.type);
                    GUI.Label( new Rect( x, hs, w, h ), e.message, logStyle );
                    hs += h;
                }
                GUI.color = Color.white; //恢复GUI颜色
            }
            GUI.EndScrollView();

            if (AutoSrollLog)
            {
                if (_sviewPosDirty)
                {
                    if (_srollviewRect.height > _srollRect.height)
                    {
                        _sviewPos = new Vector2(_sviewPos.x, _srollviewRect.height - _srollRect.height);
                    }
                    _sviewPosDirty = false;
                }
            }

            if (DrawControlButtons)
            {
                float x = _controllAreaRect.x + AreaPadding * 0.5f;
                float y = _controllAreaRect.y + AreaPadding * 0.5f; //绘制点Y数据
                float w = _controllAreaRect.width - AreaPadding;
                if (GUI.Button(new Rect(
                          x,
                          y,
                          w,
                          ButtonHeight
                    ),"Close GUI LOG"))
                {
                    DrawGUI = false;
                }

                y += ButtonHeight + InnerSpacing;
                AutoSrollLog = GUI.Toggle(new Rect(x, y, w, ButtonHeight), AutoSrollLog, "Auto Scroll Log", "button");

                y += ButtonHeight + InnerSpacing;
                DisplayLogID = GUI.Toggle(new Rect(x, y, w, ButtonHeight), DisplayLogID, "Display Log ID", "button");

                y += ButtonHeight + InnerSpacing;
                if (GUI.Button(new Rect(x, y, w, ButtonHeight), "Clear LOG Data"))
                {
                    ClearLogData();
                }


            }

        }

    }
}


