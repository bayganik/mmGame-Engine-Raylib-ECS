using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngine;

internal class FontProcessor : IProcessor
{
    int fontSize = 100;
    int glyphCount = 0;
    //
    // font processor
    //
    public object Load<T>(string path)
    {
        var fnt = Raylib.LoadFontEx(path, fontSize, Array.Empty<int>(), glyphCount);
        Raylib.SetTextureFilter(fnt.Texture, TextureFilter.Trilinear);
        return fnt;
    }

    public void Unload(object font)
    {
        Raylib.UnloadFont((Font)font);
    }
}
