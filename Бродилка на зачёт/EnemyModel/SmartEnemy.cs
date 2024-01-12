using System.IO;

namespace HodimBrodim
{
    public class SmartEnemy : IEnemy
    {
        private Point _position;
        private Point _previousPosition;
        private readonly Dictionary<Point, Point> _track;
        private readonly GameMap _map;
        private readonly Point[] _offsetPoints;
        private readonly Point _nullPoint = new(-1, -1);

        public Point Position => _position;
        public Point PreviousPosition => _previousPosition;
        private Point StartPointForBfs => _position;

        public SmartEnemy(Point position, GameMap map)
        {
            _map = map;
            _position = position;
            _previousPosition = position;
            _offsetPoints = new Point[]
            {
                new Point(-1, 0),
                new Point(1, 0),
                new Point(0, -1),
                new Point(0, 1),
            };
            _track = new();
        }

        public void Move(Point playerPosition)
        {           
            FormPathToPlayer(playerPosition);
            (_previousPosition, _position) = (_position, GetNextPointToPlayer(playerPosition));
        }
        public void Display()
        {
            Console.SetCursorPosition(_position.X, _position.Y);
            Program.Paint('!', ConsoleColor.DarkRed);
        }
        public bool CollisionWithPlayer(Point playerPosition) =>
            playerPosition == _position || playerPosition == _previousPosition;

        private Point GetNextPointToPlayer(Point playerPosition)
        {
            var pathItem = playerPosition;
            var path = new List<Point>();

            while (pathItem != _nullPoint)
            {
                path.Add(pathItem);
                pathItem = _track[pathItem];
            }
            path.Reverse();
            return path.Count > 1 ? path[1] : _position;
        }
        private void FormPathToPlayer(Point playerPosition)
        {
            _track.Clear();
            _track[StartPointForBfs] = _nullPoint;
            var visited = new HashSet<Point>();
            var pointQueue = new Queue<Point>();
            pointQueue.Enqueue(StartPointForBfs);

            while (pointQueue.Count != 0)
            {
                var point = pointQueue.Dequeue();

                if (visited.Contains(point) || !_map.IsNotWall(point))
                    continue;
                if (point == playerPosition)
                    break;

                foreach (var p in _offsetPoints)
                {
                    var nextpoint = point + p;
                    pointQueue.Enqueue(nextpoint);
                    if (!visited.Contains(nextpoint))
                        _track[nextpoint] = point;
                }
                visited.Add(point);
            }
        }
    }
}

