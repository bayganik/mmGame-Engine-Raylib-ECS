using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;
using mmGameEngine;

using MeltySynth;
//using Microsoft.Toolkit.HighPerformance.Buffers;
using System.IO;

namespace TestmmGame
{
    /*
     * Button UI element.  Mouse movements don't take camera into account.
     * Assumption is all UI elements are drawn AFTER camera draw is done
     * 
     * Button can be used as a component of an entity, in which case it will
     *      use the position dictated by Transform component
     * Button can be 'Added' to MsgBox or Panel and not have an entity associated
     *      with it. It will then use UIPosition of RenderComponent and not any
     *      Entity.
     */
    public class PianoButton : RenderComponent
    {
        public Color TextColor = Color.RayWhite;
        public Color BackgroundColor = Color.White;
        public Color BorderColor = Color.Gray;
        public TextInfo TextData;
        public bool HasBorder;
        public bool HasImage;

        Color CurrentBackgroundColor;
        Color CurrentTextColor;
        //
        // fine tune values for content position adjustmet
        //
        public int XOffset = 0;
        public int YOffset = 0;

        Texture2D _image;
        //
        // synth values
        //
        public KeyboardKey AssignedKey;
        public Synthesizer synthesizer;
        public AudioStream stream;
        short[] buffer = new short[2 * 2048];

        int velocity = 100;

        Vector2 textPosition;
        int width;
        int height;
        string content;
        public Texture2D Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                HasImage = true;
                HasBorder = false;
                BackgroundColor = Color.White;
            }
        }
        public PianoButton(int _width = 40, int _height = 200)
        {
            width = _width;
            height = _height;
            content = "";
            TextData = new TextInfo(content, TextFontTypes.Arial, 23, TextColor);
            CurrentBackgroundColor = BackgroundColor;
            CurrentTextColor = TextColor;
            UIPosition = Vector2.Zero;
            HasImage = false;
            XOffset = 0;
            YOffset = 0;
            HasBorder = true;

            //Image = Global.ButtonImage;

        }
        public PianoButton(Synthesizer _synth, int _width = 40, int _height = 200)
        {

            width = _width;
            height = _height;
            content = "";
            TextData = new TextInfo(content, TextFontTypes.Arial, 23, TextColor);
            CurrentBackgroundColor = BackgroundColor;
            CurrentTextColor = TextColor;
            UIPosition = Vector2.Zero;
            HasImage = false;
            XOffset = 0;
            YOffset = 0;
            HasBorder = true;
            synthesizer = _synth;
            //Image = Global.ButtonImage;

        }
        public unsafe override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            TransformComponent Transform = OwnerEntity.Get<TransformComponent>();
            //
            // If component is attached to an Entity, then obey his position
            //
            if (OwnerEntity != null)
            {
                UIPosition = Transform.Position;
            }

            //textPosition = UIPosition;

            //Vector2 size = Raylib.MeasureTextEx(TextData.TextFont,
            //                        TextData.Content,
            //                        TextData.FontSize,
            //                        0);

            //textPosition.X += (size.X / 2) + XOffset;
            //textPosition.Y += YOffset;

            ////if (!Transform.Enabled)
            ////    return;
            //if (Raylib.IsAudioStreamProcessed(stream))
            //{
            //    synthesizer.RenderInterleavedInt16(buffer);
            //    fixed (void* p = buffer)
            //    {
            //        Raylib.UpdateAudioStream(stream, p, 2048);
            //    }
            //}
            TestMouseOver();
            TestKeyBoard();
        }
        public override void Render()
        {
            base.Render();
            TransformComponent Transform = OwnerEntity.Get<TransformComponent>();
            if (OwnerEntity != null)
            {
                if (!OwnerEntity.IsVisible)
                    return;
                if (OwnerEntity.Get<TransformComponent>().Parent != null)
                    if (!OwnerEntity.Get<TransformComponent>().Parent.OwnerEntity.IsVisible)
                        return;
                //
                // UI is drawn according to entity
                //
                UIPosition = Transform.Position;
            }
            else
            {
                //
                // UI is drawn according to component 
                //
                if (!ComponentVisiable)
                    return;

            }

            if (HasImage)
            {
                //
                // Draw image
                //
                Raylib.DrawTexture(Image, (int)UIPosition.X, (int)UIPosition.Y, CurrentBackgroundColor);
            }
            else
            {
                //
                // Draw Rectangle filled with background color
                //
                Raylib.DrawRectangle((int)UIPosition.X, (int)UIPosition.Y,
                                     width, height, CurrentBackgroundColor);
            }

            if (HasBorder)
            {
                Raylib.DrawRectangleLines((int)UIPosition.X, (int)UIPosition.Y,
                                     width, height, BorderColor);
            }
            //
            // Draw the text
            //
            if (!string.IsNullOrEmpty(TextData.Content))
            {
                Raylib.DrawTextEx(TextData.TextFont,
                      TextData.Content,
                      textPosition,
                      (float)TextData.FontSize,
                      0,
                      CurrentTextColor);
            }
        }
        public void TestMouseOver()
        {
            if (!Enabled)
                return;

            if (HitTest(new Vector2(Raylib.GetMouseX(), Raylib.GetMouseY())))
            {
                CurrentBackgroundColor = Color.LightGray;
                CurrentTextColor = Color.Black;

                //
                // Test the last key for Left Mouse button
                //
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    synthesizer.NoteOffAll(0, false);
                    synthesizer.NoteOn(0, Tag, velocity);
                    //base.OnClick(this);              //invoike the click delegate
                }
            }
            else
            {
                CurrentBackgroundColor = BackgroundColor;
                CurrentTextColor = TextColor;
            }

        }
        public void TestKeyBoard()
        {
            //
            // if the key assigned to this button is pressed
            //
            if (Raylib.IsKeyPressed(AssignedKey))
            {
                BackgroundColor = Color.Gray;
                //synthesizer.NoteOffAll(0, false);
                //synthesizer.NoteOn(0, Tag, velocity);
            }
        }
        public virtual bool HitTest(Vector2 point)
        {
            if (point.X < UIPosition.X) { return false; }
            if (point.X >= UIPosition.X + width) { return false; }
            if (point.Y < UIPosition.Y) { return false; }
            if (point.Y >= UIPosition.Y + height) { return false; }

            return true;
        }
    }
}
