using Entitas;
using mmGameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raylib_cs;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia;

namespace TestmmGame
{
    /*
     * Key values 48 - 57 represent 0 - 9 for Octive selection
     * Key values 65 - 90 represent A - Z for active piano key
     */
    public class PianoClickSystem : IExecuteSystem
    {
        //
        // Our piano starts with 1st octave A = 22
        //
        int CurrentC_Note = 60;                 //mid C octave
        int Velocity = 127;                     //how hard we play key
        int maxKeysInOct = 12;
        bool octOnly = false;
        PianoScene ActiveScene;
        public void Execute()
        {
            ActiveScene = (PianoScene)Global.CurrentScene;
            //
            // Find being note of current octave (C note)
            //
            CurrentC_Note = ((ActiveScene.CurrentOctave - 1) * maxKeysInOct) + 24;

            var val = Raylib.GetKeyPressed();
            if ((val > 90) || (val < 65))
                return;

            switch (val)
            {
                case 65:                                    //A
                    CurrentC_Note = (0 * maxKeysInOct) + 22;
                    ActiveScene.MidiOutDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, CurrentC_Note, Velocity));
                    octOnly = true;
                    break;
                case 66:                                    //B
                    CurrentC_Note = (1 * maxKeysInOct) + 22;
                    octOnly = true;
                    break;
                case 51:                                    //3
                    CurrentC_Note = (2 * maxKeysInOct) + 22;
                    octOnly = true;
                    break;
                case 52:                                    //middle C octave
                    CurrentC_Note = (3 * maxKeysInOct) + 22;
                    octOnly = true;
                    break;
                case 53:
                    CurrentC_Note = (4 * maxKeysInOct) + 22;
                    octOnly = true;
                    break;
                case 54:
                    CurrentC_Note = (5 * maxKeysInOct) + 22;
                    octOnly = true;
                    break;
                case 55:                                    //7
                    CurrentC_Note = (6 * maxKeysInOct) + 22;
                    octOnly = true;
                    break;
            }
            // 
            // Change key colors to yellow on screen
            //
            var entities = Context<Default>.AllOf<PianoNoteComponent>().GetEntities();
            int minNote = CurrentC_Note;
            int maxNote = CurrentC_Note + maxKeysInOct;
            foreach (var entity in entities)
            {
                Sprite spr = entity.Get<Sprite>();
                if (entity.Tag == minNote)
                    spr.DrawColor = Color.Blue;

            }
        }
    }
}