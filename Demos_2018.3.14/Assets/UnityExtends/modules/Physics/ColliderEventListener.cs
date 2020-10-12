using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.module
{
    public class ColliderEventListener : MonoBehaviour
    {

        private Collider m_collider;

        public Action<Collision> CollisionEnter;
        public Action<Collision> CollisionStay;
        public Action<Collision> CollisionExit;
        
        private void Awake()
        {
            m_collider = GetComponent<Collider>();
        }

        private void OnDestroy()
        {
            CollisionEnter = null;
            CollisionStay = null;
            CollisionExit = null;
            m_collider = null;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (CollisionEnter != null) CollisionEnter(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            if (CollisionStay != null) CollisionStay(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (CollisionExit != null) CollisionExit(collision);
        }

    }


}