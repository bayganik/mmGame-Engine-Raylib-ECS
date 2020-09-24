﻿using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;

namespace TestmmGame
{
    public class Content
    {
        public const string AssetDir = "Assets";
        public static Texture2D crossHair;
        public static Texture2D fireSpritesheet;
        public static Texture2D backGround;
        public static Texture2D buttonEmpty;
        public static void LoadPlay()
        {
            crossHair = Raylib.LoadTexture("Assets/Img/crosshair.png");
            fireSpritesheet = Raylib.LoadTexture("Assets/Missile/FIR001.png");
            backGround = Raylib.LoadTexture("Assets/Img/GameBackground.png");
        }
        public static void LoadMenu()
        {
            backGround = Raylib.LoadTexture("Assets/Img/GameBackground.png");
            buttonEmpty = Raylib.LoadTexture("Assets/Img/Button_empty.png");
        }
    }
}
