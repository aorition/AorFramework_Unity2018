using UnityEngine;

namespace Framework.UI
{

    /// <summary>
    /// AorUIManagerLuncher AorUIManager启动器
    /// 
    /// 使用UIManagerLuncher多种方式之一 : 将此脚本挂在任意一个GameObject上即可实现AorUIManager的启动(带配置).
    /// 
    /// </summary>
    public class UIManagerLauncher : ManagerLauncherBase
    {

        [SerializeField]
        private UIManagerSettingAsset _UIManagerSetting;

        protected override ManagerBase onLauncherInit()
        {
            if (UIManager.IsInit()) return null;

            if (UseThisGameObject)
                UIManager.CreateInstanceOnGameObject(gameObject);
            else
                UIManager.CreateInstance(ParentTransformPovit);

            UIManager.Instance.setup(_UIManagerSetting);

            return UIManager.Instance;
        }

    }

}