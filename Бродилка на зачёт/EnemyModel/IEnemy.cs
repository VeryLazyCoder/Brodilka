using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HodimBrodim
{
    public interface IEnemy
    {
        public Point Position { get; }
        public Point PreviousPosition {  get; }

        public void Move(Point playerPosition);

        public bool CollisionWithPlayer(Point playerPosition);
    }
}
