using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngine;

internal class WaveProcessor : IProcessor
{
    //
    // wave processor
    //
    public object Load<T>(string path)
    {
        return Raylib.LoadWave(path);
    }

    public void Unload(object wave)
    {
        Raylib.UnloadWave((Wave)wave);
    }
}
