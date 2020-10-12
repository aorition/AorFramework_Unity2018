using UnityEngine;

namespace Framework.Graphic
{

    /// <summary>
    /// GraphicsManagerLuncher GraphicsManager启动器
    /// 
    /// 使用GraphicsManager多种方式之一 : 将此脚本挂在任意一个GameObject上即可实现GraphicsManager的启动(带配置).
    /// 
    /// </summary>
    public class GraphicsManagerLauncher : ManagerLauncherBase
    {
        
        [SerializeField]
        private GraphicsSettingAsset _GraphicsManagerSetting;

        [SerializeField]
        private RectTransform _GMEUIRoot;

        protected override ManagerBase onLauncherInit()
        {

            if (GraphicsManager.IsInit()) return null;

            if (UseThisGameObject)
                GraphicsManager.CreateInstanceOnGameObject(gameObject);
            else
                GraphicsManager.CreateInstance(ParentTransformPovit);

            if (_GMEUIRoot)
                GraphicsManager.Instance.Setup(_GraphicsManagerSetting, _GMEUIRoot);
            else
                GraphicsManager.Instance.Setup(_GraphicsManagerSetting);

            return GraphicsManager.Instance;
        }

    }

}