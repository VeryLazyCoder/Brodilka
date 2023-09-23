using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HodimBrodim
{
    public struct Point
    {
        public readonly int X; 
        public readonly int Y;

        public Point(int x, int y) => (X, Y) = (x, y);

        public static Point operator + (Point a, Point b)=>
            new(a.X + b.X, a.Y + b.Y);

        public static bool operator == (Point a , Point b)=>
            a.X == b.X && a.Y == b.Y;

        public static bool operator != (Point a, Point b) =>
            a.X != b.X || a.Y != b.Y;
    }
}
