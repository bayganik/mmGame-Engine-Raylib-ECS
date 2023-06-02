using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using System.Reflection;

using mmGameEngine;
using Transform = mmGameEngine.TransformComponent;

using Entitas;
using System.IO;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia;


namespace TestmmGame
{
    /*
     * MIDI file has many tracks
     * MIDI file has 16 channels
     * Each track plays one instrument over a single channel
     * Each channel has a Program Message (instrument)
     * Channel 9 is used for drums
     * It is also possible to have more than one track using the same MIDI channel. 
     *   For example, say you want to record a piano part for your composition. 
     *   You may decide to have the right hand part on one track and 
     *   the left hand on another.
     * 
     * This program ASSUMES each track has ONE channel
     * 
     */
    public class PianoScene : Scene
    {
        public int CurrentOctave = 4;           //Middle C octave
        //
        // Part of scene collection that we access in TestmmGame
        //
        Assembly assmbly;
        string assName;             //full assembly name
        string[] SceneNames;        //Scenes we want to invoke
        int sampleRate = 48000;
        int bufferSize = 2048;
        //Synthesizer synthesizer;
        AudioStream stream;
        Entity entPanel;
        Vector2 position;
        //
        // Midi device
        //
        public OutputDevice MidiOutDevice;
        public int Velocity = 127;
        int outDeviceID = 0;
        int channel = 0;
        public PianoScene()
        {
            Global.SceneHeight = 600;
            Global.SceneWidth = 1200;
            Global.SceneTitle = "Piano keys Scene";
            Global.SceneClearColor = Color.BLANK;
            SceneNames = new string[4]
                {"PlayScene", "SplashScene","CardScene",""};

            assName = (string)System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            assmbly = Assembly.Load(assName);

            //
            // Default output device (usually 0)
            //
            MidiOutDevice = new OutputDevice(outDeviceID);
        }
        public unsafe override void Play()
        {
            Global.CurrentScene = this;
            Global.DebugRenderEnabled = false;
            //
            // Contents
            //
            Content.LoadMenu();
            //------------------------------
            // Scrolling background
            //------------------------------
            Entity backG = Global.CreateGameEntity(new Vector2(0, 0));
            backG.Name = "Scrolling";
            ScrollingImage si = new ScrollingImage(Content.backGround);
            si.ScrollSpeedX = 10;
            si.FitWindow= true;

            backG.Add(si);
            backG.Get<Transform>().Visiable = true;

            //-----------------------------
            // Panel (menu type)
            //-----------------------------
            Content.LoadPiano();
            var pi85 = new PianoEntity(new Vector2(40, 200));

            AddSystem(new PianoOctiveSystem());
        }
        public void MidiNoteKey(int _note, bool _noteOff = false )
        {
            if (_noteOff)
                //MidiOutDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, channel, _note, Velocity));
            {
                for (int i = 24; i < 99; i++)
                    MidiOutDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, channel, i, Velocity));
            }
            else
                MidiOutDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, channel, _note, Velocity));

        }
        public void ActionButton(object btn)
        {
            PianoButton bt = (PianoButton)btn;
            //synthesizer.NoteOffAll(0, false);
            //synthesizer.NoteOn(0, bt.Tag, 100);
        }
    }

}
