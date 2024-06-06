using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngine;

internal class TiledMapProcessor : IProcessor
{
    //
    // TiledMap (.tmx) file processor
    //
    public object Load<T>(string path)
    {
        return new TiledMap(path);
    }

    public void Unload(object texture)
    {
        //Raylib.UnloadTexture((TiledMap)texture);
    }
}
