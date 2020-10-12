#pragma warning disable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public abstract class ManagerLauncherBase : MonoBehaviour
    {

        [SerializeField]
        protected bool UseThisGameObject = false;

        [SerializeField]
        protected Transform ParentTransformPovit;

        [SerializeField]
        protected bool DonDestroyOnLoad = true;
        
        private void Awake()
        {
            ManagerBase manager = onLauncherInit();
            if (manager)
            {
                if(DonDestroyOnLoad && manager.transform.root == null)
                    GameObject.DontDestroyOnLoad(manager.gameObject);

                //只有通过该脚本启动Manager成功才会调用后续实现
                onAfterLauncherInit();
            }
            //启动器执行完毕后自动移除自身
            GameObject.Destroy(this);
        }

        /// <summary>
        /// Manager启动实现
        /// </summary>
        protected virtual ManagerBase onLauncherInit()
        {
            //Do manger creative sh.
            throw new Exception("*** ManagerLauncherBase.onLauncherInit Error :: 该方法须要被完整覆盖,不能有base.onLauncherInit().");
            return null;
        }

        /// <summary>
        /// Manager.Init之后调用该函数，以实现Manager初始化后的后续操作。
        /// 
        /// </summary>
        protected virtual void onAfterLauncherInit() {

        }

    }
}
