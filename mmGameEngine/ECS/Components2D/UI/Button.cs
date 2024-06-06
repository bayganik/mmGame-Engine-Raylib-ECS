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
     * 
     * Button can be used as a component of an entity, in which case it will
     *      use the position dictated by Transform component
     * Button can be 'Added' to MsgBox or Panel and not have an entity associated
     *      with it. It will then use UIPosition of RenderComponent and not any
     *      Entity.
     */
    public class Button : RenderComponent
    {
        public Color TextColor = Color.RayWhite;
        public Color BackgroundColor = Color.Gray;
        public Color BorderColor = Color.White;
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



        Vector2 textPosition;
        public int Width;
        public int Height;
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
        public Button(int _width, int _height, string _content = "", int _xoffset = 0, int _yoffset = 0)
        {

            Width = _width;
            Height = _height;
            content = _content;
            TextData = new TextInfo(_content, TextFontTypes.Arial, 23, TextColor);
            CurrentBackgroundColor = BackgroundColor;
            CurrentTextColor = TextColor;
            UIPosition = Vector2.Zero;
            HasImage = false;
            XOffset = _xoffset;
            YOffset = _yoffset;
            HasBorder = true;
            Image = Global.ButtonImage;
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            //
            // If component is attached to an Entity, the obey his position
            //
            if (OwnerEntity != null)
            {
                UIPosition = OwnerEntity.Get<TransformComponent>().Position;
            }

            textPosition = UIPosition;

            Vector2 size = Raylib.MeasureTextEx(TextData.TextFont,
                                    TextData.Content,
                                    TextData.FontSize,
                                    0);

            textPosition.X += (size.X / 2) + XOffset;
            textPosition.Y += YOffset;

            //if (!Transform.Enabled)
            //    return;

            TestMouseOver();
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
                                     Width, Height, CurrentBackgroundColor);
            }

            if (HasBorder)
            {
                Raylib.DrawRectangleLines((int)UIPosition.X, (int)UIPosition.Y,
                                     Width, Height, BorderColor);
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

            if (HitTest(Global.WorldPosition(new Vector2(Raylib.GetMouseX(), Raylib.GetMouseY()))))
            {
                CurrentBackgroundColor = Color.LightGray;
                CurrentTextColor = Color.Black;

                //
                // Test the last key for Left Mouse button
                //
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
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
            if (point.X < UIPosition.X) { return false; }
            if (point.X >= UIPosition.X + Width) { return false; }
            if (point.Y < UIPosition.Y) { return false; }
            if (point.Y >= UIPosition.Y + Height) { return false; }

            return true;
        }
    }
}
