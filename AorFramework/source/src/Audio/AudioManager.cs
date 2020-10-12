using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Audio
{

    /// <summary>
    /// 基础多通道AudioManger
    /// </summary>
    [AddComponentMenu("")] //在菜单中隐藏此组件
    public class AudioManager : ManagerBase
    {

        private const string _ChannelRootNameDef = "__Channels";
        private const string _oneShotChannelRootNameDef = "__OneShotChannel";

        private static AudioManager _instance;
        public static AudioManager Instance
        {
            get { return _instance; }
        }

        public static bool HasInstance
        {
            get { return _instance != null; }
        }

        /// <summary>
        /// 创建带有独立GameObject的Instance
        /// </summary>
        public static AudioManager CreateInstance(Transform parenTransform = null)
        {
            return ManagerBase.CreateInstance<AudioManager>(ref _instance, parenTransform);
        }

        /// <summary>
        /// 在目标GameObject上的创建Instance
        /// </summary>
        public static AudioManager CreateInstanceOnGameObject(GameObject target)
        {
            return ManagerBase.CreateInstanceOnGameObject<AudioManager>(ref _instance, target);
        }

        public static void Request(Action GraphicsManagerIniteDoSh)
        {
            CreateInstance(); //自动创建Instance
            ManagerBase.Request(ref _instance, GraphicsManagerIniteDoSh);
        }

        public static bool IsInit()
        {
            return ManagerBase.VerifyIsInit(ref _instance);
        }

        //-----------------------------------------------------------------

        /// <summary>
        /// AudioClip延时缓存
        /// </summary>
        public class AudioClipKeeper
        {

            public AudioClipKeeper(string loadPath, AudioClip clip, float Survivalseconds)
            {
                this.LoadPath = loadPath;
                this.clip = clip;
                this.SurvivalSeconds = Survivalseconds;
            }

            ~AudioClipKeeper()
            {
                this.LoadPath = null;
                this.clip = null;
            }
            
            public string LoadPath;
            public AudioClip clip;
            public float SurvivalSeconds;

            private float _liveSeconds;

            public bool IsPlaying = false;

            private bool m_IsDead = false;
            public bool IsDead
            {
                get { return m_IsDead; }
            }

            public void Update(float deltaTime)
            {
                if (SurvivalSeconds == 0) return;

                if (IsPlaying)
                {
                    _liveSeconds = 0;
                    return;
                }

                _liveSeconds += deltaTime;
                if (_liveSeconds >= SurvivalSeconds)
                {
                    m_IsDead = true;
                }
            }

        }
        /// <summary>
        /// AudioSource延时停止器
        /// </summary>
        public class ACChannelStopTimer
        {

            public ACChannelStopTimer(AudioSource source, float duration)
            {
                this.Source = source;
                this.Duration = duration;
            }

            ~ACChannelStopTimer()
            {
                this.Source = null;
            }

            public AudioSource Source;

            private float _liveSeconds;
            public float Duration;

            private bool m_IsDead = false;
            public bool IsDead
            {
                get { return m_IsDead; }
            }

            public void Update(float deltaTime)
            {
                if (Source && Source.isActiveAndEnabled && Source.isPlaying)
                {
                    _liveSeconds += deltaTime;
                    if (_liveSeconds >= Duration)
                    {
                        Source.Stop();
                        Source.clip = null;
                        m_IsDead = true;
                    }
                }
                else
                    //只要停止播放，计时器则标记死亡。
                    m_IsDead = true;
            }

        }

        /// <summary>
        /// 音效(非循环声音)通道数限制
        /// </summary>
        public int ACChannelLimit = 16;
        /// <summary>
        /// 背景音乐(循环音效)通道限制
        /// </summary>
        public int BGMChannelLimit = 2;

        /// <summary>
        /// 缓存Clip最大条数;
        /// </summary>
        public int AudioClipCacheLimit = 36;

        /// <summary>
        /// 缓存Clip 未被使用时将在多少秒后被清出缓存池?
        /// </summary>
        public float AudioClipCacheSurvivalSeconds = 30f;

        /// <summary>
        /// 是否静音
        /// </summary>
        private bool m_mute = false;
        public bool Mute
        {
            get { return m_muteAC && m_muteBGM; }
            set
            {
                MuteAC = m_muteBGM = value;
            }
        }

        private bool m_muteAC = false;
        public bool MuteAC
        {
            get { return m_muteAC; }
            set
            {
                m_muteAC = value;
                _setMuteAC();
            }
        }

        private bool m_muteBGM = false;
        public bool MuteBGM
        {
            get { return m_muteBGM; }
            set
            {
                m_muteBGM = value;
                _setMuteBGM();
            }
        }

        private Transform _ChannelRoot;

        private AudioSource _oneShotChannel;
        private readonly List<AudioSource> _ACChannels = new List<AudioSource>();
        private readonly List<AudioSource> _BGMChannels = new List<AudioSource>();
        
        private readonly List<AudioClipKeeper> _clipKeeperList = new List<AudioClipKeeper>();
        private readonly List<ACChannelStopTimer> _loopStopTimerList = new List<ACChannelStopTimer>();
        //private Dictionary<string, AudioClipKeeper> _audioClipDic = new Dictionary<string, AudioClipKeeper>();

        protected override void Awake()
        {
            base.Awake();
            ManagerBase.VerifyUniqueOnAwake<AudioManager>(ref _instance, this);
        }

        protected override void init()
        {
            if (!_ChannelRoot)
            {
                _ChannelRoot = transform.Find(_ChannelRootNameDef);
                if (!_ChannelRoot)
                {
                    _ChannelRoot = new GameObject(_ChannelRootNameDef).transform;
                    _ChannelRoot.SetParent(transform, false);
                }
            }
            AudioBridge.PlayHook = Play;
            AudioBridge.PlayClipHook = PlayClip;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ManagerBase.VerifyUniqueOnDispose(ref _instance, this);
        }

        public void PlayOneShot(string path, float volumeScale = 1f)
        {
            AudioClipKeeper keeper = _clipKeeperList.Find(k => k.LoadPath == path);
            if (keeper != null)
            {
                if (keeper.clip != null)
                {
                    PlayClipOneShot(keeper.clip, volumeScale);
                    return;
                }
                else
                {
                    _clipKeeperList.Remove(keeper);
                }
            }

            if (_clipKeeperList.Count < AudioClipCacheLimit)
            {
                ResourcesLoadBridge.Load<AudioClip>(path, (clip, objs) =>
                {
                    if (clip)
                    {
                        keeper = new AudioClipKeeper(path, clip, AudioClipCacheSurvivalSeconds);
                        _clipKeeperList.Add(keeper);
                        PlayClipOneShot(keeper.clip, volumeScale);
                    }
                });
            }
            else
            {
                Debug.LogWarning("** AudioManager Warning :: 缓存Clip超限,无法加载新的AudioClip.(最大限制:" + AudioClipCacheLimit + ")");
            }

        }

        public void PlayClipOneShot(AudioClip clip, float volumeScale = 1f)
        {
            if (!_oneShotChannel) {

                Transform t = transform.Find(_oneShotChannelRootNameDef);
                if (!t)
                {
                    t = new GameObject(_oneShotChannelRootNameDef).transform;
                    t.SetParent(transform, false);
                }

                _oneShotChannel = t.gameObject.AddComponent<AudioSource>();
                _oneShotChannel.playOnAwake = false;
				_oneShotChannel.mute = m_muteAC;
            }
            if(!_oneShotChannel.mute)
                _oneShotChannel.PlayOneShot(clip, volumeScale);
        }

        ///
        // parms 扩充参数定义 :  [volume:float(音量), StereoPan:float(左右声道), pitch(播放速率) , SpatialBlend:float(2/3D融合), ReverbZoneMix:float(混响)]
        ///
        public void Play(string path, Action<AudioSource> callback = null, params object[] parms)
        {

            AudioClipKeeper keeper = _clipKeeperList.Find(k => k.LoadPath == path);
            if (keeper != null)
            {
                if (keeper.clip != null)
                {
                    PlayClip(keeper.clip, asc =>
                    {
                        if (asc) keeper.IsPlaying = true;
                        else keeper.IsPlaying = false;
                        if (callback != null) callback(asc);
                    }, parms);
                    return;
                }
                else
                {
                    _clipKeeperList.Remove(keeper);
                }
            }

            if (_clipKeeperList.Count < AudioClipCacheLimit)
            {
                ResourcesLoadBridge.Load<AudioClip>(path, (clip, objs) =>
                {
                    if (clip)
                    {
                        keeper = new AudioClipKeeper(path, clip, AudioClipCacheSurvivalSeconds);
                        _clipKeeperList.Add(keeper);
                        PlayClip(keeper.clip, asc => {
                            if (asc) keeper.IsPlaying = true;
                            else keeper.IsPlaying = false;
                            if (callback != null) callback(asc);
                        }, parms);
                        return;
                    }
                    if (callback != null) callback(null);
                });
            }
            else
            {
                Debug.LogWarning("** AudioManager Warning :: 缓存Clip超限,无法加载新的AudioClip.(最大限制:" + AudioClipCacheLimit + ")");
            }

        }

        ///
        // parms 扩充参数定义 :  [volume:float(音量), StereoPan:float(左右声道), pitch(播放速率) , SpatialBlend:float(2/3D融合), ReverbZoneMix:float(混响)]
        ///
        public void PlayClip(AudioClip clip, Action<AudioSource> callback = null, params object[] parms)
        {
            AudioSource audioSource = _getEmptyAudioSource();
            if (audioSource)
            {
                audioSource.clip = clip;
                audioSource.loop = false;
                _applyParmsForAudioSource(audioSource, parms);
                audioSource.Play();
            }
            if (callback != null) callback(audioSource);
        }

        public void PlayLoop(string path, float duration, Action<AudioSource> callback = null, params object[] parms)
        {

            AudioClipKeeper keeper = _clipKeeperList.Find(k => k.LoadPath == path);
            if (keeper != null)
            {
                if (keeper.clip != null)
                {
                    PlayClipLoop(keeper.clip, duration, asc =>
                    {
                        if (asc) keeper.IsPlaying = true;
                        else keeper.IsPlaying = false;
                        if (callback != null) callback(asc);
                    }, parms);
                    return;
                }
                else
                {
                    _clipKeeperList.Remove(keeper);
                }
            }

            if (_clipKeeperList.Count < AudioClipCacheLimit)
            {
                ResourcesLoadBridge.Load<AudioClip>(path, (clip, objs) =>
                {
                    if (clip)
                    {
                        keeper = new AudioClipKeeper(path, clip, AudioClipCacheSurvivalSeconds);
                        _clipKeeperList.Add(keeper);
                        PlayClipLoop(keeper.clip, duration, asc => {
                            if (asc) keeper.IsPlaying = true;
                            else keeper.IsPlaying = false;
                            if (callback != null) callback(asc);
                        }, parms);
                        return;
                    }
                    if (callback != null) callback(null);
                });
            }
            else
            {
                Debug.LogWarning("** AudioManager Warning :: 缓存Clip超限,无法加载新的AudioClip.(最大限制:" + AudioClipCacheLimit + ")");
            }

        }

        /// <summary>
        /// 循环播放
        /// </summary>
        public void PlayClipLoop(AudioClip clip, float duration, Action<AudioSource> callback = null, params object[] parms)
        {
            AudioSource audioSource = _getEmptyAudioSource();
            if (audioSource)
            {
                _loopStopTimerList.Add(new ACChannelStopTimer(audioSource, duration));
                audioSource.clip = clip;
                audioSource.loop = true;
                _applyParmsForAudioSource(audioSource, parms);
                audioSource.Play();
            }
            if (callback != null) callback(audioSource);
        }

        public void Stop(string path)
        {
            AudioSource audioSource = GetAudioSourceByPath(path);
            if (audioSource && audioSource.clip)
            {
                audioSource.Stop();
            }
        }

        public void StopClip(AudioClip clip)
        {
            for (int i = 0; i < _ACChannels.Count; i++)
            {
                if (_ACChannels[i] != null && _ACChannels[i].clip == clip && _ACChannels[i].loop == false)
                {
                    _ACChannels[i].Stop();
                    _ACChannels[i].clip = null;
                    return;
                }
            }
        }

        public void StopClipLoop(AudioClip clip)
        {
            for (int i = 0; i < _ACChannels.Count; i++)
            {
                if (_ACChannels[i] != null && _ACChannels[i].clip == clip && _ACChannels[i].loop == true)
                {
                    _ACChannels[i].Stop();
                    _ACChannels[i].clip = null;
                    ACChannelStopTimer stopTimer = _loopStopTimerList.Find(k => k.Source == _ACChannels[i]);
                    if (stopTimer != null) _loopStopTimerList.Remove(stopTimer);
                    return;
                }
            }
        }

        public void StopAllAC()
        {

            for (int i = 0; i < _ACChannels.Count; i++)
            {
                if (_ACChannels[i].clip && _ACChannels[i].isPlaying)
                {
                    _ACChannels[i].Stop();
                }
            }

        }

        ///
        // parms 扩充参数定义 :  [volume:float(音量), StereoPan:float(左右声道), pitch(播放速率) , SpatialBlend:float(2/3D融合), ReverbZoneMix:float(混响)]
        ///
        public void PlayBGM(string path, Action<AudioSource> callback = null, params object[] parms)
        {
            AudioClipKeeper keeper = _clipKeeperList.Find(k => k.LoadPath == path);
            if (keeper != null)
            {
				if (keeper.clip != null) {
					PlayBGMClip (keeper.clip, asc => {
						if (asc)
							keeper.IsPlaying = true;
						else
							keeper.IsPlaying = false;
						if (callback != null)
							callback (asc);
					}, parms);
					return;
				} else {
					_clipKeeperList.Remove (keeper);
				}
            }
            
			ResourcesLoadBridge.Load<AudioClip>(path, (clip, objs) =>
				{
					if (clip)
					{
						keeper = new AudioClipKeeper(path, clip, AudioClipCacheSurvivalSeconds);
						_clipKeeperList.Add(keeper);
						PlayBGMClip(keeper.clip, asc => {
							if (asc) keeper.IsPlaying = true;
							else keeper.IsPlaying = false;
							if (callback != null) callback(asc);
						}, parms);
						return;
					}
					if (callback != null) callback(null);
				});

        }

        public void PlayBGMClip(AudioClip clip, Action<AudioSource> callback = null, params object[] parms)
        {
            AudioSource audioSource = _getEmptyAudioSource(true);
            if (audioSource)
            {
                audioSource.clip = clip;
                audioSource.loop = true;
                _applyParmsForAudioSource(audioSource, parms);
                audioSource.Play();
            }
            if (callback != null) callback(audioSource);
        }

        public void StopBGM(string path)
        {
            AudioSource audioSource = GetAudioSourceByPath(path, true);
            if (audioSource && audioSource.clip)
            {
                audioSource.Stop();
            }
        }

        public void StopBGMClip(AudioClip clip)
        {
            for (int i = 0; i < _BGMChannels.Count; i++)
            {
                if(_BGMChannels[i] != null && _BGMChannels[i].clip == clip)
                {
                    _BGMChannels[i].Stop();
                }
            }
        }

        public void StopAllBGM()
        {
            for (int i = 0; i < _BGMChannels.Count; i++)
            {
                if (_BGMChannels[i] && _BGMChannels[i].clip && _BGMChannels[i].isPlaying)
                {
                    _BGMChannels[i].Stop();
                }
            }
        }

        public void StopAll()
        {
            StopAllAC();
            StopAllBGM();
        }

        public bool IsBGMPlaying(string path)
        {
            AudioSource audioSource = GetAudioSourceByPath(path);
            if (audioSource) return audioSource.isPlaying;
            return false;
        }
        
        public bool IsAcPlaying(string path)
        {
            AudioSource audioSource = GetAudioSourceByPath(path, true);
            if (audioSource) return audioSource.isPlaying;
            return false;
        }

        public AudioSource GetAudioSourceByPath(string path, bool isBGM = false)
        {
            AudioClipKeeper keeper = _clipKeeperList.Find(k => k.LoadPath == path);
            if (keeper != null && keeper.clip != null)
            {
                return isBGM ? _BGMChannels.Find(c => c.clip == keeper.clip) : _ACChannels.Find(c => c.clip == keeper.clip);
            }
            return null;
        }

        public AudioSource GetAudioSourceByClip(AudioClip clip, bool isBGM = false)
        {
            _tmpChannels = isBGM ? _BGMChannels : _ACChannels;
            _tmpChannelLimit = isBGM ? BGMChannelLimit : ACChannelLimit;
            for (int i = 0; i < _tmpChannels.Count; i++)
            {
                if (_tmpChannels[i].clip == clip)
                {
                    return _tmpChannels[i];
                }
            }
            return null;
        }

        public List<AudioSource> GetPlayingAudioSources(bool isBGM = false)
        {
            List<AudioSource> list = new List<AudioSource>();
            _tmpChannels = isBGM ? _BGMChannels : _ACChannels;
            _tmpChannelLimit = isBGM ? BGMChannelLimit : ACChannelLimit;
            for (int i = 0; i < _tmpChannels.Count; i++)
            {
                AudioSource as0 = _tmpChannels[i];
                if (as0.clip != null && as0.isPlaying)
                {
                    list.Add(as0);
                }
            }
            return list;
        }

        //-------------------------------------------------

        private void _setMuteAC()
        {
			//Debug.LogWarning ("-------------------acc count:"+ _ACChannels.Count);
            for (int i = 0; i < _ACChannels.Count; i++)
            {
                _ACChannels[i].mute = m_muteAC;
            }
            if (_oneShotChannel) _oneShotChannel.mute = m_muteAC;
        }

        private void _setMuteBGM()
        {
            for (int i = 0; i < _BGMChannels.Count; i++)
            {
                _BGMChannels[i].mute = m_muteBGM;
            }
        }

        private float m_lastRTime, m_delta;
        private int i, len;
        private readonly List<AudioClipKeeper> _dels = new List<AudioClipKeeper>();
        private readonly List<ACChannelStopTimer> _dels2 = new List<ACChannelStopTimer>();
        private void Update()
        {
            //更新delta
            m_delta = Time.realtimeSinceStartup - m_lastRTime;

            #region 处理循环音停止效计时器

            len = _loopStopTimerList.Count;
            for (i = 0; i < len; i++)
            {

                //防止外部删除
                if (_loopStopTimerList[i] == null)
                {
                    _loopStopTimerList.RemoveAt(i);
                    i--;
                    len--;
                    continue;
                }

                if (_loopStopTimerList[i].Source != null && _loopStopTimerList[i].Source.clip != null)
                {
                    _loopStopTimerList[i].Update(m_delta);
                    if (_loopStopTimerList[i].IsDead)
                    {
                        _dels2.Add(_loopStopTimerList[i]);
                    }
                }
                else
                {
                    _dels2.Add(_loopStopTimerList[i]);
                }
            }

            len = _dels2.Count;
            if (len > 0)
            {
                for (i = 0; i < len; i++)
                {
                    _loopStopTimerList.Remove(_dels2[i]);
                }
                _dels2.Clear();
            }

            #endregion

            len = _ACChannels.Count;
            for (i = 0; i < len; i++)
            {
                //防止外部删除
                if (_ACChannels[i] == null)
                {
                    _ACChannels.RemoveAt(i);
                    i--;
                    len--;
                    continue;
                }

                if (_ACChannels[i].clip && _ACChannels[i].isActiveAndEnabled && !_ACChannels[i].isPlaying)
                {
                    AudioClipKeeper keeper = _clipKeeperList.Find(k => k.clip == _ACChannels[i].clip);
                    if (keeper != null)
                    {
                        keeper.IsPlaying = false;
                    }
                    _ACChannels[i].clip = null;
                }
            }

            len = _BGMChannels.Count;
            for (i = 0; i < len; i++)
            {
                //防止外部删除
                if (_BGMChannels[i] == null)
                {
                    _BGMChannels.RemoveAt(i);
                    i--;
                    len--;
                    continue;
                }

                if (_BGMChannels[i].clip && _BGMChannels[i].isActiveAndEnabled && !_BGMChannels[i].isPlaying)
                {
                    AudioClipKeeper keeper = _clipKeeperList.Find(k => k.clip == _BGMChannels[i].clip);
                    if (keeper != null)
                    {
                        keeper.IsPlaying = false;
                    }
                    _BGMChannels[i].clip = null;
                }
            }

            len = _clipKeeperList.Count;
            for (i = 0; i < len; i++)
            {
                //防止外部删除
                if (_clipKeeperList[i] == null || _clipKeeperList[i].clip == null)
                {
                    _clipKeeperList.RemoveAt(i);
                    i--;
                    len--;
                    continue;
                }

                _clipKeeperList[i].Update(m_delta);
                if (_clipKeeperList[i].IsDead)
                {
                    _dels.Add(_clipKeeperList[i]);
                }
            }

            len = _dels.Count;
            if (len > 0)
            {
                for (i = 0; i < len; i++)
                {
                    _clipKeeperList.Remove(_dels[i]);
                }
                _dels.Clear();
            }
            
            m_lastRTime = Time.realtimeSinceStartup;
        }

        ///
        // parms 扩充参数定义 :  [volume:float(音量), StereoPan:float(左右声道), pitch(播放速率) , SpatialBlend:float(2/3D融合), ReverbZoneMix:float(混响)]
        ///
        private void _applyParmsForAudioSource(AudioSource source, params object[] parms)
        {

            //reset values
            source.volume = 1;
            source.panStereo = 0;
            source.pitch = 1;
            source.spatialBlend = 0;
            source.reverbZoneMix = 1;

            if (parms == null || parms.Length == 0)
            {
                return;
            }

            float volume = _trytoFloat(parms[0]);
            source.volume = volume;

            if (parms.Length > 1)
            {
                float StereoPan = _trytoFloat(parms[1]);
                source.panStereo = StereoPan;
            }
            if (parms.Length > 2)
            {
                float pitch = _trytoFloat(parms[2]);
                source.pitch = pitch;
            }
            if (parms.Length > 3)
            {
                float SpatialBlend = _trytoFloat(parms[2]);
                source.spatialBlend = SpatialBlend;
            }
            if (parms.Length > 4)
            {
                float ReverbZoneMix = _trytoFloat(parms[3]);
                source.reverbZoneMix = ReverbZoneMix;
            }
        }

        private float _trytoFloat(object obj)
        {
            return float.Parse(obj.ToString());
        }

        private List<AudioSource> _tmpChannels;
        private int _tmpChannelLimit;
        private AudioSource _getEmptyAudioSource(bool isBGM = false)
        {

            _tmpChannels = isBGM ? _BGMChannels : _ACChannels;
            _tmpChannelLimit = isBGM ? BGMChannelLimit : ACChannelLimit;
            AudioSource aos = null;
            for (int i = 0; i < _tmpChannels.Count; i++)
            {   
                if (_tmpChannels[i].clip == null)
                {
                    aos = _tmpChannels[i];
                    return aos;
                }
            }

            if (_tmpChannels.Count < _tmpChannelLimit)
            {
                //aos = gameObject.AddComponent<AudioSource>();
                aos = _ChannelRoot.gameObject.AddComponent<AudioSource>();
                aos.playOnAwake = false;
                aos.mute = (isBGM ? m_muteBGM : m_muteAC);
                _tmpChannels.Add(aos);
            }
            else
            {
                Debug.LogWarning("** AudioManager Warning :: 创建AudioSource超限,无法加载新的AudioSource.(最大限制:" + _tmpChannelLimit + ")");
            }

            return aos;
        }
        
    }
}