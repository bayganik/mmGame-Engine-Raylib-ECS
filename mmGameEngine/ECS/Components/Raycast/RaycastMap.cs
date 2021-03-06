﻿using System;
using System.Collections.Generic;
using System.Text;

namespace mmGameEngine
{
    public class RaycastMap
    {
        public int[,] Map;
        public int[,] Sprites;
        public int SpriteTotals;

        public RaycastMap()
        {
            Map = new int[24, 24]
            {
              {1,1,1,1,1,1,1,1,1,1,1,1,2,1,1,1,1,1,1,2,1,1,1,1},
              {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
              {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
              {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
              {2,0,0,0,0,0,7,7,7,7,7,0,0,0,0,3,3,3,3,3,0,0,0,1},
              {1,0,0,0,0,0,7,0,0,0,7,0,0,3,3,3,0,0,0,3,0,0,0,1},
              {1,0,0,5,0,0,7,0,0,0,7,0,0,3,0,3,0,0,0,3,0,0,0,1},
              {1,0,0,5,0,0,7,0,0,0,7,0,0,3,0,0,0,0,0,3,0,0,0,1},
              {1,0,5,5,5,0,7,7,0,7,7,0,0,3,3,3,0,3,3,3,0,0,0,1},
              {1,0,0,5,0,0,4,0,0,0,0,0,0,0,0,3,0,3,0,0,0,0,0,2},
              {1,0,0,5,0,0,4,0,0,0,0,-1,0,0,0,3,0,3,0,0,0,0,0,1},
              {1,0,0,0,0,0,4,0,0,0,0,0,0,4,4,4,0,4,4,4,4,0,0,1},
              {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,1},
              {1,0,0,0,0,0,4,0,0,0,6,0,0,7,7,7,0,0,0,0,4,0,0,1},
              {2,0,0,0,0,0,4,0,0,0,6,0,0,7,0,7,0,0,0,0,4,0,0,1},
              {1,0,0,0,0,0,4,0,0,0,6,0,0,7,0,7,0,0,0,0,4,0,0,1},
              {1,0,0,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,4,4,4,1},
              {1,0,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
              {1,0,0,0,0,0,0,0,4,0,0,0,0,0,1,1,1,0,1,0,1,1,0,1},
              {1,0,0,4,0,0,0,0,4,0,0,0,0,0,1,0,0,0,1,0,1,1,0,1},
              {1,0,0,4,4,4,4,4,4,0,0,0,0,0,1,0,1,1,1,0,1,1,0,1},
              {1,0,-1,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,1},
              {1,0,0,0,0,0,0,0,0,0,0,0,0,0,-1,0,1,0,0,0,0,0,0,1},
              {1,2,1,1,1,1,1,1,1,2,1,1,2,1,1,1,1,1,1,1,1,1,1,1}
            };
            Sprites = new int[24, 24];
            SpriteTotals = 0;
            for (int x=0; x < 24; x++)
                for (int y=0; y < 24; y++)
                {
                    if (Map[x, y] < 0)
                    {
                        Sprites[x, y] = Math.Abs(Map[x, y]);
                        SpriteTotals += 1;
                    }
                    else
                        Sprites[x, y] = 0;
                }
        }
    }
}
