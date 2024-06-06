using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;

namespace mmGameEngine
{
    /*
     * Text content to be displayed on game scene (attached to an entity)
     * if entity moves, then text will move with it (e.g. labeling your opponents)
     */
    public class Text : RenderComponent
    {
        //public Color FontColor = Color.Black;
        int _fontSize = 30;
        string _content;

        public TextInfo TextData;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                TextData.Content = value;
            }
        }
        public int FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                _fontSize = value;
                TextData.FontSize = value;
            }
        }
        public Text(string _text, TextFontTypes _fontType, int _size = 25)
        {
            _fontSize = _size;
            _content = _text;
            TextData = new TextInfo(_content, _fontType, _size, Color.Black);
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            if (OwnerEntity == null)
                return;
            if (!Enabled)
                return;
        }

        public override void Render()
        {
            if (OwnerEntity == null)
                return;
            if (!OwnerEntity.IsVisible)
                return;

            if (Global.DebugRenderEnabled)
                RenderDebug();

            TransformComponent Transform = OwnerEntity.Get<TransformComponent>();

            Raylib.DrawTextEx(TextData.TextFont, 
                              TextData.Content,
                              Transform.Position, 
                              (float)TextData.FontSize, 
                              0,
                              TextData.FontColor);

        }
        public void RenderDebug()
        {
            Vector2 size = Raylib.MeasureTextEx(TextData.TextFont, 
                                                TextData.Content, 
                                                TextData.FontSize, 
                                                -2);
            size.X += size.X * 0.45f ;
            size.Y += 10;

            TransformComponent Transform = OwnerEntity.Get<TransformComponent>();

            Rectangle rect = new Rectangle(Transform.Position.X, Transform.Position.Y, size.X, size.Y);
            //Vector2 orig = new Vector2(rect.Width * 0.5f, rect.Height * 0.5f);
            Vector2 orig = new Vector2(0,0);
            Raylib.DrawRectanglePro(rect, orig, Transform.Rotation, Color.Red);


        }
    }
}
