using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HodimBrodim
{
    public class CommomEnemy : IEnemy
    {

        private Point _position;
        private Point _previousPosition;
        private GameMap _map;
        public Point Position => _position;
        public Point PreviousPosition => _previousPosition;
        public CommomEnemy(Point position, GameMap map)
        {
            _map = map;
            _position = position;
            _previousPosition = position;
        }
        public void Move(Point playerPosition)
        {
            Random rd = new Random();
            bool rightPosition = false;

            while (rightPosition == false)
            {
                _previousPosition = _position;
                Point newPosition;
                switch (rd.Next(1, 5))
                {
                    case 1:
                        newPosition = new(_position.X - 1, _position.Y);
                        if (_map.IsEmptyCell(newPosition))
                        {
                            _position = newPosition;
                            rightPosition = true;
                        }
                        break;
                    case 2:
                        newPosition = new(_position.X - 1, _position.Y);
                        if (_map.IsEmptyCell(newPosition))
                        {
                            _position = newPosition;
                            rightPosition = true;
                        }
                        break;
                    case 3:
                        newPosition = new(_position.X, _position.Y - 1);
                        if (_map.IsEmptyCell(newPosition))
                        {
                            _position = newPosition;
                            rightPosition = true;
                        }
                        break;
                    case 4:
                        newPosition = new(_position.X, _position.Y + 1);
                        if (_map.IsEmptyCell(newPosition))
                        {
                            _position = newPosition;
                            rightPosition = true;
                        }
                        break;
                }
            }
        }

        public bool CollisionWithPlayer(Point playerPosition)
        {
            return playerPosition == _position || playerPosition == _previousPosition;
        }

        public void Display()
        {
            Console.SetCursorPosition(_position.X, _position.Y);
            Program.Paint('!', ConsoleColor.Red);
        }
    }
}

