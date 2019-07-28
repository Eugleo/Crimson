using Crimson.Entities;

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
