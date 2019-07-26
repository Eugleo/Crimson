using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crimson.Components;
using Crimson.Systems;
using Crimson.Entities;
using System.Drawing.Imaging;
using System.Drawing;

namespace Crimson.Systems
{
    class Map
    {
        public readonly EntityHandle[,] Plan;
        public int TileSize { get; }
        public int Width { get; }
        public int Height { get; }

        public Map(int width, int height, int tileSize)
        {
            Plan = new EntityHandle[width, height];
            TileSize = tileSize;
            Width = width;
            Height = height;
        }
    }
}
