using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crimson
{
    struct Vector : IEquatable<Vector>
    {
        public double X { get; }
        public double Y { get; }

        public double Size
        {
            get
            {
                return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
            }
        }

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector((double X, double Y) coords)
        {
            X = coords.X;
            Y = coords.Y;
        }

        public static Vector FromPoint(System.Drawing.Point p)
        {
            return new Vector(p.X, p.Y);
        }

        public Vector Normalized()
        {
            return Size != 0 ? new Vector(X / Size, Y / Size) : this;
        }

        public Vector Orthogonalized()
        {
            return new Vector(-Y, X);
        }

        public Vector ScaledBy(double c)
        {
            return new Vector(X * c, Y * c);
        }

        public Vector Normalized(double size)
        {
            return new Vector(size * X / Size, size * Y / Size);
        }

        public bool Equals(Vector other)
        {
            return X == other.X && Y == other.Y;
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public static bool operator ==(Vector a, Vector b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((Vector)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                return hashCode;
            }
        }
    }
}
