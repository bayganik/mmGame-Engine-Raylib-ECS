﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;
using System.Linq;

namespace mmGameEngine
{
    /*
     * Message Box (like a Panel) is a special UI element.  Used to give info to user and wait until "OK" button is pushed.
     */
    public class MsgBox : RenderComponent
    {
        public Color BackgroundColor = Color.GRAY;
        public Color BorderColor = Color.WHITE;
        public int BorderThickness = 4;
        public List<RenderComponent> PanelComponents = new List<RenderComponent>();

        public int Width;
        public int Height;
        //string content;
        
        public MsgBox(Vector2 _position, int _width, int _height, Color _backgroundColor)
        {
            Width = _width;
            Height = _height;
            BackgroundColor = _backgroundColor;
            CompPosition = _position;
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            //
            // This componenet obeys its CompPosition given
            //
            // If component is attached to an Entity, the obey his position
            //
            //if (CompEntity != null)
            //{
            //    CompPosition = Transform.Position;
            //}
            //
            // Panel will call component Update method
            //
            foreach (RenderComponent control in PanelComponents)
            {
                control.Update(deltaTime);
            }
        }
        public override void Render()
        {
            base.Render();
            if (!base.Visiable)
                return;
            //
            // Draw Rectangle filled + line around it
            //
            //Raylib.DrawRectangle((int)Transform.Position.X, (int)Transform.Position.Y,
            //                     Width, Height, BackgroundColor);
            ////Raylib.DrawRectangleLines((int)Transform.Position.X, (int)Transform.Position.Y,
            ////                     width, height, BorderColor);
            //Raylib.DrawRectangleLinesEx(new Rectangle((int)Transform.Position.X, (int)Transform.Position.Y,
            //                            Width, Height), BorderThickness, BorderColor);

            Raylib.DrawRectangle((int)CompPosition.X, (int)CompPosition.Y,
                     Width, Height, BackgroundColor);
            //Raylib.DrawRectangleLines((int)Transform.Position.X, (int)Transform.Position.Y,
            //                     width, height, BorderColor);
            Raylib.DrawRectangleLinesEx(new Rectangle((int)CompPosition.X, (int)CompPosition.Y,
                                        Width, Height), BorderThickness, BorderColor);
            //
            // Scene will call this Render method
            //
            foreach (RenderComponent control in PanelComponents)
            {
                control.Render();
            }

        }
        public void AddButton(Button ok)
        {
            Vector2 pos = new Vector2((Width / 2) - 20, Height - 45);
            ok.CompPosition = new Vector2(ok.CompPosition.X + CompPosition.X, ok.CompPosition.Y + CompPosition.Y);

            PanelComponents.Add(ok);
        }
        public void AddMsg(Label lbl)
        {

            lbl.CompPosition = new Vector2(lbl.CompPosition.X + CompPosition.X, lbl.CompPosition.Y + CompPosition.Y);

            PanelComponents.Add(lbl);
        }
    }
}
