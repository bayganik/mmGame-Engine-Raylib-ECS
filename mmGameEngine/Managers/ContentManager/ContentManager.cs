using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;
using Entitas;

namespace mmGameEngine;
/*
 * By default we look into Assets folder for content
 * use BaseContnetFolder to change the location.
 */
public class ContentManager : IDisposable
{
    public string BaseContnetFolder { get; set; }
    
    private readonly List<object> _content;
    private readonly Dictionary<Type, IProcessor> _processor;

    public ContentManager()
    {
        BaseContnetFolder = "Assets";
        _content = new List<object>();
        _processor = new Dictionary<Type, IProcessor>();

        _processor.Add(typeof(TiledMap), new TiledMapProcessor());      //Tiled tmx file
        _processor.Add(typeof(Sound), new SoundProcessor());            //Soundfx
        _processor.Add(typeof(Texture2D), new TextureProcessor());      //Texture2d
        _processor.Add(typeof(Model), new ModelProcessor());            //Model3D
        _processor.Add(typeof(Image), new ImageProcessor());            //Image
    }

    public T Load<T>(string path)
    {
        if(_processor.TryGetValue(typeof(T), out IProcessor? processor))
        {
            return (T)processor.Load<T>($"{BaseContnetFolder}/{path}");
        }
        return default!;
    }

    public void Unload(object texture)
    {
        Type type = texture.GetType();
        TryGetProcessor(type, (a) => {
            _processor[type].Unload(texture);
            _content.Remove(texture);
        });
    }

    public IProcessor TryGetProcessor(Type type,Action<IProcessor> action)
    {
        if (_processor.TryGetValue(type, out IProcessor? processor))
        {
            action.Invoke(processor);
            return processor;
        }
        Console.WriteLine($"No processor found for {type}");
        return null!;
    }

    public void Dispose()
    {
        _content.ForEach(content => {
            IProcessor processor = TryGetProcessor(content.GetType(), (a) => {
                a.Unload(content);
            });
         });
        //textures.Values.ToList().ForEach(texture => Raylib.UnloadTexture(texture));
    }
}
