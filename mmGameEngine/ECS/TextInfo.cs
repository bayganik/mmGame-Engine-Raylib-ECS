using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;


namespace mmGameEngine
{
    public enum TextFontTypes
    {
        Default,
        Default2,
        Arial,
        Toon,
        Vera,
        Digital,
        OpenSans
    }
    public class TextInfo
    {
        public Color FontColor = Color.WHITE;
        public int FontSize = 13;
        public Font TextFont;
        public string Content;

        TextFontTypes fontType;
        public TextInfo(string _content, TextFontTypes _fontType, int _fontSize, Color _fontColor)
        {
            Content = _content;
            fontType = _fontType;
            FontSize = _fontSize;
            FontColor = _fontColor;
            switch((int)fontType)
            {
                case (int)TextFontTypes.Default:
                    TextFont = Raylib.LoadFont("AssetsEngine/Fonts/Default.ttf");
                    break;
                case (int)TextFontTypes.Default2:
                    TextFont = Raylib.LoadFont("AssetsEngine/Fonts/Default2.ttf");
                    break;
                case (int)TextFontTypes.Arial:
                    TextFont = Raylib.LoadFont("AssetsEngine/Fonts/arial.ttf");
                    break;
                case (int)TextFontTypes.Toon:
                    TextFont = Raylib.LoadFont("AssetsEngine/Fonts/toon.ttf");
                    break;
                case (int)TextFontTypes.Vera:
                    TextFont = Raylib.LoadFont("AssetsEngine/Fonts/VeraMono.ttf");
                    break;
                case (int)TextFontTypes.Digital:
                    TextFont = Raylib.LoadFont("AssetsEngine/Fonts/Digitalt.ttf");
                    break;
                case (int)TextFontTypes.OpenSans:
                    TextFont = Raylib.LoadFont("AssetsEngine/Fonts/OpenSans.ttf");
                    break;
            }

        }
    }
}
