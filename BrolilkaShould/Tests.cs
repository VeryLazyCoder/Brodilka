using HodimBrodim;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace BrolilkaShould
{
    public class Tests
    {

        [Test]
        public void Test1()
        {
            GameMap.GetMovesOnChoosenMap(1);
            var map = new GameMap();
            Assert.That(4, Is.EqualTo(HodimBrodim.Program.GetEnemies(map, 4).Count));
        }
    }
}