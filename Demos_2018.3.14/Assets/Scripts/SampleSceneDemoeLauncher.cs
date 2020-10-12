using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Lua;

public class SampleSceneDemoeLauncher : MonoBehaviour
{

    private void Awake()
    {

        //初始化 LuaManager
        LuaClient lc = gameObject.AddComponent<LuaClient>();
        LuaManager luaMgr = LuaManager.CreateInstanceOnGameObject(gameObject);
        luaMgr.Setup(lc, Application.dataPath + "/Lua", true, false,()=> {
            //
        });
    }
    


}
