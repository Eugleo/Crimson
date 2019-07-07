using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Crimson
{
    struct CHealth
    {
        public int MaxHealth;
        public int CurrentHealth;
        public bool Invincible;


        public CHealth(int maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            Invincible = false;
        }
    }

    struct CMovement
    {
        public (int X, int Y) Speed;
        public (double X, double Y) Acceleration;

        public CMovement((int, int) speed, (double, double) acceleration)
        {
            Speed = speed;
            Acceleration = acceleration;
        }

        public int Id => 2;
    }

    struct CPosition
    {
        public (double X, double Y) Coords;
        public bool Changed;

        public CPosition(double x, double y)
        {
            Coords = (x, y);
            Changed = true;
        }

        public CPosition((double, double) coords)
        {
            Coords = coords;
            Changed = true;
            Debug.WriteLine("Create");
        }
    }

    struct CGraphics
    {
        public Image Image { get; }

        public CGraphics(Image image)
        {
            Image = image;
        }
    }

    struct CKeyboardNavigation { }

    struct CInput
    {
        public List<KeyEventArgs> KeyEventArgs { get; }

        public CInput(List<KeyEventArgs> keyEventArgs)
        {
            KeyEventArgs = keyEventArgs;
        }
    }

    struct CCamera
    {

    }

    struct CCollision
    {

    }

    struct CTile
    {
        public enum TileType
        {
            Rock, Grass
        }

        public TileType Type { get; }

        public CTile(TileType type)
        {
            Type = type;
        }
    }
}