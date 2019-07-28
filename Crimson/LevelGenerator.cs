using Crimson.Components;
using Crimson.Entities;
using Crimson.Systems;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Crimson
{
    class LevelGenerator
    {

        readonly World _world;
        public Map Map { get; }

        readonly Random rnd = new Random();

        LevelGenerator(World world, Map map)
        {
            _world = world;
            Map = map;
        }

        public static Map Generate(World world, int width, int height, int tileSize)
        {
            var rnd = new Random();
            var lg = new LevelGenerator(world, new Map(width, height, tileSize));
            lg.FillWithDesert();
            foreach (var _ in Enumerable.Range(0, rnd.Next(3, 8)))
            {
                lg.AddLake(rnd.Next(3, 6));
            }
            lg.AddGrass(7);
            lg.AddTrees(0.1);
            lg.AddBushes(0.05);
            return lg.Map;
        }

        void FillWithDesert()
        {
            foreach (var i in Enumerable.Range(0, Map.Width))
            {
                foreach (var j in Enumerable.Range(0, Map.Height))
                {
                    var ts = Map.TileSize;

                    var tile = _world.CreateEntity();
                    Map.Plan[i, j] = tile;
                    tile.AddComponent(new CTile());
                    tile.AddComponent(new CTransform(i * ts, j * ts));
                    tile.AddComponent(new CSumbergable(Properties.Resources.pond));
                    tile.AddComponent(new CGraphics(Utilities.ResizeImage(Properties.Resources.desert, ts, ts)));
                    //tile.AddComponent(new CFlammable(Utilities.ResizeImage(Properties.Resources.ohen, Map.TileSize, Map.TileSize)));
                }
            }
        }

        void AddGrass(double sensitivity)
        {
            foreach (var i in Enumerable.Range(0, Map.Width))
            {
                foreach (var j in Enumerable.Range(0, Map.Height))
                {
                    var correction = (rnd.Next(2) == 0 ? -1 : 1) * rnd.Next((int)Math.Floor(sensitivity / 4));
                    if (IsNearGrass((i, j), sensitivity + correction))
                    {
                        Map.Plan[i, j].AddComponent(new CGraphics(Utilities.ResizeImage(Properties.Resources.Grass, Map.TileSize, Map.TileSize)));
                        Map.Plan[i, j].AddComponent(new CFlammable(Utilities.ResizeImage(Properties.Resources.ohen, Map.TileSize, Map.TileSize)));
                    }
                }
            }
        }

        bool IsOnMap(int x, int y)
        {
            return x >= 0 && x <= Map.Width - 1 && y >= 0 && y <= Map.Height - 1;
        }

        bool IsNearGrass((int X, int Y) point, double distance)
        {
            foreach (var i in Enumerable.Range(0, Map.Width))
            {
                foreach (var j in Enumerable.Range(0, Map.Height))
                {
                    if (Distance(point, (i, j)) <= distance && Map.Plan[i, j].HasComponent<CWet>()) { return true; }
                }
            }
            return false;
        }

        void AddLake(int sources)
        {
            var i = rnd.Next(Map.Width);
            var j = rnd.Next(Map.Height);
            var size = rnd.Next(4, 7);
            Map.Plan[i, j].AddComponent(new CWet(size));

            foreach (var _ in Enumerable.Range(0, sources - 1))
            {
                var miniSize = rnd.Next(2, size - 1);
                var (x, y) = RandomPointNear((i, j), rnd.Next(2, size - 2));
                Map.Plan[x, y].AddComponent(new CWet(miniSize));
            }
        }

        (int X, int Y) RandomPointNear((int X, int Y) point, double distance)
        {
            var d = rnd.NextDouble() * distance;
            var angle = rnd.Next(360);
            var x = Math.Round(d * Math.Cos(angle * Math.PI / 180));
            var y = Math.Round(d * Math.Sin(angle * Math.PI / 180));

            var newX = point.X + (int)x;
            var newY = point.Y + (int)y;

            return IsOnMap(newX, newY) ? (newX, newY) : RandomPointNear(point, distance);
        }

        double Distance((int X, int Y) a, (int X, int Y) b)
        {
            return (new Vector(a) - new Vector(b)).Size;
        }

        void AddBushes(double probability)
        {
            foreach (var i in Enumerable.Range(0, Map.Width))
            {
                foreach (var j in Enumerable.Range(0, Map.Height))
                {
                    // Nemá CFLammable => je to poušt
                    if (!Map.Plan[i, j].HasComponent<CFlammable>())
                    {
                        switch (rnd.Next((int)Math.Round(1 / probability)))
                        {
                            case 0:
                                var bush = MakeBush(i, j);
                                if (bush != default)
                                {
                                    Map.Plan[i, j].GetComponent<CTile>().Occupied = true;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (rnd.Next((int)Math.Round(10 / probability)))
                        {
                            case 0:
                                MakeBush(i, j);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        List<EntityHandle> AddTrees(double probability)
        {
            var trees = new List<EntityHandle>();
            foreach (var i in Enumerable.Range(0, Map.Width))
            {
                foreach (var j in Enumerable.Range(0, Map.Height))
                {
                    // Má CFLammable => je to tráva
                    if (Map.Plan[i, j].HasComponent<CFlammable>())
                    {
                        switch (rnd.Next((int)Math.Round(1 / probability)))
                        {
                            case 0:
                                var tree = MakeTree(i, j);
                                if (tree != default)
                                {
                                    trees.Add(tree);
                                    Map.Plan[i, j].GetComponent<CTile>().Occupied = true;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return trees;
        }

        EntityHandle MakeTree(int X, int Y)
        {
            var ts = Map.TileSize;
            Image treeImage;
            int size;
            switch (rnd.Next(3))
            {
                case 0:
                    size = ts * 1;
                    treeImage = Utilities.ResizeImage(Properties.Resources.baobab, size, size);
                    break;
                default:
                    size = ts * 1;
                    treeImage = Utilities.ResizeImage(Properties.Resources.smrk, size, size);
                    break;
            }
            var x = X * ts; //- size / 4;
            var y = Y * ts; //- size / 2;

            if (x < 0 || y < 0 || x + size >= Map.Width * ts || y + size >= Map.Height * ts) { return default; }

            var tree = _world.CreateEntity();
            tree.AddComponent(new CTransform(x, y));
            tree.AddComponent(new CGraphics(treeImage));
            tree.AddComponent(new CGameObject());
            tree.AddComponent(new CCollidable(size / 2));
            tree.AddComponent(new CHealth(100, 100));
            tree.AddComponent(new CFlammable(Utilities.ResizeImage(Properties.Resources.ohen, size, size)));
            tree.AddComponent(new CSumbergable(Utilities.ResizeImage(Properties.Resources.water, 64, 64)));
            return tree;
        }

        EntityHandle MakeBush(int X, int Y)
        {
            var ts = Map.TileSize;
            Image image = Utilities.ResizeImage(Properties.Resources.bush, ts, ts);
            var x = X * ts;
            var y = Y * ts;

            var bush = _world.CreateEntity();
            bush.AddComponent(new CTransform(x, y));
            bush.AddComponent(new CGraphics(image));
            bush.AddComponent(new CGameObject());
            bush.AddComponent(new CCollidable(ts / 2));
            bush.AddComponent(new CHealth(100, 100));
            bush.AddComponent(new CFlammable(Utilities.ResizeImage(Properties.Resources.ohen, ts, ts)));
            bush.AddComponent(new CSumbergable(Utilities.ResizeImage(Properties.Resources.water, ts, ts)));
            return bush;
        }

        void AddBoulders(double probability)
        {
            foreach (var i in Enumerable.Range(0, Map.Width))
            {
                foreach (var j in Enumerable.Range(0, Map.Height))
                {
                    switch (rnd.Next((int)Math.Round(1 / probability)))
                    {
                        case 0:
                            MakeBoulder(i, j);
                            Map.Plan[i, j].GetComponent<CTile>().Occupied = true;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        void MakeBoulder(int X, int Y)
        {
            var ts = Map.TileSize;
            Image image = Utilities.ResizeImage(Properties.Resources.boulder, ts / 2, ts / 2);
            var x = X * ts;
            var y = Y * ts;
            var boulder = _world.CreateEntity();
            boulder.AddComponent(new CTransform(x, y));
            boulder.AddComponent(new CGraphics(image));
            boulder.AddComponent(new CGameObject());
            boulder.AddComponent(new CCollidable(ts / 4));
            boulder.AddComponent(new CHealth(1000, 1000));
            boulder.AddComponent(new CSumbergable(Utilities.ResizeImage(Properties.Resources.water, 64, 64)));
        }
    }
}
