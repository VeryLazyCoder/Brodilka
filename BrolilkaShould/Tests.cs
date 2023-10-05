using HodimBrodim;

namespace BrolilkaShould
{
    [TestFixture]
    public class Tests
    {
        private static readonly Point _startPoint = new Point(11, 5);
        private static readonly int _startMoves = 100;

        [Test]
        public void Test1()
        {
            var enemy = new SmartEnemy(new Point(1, 1), null);
            Assert.IsFalse(enemy.CollisionWithPlayer(new Point(2, 2)));
        }
        [Test]
        public void Test2()
        {
            var enemy = new SmartEnemy(new Point(1, 1), null);
            Assert.IsTrue(enemy.CollisionWithPlayer(enemy.Position));
        }

        [TestCase(ConsoleKey.W)]
        [TestCase(ConsoleKey.G)]
        public void PlayerMovementTest(ConsoleKey pressedKey)
        {
            GameMap.GetMovesOnChoosenMap(1);
            var map = new GameMap();
            var player = new Player(_startPoint, _startMoves);

            player.Move(pressedKey, map);
            Assert.AreEqual(player.Position == _startPoint, player.MovesAvailable == _startMoves);
        }
    }
}