using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphic.FastShadowProjector
{
    /// <summary>
    /// GlobalProjectorManagerLauncher GlobalProjectorManager启动器
    /// 
    /// 使用GlobalProjectorManager多种方式之一 : 将此脚本挂在任意一个GameObject上即可实现GlobalProjectorManager的启动(带配置).
    /// 
    /// </summary>
    public class GlobalProjectorManagerLauncher : ManagerLauncherBase
    {
        
        [SerializeField]
        private FSPConfigAsset _SettingAsset;

        [SerializeField]
        private Camera _viewCamera;

        protected override ManagerBase onLauncherInit()
        {
            if (GlobalProjectorManager.IsInit()) return null;

            if (UseThisGameObject)
                GlobalProjectorManager.CreateInstanceOnGameObject(gameObject);
            else
                GlobalProjectorManager.CreateInstance(ParentTransformPovit);

            if (_viewCamera)
                GlobalProjectorManager.Instance.Setup(_viewCamera, _SettingAsset);
            else
                GlobalProjectorManager.Instance.Setup(_SettingAsset);

            return GlobalProjectorManager.Instance;
        }
        
    }
}
