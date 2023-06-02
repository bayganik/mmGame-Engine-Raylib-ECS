using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;

namespace mmGameEngine
{
    /*
     * Label UI element.  Used for display of messages
     */
    public class Label : RenderComponent
    {

        public TextInfo TextData;

        Color CurrentTextColor;
        string content;
        
        public Label( string _content)
        {
            content = _content;
            TextData = new TextInfo(_content, TextFontTypes.Arial, 25, Color.WHITE);
            CurrentTextColor = TextData.FontColor;
            UIPosition = Vector2.Zero;
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            //
            // If component is attached to an Entity, the obey his position
            //
            if (OwnerEntity != null)
            {
                UIPosition = Transform.Position;
            }
        }
        public override void Render()
        {
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
            CurrentTextColor = TextData.FontColor;
            //
            // Draw the text
            //
            Raylib.DrawTextEx(TextData.TextFont,
                  TextData.Content,
                  UIPosition,
                  (float)TextData.FontSize,
                  0,
                  CurrentTextColor);
        }
    }
}
