using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HodimBrodim
{
    public class SmartEnemy : IEnemy
    {
        private Point _position;
        private Point _previousPosition;
        private GameMap _map;
        public Point Position => _position;
        public Point PreviousPosition => _previousPosition;

        public SmartEnemy(Point position, GameMap map)
        {
            _map = map;
            _position = position;
            _previousPosition = position;
        }
        public void Move(Point playerPosition)
        {
            var start = _position;
            var track = new Dictionary<Point, Point>();
            var nullPoint = new Point(-1, -1);
            track[start] = nullPoint;
            var visited = new HashSet<Point>();
            var queue = new Queue<Point>();
            queue.Enqueue(start);
            while (queue.Count != 0)
            {
                var point = queue.Dequeue();

                if (visited.Contains(point))
                    continue;
                if (!_map.IsEmptyCell(point))
                    continue;
                if (point == playerPosition)
                    break;

                var offsetPoints = new List<Point>
                {
                    new Point(-1, 0),
                    new Point(1, 0),
                    new Point(0, -1),
                    new Point(0, 1),
                };

                foreach (var p in offsetPoints)
                {
                    var nextpoint = new Point(point.X + p.X, p.Y + point.Y);
                    queue.Enqueue(nextpoint);
                    if (!visited.Contains(nextpoint))
                        track[nextpoint] = point;
                }
                visited.Add(point);
            }


            var pathItem = playerPosition;
            var result = new List<Point>();

            while (pathItem != nullPoint)
            {
                result.Add(pathItem);
                pathItem = track[pathItem];
            }
            result.Reverse();
            _previousPosition = _position;
            _position = result.Count > 1 ? result[1] : _previousPosition;

            Console.SetCursorPosition(_position.X, _position.Y);
            Program.Paint('!', ConsoleColor.Red);
        }

        public bool CollisionWithPlayer(Point playerPosition)
        {
            return playerPosition == _position || playerPosition == _previousPosition;
        }
    }
}

