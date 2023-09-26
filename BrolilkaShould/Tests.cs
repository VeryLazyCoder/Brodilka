using HodimBrodim;

namespace BrolilkaShould
{
    [TestFixture]
    public class Tests
    {
        //test comment
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
    }
}