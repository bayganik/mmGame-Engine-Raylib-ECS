using Entitas;
using mmGameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raylib_cs;
using Sanford.Multimedia.Midi;
using System.Security.Cryptography;

namespace TestmmGame
{
    /*
     * Key values 48 - 57 represent 0 - 9 for Octive selection
     * Key values 65 - 90 represent A - Z for active piano key
     */
    public class PianoOctiveSystem : IExecuteSystem
    {
        //
        // Our piano starts with 1st octave A = 22
        //
        int CurrentC_Note = 60;                 //mid C octave
        int NotePlaying = 0;                    //current note being played
        int Velocity = 127;                     //how hard we play key
        int maxKeysInOct = 12;
        bool octOnly = false;
        bool noteOff = true;
        PianoScene ActiveScene;
        Sprite spr;
        public void Execute()
        {
            ActiveScene = (PianoScene)Global.CurrentScene;

            int keyval = Raylib.GetKeyPressed();
            //
            // 1 - 7 are the octave on keyboard
            //
            octOnly = false;
            switch (keyval)
            {
                case 48:                                    //0
                    ActiveScene.CurrentOctave = 1;
                    octOnly = true;
                    break;
                case 49:                                    //1
                    ActiveScene.CurrentOctave = 1;
                    octOnly = true;
                    break;
                case 50:                                    //2
                    ActiveScene.CurrentOctave = 2;
                    octOnly = true;
                    break;
                case 51:                                    //3
                    ActiveScene.CurrentOctave = 3;
                    octOnly = true;
                    break;
                case 52:                                    //middle C octave
                    ActiveScene.CurrentOctave = 4;
                    octOnly = true;
                    break;
                case 53:
                    ActiveScene.CurrentOctave = 5;          //5
                    octOnly = true;
                    break;
                case 54:
                    ActiveScene.CurrentOctave = 6;          //6
                    octOnly = true;
                    break;
                case 55:                                    //7
                    ActiveScene.CurrentOctave = 7;
                    octOnly = true;
                    break;
            }
            //----------------------------------------------------
            // Octave changes (1-7), leave till next frame 
            //----------------------------------------------------
            if (octOnly)
                return;
            //----------------------------------------------------
            // Key pressed (Left Hand)
            //----------------------------------------------------
            CurrentC_Note = ((ActiveScene.CurrentOctave - 1) * maxKeysInOct) + 24;

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_A))             //C
            {
                NotePlaying = CurrentC_Note;
                ActiveScene.MidiNoteKey(NotePlaying);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.KEY_S))        //D
            {
                NotePlaying = CurrentC_Note + 2;
                ActiveScene.MidiNoteKey(NotePlaying);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.KEY_D))        //E
            {
                NotePlaying = CurrentC_Note + 4;
                ActiveScene.MidiNoteKey(NotePlaying);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.KEY_F))        //F
            {
                NotePlaying = CurrentC_Note + 5;
                ActiveScene.MidiNoteKey(NotePlaying);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.KEY_W))        //C#
            {
                NotePlaying = CurrentC_Note + 1;
                ActiveScene.MidiNoteKey(NotePlaying);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.KEY_E))        //D#
            {
                NotePlaying = CurrentC_Note + 3;
                ActiveScene.MidiNoteKey(NotePlaying);
            }
            //
            // Key is released (Left hand)
            //
            if (Raylib.IsKeyReleased(KeyboardKey.KEY_A))
            {
                ActiveScene.MidiNoteKey(NotePlaying, noteOff);              
                NotePlaying = 0;
            }
            else if (Raylib.IsKeyReleased(KeyboardKey.KEY_S))   
            {
                ActiveScene.MidiNoteKey(NotePlaying, noteOff);
                NotePlaying = 0;
            }
            else if (Raylib.IsKeyReleased(KeyboardKey.KEY_D))  
            {
                ActiveScene.MidiNoteKey(NotePlaying, noteOff);
                NotePlaying = 0;
            }
            else if (Raylib.IsKeyReleased(KeyboardKey.KEY_F))  
            {
                ActiveScene.MidiNoteKey(NotePlaying, noteOff);
                NotePlaying = 0;
            }
            else if (Raylib.IsKeyReleased(KeyboardKey.KEY_W))  
            {
                ActiveScene.MidiNoteKey(NotePlaying, noteOff);
                NotePlaying = 0;
            }
            else if (Raylib.IsKeyReleased(KeyboardKey.KEY_E))
            {
                ActiveScene.MidiNoteKey(NotePlaying, noteOff);
                NotePlaying = 0;
            }
            //----------------------------------------------------
            // Key Pressed (Right hand)
            //----------------------------------------------------
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_J))             //G
            {
                NotePlaying = CurrentC_Note + 7;
                ActiveScene.MidiNoteKey(NotePlaying);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.KEY_K))        //A
            {
                NotePlaying = CurrentC_Note + 9;
                ActiveScene.MidiNoteKey(NotePlaying);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.KEY_L))        //B
            {
                NotePlaying = CurrentC_Note + 11;
                ActiveScene.MidiNoteKey(NotePlaying);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.KEY_SEMICOLON)) //C (next octave)
            {
                NotePlaying = CurrentC_Note + 12;
                ActiveScene.MidiNoteKey(NotePlaying);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.KEY_U))        //F#
            {
                NotePlaying = CurrentC_Note + 6;
                ActiveScene.MidiNoteKey(NotePlaying);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.KEY_I))        //G#
            {
                NotePlaying = CurrentC_Note + 8;
                ActiveScene.MidiNoteKey(NotePlaying);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.KEY_O))        //A#
            {
                NotePlaying = CurrentC_Note + 10;
                ActiveScene.MidiNoteKey(NotePlaying);
            }
            //
            // Key is released (Right hand)
            //
            if (Raylib.IsKeyReleased(KeyboardKey.KEY_J))             //G
            {
                ActiveScene.MidiNoteKey(NotePlaying, noteOff);
                NotePlaying = 0;
            }
            else if (Raylib.IsKeyReleased(KeyboardKey.KEY_K))        //A
            {
                ActiveScene.MidiNoteKey(NotePlaying, noteOff);
                NotePlaying = 0;
            }
            else if (Raylib.IsKeyReleased(KeyboardKey.KEY_L))        //B
            {
                ActiveScene.MidiNoteKey(NotePlaying, noteOff);
                NotePlaying = 0;
            }
            else if (Raylib.IsKeyReleased(KeyboardKey.KEY_SEMICOLON)) //C (next octave)
            {
                ActiveScene.MidiNoteKey(NotePlaying, noteOff);
                NotePlaying = 0;
            }
            else if (Raylib.IsKeyReleased(KeyboardKey.KEY_U))        //F#
            {
                ActiveScene.MidiNoteKey(NotePlaying, noteOff);
                NotePlaying = 0;
            }
            else if (Raylib.IsKeyReleased(KeyboardKey.KEY_I))        //G#
            {
                ActiveScene.MidiNoteKey(NotePlaying, noteOff);
                NotePlaying = 0;
            }
            else if (Raylib.IsKeyReleased(KeyboardKey.KEY_O))        //A#
            {
                ActiveScene.MidiNoteKey(NotePlaying, noteOff);
                NotePlaying = 0;
            }
            //---------------------------------
            // Color the current Octave
            //---------------------------------
            ColorOctave();
        }
        private void ColorOctave()
        {
            //
            // Find being note of current octave (C note)
            //
            CurrentC_Note = ((ActiveScene.CurrentOctave - 1) * maxKeysInOct) + 24;
            //
            // Find all keyboard entities and label entities to go with them
            //
            var entities = Context<Default>.AllOf<PianoNoteComponent>().GetEntities();
            var entLabels = Context<Default>.AllOf< PianoNoteLabelComponent >().GetEntities();
            //
            // set all labels to NOT visible
            //
            foreach (Entity labEnt in entLabels)
                labEnt.IsVisible = false;
            //
            // find min/max note value in this octave
            //
            int minNote = CurrentC_Note;
            int maxNote = CurrentC_Note + maxKeysInOct;
            //
            // loop thru all piano keys and set the proper color
            //
            foreach (var entity in entities)
            {
                //
                // color Blue for the note being played
                //
                if (entity.Tag == NotePlaying)
                {
                    spr = entity.Get<Sprite>();
                    spr.DrawColor = Color.BLUE;
                }
                else
                {
                    //
                    // Yellow out current octave
                    //
                    spr = entity.GetComponent<Sprite>();                 //sprite component of the key
                    if (entity.Tag >= minNote && entity.Tag <= maxNote)
                    {
                        //
                        // turn label of key to visible
                        //
                        foreach(Entity labEnt in entLabels)
                        {
                            if (labEnt.Name == "plbl" + entity.Tag.ToString())
                                labEnt.IsVisible = true;
                        }
                        spr.DrawColor = Color.YELLOW;           //key in octave
                    } 
                    else
                        spr.DrawColor = Color.WHITE;            //all keys turn white
                }
            }
        }
    }
}