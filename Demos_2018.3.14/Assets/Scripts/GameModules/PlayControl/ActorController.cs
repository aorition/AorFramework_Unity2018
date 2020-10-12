using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.PlayControl
{
    /// <summary>
    /// 角色控制器(DEV)
    /// </summary>
    public class ActorController : MonoBehaviour
    {

        public bool UseFixedUpdate = true;

        protected virtual void Awake()
        {

        }

        protected bool m_isStarted = false;
        protected virtual void Start()
        {
            m_isStarted = true;
        }

        private void FixedUpdate()
        {
            if (!m_isStarted) return;
            if (UseFixedUpdate) process(Time.fixedDeltaTime);
        }

        // Update is called once per frame
        private void Update()
        {
            if (!m_isStarted) return;
            if (!UseFixedUpdate) process(Time.deltaTime);
        }

        protected virtual void process(float delta)
        {

        }

    }
}


