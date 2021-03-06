﻿using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// 延迟执行行为, 每帧检查条件,直到条件达成.
/// </summary>
public class DelayForCondition : DelayActionBase
{

    public DelayForCondition(Func<bool> condition, Action action)
        : base("DelayForCondition")
    {
        this.action = action;
        _condition = condition;

        if (_condition())
        {
            Action tmp = action;
            tmp();
            _condition = null;
            action = null;
            dead = true;
        }

    }

    private Action action;
    private Func<bool> _condition;
    public override void Update()
    {
        if (_condition())
        {
            Action tmp = action;
            tmp();
            _condition = null;
            action = null;
            dead = true;
        }
    }

    public override void Dispose()
    {
        _condition = null;
        action = null;
    }

}
