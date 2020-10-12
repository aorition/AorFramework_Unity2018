using UnityEngine;
using System;
using LuaInterface;
public class LuaDelayByFrames : DelayActionBase
{

    public LuaDelayByFrames(int frames, LuaFunction action)
        : base("LuaDelayToNextFrame")
    {
        _max = frames < 1 ? 1 : frames;
        m_luaFunc = action;
    }

    private LuaFunction m_luaFunc;

    private int _max;
    private int _ct;

    public override void Init()
    {
        _ct = 0;
    }

    public override void Update()
    {
        _ct++;
        if (_ct >= _max)
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