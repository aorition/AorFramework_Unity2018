﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.editor
{

    public class PaintEditor : PaintEditorBase
    {
        /// <summary>
        /// 笔刷绘制处理方法, 笔刷在探测可绘制时调用此函数
        /// </summary>
        protected sealed override void OnPainting(Event e, RaycastHit raycastHit)
        {
            //默认实现 鼠标左键按下(拖动) 和 鼠标弹起

#if UNITY_2018
            if (e.type == EventType.MouseDown)
#else // Unity5.x
            if (e.type == EventType.mouseDown)
#endif
            {
                if (e.button == 0)
                {
                    //mouseDown
                    OnPaintingMouseDown(raycastHit, e.control || e.command, e.alt, e.shift);
                }
                else if (e.button == 1)
                {
                    //mouseDown Right
                    OnPaintingMouseDownRight(raycastHit, e.control || e.command, e.alt, e.shift);
                }else if (e.button == 2)
                {
                    OnPaintingMouseDownMiddle(raycastHit, e.control || e.command, e.alt, e.shift);
                }
            }
#if UNITY_2018
            else if (e.type == EventType.MouseDrag)
#else // Unity5.x
            else if (e.type == EventType.mouseDrag)
#endif
            {
                //mouseDrag
                if (e.button == 0)
                {
                    //mouseDown
                    OnPaintingMouseDrag(raycastHit, e.control || e.command, e.alt, e.shift);
                }
                 else if (e.button == 1)
                {
                    //mouseDown Right
                    OnPaintingMouseDownRight(raycastHit, e.control || e.command, e.alt, e.shift);
                }
                else if (e.button == 2)
                {
                    OnPaintingMouseDownMiddle(raycastHit, e.control || e.command, e.alt, e.shift);
                }
            }
#if UNITY_2018
            else if (e.type == EventType.MouseUp && e.button == 0)
#else // Unity5.x
            else if (e.type == EventType.mouseUp && e.button == 0)
#endif
            {
                //mouseUp
                if (e.button == 0)
                {
                    //MouseUp
                    OnPaintingMouseUp(raycastHit, e.control || e.command, e.alt, e.shift);
                }
                else if (e.button == 1)
                {
                    //MouseUp Right
                    OnPaintingMouseRightUp(raycastHit, e.control || e.command, e.alt, e.shift);
                }else if (e.button == 2)
                {
                    OnPaintingMouseRightMiddle(raycastHit, e.control || e.command, e.alt, e.shift);
                }
            }
        }

#region Event Drag

        protected virtual void OnPaintingMouseDrag(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
        }

        protected virtual void OnPaintingMouseDragRight(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
        }

        protected virtual void OnPaintingMouseDragMiddle(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
        }

#endregion

#region Event Down

        protected virtual void OnPaintingMouseDown(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
        }

        protected virtual void OnPaintingMouseDownRight(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
        }

        protected virtual void OnPaintingMouseDownMiddle(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
        }

#endregion

#region Event mouseUp

        protected virtual void OnPaintingMouseUp(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
            //mouseUp
        }

        protected virtual void OnPaintingMouseRightUp(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
            //mouseUp
        }

        protected virtual void OnPaintingMouseRightMiddle(RaycastHit raycastHit, bool ctrl, bool alt, bool shift)
        {
            //mouseUp
        }

#endregion


    }

}


