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
            Image img = Raylib.GetTextureData(_texture);                    //load into an image
            IntPtr mapPixelsData = Raylib.LoadImageColors(img);                //get image data IntPtr
            Color* mapPixels = (Color*)mapPixelsData.ToPointer();           //IntPtr points to Color data    
            //
            // Find color DATA going thru width of the texture
            //
            ImageData = new Color[img.height * img.width];

            for (int x = 0; x < img.width; x++)
            {
                for (int y = 0; y < img.height; y++)
                {
                    ImageData[y * img.width + x] = mapPixels[y * img.width + x];
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
                Color[,] ShadeOfDarkData = new Color[img.width, img.height];
                for (int x = 0; x < img.width; x++)
                {
                    for (int y = 0; y < img.height; y++)
                    {
                        var index = x + (img.width * y);                //find pixel going across the texture image
                        Color pixel = ImageData[index];
                        Color tmpPixel = new Color();
                        //
                        // pack the color bytes into int, shift the bits to get dark color
                        //
                        int tmp = (Byte)pixel.a << 24 | (Byte)(pixel.b / darkness) << 16 | (Byte)(pixel.g / darkness) << 8 |
                                (Byte)(pixel.r / darkness);
                        //
                        // unpack the int back to bytes to update the Color values (with darkness)
                        //
                        var bytes = BitConverter.GetBytes(tmp);

                        tmpPixel.r = bytes[0];
                        tmpPixel.g = bytes[1];
                        tmpPixel.b = bytes[2];
                        tmpPixel.a = bytes[3];

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
