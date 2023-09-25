using System.Drawing;

namespace HodimBrodim
{
    public class Player : Fighter
    {
        public Point Position { get; private set; }
        public int MovesAvailable { get; set; }
        public bool PlayerIsDead { get; private set; }
        public int TreasureCount { get; private set; }

        private readonly List<Fighter> _enemyFighters;

        public Player(Point position, int moves) : base("игрок", 150, 2, 25, "Хороший вопрос")
        {
            Random rand = new Random();
            Position = position;
            MovesAvailable = moves;

            _enemyFighters = new List<Fighter>()
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
        public void Move(ConsoleKeyInfo pressedKey, GameMap map)
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
            Fighter enemyFighter = GetRandomEnemy();
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
            Random random = new Random();
            if (_enemyFighters.Count <= 1)
                AddSecretEnemies();

            Fighter enemyFighter = _enemyFighters[random.Next(_enemyFighters.Count)];
            return enemyFighter;
        }
        private void ShowResult()
        {
            if (Health <= 0)
            {
                PlayerIsDead = true;
                Console.WriteLine("Вы проиграли");
            }
            else
                Console.WriteLine("Вы победили этого противника, пока что...");

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
            var random = new Random();
            _enemyFighters.Add(new Fighter("Браго", int.MaxValue / 500, 0, int.MaxValue / 500, "Это Браго"));
            _enemyFighters.Add(new Fighter("Мурад", random.Next(0, int.MaxValue / 500),
                random.Next(0, 11),
                random.Next(0, int.MaxValue / 500), "Загадочный и непостижимый"));
        }
        private Point GetOffsetPoint(ConsoleKeyInfo pressedKey) => pressedKey.Key switch
        {
            ConsoleKey.W => new(0, -1),
            ConsoleKey.A => new(-1, 0),
            ConsoleKey.S => new(0, 1),
            ConsoleKey.D => new(1, 0),
            _ => new(0, 0)
        };
    }
}
