#define USING_DOTWEENING
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using LuaInterface;

using BindType = ToLuaMenu.BindType;
using System.Reflection;

using UnityEngine.EventSystems;
//using Framework.Network;
//using Framework.Resource;
using Framework.Graphic;
using Framework.Audio;
//using Framework.UI;
using Framework;
using UnityEditor;
using AorBaseUtility;
//using ByteBuffer = Framework.Network.ByteBuffer;
//using Framework.module;

public static class CustomSettings
{

    public static string saveDir = Application.dataPath + "/Source/Generate/";
    public static string luaDir = Application.dataPath + "/Lua/";
    public static string toluaBaseType = Application.dataPath + "/ToLua/BaseType/";
    public static string baseLuaDir = Application.dataPath + "/Tolua/Lua/";
    public static string injectionFilesPath = Application.dataPath + "/ToLua/Injection/";

    //导出时强制做为静态类的类型(**** 注意customTypeList 还要添加这个类型才能导出)
    //unity 有些类作为sealed class, 其实完全等价于静态类
    public static List<Type> staticClassTypes = new List<Type>
    {        
        typeof(UnityEngine.Application),
        typeof(UnityEngine.Time),
        typeof(UnityEngine.Screen),
        typeof(UnityEngine.SleepTimeout),
        typeof(UnityEngine.Input),
        typeof(UnityEngine.Resources),
        typeof(UnityEngine.Physics),
        typeof(UnityEngine.RenderSettings),
        typeof(UnityEngine.QualitySettings),
        typeof(UnityEngine.GL),
        //typeof(LuaHelper),
        //typeof(Framework.Events.AEvents)
    };

    //附加导出委托类型(在导出委托时, customTypeList 中牵扯的委托类型都会导出， 无需写在这里)
    public static DelegateType[] customDelegateList = 
    {      
        
        _DT(typeof(Action)),
        _DT(typeof(System.Action<int>)),
        _DT(typeof(System.Action<string>)),
        _DT(typeof(System.Action<object[]>)),
        _DT(typeof(System.Action<UnityEngine.Object>)),
        _DT(typeof(System.Action<UnityEngine.AudioSource>)),

        _DT(typeof(UnityEngine.Events.UnityAction)),
        _DT(typeof(System.Predicate<int>)),

        _DT(typeof(System.Comparison<int>)),

    };

    //在这里添加你要导出注册到lua的类型列表
    public static BindType[] customTypeList =
    {    
		//_GT(typeof(LoopScrollView)),
		//_GT(typeof(AnimatorDeactive)),
		//_GT(typeof(DigitalRollingEx)),
		//_GT(typeof(DoubleFormat)),
		//_GT(typeof(FrameAnimImageHandler)),
        //
		//_GT(typeof(UniWebView)),
		//_GT(typeof(UniWebViewTransitionEdge)),

        //------------------------为例子导出--------------------------------
        //_GT(typeof(TestEventListener)),
        //_GT(typeof(TestProtol)),
        //_GT(typeof(TestAccount)),
        //_GT(typeof(Dictionary<int, TestAccount>)).SetLibName("AccountMap"),
        //_GT(typeof(KeyValuePair<int, TestAccount>)),    
        //_GT(typeof(TestExport)),
        //_GT(typeof(TestExport.Space)),
        //-------------------------------------------------------------------        
        //_GT(typeof(Debugger)).SetNameSpace(null),        

#if USING_DOTWEENING
        _GT(typeof(DG.Tweening.AutoPlay)),
        _GT(typeof(DG.Tweening.AxisConstraint)),
        _GT(typeof(DG.Tweening.LogBehaviour)),
        _GT(typeof(DG.Tweening.ScrambleMode)),
        _GT(typeof(DG.Tweening.TweenType)),
        _GT(typeof(DG.Tweening.UpdateType)),
        
        _GT(typeof(DG.Tweening.DOVirtual)),
        _GT(typeof(DG.Tweening.EaseFactory)),
        _GT(typeof(DG.Tweening.TweenParams)),

        _GT(typeof(DG.Tweening.Core.ABSSequentiable)),

        _GT(typeof(DG.Tweening.DOTween)),
        _GT(typeof(DG.Tweening.Tween)).SetBaseType(typeof(System.Object)).AddExtendType(typeof(DG.Tweening.TweenExtensions)),
        _GT(typeof(DG.Tweening.Sequence)).AddExtendType(typeof(DG.Tweening.TweenSettingsExtensions)),
        _GT(typeof(DG.Tweening.Tweener)).AddExtendType(typeof(DG.Tweening.TweenSettingsExtensions)),
        _GT(typeof(DG.Tweening.LoopType)),
        _GT(typeof(DG.Tweening.PathMode)),
        _GT(typeof(DG.Tweening.PathType)),
        _GT(typeof(DG.Tweening.RotateMode)),
        _GT(typeof(DG.Tweening.Ease)),
 
        _GT(typeof(Component)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        _GT(typeof(Transform)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        _GT(typeof(Material)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        _GT(typeof(Rigidbody)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        _GT(typeof(Camera)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        _GT(typeof(AudioSource)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        //_GT(typeof(LineRenderer)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),
        //_GT(typeof(TrailRenderer)).AddExtendType(typeof(DG.Tweening.ShortcutExtensions)),    

        _GT(typeof(DG.Tweening.Core.TweenerCore<float,float,DG.Tweening.Plugins.Options.FloatOptions>)),
        _GT(typeof(DG.Tweening.Core.TweenerCore<Vector2,Vector2,DG.Tweening.Plugins.Options.VectorOptions>)),
        _GT(typeof(DG.Tweening.Core.TweenerCore<Vector3,Vector3,DG.Tweening.Plugins.Options.VectorOptions>)),
        _GT(typeof(DG.Tweening.Core.TweenerCore<Vector4,Vector4,DG.Tweening.Plugins.Options.VectorOptions>)),
        _GT(typeof(DG.Tweening.Core.TweenerCore<UnityEngine.Color,UnityEngine.Color,DG.Tweening.Plugins.Options.ColorOptions>)),
        
#else
      
        _GT(typeof(Component)),
        _GT(typeof(Transform)),
        _GT(typeof(Material)),
        //_GT(typeof(Light)),
        _GT(typeof(Rigidbody)),
        _GT(typeof(Camera)),
        _GT(typeof(AudioSource)),
        //_GT(typeof(LineRenderer))
        //_GT(typeof(TrailRenderer))
#endif
        _GT(typeof(Light)),
        _GT(typeof(Behaviour)),
        _GT(typeof(MonoBehaviour)),        
        _GT(typeof(GameObject)),
        _GT(typeof(TrackedReference)),
        _GT(typeof(Application)),
        _GT(typeof(Physics)),
        _GT(typeof(Collider)),
        _GT(typeof(Time)),        
        _GT(typeof(Texture)),
        _GT(typeof(Texture2D)),
        _GT(typeof(Shader)),        
        _GT(typeof(Renderer)),
        _GT(typeof(WWW)),
		_GT(typeof(WWWForm)),
        _GT(typeof(Screen)),     
		_GT(typeof(ScreenOrientation)),   
        _GT(typeof(CameraClearFlags)),
        _GT(typeof(AudioClip)),        
        _GT(typeof(AssetBundle)),
        _GT(typeof(ParticleSystem)),
        _GT(typeof(AsyncOperation)).SetBaseType(typeof(System.Object)),        
        //_GT(typeof(LightType)),
        _GT(typeof(SleepTimeout)),
#if UNITY_5_3_OR_NEWER
        //_GT(typeof(UnityEngine.Experimental.Director.DirectorPlayer)),
#endif
        _GT(typeof(Animator)),
        _GT(typeof(Input)),
        _GT(typeof(KeyCode)),
        _GT(typeof(SkinnedMeshRenderer)),
        _GT(typeof(Space)),
        _GT(typeof(CanvasGroup)),

        _GT(typeof(MeshRenderer)),

#if !UNITY_5_4_OR_NEWER
        _GT(typeof(ParticleEmitter)),
        _GT(typeof(ParticleRenderer)),
        _GT(typeof(ParticleAnimator)), 
#endif
                    
        _GT(typeof(BoxCollider)),
        _GT(typeof(MeshCollider)),
        _GT(typeof(SphereCollider)),        
        _GT(typeof(CharacterController)),
        _GT(typeof(CapsuleCollider)),
        
        _GT(typeof(Animation)),        
        _GT(typeof(AnimationClip)).SetBaseType(typeof(UnityEngine.Object)),        
        _GT(typeof(AnimationState)),
        _GT(typeof(AnimationBlendMode)),
        _GT(typeof(QueueMode)),  
        _GT(typeof(PlayMode)),
        _GT(typeof(WrapMode)),

        _GT(typeof(QualitySettings)),
        _GT(typeof(RenderSettings)),                                                   
        _GT(typeof(BlendWeights)),           
        _GT(typeof(RenderTexture)),
        //_GT(typeof(EventTriggerListener)),
        _GT(typeof(Resources)),
        _GT(typeof(EventSystem)),
        _GT(typeof(EventTrigger)),

        _GT(typeof(Graphic)),
        _GT(typeof(MaskableGraphic)),
        _GT(typeof(Image)),
        _GT(typeof(Text)),
        _GT(typeof(Sprite)),
        _GT(typeof(Toggle)),
        _GT(typeof(Toggle.ToggleEvent)),
        _GT(typeof(ToggleGroup)),
        _GT(typeof(InputField)),
        _GT(typeof(InputField.SubmitEvent)),
        _GT(typeof(Rect)),
        _GT(typeof(LayoutGroup)),
        _GT(typeof(HorizontalOrVerticalLayoutGroup)),
        _GT(typeof(VerticalLayoutGroup)),
        _GT(typeof(HorizontalLayoutGroup)),
        _GT(typeof(ContentSizeFitter)),

        _GT(typeof(Dropdown)),
		_GT(typeof(Dropdown.OptionData)),
		_GT(typeof(Dropdown.DropdownEvent)),

        _GT(typeof(Mask)),
        _GT(typeof(RectMask2D)),
        _GT(typeof(LayoutElement)),

        _GT(typeof(Slider)),
        _GT(typeof(Slider.SliderEvent)),

        _GT(typeof(Scrollbar)),

        _GT(typeof(RectTransform)),
        _GT(typeof(RectTransformUtility)),
        _GT(typeof(PointerEventData)),
         
        _GT(typeof(UIBehaviour)),
        _GT(typeof(Selectable)),
        _GT(typeof(Button)),
        _GT(typeof(Button.ButtonClickedEvent)),
        _GT(typeof(UnityEngine.Events.UnityEvent)),
        _GT(typeof(Canvas)),        
        _GT(typeof(CanvasScaler)),
        _GT(typeof(BaseRaycaster)),
        _GT(typeof(GraphicRaycaster)),


        _GT(typeof(LuaBehaviour)),
        //_GT(typeof(UILuaBehaviour)),
        //_GT(typeof(GameResFactory)),

        _GT(typeof(ScrollRect)),
        _GT(typeof(ScrollRect.ScrollRectEvent)),
        //_GT(typeof(LoopScrollRect)),
        //_GT(typeof(LoopVerticalScrollRect)),
        //_GT(typeof(LoopHorizontalScrollRect)),

        //_GT(typeof(Base)),
        //_GT(typeof(Manager)),
        _GT(typeof(ByteBuffer)),
        

        #region Managers

        _GT(typeof(ManagerBase)),
        //_GT(typeof(UIManager)),
        _GT(typeof(GraphicsManager)),
        _GT(typeof(AudioManager)),
        //_GT(typeof(AudioMgrBinder.AudioType)).SetNameSpace("AudioManager"),
        //_GT(typeof(NetworkManager)),
       // _GT(typeof(NetworkClientController)),
       // _GT(typeof(ResourceManager)),
        //_GT(typeof(UnityMessageManager)),
        //_GT(typeof(GameManager)),
        //_GT(typeof(HotfixManager)),
       // _GT(typeof(Framework.Events.AEventManager)),
        _GT(typeof(DelayActionManager)),
        _GT(typeof(QueueActionManager)),
        _GT(typeof(QueueAction)),
        //_GT(typeof(UniWebViewController)),
        #endregion


        //_GT(typeof(LuaHelper)),
        //_GT(typeof(Globals)),

        /////////////////////////////////////LeanTween     
        //_GT(typeof(LeanTweenType)),
        //_GT(typeof(LTDescrImpl)),
        //_GT(typeof(LTBezier)),
        //_GT(typeof(LTBezierPath)),
        //_GT(typeof(LTEvent)),
        //_GT(typeof(LTSpline)),
        //_GT(typeof(LeanTween)),
        //_GT(typeof(LeanAudio)),
        /////////////////////////////////////LeanTween  end


        //_GT(typeof(MTabWindow)),
       // _GT(typeof(AssetPacker)),
        //_GT(typeof(TimerController)),

        _GT(typeof(SystemInfo)),
        _GT(typeof(PlayerPrefs)),

        //_GT(typeof(AlertPanel)),

        _GT(typeof(UnityEngine.RuntimePlatform)),
#if !UNITY_EDITOR && !UNITY_STANDALONE_OSX && !UNITY_STANDALONE_WIN
        _GT(typeof(Handheld)),
#endif
        //_GT(typeof(CLogicUtility)),
        //_GT(typeof(TriggerEvent)),
        _GT(typeof(BoxCollider2D)),
        _GT(typeof(UnityEngine.Debug)),

        //_GT(typeof(DragEnhanceView)),
        //_GT(typeof(EnhanceItem)),
        //_GT(typeof(EnhanceScrollView)),
        //_GT(typeof(EnhanceScrollViewDragController)),
        //_GT(typeof(UDragEnhanceView)),
        //      _GT(typeof(MyUGUIEnhanceItem)),

       // _GT(typeof(Framework.Events.AEvents)),
        //_GT(typeof(LuaBehaviourListener)),

       // _GT(typeof(BaseImage)),
       // _GT(typeof(RoundedImage)),
      //  _GT(typeof(CircleImage)),
      //  _GT(typeof(BaseRawImage)),
     //   _GT(typeof(RoundedRawImage)),

        _GT(typeof(SpriteRenderer)),

        _GT(typeof(Color)),
        _GT(typeof(Color32)),

        _GT(typeof(RaycastHit)),
       // _GT(typeof(TargetFollowHandler)),
        _GT(typeof(PrefabEffectHandler)),
        _GT(typeof(RenderQueueOverrider)),

       // _GT(typeof(ColliderEventListener)),
        _GT(typeof(Collision)),
		//_GT(typeof(QRCreator)),
        
        #region Utils
        
        _GT(typeof(RuntimeAnimatorController)),

        #endregion

    };

    public static List<Type> dynamicList = new List<Type>()
    {
        typeof(MeshRenderer),
#if !UNITY_5_4_OR_NEWER
        typeof(ParticleEmitter),
        typeof(ParticleRenderer),
        typeof(ParticleAnimator),
#endif

        typeof(BoxCollider),
        typeof(MeshCollider),
        typeof(SphereCollider),
        typeof(CharacterController),
        typeof(CapsuleCollider),

        typeof(Animation),
        typeof(AnimationClip),
        typeof(AnimationState),
        typeof(RuntimeAnimatorController),

        typeof(BlendWeights),
        typeof(RenderTexture),
        typeof(Rigidbody),
        //typeof(TriggerEvent),
        typeof(BoxCollider2D),
    };

    //重载函数，相同参数个数，相同位置out参数匹配出问题时, 需要强制匹配解决
    //使用方法参见例子14
    public static List<Type> outList = new List<Type>()
    {
        
    };
        
    //ngui优化，下面的类没有派生类，可以作为sealed class
    public static List<Type> sealedList = new List<Type>()
    {
        /*typeof(Transform),
        typeof(UIRoot),
        typeof(UICamera),
        typeof(UIViewport),
        typeof(UIPanel),
        typeof(UILabel),
        typeof(UIAnchor),
        typeof(UIAtlas),
        typeof(UIFont),
        typeof(UITexture),
        typeof(UISprite),
        typeof(UIGrid),
        typeof(UITable),
        typeof(UIWrapGrid),
        typeof(UIInput),
        typeof(UIScrollView),
        typeof(UIEventListener),
        typeof(UIScrollBar),
        typeof(UICenterOnChild),
        typeof(UIScrollView),        
        typeof(UIButton),
        typeof(UITextList),
        typeof(UIPlayTween),
        typeof(UIDragScrollView),
        typeof(UISpriteAnimation),
        typeof(UIWrapContent),
        typeof(TweenWidth),
        typeof(TweenAlpha),
        typeof(TweenColor),
        typeof(TweenRotation),
        typeof(TweenPosition),
        typeof(TweenScale),
        typeof(TweenHeight),
        typeof(TypewriterEffect),
        typeof(UIToggle),
        typeof(Localization),*/
    };

    public static BindType _GT(Type t)
    {
        return new BindType(t);
    }

    public static DelegateType _DT(Type t)
    {
        return new DelegateType(t);
    }    


    [MenuItem("Lua/Attach Profiler", false, 151)]
    static void AttachProfiler()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("警告", "请在运行时执行此功能", "确定");
            return;
        }

        LuaClient.Instance.AttachProfiler();
    }

    [MenuItem("Lua/Detach Profiler", false, 152)]
    static void DetachProfiler()
    {
        if (!Application.isPlaying)
        {            
            return;
        }

        LuaClient.Instance.DetachProfiler();
    }
}
