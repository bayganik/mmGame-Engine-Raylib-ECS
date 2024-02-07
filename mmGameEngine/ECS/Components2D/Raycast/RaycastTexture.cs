using System;
using System.Collections.Generic;
using System.Text;
using Raylib_cs;


namespace mmGameEngine
{
    public class RaycastTexture
    {
        private const int DARKNESSLEVLES = 47;
        // key is darkness(0 - 47) then arrya of Color X, Color Y

        public Color[] ImageData;
        static List<Color[,]> _textureData;
        static string _textureName;
        public string TextureName
        {
            get { return _textureName; }
            set { _textureName = value; }
        }
        public List<Color[,]> TextureData
        {
            get { return _textureData; }
            set { _textureData = value; }
        }
        public RaycastTexture(Texture2D _texture, string _name = "")
        {
            LoadTextureData(_texture, _name);
        }
        public unsafe void LoadTextureData(Texture2D _texture, string _name = "")
        {
            _textureName = _name;
            //Image img = Raylib.GetTextureData(_texture);                    //load into an image
            //IntPtr mapPixelsData = Raylib.LoadImageColors(img);                //get image data IntPtr
            //Color* mapPixels = (Color*)mapPixelsData.ToPointer();           //IntPtr points to Color data  
            Image img = Raylib.LoadImageFromTexture(_texture);                    //load into an image
            Color* mapPixelsData = Raylib.LoadImageColors(img);                //get image data IntPtr
  
            //
            // Find color DATA going thru width of the texture
            //
            ImageData = new Color[img.Height * img.Width];

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    ImageData[y * img.Width + x] = mapPixelsData[y * img.Width + x];
                }
            }
            Raylib.UnloadImage(img);                                        // Unload image from RAM
            
            float darkness = 1.0f;
            //
            // arrange the Data to be in 2d coordinants [x,y]
            // 
            _textureData = new List<Color[,]>();
            for (int d = 0; d <= DARKNESSLEVLES; d++)
            {
                Color[,] ShadeOfDarkData = new Color[img.Width, img.Height];
                for (int x = 0; x < img.Width; x++)
                {
                    for (int y = 0; y < img.Height; y++)
                    {
                        var index = x + (img.Width * y);                //find pixel going across the texture image
                        Color pixel = ImageData[index];
                        Color tmpPixel = new Color();
                        //
                        // pack the color bytes into int, shift the bits to get dark color
                        //
                        int tmp = (Byte)pixel.A << 24 | (Byte)(pixel.B / darkness) << 16 | (Byte)(pixel.G / darkness) << 8 |
                                (Byte)(pixel.R / darkness);
                        //
                        // unpack the int back to bytes to update the Color values (with darkness)
                        //
                        var bytes = BitConverter.GetBytes(tmp);

                        tmpPixel.R = bytes[0];
                        tmpPixel.G = bytes[1];
                        tmpPixel.B = bytes[2];
                        tmpPixel.A = bytes[3];

                        ShadeOfDarkData[x, y] = tmpPixel;
                    }
                }
                //
                // add a shade of dark data
                //
                _textureData.Add(ShadeOfDarkData);
                darkness += 0.0f + (d * 0.0125f) + (d * d * 0.00025f);          //change darkness
            }
        }
    }
}
