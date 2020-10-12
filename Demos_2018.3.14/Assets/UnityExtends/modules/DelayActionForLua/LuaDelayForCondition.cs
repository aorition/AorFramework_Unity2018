using System;
using UnityEngine;
using System.Collections;
using LuaInterface;
/// <summary>
/// 延迟执行行为, 每帧检查条件,直到条件达成.
/// </summary>
public class LuaDelayForCondition : DelayActionBase
{

    public LuaDelayForCondition(LuaFunction condition, LuaFunction luaFunc)
        : base("LuaDelayForCondition")
    {
        m_condition = condition;
        m_luaFunc = luaFunc;

        if (m_condition.Invoke<bool>())
        {
            if(m_luaFunc != null)
            {
                m_luaFunc.Call();
            }
            Dispose();
            dead = true;
        }

    }

    private LuaFunction m_luaFunc;
    private LuaFunction m_condition;
    public override void Update()
    {
        if (m_condition.Invoke<bool>())
        {
            if (m_luaFunc != null)
                m_luaFunc.Call();
            Dispose();
            dead = true;
        }
    }

    public override void Dispose()
    {
        if(m_condition != null)
        {
            m_condition.Dispose();
            m_condition = null;
        }

        if (m_luaFunc != null)
        {
            m_luaFunc.Dispose();
            m_luaFunc = null;
        }

    }

}
