using System;
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
        public Color BackgroundColor = Color.Gray;
        public Color BorderColor = Color.White;
        public int BorderThickness = 4;
        public List<RenderComponent> PanelComponents = new List<RenderComponent>();

        public Button MsgButton;
        public Label MsgLabel;

        public int Width;
        public int Height;

        bool FirstTime = true;
        //string content;
        
        public MsgBox(int _width, int _height, Color _backgroundColor)
        {
            Width = _width;
            Height = _height;
            BackgroundColor = _backgroundColor;
            UIPosition = Vector2.Zero;

            //-----------------
            // label as Msg on top
            //-----------------
            MsgLabel = new Label("This is a Menu");
            MsgButton = new Button(35, 35, "OK", 7,7);

        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            //
            // Set UIPosition so we can add other components 
            //
            if (FirstTime)
            {
                UIPosition = OwnerEntity.Get<TransformComponent>().Position;
                AddMsg(MsgLabel, new Vector2(10, 10));
                AddButton(MsgButton, new Vector2((Width / 2) - MsgButton.Width, Height - MsgButton.Height - 10));
                FirstTime = false;
            }
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
            if (OwnerEntity != null)
            {
                if (!OwnerEntity.IsVisible)
                    return;
                if (OwnerEntity.Get<TransformComponent>().Parent != null)
                    if (!OwnerEntity.Get<TransformComponent>().Parent.OwnerEntity.IsVisible)
                        return;
                //
                // UI is drawn according to entity its attached to
                //
                UIPosition = OwnerEntity.Get<TransformComponent>().Position;
            }
            else
            {
                //
                // UI is drawn according to component 
                //
                if (!ComponentVisiable)
                    return;

            }
            Raylib.DrawRectangle((int)UIPosition.X, (int)UIPosition.Y,
                     Width, Height, BackgroundColor);

            Raylib.DrawRectangleLinesEx(new Rectangle((int)UIPosition.X, (int)UIPosition.Y,
                                        Width, Height), BorderThickness, BorderColor);
            //
            // Scene will call this Render method
            //
            foreach (RenderComponent control in PanelComponents)
            {
                control.Render();
            }

        }
        internal void AddButton(Button ok, Vector2 _location)
        {
            ok.UIPosition = _location;
            //Vector2 pos = new Vector2((Width / 2) - 20, Height - 45);
            ok.UIPosition = new Vector2(ok.UIPosition.X + UIPosition.X, ok.UIPosition.Y + UIPosition.Y);

            PanelComponents.Add(ok);
        }
        internal void AddMsg(Label lbl, Vector2 _location)
        {
            lbl.UIPosition = _location;
            lbl.UIPosition = new Vector2(lbl.UIPosition.X + UIPosition.X, lbl.UIPosition.Y + UIPosition.Y);

            PanelComponents.Add(lbl);
        }
    }
}
