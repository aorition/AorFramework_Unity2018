using UnityEngine;
using System;
using LuaInterface;

public class LuaDelayBySeconds : DelayActionBase
{

    public LuaDelayBySeconds(float seconds, LuaFunction luaFunc)
        : base("LuaDelayBySeconds")
    {
        m_luaFunc = luaFunc;
        _start = Time.realtimeSinceStartup;
        _max = seconds < 0.1f ? 0.1f : seconds;
    }

    private LuaFunction m_luaFunc;
    private float _start;
    private float _max;

    public override void Update()
    {
        if (Time.realtimeSinceStartup - _start >= _max)
        {
            if(m_luaFunc != null)
            {
                m_luaFunc.Call();
                m_luaFunc.Dispose();
                m_luaFunc = null;
            }
            dead = true;
        }
    }

    public override void Dispose()
    {
        if (m_luaFunc != null)
        {
            m_luaFunc.Dispose();
            m_luaFunc = null;
        }
    }

}
