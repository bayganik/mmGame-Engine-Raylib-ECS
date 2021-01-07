using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;

namespace mmGameEngine
{
    /*
     * Button UI element.  Mouse movements don't take camera into account.
     * Assumption is all UI elements are drawn AFTER camera draw is done
     */
    public class Button : RenderComponent
    {
        public Color TextColor = Color.WHITE;
        public Color BackgroundColor = Color.GRAY;
        public Color BorderColor = Color.WHITE;
        public TextInfo TextData;
        public bool HasBorder;

        Color CurrentBackgroundColor;
        Color CurrentTextColor;
        //
        // fine tune values for content position adjustmet
        //
        public int XOffset = 0;
        public int YOffset = 0;

        Texture2D _image;
        bool hasImage;


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
                hasImage = true;
                HasBorder = false;
                BackgroundColor = Color.WHITE;
            }
        }
        public Button(Vector2 _position, int _width, int _height, string _content = "", int _xoffset = 0, int _yoffset = 0)
        {

            width = _width;
            height = _height;
            content = _content;
            TextData = new TextInfo(_content, TextFontTypes.Arial, 23, Color.WHITE);
            CurrentBackgroundColor = BackgroundColor;
            CurrentTextColor = TextColor;
            CompPosition = _position;
            hasImage = false;
            XOffset = _xoffset;
            YOffset = _yoffset;
            HasBorder = true;
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            //
            // If component is attached to an Entity, the obey his position
            //
            if (CompEntity != null)
            {
                CompPosition = Transform.Position;
            }

            textPosition = CompPosition;

            Vector2 size = Raylib.MeasureTextEx(TextData.TextFont,
                                    TextData.Content,
                                    TextData.FontSize,
                                    0);
            textPosition.X += (size.X / 2) + XOffset;
            textPosition.Y += YOffset;

            if (!Transform.Enabled)
                return;

            TestMouseOver();
        }
        public override void Render()
        {
            base.Render();
            if (!base.Visiable)
                return;

            if (hasImage)
            {
                //
                // Draw image
                //
                Raylib.DrawTexture(Image, (int)CompPosition.X, (int)CompPosition.Y, CurrentBackgroundColor);
            }
            else
            {
                //
                // Draw Rectangle filled with background color
                //
                Raylib.DrawRectangle((int)CompPosition.X, (int)CompPosition.Y,
                                     width, height, CurrentBackgroundColor);
            }

            if (HasBorder)
            {
                Raylib.DrawRectangleLines((int)CompPosition.X, (int)CompPosition.Y,
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
                CurrentBackgroundColor = Color.LIGHTGRAY;
                CurrentTextColor = Color.BLACK;

                //
                // Test the last key for Left Mouse button
                //
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                {
                    base.OnClick(this);              //invoike the click delegate
                }
            }
            else
            {
                CurrentBackgroundColor = BackgroundColor;
                CurrentTextColor = TextColor;
            }

        }
        public virtual bool HitTest(Vector2 point)
        {
            if (point.X < CompPosition.X) { return false; }
            if (point.X >= CompPosition.X + width) { return false; }
            if (point.Y < CompPosition.Y) { return false; }
            if (point.Y >= CompPosition.Y + height) { return false; }

            return true;
        }
    }
}
