﻿using System;
using UnityEngine;

public class DelayActionBase
{

    public DelayActionBase(string name)
    {
        this.dead = false;
        this.hash = GetHashCode();
    }
    
    public string name { get; protected set; }
    public int hash { get; protected set; }

    /// <summary>
    /// 初始化 必要数据
    /// </summary>
    public virtual void Init() {
        //
    }

    /// <summary>
    /// Update
    /// </summary>
    public virtual void Update()
    {
        //
    }
    public bool dead { get; protected set; }

    public virtual void Dispose() {
        //
    }

}