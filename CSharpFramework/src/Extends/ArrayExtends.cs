using System;
using System.Collections.Generic;

namespace AorBaseUtility.Extends
{
    public static class ArrayExtends
    {

        /// <summary>
        /// 克隆一个数组
        /// </summary>
        public static T[] Clone<T>(this T[] Array) where T : class
        {
            if (Array == null) return null;

            int i, len = Array.Length;
            T[] newArray = new T[len];

            for (i = 0; i < len; i++)
            {
                newArray[i] = Array[i];
            }

            return newArray;
        }
        
        public static T[] Add<T>(this T[] Array, T add) where T : class {
            if (Array == null) Array = new T[0];
            int len = Array.Length;
            T[] nList = new T[len + 1];
            for (int i = 0; i < len; i++)
            {
                nList[i] = Array[i];
            }
            nList[len] = add;
            return nList;
        }

        public static bool Contains<T>(this T[] Array, T key) where T : class
        {
            if (Array != null)
            {
                for (int i = 0; i < Array.Length; i++)
                {
                    if (key == Array[i]) return true;
                }
            }
            return false;
        }

        public static int IndexOf<T>(this T[] Array, T key) where T : class
        {
            if (Array != null)
            {
                for (int i = 0; i < Array.Length; i++)
                {
                    if (key == Array[i]) return i;
                }
            }
            return -1;
        }

    }
}
