using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Raylib_cs;

namespace mmGameEngine
{
    public class TiledMap : RenderComponent
    {
        public TmxMap Map;
        public string MapPath;
        public string[] TileSetImagePath;
        //public CanvasBitmap[] TileSetImage;
        public TmxTileset MyTileSet;

        //int tilesetTilesWide;
        //int tilesetTilesHigh;
        public TiledMap(string _mapFilePath)
        {
            MapPath = _mapFilePath;
            Map = new TmxMap();
            Map = TiledMapLoader.LoadTmxMap(Map, _mapFilePath);
            RenderLayer = Global.TILEMAP_LAYER;


            Global.WorldWidth = Map.WorldWidth;
            Global.WorldHeight = Map.WorldHeight;
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }
        public override void Render()
        {
            base.Render();
            TiledRendering.RenderMap(Map, Transform.Position, Transform.Scale, RenderLayer);
        }
    }
}
