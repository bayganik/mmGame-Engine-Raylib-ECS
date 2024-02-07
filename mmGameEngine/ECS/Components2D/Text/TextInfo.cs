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
    /// <summary>
    /// Data about the text being displayed (string value, font, size, color)
    /// </summary>
    public class TextInfo
    {
        public Color FontColor = Color.White;
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
                    TextFont = Global.EngineFonts[0];
                    break;
                case (int)TextFontTypes.Default2:
                    TextFont = Global.EngineFonts[1];
                    break;
                case (int)TextFontTypes.Arial:
                    TextFont = Global.EngineFonts[2];
                    break;
                case (int)TextFontTypes.Toon:
                    TextFont = Global.EngineFonts[3];
                    break;
                case (int)TextFontTypes.Vera:
                    TextFont = Global.EngineFonts[4];
                    break;
                case (int)TextFontTypes.Digital:
                    TextFont = Global.EngineFonts[5];
                    break;
                case (int)TextFontTypes.OpenSans:
                    TextFont = Global.EngineFonts[6];
                    break;
            }

        }
    }
}
