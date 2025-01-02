using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mmGameEngine;

public class ModelProcessor : IProcessor
{
    //
    // 3D model processor
    //
    public object Load<T>(string path)
    {
        return Raylib.LoadModel(path);
    }

    public void Unload(object model)
    {
        Raylib.UnloadModel((Model)model);
    }
}
