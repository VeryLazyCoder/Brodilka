using HodimBrodim;

namespace BrodilkaShould
{
    [TestFixture]
    public class Tests
    {
        private static readonly Point StartPoint = new(11, 5);
        private const int START_MOVES = 100;

        [Test]
        public void Test1()
        {
            var enemy = new SmartEnemy(new Point(1, 1), null);
            Assert.That(enemy.CollisionWithPlayer(new Point(2, 2)), Is.False);
        }
        [Test]
        public void Test2()
        {
            var enemy = new SmartEnemy(new Point(1, 1), null);
            Assert.That(enemy.CollisionWithPlayer(enemy.Position), Is.True);
        }

        [TestCase(ConsoleKey.W)]
        public void PlayerMovementTest(ConsoleKey pressedKey)
        {
            GameMap.GetMovesOnChosenMap(1);
            var map = new GameMap();
            var player = new Player(StartPoint, START_MOVES);

            player.Move(pressedKey, map);
            Assert.That(player.MovesAvailable == START_MOVES, Is.EqualTo(player.Position == StartPoint));
        }
    }
}