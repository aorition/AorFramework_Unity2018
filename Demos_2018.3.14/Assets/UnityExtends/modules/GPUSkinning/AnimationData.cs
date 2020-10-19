using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.GPUSkinning
{
    public class AnimationData
    {

        public const double FRAME = 60d;
        public const double FRAME_DELTA = 1 / FRAME;

        //public GPUInstancingTransform _transform;
        //public GPUInstancingTransform transform
        //{
        //    set
        //    {
        //        _transform = value;
        //        _transform.setPropertyBlockValue(new string[] { "_Frame", "_Color", "_DeadTime", "_targetChoose" }, new GPUInstancingTransform.KeyType[] { GPUInstancingTransform.KeyType.Float, GPUInstancingTransform.KeyType.Vecter, GPUInstancingTransform.KeyType.Float, GPUInstancingTransform.KeyType.Float }, _Update);
        //        transform.keyValue[2].FloatValues = 0f;
        //        transform.keyValue[3].FloatValues = 0f;
        //    }
        //    get { return _transform; }
        //}
        public Vector2[] frames;
        public string[] frameNames;

        public AnimationData(GPUSkinningTexturen Gpubase)
        {
            frames = Gpubase.frames;
            frameNames = Gpubase.frameNames;
        }

        //public void SetSoldierColor(Vector4 color)
        //{
        //    transform.keyValue[1].Ve4Values = color;
        //}

        //public void SetTheSoldierDeadTime(float time)
        //{
        //    transform.keyValue[2].FloatValues = time;
        //}

        //public void SetTheTargetBeingChoose(bool isChoose)
        //{
        //    transform.keyValue[3].FloatValues = isChoose ? 1 : 0;
        //}

        protected string currentAname = "";
        protected int currentAid = 0;
        /// <summary>
        /// 是否停止播放
        /// </summary>
        public bool isStop;
        /// <summary>
        /// 速度
        /// </summary>
        public float Speed = 1;
        public string[] animaArray;
        protected int aid;
        /// <summary>
        /// 播放指定动画
        /// </summary>
        /// <param name="aname"></param>
        public virtual void Play(int start_frame = 0, params string[] aname)
        {
            isPlayEnd = false;
            animaArray = aname;
            aid = 1;
            Play(aname[0], start_frame);
        }
        WrapMode _loopType = WrapMode.Loop;
        public virtual void Play(WrapMode type, float speed, int start_frame = 0, params string[] aname)
        {
            Speed = speed;
            _loopType = type;
            Play(start_frame, aname);
        }
        public void Play(string aname, int start_frame = 0)
        {
            int _aid = getAid(aname);
            if(_aid != -1)
            {
                currentAname = aname;
                currentAid = _aid;
                currentFrame = frames[currentAid];
                second = currentFrame[0] + start_frame;
            }
        }
        protected bool isPlayEnd;
        /// <summary>
        /// 播放结束
        /// </summary>
        public void PlayEnd()
        {
            if(animaArray != null && animaArray.Length > aid)
            {

                Play(animaArray[aid]);
                aid++;
            }
            else if(_loopType == WrapMode.Once)
            {
                isPlayEnd = true;
                second = currentFrame[1];
                return;
            }
            second = currentFrame[0];
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            isStop = true;
        }
        /// <summary>
        /// 开始播放
        /// </summary>
        public void Play()
        {
            isStop = false;
        }

        public float second = 0.0f;
        public bool HasAnim(string aname)
        {
            return getAid(aname) != -1;
        }


        Vector2 currentFrame;


        public int getAid(string aname)
        {
            for(int i = 0; i < frameNames.Length; i++)
            {
                if(frameNames[i] == aname)
                {
                    return i;
                }
            }
            return -1;
        }
        const float f = 1f / 60f * 2f;
        int _frame;
        float jiange = 0;
        public void _Update()
        {
            if(!isStop && !isPlayEnd)
            {
                _frame++;
                jiange += Time.deltaTime;
                if(_frame % 2 == 0)//采集动画用了30帧，而播放的时候是60帧，所以2帧执行一次动画
                {
                    second += Speed * jiange / f;
                    jiange = 0;
                    if(second > currentFrame[1])
                    {
                        //一个动画播放到结尾
                        PlayEnd();
                    }
                }
            }
            //transform.keyValue[0].FloatValues = second;
        }

        public float GetAnimLength()
        {
            return (float)((currentFrame.y - currentFrame.x) * FRAME_DELTA);
        }
    }
}