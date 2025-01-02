using Raylib_cs;

namespace mmGameEngine;

public class ImageProcessor : IProcessor
{
    public object Load<T>(string path)
    {
        return Raylib.LoadImage(path);
    }

    public void Unload(object image)
    {
        Raylib.UnloadImage((Image)image);
    }
}