using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;
using Entitas;

namespace mmGameEngine
{
    /*
     * Panel UI element is a container that holds other UI elements within it.  Moving
     * the Panel will move all of its elements with it.
     */
    public class Panel : RenderComponent
    {
        public Color BackgroundColor = Color.Gray;
        public Color BorderColor = Color.White;
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
            UIPosition = _position;
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            //
            // If component is attached to an Entity, the obey its position
            //
            if (OwnerEntity != null)
            {
                UIPosition = OwnerEntity.Get<TransformComponent>().Position;
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
                // UI is drawn according to entity
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
            //
            // Draw Rectangle filled + line around it
            //
            TransformComponent Transform = OwnerEntity.Get<TransformComponent>();

            Raylib.DrawRectangle((int)Transform.Position.X, (int)Transform.Position.Y,
                                 width, height, BackgroundColor);

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
        public void AddComponent(RenderComponent _uiElement, Vector2 _location)
        {
            _uiElement.UIPosition = _location;

            _uiElement.UIPosition = new Vector2(_uiElement.UIPosition.X + UIPosition.X, _uiElement.UIPosition.Y + UIPosition.Y);
            PanelComponents.Add(_uiElement);
        }
    }
}
