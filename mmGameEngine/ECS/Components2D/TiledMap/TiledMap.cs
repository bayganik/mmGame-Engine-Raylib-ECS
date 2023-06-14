using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;

namespace mmGameEngine
{
    /*
     * Tiled map component allows acess to all the layers & objects of the map
     * TiledTmx folder has all the update/render code
     */
    public class TiledMap : RenderComponent
    {
        public TmxMap Map;
        public string MapPath;
        public string[] TileSetImagePath;               //all the image png files used (sprite sheets)
        public TmxTileset MyTileSet;

        public TiledMap(string _mapFilePath)
        {
            MapPath = _mapFilePath;
            Map = new TmxMap();
            Map = TiledMapLoader.LoadTmxMap(Map, _mapFilePath);
            RenderLayer = Global.TILEMAP_LAYER;


            Global.WorldWidth = Map.WorldWidth;
            Global.WorldHeight = Map.WorldHeight;
        }

        public override void Render()
        {
            base.Render();
            TransformComponent Transform = OwnerEntity.Get<TransformComponent>();
            TiledRendering.RenderMap(Map, Transform.Position, Transform.Scale, RenderLayer);
        }
    }
}
