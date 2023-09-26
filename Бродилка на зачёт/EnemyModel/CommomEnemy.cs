namespace HodimBrodim
{
    public class CommomEnemy : IEnemy
    {
        private Point _position;
        private Point _previousPosition;
        private GameMap _map;
        private Random _random;
        private Point[] _offsetPoints; 

        public Point Position => _position;
        public Point PreviousPosition => _previousPosition;

        public CommomEnemy(Point position, GameMap map)
        {
            _map = map;
            _position = position;
            _previousPosition = position;
            _random = new Random();
            _offsetPoints = new Point[] { new Point(0, 1), new Point(0, -1), new Point(1, 0), new Point(-1, 0) };
        }

        public void Move(Point playerPosition)
        {
            while (true)
            {
                var nextPosition = Position + _offsetPoints[_random.Next(_offsetPoints.Length)];
                if (_map.IsNotWall(nextPosition))
                {
                    (_position, _previousPosition) = (nextPosition, _position);
                    break;
                }
            }
            
        }

        public bool CollisionWithPlayer(Point playerPosition) =>
            playerPosition == _position || playerPosition == _previousPosition;

        public void Display()
        {
            Console.SetCursorPosition(_position.X, _position.Y);
            Program.Paint('!', ConsoleColor.Red);
        }
    }
}

