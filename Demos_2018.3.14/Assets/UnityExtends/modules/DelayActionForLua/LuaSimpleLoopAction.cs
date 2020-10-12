using UnityEngine;
using System.Collections;
using System;
using LuaInterface;
public class LuaSimpleLoopAction : DelayActionBase
{

    public LuaSimpleLoopAction(LuaFunction luafunc, float inverval)
        : base("LuaSimpleLoopAction")
    {
        this.m_luafunc = luafunc;
        this.inverval = inverval;
    }
    private LuaFunction m_luafunc;
    private float lastRunTime = 0;
    private float inverval = 0;
    public override void Init()
    {
        lastRunTime = Time.realtimeSinceStartup;
    }

    public override void Update()
    {

        //float curTime = Time.realtimeSinceStartup;
        //while (curTime > lastRunTime + inverval)
        //{
        //    action();
        //    lastRunTime += inverval;
        //}

        float curTime2 = Time.realtimeSinceStartup - lastRunTime;
        if (curTime2 >= inverval) {
            lastRunTime = Time.realtimeSinceStartup;
            if(m_luafunc != null)
                m_luafunc.Call();
        }

    }

    public override void Dispose()
    {
        if (m_luafunc != null)
        {
            m_luafunc.Dispose();
            m_luafunc = null;
        }
    }

}