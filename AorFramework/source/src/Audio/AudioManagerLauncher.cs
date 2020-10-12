using UnityEngine;

namespace Framework.Audio
{
    /// <summary>
    /// AudioManagerLauncher AudioManager启动器
    /// 
    /// 使用AudioManagerLuncher多种方式之一 : 将此脚本挂在任意一个GameObject上即可实现AorUIManager的启动(带配置).
    /// 
    /// </summary>
    public class AudioManagerLauncher : ManagerLauncherBase
    {

        protected override ManagerBase onLauncherInit()
        {
            if (AudioManager.IsInit()) return null;

            if (UseThisGameObject)
                AudioManager.CreateInstanceOnGameObject(gameObject);
            else
                AudioManager.CreateInstance(ParentTransformPovit);

            return AudioManager.Instance;
        }

    }
}
