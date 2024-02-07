using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using System.Reflection;

using mmGameEngine;
using Transform = mmGameEngine.TransformComponent;

using Entitas;
using MeltySynth;
//using Microsoft.Toolkit.HighPerformance.Buffers;
using System.IO;
//using System.Reflection.Emit;


namespace TestmmGame
{
    public class PianoMeltyScene : Scene
    {
        //
        // Part of scene collection that we access in TestmmGame
        //
        Assembly assmbly;
        string assName;             //full assembly name
        string[] SceneNames;        //Scenes we want to invoke
        int sampleRate = 48000;
        int bufferSize = 2048;
        Synthesizer synthesizer;
        AudioStream stream;
        Entity entPanel;
        Vector2 position;
        public PianoMeltyScene()
        {
            Global.SceneHeight = 600;
            Global.SceneWidth = 1200;
            Global.SceneTitle = "Piano keys Scene";
            Global.SceneClearColor = Color.Blank;
            SceneNames = new string[4]
                {"PlayScene", "SplashScene","CardScene",""};

            assName = (string)System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            assmbly = Assembly.Load(assName);

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
            //textureImage = Raylib.LoadTexture("Assets/Img/background.png");
            ScrollingImage si = new ScrollingImage(Content.backGround);
            si.ScrollSpeedX = 10;
            si.FitWindow= true;

            backG.Add(si);
            backG.Get<Transform>().Visiable = true;
            //-----------------------------
            // Panel (menu type)
            //-----------------------------
            entPanel = Global.CreateSceneEntity(new Vector2(150, 150));                //position of panel
            entPanel.Name = "panel";
            position = entPanel.Get<Transform>().Position;

            Panel menuPanel = new Panel(position, 600, 300, Color.Blank);
            entPanel.Add(menuPanel);

            //-----------------
            // label on top
            //-----------------
            Label lbl = new Label("This is a Piano");
            menuPanel.AddComponent(lbl, new Vector2(80, 10));

            Raylib.SetAudioStreamBufferSizeDefault(bufferSize);

            stream = Raylib.LoadAudioStream((uint)sampleRate, 16, 2);
            var buffer = new short[2 * bufferSize];

            Raylib.PlayAudioStream(stream);
            synthesizer = new Synthesizer(@"Assets\TimGM6mb.sf2", sampleRate);
            //--------------------------------------------------------
            // play button (position relative to the panel)
            //--------------------------------------------------------
            PianoButton playBt = new PianoButton(synthesizer);
            playBt.Tag = 58;
            playBt.stream = stream;
            playBt.AssignedKey = KeyboardKey.A;
            menuPanel.AddComponent(playBt, new Vector2(110, 40));

            playBt = new PianoButton(synthesizer);
            playBt.Tag = 59;
            playBt.stream = stream;
            playBt.AssignedKey = KeyboardKey.S;
            menuPanel.AddComponent(playBt, new Vector2(150, 40));

            playBt = new PianoButton(synthesizer);
            playBt.Tag = 60;                //middle c
            playBt.stream = stream;
            playBt.AssignedKey = KeyboardKey.D;
            menuPanel.AddComponent(playBt, new Vector2(190, 40));

            playBt = new PianoButton(synthesizer);
            playBt.Tag = 61;
            playBt.stream = stream;
            playBt.AssignedKey = KeyboardKey.F;
            menuPanel.AddComponent(playBt, new Vector2(230, 40));
            playBt = new PianoButton(synthesizer);
            playBt.Tag = 62;
            playBt.stream = stream;
            playBt.AssignedKey = KeyboardKey.J;
            menuPanel.AddComponent(playBt, new Vector2(270, 40));
            playBt = new PianoButton(synthesizer);
            playBt.Tag = 63;
            playBt.stream = stream;
            playBt.AssignedKey = KeyboardKey.K;
            menuPanel.AddComponent(playBt, new Vector2(310, 40));
            playBt = new PianoButton(synthesizer);
            playBt.Tag = 64;
            playBt.stream = stream;
            playBt.AssignedKey = KeyboardKey.L;
            menuPanel.AddComponent(playBt, new Vector2(350, 40));


        }
        short[] buffer = new short[2 * 2048];
        public unsafe override void LateUpdate()
        {
            if (Raylib.IsAudioStreamProcessed(stream))
            {
                synthesizer.RenderInterleavedInt16(buffer);
                fixed (void* p = buffer)
                {
                    Raylib.UpdateAudioStream(stream, p, 2048);
                }
            }
        }
        public void ExitButton(object btn)
        {
            ForceEndScene = true;
        }
        public void ActionButton(object btn)
        {
            PianoButton bt = (PianoButton)btn;
            //synthesizer.NoteOffAll(0, false);
            //synthesizer.NoteOn(0, bt.Tag, 100);
        }
    }

}
