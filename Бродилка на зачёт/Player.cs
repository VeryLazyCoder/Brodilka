using System.Drawing;

namespace HodimBrodim
{
    public class Player : Fighter
    {
        public Point Position { get; private set; }
        public bool PlayerIsDead { get; private set; }
        public int TreasureCount { get; private set; }

        private readonly List<Fighter> _fighters;
        public Player(GameMap map, Point position) : base("игрок", 150, 2, 25, "Хороший вопрос")
        {
            Random rand = new Random();
            Position = position;
            map[Position] = 'T';

            _fighters = new List<Fighter>()
            {
                new Fighter("Сумасшедший Маньяк", 200f + rand.Next(-20,21) ,
            2 + rand.Next(-1,2), 75f + rand.Next(-7,8),
                "в зависимости от степени чесания головы меняет свои характеристики"),
                new Fighter("Сын маминой подруги", 10f, 9.25f, 100f,"обладает сюжетной бронёй"),
                new Fighter("Обезьяна", 500f, 2, 25f," мозгов нет, здоровья много"),
                new Fighter("Ноутбук ирбис", 1000f, 8, 3.5f,"легенда, если проиграет попадёт к вам на стол." +
                "Вы точно хотите этого?")
            };
        }

        public void AddTreasure()
        {
            TreasureCount++;
        }
        public void Move(ConsoleKeyInfo pressedKey, char[,] map)
        {
            int[] direction = GetDirection(pressedKey);
            int nextPositionX = Position.X + direction[0];
            int nextPositionY = Position.Y + direction[1];
            if (map[nextPositionX, nextPositionY] != '-' & map[nextPositionX, nextPositionY] != '|')
                Position = new(nextPositionX, nextPositionY);
        }
        private static int[] GetDirection(ConsoleKeyInfo pressedKey)
        {
            int[] direction = { 0, 0 };
            if (pressedKey.Key == ConsoleKey.W)
                direction[1]--;

            else if (pressedKey.Key == ConsoleKey.S)
                direction[1]++;

            else if (pressedKey.Key == ConsoleKey.A)
                direction[0]--;

            else if (pressedKey.Key == ConsoleKey.D)
                direction[0]++;

            else if (pressedKey.Key == ConsoleKey.Escape)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("ZЗря вы покинули такую прекрасную игру :(");
                Environment.Exit(0);
            }
            return direction;
        }

        public void ShowPlayerStatistic()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(40, 0);
            Console.WriteLine($"Ваше здоровье {Health}");
            Console.SetCursorPosition(40, 1);
            Console.WriteLine($"Ваш урон составляет {Damage}");
            Console.SetCursorPosition(40, 2);
            Console.WriteLine($"Ваша броня  {Armor}");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.SetCursorPosition(0, 21);
            Console.WriteLine($"Осталось ходов: {--Program.MovesAvailable} ");
            Console.WriteLine($"Счётчик сокровищ: {TreasureCount}");
        }
        public void FightWithEnemy()
        {
            Random randomEnemy = new Random();
            if (_fighters.Count <= 1)
            {
                _fighters.Add(new Fighter("Браго", int.MaxValue / 500, 0, int.MaxValue / 500, "Это Браго"));
                _fighters.Add(new Fighter("Мурад", randomEnemy.Next(0, int.MaxValue / 500),
                    randomEnemy.Next(0, 11),
            randomEnemy.Next(0, int.MaxValue / 500), "Загадочный и непостижимый"));
            }
            int enemyFighterNumber = randomEnemy.Next(0, _fighters.Count);
            Fighter enemyFighter = _fighters[enemyFighterNumber];
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Противник догнал вас, но вы готовы дать ему отпор!\n\n" +
                "Вашим соперником окажется кто-то из нижеперечисленных бойцов\n");
            for (int i = 0; i < _fighters.Count; i++)
            {
                Console.Write(i + 1 + " ");
                _fighters[i].ShowFighterStats();
                Console.WriteLine();
            }
            Console.WriteLine("Нажмите любую клавишу, чтобы встретиться со своей судьбой");
            Console.ReadKey();
            Console.WriteLine($"Вашим сопреником оказался {enemyFighter.Name}");
            enemyFighter.ShowFighterStats();
            Thread.Sleep(1500);
            Console.WriteLine("Битва началась");
            while (Health > 0 & enemyFighter.Health > 0)
            {
                Thread.Sleep(150);
                enemyFighter.TakeDamage(Damage);
                TakeDamage(enemyFighter.Damage);
                ShowRoundStatistic(enemyFighter.Damage);
                enemyFighter.ShowRoundStatistic(Damage);
                Console.WriteLine(new string('-', 70));
            }
            if (Health <= 0)
            {
                PlayerIsDead = true;
                Console.WriteLine("Вы проиграли");
            }
            else
            {
                _fighters.Remove(enemyFighter);
                Console.WriteLine("Вы победили этого противника, пока что...");
            }
            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить\n" +
                "Не пугайтесь, что увидите пустую карту, после первго же вашего хода " +
                "всё вернётся в норму, если вы выжили, конечно");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
