namespace HodimBrodim
{
    public readonly struct Point
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

        public bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is Point other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
