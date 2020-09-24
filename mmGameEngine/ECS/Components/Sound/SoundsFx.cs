using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;

namespace mmGameEngine
{
    /*
     * Play a sound fx ONE time (like explosion)
     */
    public class SoundsFx : RenderComponent
    {
        public Sound SoundFx;
        public SoundState SoundFxState;
        public SoundsFx(Sound wavSound)
        {
            SoundFx = wavSound;
            SoundFxState = SoundState.None;
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }
        public void Play()
        {
            SoundFxState = SoundState.Play;
        }
        public override void Render()
        {
            if (SoundFxState == SoundState.Play)
            {
                Raylib.PlaySound(SoundFx);
                SoundFxState = SoundState.Completed;
            }

        }
    }
    public enum SoundState
    {
        None,
        Play,
        Pause,
        Completed
    }
}
