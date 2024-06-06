using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngine;

internal class SoundProcessor : IProcessor
{
    //
    // Texture2D processor
    //
    public object Load<T>(string path)
    {
        return Raylib.LoadSound(path);
    }

    public void Unload(object texture)
    {
        Raylib.UnloadSound((Sound)texture);
    }
}
