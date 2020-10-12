using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Framework.UI.Utility
{

    [ExecuteInEditMode]
    public class RectTransformScaler : MonoBehaviour
    {

        [Serializable]
        public enum RectTransformScaleMode {
            MatchWidth, //匹配宽度
            MatchHeight,//匹配高度
            Expand, //扩展
            Shrink //收缩
        }

        public bool AutoRefresh = true;

        public RectTransform Tartget;
        public Vector2 DesignSize = new Vector2(100, 100);
        public RectTransformScaleMode ScaleMode = RectTransformScaleMode.Expand;

        private RectTransform _rt;
        private bool m_isStarted;

        private void OnEnable()
        {
            _rt = GetComponent<RectTransform>();
            if(m_isStarted) Refresh();
        }

        private void OnDestroy()
        {
            Tartget = null;
            _rt = null;
        }

        private void Start()
        {
            Refresh();
            m_isStarted = true;
        }

        private Vector2 _targetWHCache;
        private float _aspectCache;
        private void Update()
        {
            if (AutoRefresh) Refresh();
        }

        public void Refresh()
        {
            if (!Tartget) return;

            Vector2 wh = new Vector2(Tartget.rect.width, Tartget.rect.height);
            float aspect = DesignSize.x / DesignSize.y;

            if (!_targetWHCache.Equals(wh) || !_aspectCache.Equals(aspect))
            {
                _targetWHCache = wh;
                _aspectCache = aspect;

                float aspect_t = _targetWHCache.x / _targetWHCache.y;

                switch (ScaleMode)
                {
                    case RectTransformScaleMode.MatchWidth:
                        _rt.sizeDelta = new Vector2(_targetWHCache.x, _targetWHCache.x / _aspectCache);
                        break;
                    case RectTransformScaleMode.MatchHeight:
                        _rt.sizeDelta = new Vector2(_targetWHCache.y * _aspectCache, _targetWHCache.y);
                        break;
                    case RectTransformScaleMode.Shrink:
                        if(aspect_t > aspect)
                            _rt.sizeDelta = new Vector2(_targetWHCache.y * _aspectCache, _targetWHCache.y);
                        else
                            _rt.sizeDelta = new Vector2(_targetWHCache.x, _targetWHCache.x / _aspectCache);
                        break;
                    case RectTransformScaleMode.Expand:
                        if (aspect_t > aspect)
                            _rt.sizeDelta = new Vector2(_targetWHCache.x, _targetWHCache.x / _aspectCache);
                        else
                            _rt.sizeDelta = new Vector2(_targetWHCache.y * _aspectCache, _targetWHCache.y);
                        break;
                }

            }
        }

    }

}
