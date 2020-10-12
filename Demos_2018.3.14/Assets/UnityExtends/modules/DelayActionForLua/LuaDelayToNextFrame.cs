using UnityEngine;
using System.Collections;
using System;
using LuaInterface;

public class LuaDelayToNextFrame : DelayActionBase
{

    public LuaDelayToNextFrame(LuaFunction luaFunc) 
        : base("LuaDelayToNextFrame")
    {
        this.m_luaFunc = luaFunc;
    }
    private LuaFunction m_luaFunc;
    private bool _init;
    public override void Init()
    {
        _init = true;
    }

    public override void Update()
    {
        if (_init) {
            _init = false;
            return;
        }
        if (m_luaFunc != null) {
            m_luaFunc.Call();
            m_luaFunc.Dispose();
            m_luaFunc = null;
        }
        dead = true;
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
