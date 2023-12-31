﻿namespace HodimBrodim
{
    public class Player : Fighter
    {
        public Point Position { get; private set; }
        public int MovesAvailable { get; set; }
        public bool IsDead => MovesAvailable <= 0 || Health <= 0;
        public int TreasureCount { get; private set; }

        private readonly List<Fighter> _enemyFighters;
        private Random _random;

        public Player(Point position, int moves) : base("игрок", 150, 2, 25, "Хороший вопрос")
        {
            _random = new Random();
            Position = position;
            MovesAvailable = moves;

            _enemyFighters = new List<Fighter>()
            {
                new Fighter("Сумасшедший Маньяк", 200f + _random.Next(-20,21) ,
            2 + _random.Next(-1,2), 75f + _random.Next(-7,8),
                "в зависимости от степени чесания головы меняет свои характеристики"),
                new Fighter("Сын маминой подруги", 10f, 9.25f, 100f,"обладает сюжетной бронёй"),
                new Fighter("Обезьяна", 500f, 2, 25f," мозгов нет, здоровья много"),
                new Fighter("Ноутбук ирбис", 1000f, 2, 3.5f,"легенда, если проиграет попадёт к вам на стол." +
                "Вы точно хотите этого?")
            };
        }

        public void AddTreasure()
        {
            TreasureCount++;
        }
        public void Move(ConsoleKey pressedKey, GameMap map)
        {
            var offset = GetOffsetPoint(pressedKey);
           
            if (map.IsNotWall(Position + offset))
                Position += offset;

            MovesAvailable--;
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
            Console.WriteLine($"Осталось ходов: {MovesAvailable} ");
            Console.WriteLine($"Счётчик сокровищ: {TreasureCount}");
        }
        public void FightWithEnemy()
        {
            var enemyFighter = GetRandomEnemy();
            ShowEnemyStats(enemyFighter);
            LaunchFight(enemyFighter);
            ShowResult();
        }
        public void RaiseStats()
        {
            Health *= 1.1f;
            Damage *= 1.1f;
            Armor += 0.2f;
        }

        private Fighter GetRandomEnemy()
        {
            if (_enemyFighters.Count <= 1)
                AddSecretEnemies();

            var enemyFighter = _enemyFighters[_random.Next(_enemyFighters.Count)];
            return enemyFighter;
        }
        private void ShowResult()
        {
            Console.WriteLine(IsDead ? "Вы проиграли" : "Вы победили этого противника, пока что...");

            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить\n");
            Console.ReadKey();
            Console.Clear();
        }
        private void LaunchFight(Fighter enemyFighter)
        {
            LaunchBattleCycle(enemyFighter);
            if (Health > 0)
                _enemyFighters.Remove(enemyFighter);
        }
        private void LaunchBattleCycle(Fighter enemyFighter)
        {
            while (Health > 0 && enemyFighter.Health > 0)
            {
                Thread.Sleep(150);
                enemyFighter.TakeDamage(Damage);
                TakeDamage(enemyFighter.Damage);
                ShowRoundStatistic(enemyFighter.Damage);
                enemyFighter.ShowRoundStatistic(Damage);
                Console.WriteLine(new string('-', 70));
            }
        }
        private void ShowEnemyStats(Fighter enemyFighter)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Противник догнал вас, но вы готовы дать ему отпор!\n\n" +
                "Вашим соперником окажется кто-то из нижеперечисленных бойцов\n");
            for (int i = 0; i < _enemyFighters.Count; i++)
            {
                Console.Write(i + 1 + " ");
                _enemyFighters[i].ShowFighterStats();
                Console.WriteLine();
            }
            Console.WriteLine("Нажмите любую клавишу, чтобы встретиться со своей судьбой");
            Console.ReadKey();
            Console.WriteLine($"Вашим сопреником оказался {enemyFighter.Name}");
            enemyFighter.ShowFighterStats();
            Thread.Sleep(1500);
            Console.WriteLine("Битва началась");
        }
        private void AddSecretEnemies()
        {
            const int veryBigHealth = 4294967;
            _enemyFighters.Add(new Fighter("Браго", veryBigHealth, 0, veryBigHealth, "Это Браго"));
            _enemyFighters.Add(new Fighter("Мурад", _random.Next(0, veryBigHealth),
                _random.Next(0, 11),
                _random.Next(0, int.MaxValue / 500), "Загадочный и непостижимый"));
        }
        private static Point GetOffsetPoint(ConsoleKey pressedKey) => pressedKey switch
        {
            ConsoleKey.W => new Point(0, -1),
            ConsoleKey.A => new Point(-1, 0),
            ConsoleKey.S => new Point(0, 1),
            ConsoleKey.D => new Point(1, 0),
            _ => new Point(0, 0)
        };
    }
}
