using HodimBrodim;

namespace BrodilkaManualTesting
{
    internal class ManualTests
    {
        private static readonly Point _startPoint = new Point(11, 5);
        private static readonly int _startMoves = 100;
        public static void Main(string[] args)
        {
            GameMap.GetMovesOnChosenMap(1);

            Console.WriteLine("Давайте проведём позитивный тест, введите правильную клавишу" +
                "Игрок перемещается клавишами W,A,S,D при их нажатии количество ходов уменьшается");
            LaunchTest();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n\nВремя для негативного теста, введите неверную клавишу,\n" +
                "игрок не должен сдвинуться с места, количество ходов измениться не должно");
            LaunchTest();
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void LaunchTest()
        {
            var map = new GameMap();
            var player = new Player(_startPoint, _startMoves);

            Console.WriteLine("Введите клавишу");
            var userKey = Console.ReadKey(true);

            player.Move(userKey.Key, map);
            if (player.Position != _startPoint && player.MovesAvailable != _startMoves)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Персонаж сдвинулся, ходов стало меньше тест успешен");
            }
            else if (player.Position == _startPoint && player.MovesAvailable == _startMoves)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Персонаж сдвинулся, количество ходов не изменилось, тест провален");
            }
            else if (player.Position == _startPoint && player.MovesAvailable != _startMoves)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Персонаж не сдвинулся, ходов стало меньше, тест провален");
            }
            else if (player.Position == _startPoint && player.MovesAvailable == _startMoves)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Персонаж не сдвинулся, количество ходов не изменилось, ход не засчитан");
            }
        }
    }
}
