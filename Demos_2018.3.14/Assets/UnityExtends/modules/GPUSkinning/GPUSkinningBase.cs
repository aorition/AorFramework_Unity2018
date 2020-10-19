using UnityEngine;
using System.Collections.Generic;

namespace Framework.GPUSkinning
{
    public class GPUSkinningBase :MonoBehaviour
    {
        //protected string currentAname = "";
        //protected int currentAid = 0;
        ///// <summary>
        ///// 是否停止播放
        ///// </summary>
        //public bool isStop;
        ///// <summary>
        ///// 速度
        ///// </summary>
        //public float Speed = 1;
        //public string[] animaArray;
        //protected int aid;
        ///// <summary>
        ///// 播放指定动画
        ///// </summary>
        ///// <param name="aname"></param>
        //[LuaCallCSharp]
        //public virtual void PlayArr(params string[] aname)
        //{
        //    isPlayEnd = false;
        //    animaArray = aname;
        //    aid = 1;
        //    Play(aname[0]);
        //}
        //protected WrapMode _loopType = WrapMode.Loop;
        //[LuaCallCSharp]
        //public virtual void PlayComplex(string type, float speed, params string[] aname)
        //{
        //    switch (type)
        //    {
        //        case "one":
        //            _loopType = WrapMode.Once;
        //            break;                
        //        case "loop":
        //            _loopType = WrapMode.Loop;
        //            break;
        //    }
        //    Speed = speed;       
        //    PlayArr(aname);
        //}
        //protected virtual void Play(string aname)
        //{
        //}
        //protected bool isPlayEnd;
        ///// <summary>
        ///// 播放结束
        ///// </summary>
        //public virtual void PlayEnd()
        //{
        //    if (animaArray != null && animaArray.Length > aid)
        //    {

        //        Play(animaArray[aid]);
        //        aid++;
        //    }
        //    else if (_loopType == WrapMode.Once)
        //    {
        //        isPlayEnd = true;
        //    }
        //}

        ///// <summary>
        ///// 停止
        ///// </summary>
        //public void gStop()
        //{
        //    isStop = true;
        //}
        ///// <summary>
        ///// 开始播放
        ///// </summary>
        //public void gPlay()
        //{
        //    isStop = false;
        //}

        //protected virtual int getAid(string aname)
        //{
        //    return -1;
        //}

        //protected float second = 0.0f;

        //protected virtual void UpdateBoneAnimationMatrix()
        //{
        //}

        //public virtual float GetAnimLength()
        //{
        //    return 0;
        //}

        //public bool HasAnim(string aname)
        //{
        //    return getAid(aname) != -1;
        //}
    }

}
