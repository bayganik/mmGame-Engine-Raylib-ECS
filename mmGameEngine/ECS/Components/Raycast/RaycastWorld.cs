using mmGameEngine;
using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;
using System.Numerics;
using System.Runtime.CompilerServices;
//using System.Drawing;

namespace mmGameEngine
{
    /*
     * https://lodev.org/cgtutor/raycasting.html
     * 
     * Raycast world consists of a 2D level map and a camera for movement
     * Each point on the map is indicated by a number.
     *      0 = empty space to walk on
     *      1 = wall with a particular texture
     *      2 = other walls and textures
     *      9 = floor (default)
     *      10 = ceiling (default)
     *      
     * Screen data is calculated, converted into an image, truned into texture
     *      and displayed.
     */
    public class RaycastWorld : RenderComponent
    {
        public RaycastCamera LevelCamera;
        public RaycastMap LevelMap;
        
        //public int TextureWidth = 64;
        //public int TextureHeight = 64;

        public int FloorIndex = 9;
        public int CeilingIndex = 10;

        public int DistanceDarkness = 47;             // if you want to light up distance walls, reduce this number

        Vector2 mousePrev = new Vector2(0,0);
        int screenWidth;
        int screenHeight;
        
        Double[] lookupTableFloorDist;   // Some cached lookup tables
        int[,] fIndexLookup;           // Note: Calculations might actually be faster
        int[,] cIndexLoopup;

        readonly List<MaterialData> textureLibrary;
        readonly List<MaterialData> spriteLibrary;

        MaterialData floorTexture;
        MaterialData ceilingTexture;
        MaterialData currentTexture;
        //
        // Data to be rendered
        //
        Color[] screenData;
        // 1-dimensional zbuffer
        double[] ZBuffer = new double[Global.SceneWidth];
        public RaycastWorld()
        {
            LevelMap = new RaycastMap();
            LevelCamera = new RaycastCamera();

            screenWidth = Global.SceneWidth;
            screenHeight = Global.SceneHeight;
            //
            // texture collection with blank index 0, so we can match obj numbers on map
            //
            textureLibrary = new List<MaterialData>();
            textureLibrary.Add(new MaterialData());      

            spriteLibrary = new List<MaterialData>();
            spriteLibrary.Add(new MaterialData());

            screenData = new Color[screenWidth * screenHeight];                 //used for disp of entire screen
            //
            // Floor and Cieling lookup tables
            //
            lookupTableFloorDist = new double[screenHeight];
            for (int y = 0; y < screenHeight; y++)
                lookupTableFloorDist[y] = screenHeight / (2.0 * y - screenHeight);
            
            fIndexLookup = new Int32[screenWidth, screenHeight];
            for (int x = 0; x < screenWidth; x++)
                for (int y = 0; y < screenHeight; y++)
                    fIndexLookup[x,y] = x + (screenWidth * y);

            cIndexLoopup = new Int32[screenWidth, screenHeight];
            for (int x = 0; x < screenWidth; x++)
                for (int y = 0; y < screenHeight; y++)
                    cIndexLoopup[x, y] = x + (screenWidth * (screenHeight - y)); 
        }
        public void AddTexture(Texture2D _texture, string _name = "")
        {
            //
            // Textures need to be added in order of the numbers used in the map
            //
            RaycastTexture tmp = new RaycastTexture(_texture, _name);
            //
            // storing data into a "Struct" allowed this to work (don't know why!)
            //
            MaterialData mat = new MaterialData();
            mat.TextureName = tmp.TextureName;
            mat.TextureData = tmp.TextureData;
            mat.TextureWidth = _texture.width;
            mat.TextureHeight = _texture.height;

            textureLibrary.Add(mat); 
        }
        public void AddSprite(Texture2D _texture, string _name = "")
        {
            //
            // Sprites need to be added in order of the numbers used in the map
            //
            RaycastTexture tmp = new RaycastTexture(_texture, _name);
            //
            // storing data into a "Struct" allowed this to work (don't know why!)
            //
            MaterialData mat = new MaterialData();
            mat.TextureName = tmp.TextureName;
            mat.TextureData = tmp.TextureData;
            mat.TextureWidth = _texture.width;
            mat.TextureHeight = _texture.height;
            mat.ImageData = tmp.ImageData;
            spriteLibrary.Add(mat);
        }
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            LevelCamera.Update(new GTime());

            int camPosX = 0;
            int camPosY = 0;

            if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
            {
                camPosX = (Int32)(LevelCamera.Position.X + LevelCamera.Direction.X);
                camPosY = (Int32)(LevelCamera.Position.Y);
                if (LevelMap.Map[camPosX, camPosY] == 0)
                {
                    camPosX = (Int32)(LevelCamera.Position.X);
                    camPosY = (Int32)(LevelCamera.Position.Y + LevelCamera.Direction.Y);
                    if (LevelMap.Map[camPosX, camPosY] == 0)
                    {
                        LevelCamera.MoveForward();
                    }
                }
            }

            if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
            {
                camPosX = (Int32)(LevelCamera.Position.X - LevelCamera.Direction.X);
                camPosY = (Int32)(LevelCamera.Position.Y);
                if (LevelMap.Map[camPosX, camPosY] == 0)
                {
                    camPosX = (Int32)(LevelCamera.Position.X);
                    camPosY = (Int32)(LevelCamera.Position.Y - LevelCamera.Direction.Y);
                    if (LevelMap.Map[camPosX, camPosY] == 0)
                    {
                        LevelCamera.MoveBackwards();
                    }
                }
            }
            //
            // Rotating
            //
            //Vector2 mousePos = Raylib.GetMousePosition();
            //if ((mousePos.X - mousePrev.X) > 0)
            //    LevelCamera.RotateRight();
            //if ((mousePos.X - mousePrev.X) < 0)
            //    LevelCamera.RotateLeft();
            //mousePrev = mousePos;

            if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
                LevelCamera.RotateRight();
            if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
                LevelCamera.RotateLeft();



        }
        public override void Render()
        {
            //
            // Create a texture2D image of what you see
            //
            RenderLevel(LevelMap.Map, LevelMap.Sprites);
            //RenderSprites();
            //
            // extermly slow FPS rate (by %50)
            //
            //int indx = 0;
            //for (int y = 0; y < Global.SceneHeight; y++)
            //    for (int x = 0; x < Global.SceneWidth; x++)
            //    {
            //        Raylib.DrawPixel(x, y, screenData[indx]);
            //        indx++;
            //    }

            //
            // Write the data into an image, then into a texture 
            //
            //Image img = Raylib.LoadImageEx(screenData, Global.SceneWidth, Global.SceneHeight);
            Image img = Raylib.LoadImage("junk/notused/broken change in Raylib");
            //
            //LoadImageEx is removed from Raylib 3.5
            //Some Bogus access is put here to allow the mmGameEngine to work
            //
            Texture2D gameDisp = Raylib.LoadTextureFromImage(img);
            Raylib.UnloadImage(img);
            Rectangle rect = new Rectangle(0, 0, gameDisp.width, gameDisp.height);
            //
            // render it always at 0,0
            //
            Raylib.DrawTexturePro(gameDisp,
                                  rect,
                                  rect,
                                  Vector2.Zero,
                                  0f,
                                  Color.WHITE);
        }
        private void RenderLevel(int[,] _levelMap, int[,] _levelSprite)
        {
            int texNum;
            //
            // Raycast (given a level map)
            //
            for (int scrnWidthX = 0; scrnWidthX < screenWidth; scrnWidthX++)
            {
                //calculate ray position and direction
                double cameraX = 2 * scrnWidthX / (double)(screenWidth) - 1;             //x-coordinate in RayCasterCamera space

                //--rays start at camera/player position--//
                double rayPosX = LevelCamera.Position.X;
                double rayPosY = LevelCamera.Position.Y;

                double rayDirX = LevelCamera.Direction.X + LevelCamera.CameraPlane.X * cameraX;
                double rayDirY = LevelCamera.Direction.Y + LevelCamera.CameraPlane.Y * cameraX;

                // coordinants of the level map (x,y) 
                int mapX = (int)(rayPosX);
                int mapY = (int)(rayPosY);

                // length of ray from current position to next x or y-side
                // sideDistX is the distance from the ray starting position to the first side to the left
                //
                double sideDistX;
                double sideDistY;

                // length of ray from one x or y-side to next x or y-side
                double deltaDistX = Math.Sqrt(1 + (rayDirY * rayDirY) / (rayDirX * rayDirX));
                double deltaDistY = Math.Sqrt(1 + (rayDirX * rayDirX) / (rayDirY * rayDirY));
                // perpendicular wall distance
                double perpWallDist;

                //what direction to step in x or y-direction (either +1 or -1)
                int stepX;
                int stepY;
                //
                // side will contain if an x-side or a y-side of a wall was hit.
                // If an x-side was hit, side is set to 0, if an y-side was hit, side will be 1.
                // By x-side and y-side, I mean the lines of the grid that are the borders between two squares.
                //
                int hit = 0;                //0 x-side, 1 y-side
                int side = 0;               //was a north-south or a east-west wall hit?

                if (rayDirX < 0)                // Pointing left
                {
                    stepX = -1;
                    sideDistX = (rayPosX - mapX) * deltaDistX;
                }
                else
                {
                    stepX = 1;              //first side to the right 
                    sideDistX = (mapX + 1.0 - rayPosX) * deltaDistX;
                }
                if (rayDirY < 0)
                {
                    stepY = -1;             //first side above 
                    sideDistY = (rayPosY - mapY) * deltaDistY;
                }
                else
                {
                    stepY = 1;              //first side below
                    sideDistY = (mapY + 1.0 - rayPosY) * deltaDistY;
                }
                //------------------------------------------------------------------------------------------------
                //     Perform DDA "Digital Differential Analysis"
                // fast algorithm typically used on square grids to find which squares a line hits
                //
                //------------------------------------------------------------------------------------------------
                // if hit = 1 then we hit a wall
                // if hit = 2 then outside the bounds of map
                //
                // so while hit = 0, we go to next sqr on map
                //
                // The DDA algorithm will always jump exactly one square of level map each loop, 
                // either a square in the x-direction, or a square in the y-direction
                //
                while (hit == 0)
                {
                    //jump to next RayCasterMap square, OR in x-direction, OR in y-direction
                    if (sideDistX < sideDistY)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = 0;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        side = 1;
                    }

                    //Check if ray has hit a wall or sprite
                    if (_levelMap[mapX, mapY] > 0)
                        hit = 1;                    //its a wall 
                    if (_levelMap[mapX, mapY] < 0)
                        hit = 2;                    //its a sprite object
                }
                //
                // After the DDA is done, we have to calculate the distance of the ray to the wall, 
                // so that we can calculate how high the wall has to be drawn after this.
                //
                // if side = 0 then x side of wall was hit
                // if side = 1 then y side of wall was hit
                //
                //Calculate distance of perpendicular wall distance (oblique distance will give fisheye effect!)
                //
                if (side == 0)
                    perpWallDist = Math.Abs((mapX - rayPosX + (1 - stepX) / 2) / rayDirX);
                else
                    perpWallDist = Math.Abs((mapY - rayPosY + (1 - stepY) / 2) / rayDirY);

                //Calculate height of line to draw on screen
                int lineHeight = Math.Abs((int)(screenHeight / perpWallDist));

                //calculate lowest and highest pixel to fill in current stripe
                int drawStart = -lineHeight / 2 + screenHeight / 2;
                if (drawStart < 0)
                    drawStart = 0;

                int drawEnd = lineHeight / 2 + screenHeight / 2;
                if (drawEnd >= screenHeight)
                    drawEnd = screenHeight - 1;

                //------------------------------------------------------------------------------------------------
                //  Darkness Color of the wall is chosen here (depending what number is in map)
                //------------------------------------------------------------------------------------------------
                if (hit == 1)
                {
                    texNum = _levelMap[mapX, mapY];          //texture index for the wall
                    if (texNum < 0)
                        texNum = 1;
                    //
                    // Texture of the wall
                    //
                    currentTexture = textureLibrary[texNum];
                }
                else
                {
                    texNum = _levelSprite[mapX, mapY];      //texture index for the sprite
                    if (texNum < 0)
                        texNum = 7;
                    //
                    // Texture of the wall
                    //
                    currentTexture = spriteLibrary[texNum];
                }


                
                double wallX;                               //where exactly the wall was hit by ray
                if (side == 1)
                    wallX = rayPosX + ((mapY - rayPosY + (1 - stepY) / 2) / rayDirY) * rayDirX;
                else
                    wallX = rayPosY + ((mapX - rayPosX + (1 - stepX) / 2) / rayDirX) * rayDirY;

                wallX -= Math.Floor(wallX);
                //
                // x coordinate on the texture data
                //
                var texX = (int)((wallX) * (double)currentTexture.TextureWidth);

                if (side == 0 && rayDirX > 0)
                    texX = currentTexture.TextureWidth - texX - 1;
                if (side == 1 && rayDirY < 0)
                    texX = currentTexture.TextureWidth - texX - 1;

                //
                // Get the wall or sprite with proper darkness levels from texture library
                //
                int index = scrnWidthX + (screenWidth * drawStart);
                Color pixelData;
                //draw the pixels of the stripe as a vertical line

                for (int y = drawStart; y < drawEnd; y++)
                {
                    int d = y * 256 - screenHeight * 128 + lineHeight * 128;
                    int texY = ((d * currentTexture.TextureHeight) / lineHeight) / 256;
                    //
                    // Darkness Calculations
                    //
                    var darknessIndex = (int)(perpWallDist * 2);
                    darknessIndex = Math.Min(DistanceDarkness, darknessIndex);
                    //
                    // texture data for one pixel at the darkness level
                    //
                    pixelData = currentTexture.TextureData[darknessIndex][texX, texY];
                    //
                    //if (hit == 2)
                    //    pixelData = currentTexture.TextureData[darknessIndex][texX, texY];

                    // index goes down a perpendicular line (by adding the width)
                    //
                    index += screenWidth;
                    screenData[index] = pixelData;
                }

                ZBuffer[scrnWidthX] = perpWallDist;         //perpendicular distance for Sprite Cast

                //----------------------------------------------
                //      Floor & Ceiling
                //----------------------------------------------
                double floorXWall, floorYWall;                  //x, y position of the floor texel at the bottom of the wall


                if (side == 0 && rayDirX > 0)                   //4 different wall directions possible
                {
                    floorXWall = mapX;
                    floorYWall = mapY + wallX;
                }
                else if (side == 0 && rayDirX < 0)
                {
                    floorXWall = mapX + 1.0;
                    floorYWall = mapY + wallX;
                }
                else if (side == 1 && rayDirY > 0)
                {
                    floorXWall = mapX + wallX;
                    floorYWall = mapY;
                }
                else
                {
                    floorXWall = mapX + wallX;
                    floorYWall = mapY + 1.0;
                }

                double distWall = perpWallDist;
                double distPlayer = 0.0;

                floorTexture = textureLibrary[FloorIndex];
                ceilingTexture = textureLibrary[CeilingIndex];

                if (drawEnd < 0)
                    drawEnd = screenHeight;

                //Draw the floor from drawEnd to the bottom of the screen
                for (int y = drawEnd + 1; y < screenHeight; y++)
                {
                    double currentDist = lookupTableFloorDist[y];

                    double weight = (currentDist - distPlayer) / (distWall - distPlayer);

                    double currentFloorX = weight * floorXWall + (1.0 - weight) * LevelCamera.Position.X;
                    double currentFloorY = weight * floorYWall + (1.0 - weight) * LevelCamera.Position.Y;

                    int floorTexX = (int)(currentFloorX * floorTexture.TextureWidth) % floorTexture.TextureWidth;
                    int floorTexY = (int)(currentFloorY * floorTexture.TextureHeight) % floorTexture.TextureHeight;

                    int darknessIndex = (int)(currentDist * 2);
                    darknessIndex = Math.Min(DistanceDarkness, darknessIndex);

                    var floorColor = floorTexture.TextureData[darknessIndex][floorTexY, floorTexX];
                    var ceilingColor = ceilingTexture.TextureData[darknessIndex][floorTexY, floorTexX];
                    //----------------------------------------------
                    // Write pixel data to the screen
                    //----------------------------------------------
                    screenData[fIndexLookup[scrnWidthX, y]] = floorColor;
                    screenData[cIndexLoopup[scrnWidthX, y]] = ceilingColor;
                }

                //RenderSprites();
            }
        }
        private void RenderSprites()
        {
            //-------------------------------------
            // SPRITE CASTING
            //-------------------------------------
            // sort sprites from far to close
            int uDiv = 1;           // =2 makes sprite float
            int vDiv = 1;           // =2 makes sprite float
            float vMove = 0.0f;    // size of texture or Zero
            int vMoveScreen;
            // arrays used to sort the sprites
            int[] spriteOrder;
            double[] spriteDistance;

            //for (int i = 0; i < LevelMap.SpriteTotals; i++)
            //{
            //    LevelMap.spriteOrder[i] = i;
            //    //level.spriteDistance[i] = ((player.position.X - level.sprites[i].position.X) * (player.position.X - level.sprites[i].position.X)
            //    //    + (player.position.Y - level.sprites[i].position.Y) * (player.position.Y - level.sprites[i].position.Y));
            //}
            //CombSort(ref level.spriteOrder, ref level.spriteDistance, level.numSprites);

            // after sorting the sprites, do the projection and then draw them
            int texNum;

            for (int x=0; x < 24; x++)
                for (int y=0; y < 24; y++)
                {

                    if (LevelMap.Sprites[x, y] <= 0)
                        continue;

                    texNum = Math.Abs(LevelMap.Sprites[x, y]);
                    currentTexture = spriteLibrary[texNum];
            //for (int i = 0; i < level.numSprites; i++)
            //{
            // translate sprite position relative to camera
                double spriteX = x - LevelCamera.Position.X;
                double spriteY = y - LevelCamera.Position.Y;

                // transform sprite with the inverse camera matrix (1/(ad-bc))
                double invDet = 1.0 / (LevelCamera.CameraPlane.X * LevelCamera.Direction.Y - LevelCamera.Direction.X * LevelCamera.CameraPlane.Y);

                double transformX = invDet * (LevelCamera.Direction.Y * spriteX - LevelCamera.Direction.X * spriteY);
                double transformY = invDet * (-LevelCamera.CameraPlane.Y * spriteX + LevelCamera.CameraPlane.X * spriteY);

                vMoveScreen = (int)(vMove / transformY);
                int spriteScreenX = (int)((Global.SceneWidth / 2) * (1 + transformX / transformY));

                // calculate height of sprite on screen
                int spriteHeight = Math.Abs((int)(Global.SceneHeight / transformY)) / vDiv;

                // calculate lowest and highest pixel to fill in current stripe
                int drawStartY = -spriteHeight / 2 + Global.SceneHeight / 2 + vMoveScreen;
                if (drawStartY < 0)
                    drawStartY = 0;
                int drawEndY = spriteHeight / 2 + Global.SceneHeight / 2 + vMoveScreen;
                if (drawEndY >= Global.SceneHeight)
                    drawEndY = Global.SceneHeight - 1;

                // calculate width of the sprite
                int spriteWidth = Math.Abs((int)(Global.SceneHeight / transformY)) / uDiv;
                int drawStartX = -spriteWidth / 2 + spriteScreenX;
                if (drawStartX < 0)
                    drawStartX = 0;
                int drawEndX = spriteWidth / 2 + spriteScreenX;
                if (drawEndX >= Global.SceneWidth)
                    drawEndX = Global.SceneWidth - 1;

                // loop through every vertical stripe of the sprite on screen
                for (int stripe = drawStartX; stripe < drawEndX; stripe++)
                {
                    int texX = (int)(256 * (stripe - (-spriteWidth / 2 + spriteScreenX)) * currentTexture.TextureWidth / spriteWidth) / 256;
                    // the conditions of the if are:
                    // 1. it's in front of the camera plane so we don't draw things behind the player
                    // 2. it's on the screen (left or right)
                    // 3. ZBuffer, with perpendicular distance
                    if (transformY > 0 && stripe > 0 && stripe < Global.SceneWidth && transformY < ZBuffer[stripe])
                    {
                        for (int yyy = drawStartY; y < drawEndY; yyy++) // for every pixel of the current stripe
                        {
                                int d = (yyy - vMoveScreen) * 256 - Global.SceneHeight * 128 + spriteHeight * 128;
                                int texY = ((d * currentTexture.TextureHeight) / spriteHeight) / 256;
                                //
                                // Darkness Calculations
                                //
                                var darknessIndex = (int)(ZBuffer[stripe] * 2);
                                darknessIndex = Math.Min(DistanceDarkness, darknessIndex);
                                //
                                // texture data for one pixel at the darkness level
                                //
                                Color pixelData = currentTexture.ImageData[currentTexture.TextureWidth * texY + texX];
                                uint u = pixelData.a;
                                u <<= 8;    // now u would be 11001010 00000000
                                u |= pixelData.b;     // now u would be 11001010 11111110
                                u <<= 8;    // now u would be 11001010 11111110 00000000
                                u |= pixelData.g;     // now u would be 11001010 11111110 10111010
                                u <<= 8;    // now u would be 11001010 11111110 10111010 00000000
                                u |= pixelData.r;     // now u would be 11001010 11111110 10111010 10111110
                                                      // This is how        a        b        c        d    
                                                      // are packed into one integer u.

                                //Color toAdd = rawData[level.sprites[level.spriteOrder[i]].texture][currentTexture.TextureWidth * texY + texX];
                                if ((u & 0x00FFFFFF) != 0)
                                    screenData[Global.SceneWidth * y + stripe] = pixelData;
                            }
                        }
                }
            }
        }
        private void CombSort(ref int[] order, ref double[] dist, int amount)
        {
            int tempint;
            double tempdouble;
            int gap = amount;
            bool swapped = false;
            while (gap > 1 || swapped)
            {
                // shrink factor of 1.3
                gap = (gap * 10) / 13;
                if (gap == 9 || gap == 10)
                    gap = 11;
                if (gap < 1)
                    gap = 1;
                swapped = false;
                for (int i = 0; i < amount - gap; i++)
                {
                    int j = i + gap;
                    if (dist[i] < dist[j])
                    {
                        tempdouble = dist[i];
                        dist[i] = dist[j];
                        dist[j] = tempdouble;

                        tempint = order[i];
                        order[i] = order[j];
                        order[j] = tempint;
                        swapped = true;
                    }
                }
            }
        }
    }
}
