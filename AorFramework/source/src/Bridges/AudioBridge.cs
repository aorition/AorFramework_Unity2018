using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class AudioBridge
    {

        #region Play

        public static Action<string, Action<AudioSource>, object[]> PlayHook;

        public static void Play(string name, Action<AudioSource> callback = null, params object[] parms)
        {

            if (PlayHook != null)
            {
                PlayHook(name, callback, parms);
                return;
            }

            //default:

        }

        #endregion

        #region PlayClip

        public static Action<AudioClip, Action<AudioSource>, object[]> PlayClipHook;

        public static void PlayClip(AudioClip clip, Action<AudioSource> callback = null, params object[] parms)
        {
            if (PlayClipHook != null)
            {
                PlayClipHook(clip, callback, parms);
                return;
            }

            //default:
            // do nothing
        }

        #endregion

        #region PlayLoop

        public static Action<string, float, Action<AudioSource>, object[]> PlayLoopHook;

        public static void PlayLoop(string name, float duration, Action<AudioSource> callback = null, params object[] parms) {

            if (PlayLoopHook != null)
            {
                PlayLoopHook(name, duration, callback, parms);
                return;
            }

            //default:
        }

        #endregion

        #region PlayClipLoop

        public static Action<AudioClip, float, Action<AudioSource>, object[]> PlayClipLoopHook;

        public void PlayClipLoop(AudioClip clip, float duration, Action<AudioSource> callback = null, params object[] parms) {

            if (PlayClipLoopHook != null)
            {
                PlayClipLoopHook(clip, duration, callback, parms);
                return;
            }

            //default:

        }

        #endregion
    }
}
