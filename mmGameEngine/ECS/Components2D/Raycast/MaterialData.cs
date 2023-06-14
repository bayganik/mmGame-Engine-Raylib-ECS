using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;

namespace mmGameEngine
{
    public struct MaterialData
    {
        public string TextureName;
        public List<Color[,]> TextureData;                  //list of 48 darkness levels of the same texture
        public int TextureWidth;
        public int TextureHeight;
        public Color[] ImageData;
    }
}
