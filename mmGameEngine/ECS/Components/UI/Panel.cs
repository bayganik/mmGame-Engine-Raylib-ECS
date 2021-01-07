using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;
using System.Linq;

namespace mmGameEngine
{
    /*
     * Panel UI element is a container that holds other UI elements within it.  Moving
     * the Panel will move all of its elements with it.
     */
    public class Panel : RenderComponent
    {
        public Color BackgroundColor = Color.GRAY;
        public Color BorderColor = Color.WHITE;
        public int BorderThickness = 4;
        public List<RenderComponent> PanelComponents = new List<RenderComponent>();

        public TextInfo TextData;

        int width;
        int height;
        //string content;
        
        public Panel(Vector2 _position, int _width, int _height, Color _backgroundColor)
        {
            width = _width;
            height = _height;
            BackgroundColor = _backgroundColor;
            CompPosition = _position;
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            //
            // If component is attached to an Entity, the obey its position
            //
            if (CompEntity != null)
            {
                CompPosition = Transform.Position;
            }
            //
            // Panel will call component Update method
            //
            foreach (RenderComponent control in PanelComponents)
            {
                control.Update(deltaTime);
            }

            if (!Transform.Enabled)
                return;

        }
        public override void Render()
        {
            base.Render();
            if (!base.Visiable)
                return;
            //
            // Draw Rectangle filled + line around it
            //
            Raylib.DrawRectangle((int)Transform.Position.X, (int)Transform.Position.Y,
                                 width, height, BackgroundColor);
            //Raylib.DrawRectangleLines((int)Transform.Position.X, (int)Transform.Position.Y,
            //                     width, height, BorderColor);
            Raylib.DrawRectangleLinesEx(new Rectangle((int)Transform.Position.X, (int)Transform.Position.Y,
                                        width, height), BorderThickness, BorderColor);
            //
            // Scene will call this Render method
            //
            foreach (RenderComponent control in PanelComponents)
            {
                control.Render();
            }

        }
        public void AddComponent(RenderComponent _uiElement)
        {
            // convert relative position to absolute
            _uiElement.CompPosition = new Vector2(_uiElement.CompPosition.X + CompPosition.X, _uiElement.CompPosition.Y + CompPosition.Y);
            PanelComponents.Add(_uiElement);
        }

    }
}
