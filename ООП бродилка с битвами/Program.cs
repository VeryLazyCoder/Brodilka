

namespace HodimBrodim
{
    class GovnoCode
    {
        static void Main()
        {
            bool showresult = false;
            int previousResult = 0;
            bool ifWin = false;
            bool everWin = false;
            int maxWinResult = int.MaxValue;
            int maxLoseResult = 0;
            string path;
            int movesAvailable = 300;
            GetMapSize(out path,ref movesAvailable);
            Console.WriteLine("Выберите число противников: 2,4, либо 6\nЕсли введёте что-то другое - будете играть с 6 противниками ");
            string enemyCount = Console.ReadLine();
            if (enemyCount != "2" & enemyCount != "4" & enemyCount != "6")
            {
                enemyCount = "6";
            }
            int startMoves = movesAvailable;
            Random randFighterStatsModificator = new Random();
            GiveHelpToPlayer();
        loop1:
            List<Fighter> fighters = new List<Fighter>()
            {
                new Fighter("игрок",150,2,25,"это игрок"),
                new Fighter("Копоч", 200f + randFighterStatsModificator.Next(-20,21) ,
                2 + randFighterStatsModificator.Next(-1,2), 75f + randFighterStatsModificator.Next(-7,8),
                "в зависимости от степени чесания головы меняет свои характеристики"),
                new Fighter("Спартак", 10f, 9.25f, 100f,"обладает сюжетной бронёй"),
                new Fighter("Голышев", 500f, 2, 25f,"обезьяна, мозгов нет, но хп много"),
                new Fighter("Ноутбук ирбис", 1000f, 8, 3.5f,"легенда, если проиграет попадёт к вам на стол." +
                "Вы точно хотите этого?")

            };
            char[,] map = ReadMap(path);
            DrawSymbolOnEmptyCell(map, 'A');
            DrawSymbolOnEmptyCell(map, 'D');
            DrawSymbolOnEmptyCell(map, 'H');
            Random random = new Random();
            int x = random.Next(1, map.GetLength(0));
            List<Enemy> enemies = new List<Enemy>()
            {
                new Enemy(x,GetY(map,x)),
                new Enemy(x,GetY(map,x)),
                new Enemy(map.GetLength(0)-2,map.GetLength(1)-2),
                new Enemy(1,1),
                new Enemy(x,GetY(map,x)),
                new Enemy(x,GetY(map,x)),
            };
            ChooseEnemyCount(enemies, enemyCount);
            bool wannaPlay = true;

            int playerX, playerY;
            PlayerSpot(map, out playerX, out playerY);
            int Treasures = 0;
            movesAvailable = startMoves;

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 'X')
                    {
                        Treasures += 1;
                    }
                }
            }
            int TreasureCount = 0;
            Console.Clear();

            while (wannaPlay == true)
            {

                if (showresult == true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    if (ifWin == true)
                    {
                        Console.SetCursorPosition(40, 16);
                        Console.WriteLine($"Ваш предудущий результат {previousResult} ходов");
                        Console.SetCursorPosition(40, 17);
                        Console.WriteLine($"Ваш лучший результат {maxWinResult} ходов");
                    }
                    else
                    {
                        Console.SetCursorPosition(40, 16);
                        Console.WriteLine($"Ваш предудущий результат {previousResult} собранных сокровищ");
                        Console.SetCursorPosition(40, 17);
                        if (maxLoseResult > Treasures)
                        {
                            Console.WriteLine($"Ваш лучший результат {maxLoseResult} ходов");
                        }
                        else
                        {
                            Console.WriteLine($"Ваш лучший результат {maxLoseResult} собранных сокровищ");
                        }
                    }
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(40, 0);
                Console.WriteLine($"Ваше здоровье {fighters[0].Health}");
                Console.SetCursorPosition(40, 1);
                Console.WriteLine($"Ваш урон составляет {fighters[0].Damage}");
                Console.SetCursorPosition(40, 2);
                Console.WriteLine($"Ваша броня  {fighters[0].Armor}");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.SetCursorPosition(0, 21);
                Console.WriteLine($"Осталось ходов: {movesAvailable--} ");

                Console.WriteLine($"Счётчик сокровищ: {TreasureCount}");
                DrawMap(map, ConsoleColor.DarkYellow, ConsoleColor.Cyan);
                bool playerKilled = false;
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].EnemyBehavior(map, playerY, playerX);
                    if (enemies[i].CollisionWithEnemy(playerY, playerX) == true)
                    {
                        FightWithEnemy(fighters, ref playerKilled);
                        enemies.RemoveAt(i);
                        DrawMap(map, ConsoleColor.DarkYellow, ConsoleColor.Cyan);

                        break;
                    }
                }
                if (movesAvailable <= 0 | playerKilled == true)
                {
                    Console.Clear();
                    Console.WriteLine("Вы не справились :( Игра окончена");
                    wannaPlay = false;
                    Console.WriteLine("Хотите улучшить результат? да/нет");
                    string A = Console.ReadLine();
                    showresult = true;
                    if (A == "да" | A == "if" | A == "lf" | A == "fl" | A == "ад")
                    {
                        Console.Clear();
                        previousResult = TreasureCount;
                        ifWin = false;
                        switch (everWin)
                        {
                            case true:
                                maxLoseResult = maxWinResult;
                                break;
                            case false:
                                if (previousResult >= maxLoseResult)
                                {
                                    maxLoseResult = previousResult;
                                }
                                break;
                        }
                        goto loop1;
                    }
                }
                Console.SetCursorPosition(playerX, playerY);
                Painter('T', ConsoleColor.Green);
                ConsoleKeyInfo pressedKey = Console.ReadKey();
                HandleInput(pressedKey, ref playerX, ref playerY, map);

                if (map[playerX, playerY] == 'X')
                {
                    TreasureCount += 1;
                    map[playerX, playerY] = ' ';
                    fighters[0].Health = fighters[0].Health / 10;
                    fighters[0].Damage = fighters[0].Damage / 10;
                }
                if (map[playerX, playerY] == '@')
                {
                    movesAvailable -= 10;

                }
                if (map[playerX, playerY] == 'D')
                {
                    fighters[0].Damage = fighters[0].Damage / 4;
                    map[playerX, playerY] = ' ';
                }
                if (map[playerX, playerY] == 'A')
                {
                    fighters[0].Armor = 2;
                    map[playerX, playerY] = ' ';
                }
                if (map[playerX, playerY] == 'H')
                {
                    fighters[0].Health = fighters[0].Health / 4;
                    map[playerX, playerY] = ' ';
                }
                if (TreasureCount == Treasures | enemies.Count == 0)
                {
                    Console.Clear();
                    Console.WriteLine($"Вы победиди за {startMoves - movesAvailable} ходов, поздравляю!!!");
                    wannaPlay = false;
                    everWin = true;
                    previousResult = startMoves - movesAvailable;
                    showresult = true;
                    ifWin = true;
                    if (previousResult <= maxWinResult)
                    {
                        maxWinResult = previousResult;
                    }
                    Console.WriteLine("Хотите улучшить результат? да/нет");
                    string A = Console.ReadLine();
                    if (A == "да" | A == "if" | A == "lf" | A == "fl" | A == "ад")
                    {
                        Console.Clear();
                        goto loop1;
                    }
                }
            }
            Console.Clear();
            if (everWin == true)
            {
                Console.WriteLine($" Ваш лучший результат составил {maxWinResult} ходов(а)");
            }
            else if (everWin == false)
            {
                Console.WriteLine(" Ваш лучший результат составил " + TreasureCount + " сокровищ(а)");
            }
            Console.ReadKey();
        }


        public static void Painter(char entity, ConsoleColor color)
        {
            ConsoleColor defaultcolor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(entity);
            Console.ForegroundColor = defaultcolor;
        }
        static void DogPainter(string entity, ConsoleColor color)
        {
            ConsoleColor defaultcolor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(entity);
            Console.ForegroundColor = defaultcolor;
        }
        static void GiveHelpToPlayer()
        {
            Console.CursorVisible = false;
            Console.Clear();
            Console.WriteLine("Краткая справка:\n");
            Console.WriteLine($"Добро пожаловать в тестовую бродилку. Ваш персонаж обозначен буквой Т" +
            "\nЧтобы двигаться вверх нажмите W, вниз S, вправо D, а влево A. " +
            "\nНо берегитесь врагов, обозначенных '!' " +
            "\nДля выхода нажмите Esc");
            Console.WriteLine("По лабиринту разбросаны сокровища. Они обозначены буквой Х. " +
            "Соберите их все и сможете победить!");
            DogPainter("Берегитесь злых собак '@', они вас сильно задерживают!!!\n\n", ConsoleColor.Red);
            Console.WriteLine("Теперь игра не заканчивается, если вы встретились с врагом\n" +
                "Вы можете дать ему отпор, ваше здоровье, броня и атака указаны в правом верхнем углу.\n" +
                "Напомню, что каждая единица брони уменьшает входящий урон на 10%\n" +
                "С каждым собранным сокровищем ваши характеристики улучшаются\n" +
                "Кроме того на карте находятся специальные предметы (D - атака,A - защита,H - здоровье),\n " +
                "значительно повышающие ваши характеристики.\n" +
                "Их не обязательно собирать, но они сильно помогут вам в схватке с врагами");
            Console.WriteLine("Если вы готовы, нажмите любую кнопку и вперёд!");
            Console.ReadKey();
        }
        static void PlayerSpot(char[,] map, out int playerY, out int playerX)
        {
            Random rand = new Random();
        loop3:
            int RandEnemyX = rand.Next(0, map.GetLength(0));
            int RandEnemyY = rand.Next(0, map.GetLength(1));
            if (map[RandEnemyX, RandEnemyY] != '-' & map[RandEnemyX, RandEnemyY] != '|' & map[RandEnemyX, RandEnemyY] != 'X' & map[RandEnemyX, RandEnemyY] != '@')
            {
                playerX = RandEnemyY;
                playerY = RandEnemyX;
            }
            else
            {
                goto loop3;
            }
        }
        static string GetMapSize(out string path,ref int movesAvalaible)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Выберите размер карты:\n1 - маленькая \n2 - средняя" +
                "\n3 - большая, \nВнимание!!! Eсли введёте что-то другое придётся играть на большой карте");
            string mapVariant = Console.ReadLine();
            switch (mapVariant)
            {
                case "1":
                    path = "little.txt";
                    movesAvalaible = 120;
                    break;
                case "2":
                    path = "qwerty.txt";
                    movesAvalaible = 160;
                    break;
                case "3":
                    path = "bigmap.txt";
                    break;
                default:
                    path = "bigmap.txt";
                    break;
            }
            return path;
        }
        private static void HandleInput(ConsoleKeyInfo pressedKey, ref int playerX, ref int playerY, char[,] map)
        {
            int[] direction = GetDirection(pressedKey);
            int nextPositionX = playerX + direction[0];
            int nextPositionY = playerY + direction[1];
            if (map[nextPositionX, nextPositionY] != '-' & map[nextPositionX, nextPositionY] != '|')
            {
                playerX = nextPositionX;
                playerY = nextPositionY;
            }
        }
        private static int[] GetDirection(ConsoleKeyInfo pressedKey)
        {
            int[] direction = { 0, 0 };
            if (pressedKey.Key == ConsoleKey.W)
            {
                direction[1]--;
            }
            else if (pressedKey.Key == ConsoleKey.S)
            {
                direction[1]++;
            }
            else if (pressedKey.Key == ConsoleKey.A)
            {
                direction[0]--;
            }
            else if (pressedKey.Key == ConsoleKey.D)
            {
                direction[0]++;
            }
            else if (pressedKey.Key == ConsoleKey.Escape)
            {
                Console.Clear();
                Console.WriteLine("пЖаль, что вы уходите :(");
                Environment.Exit(0);
            }
            return direction;
        }
        private static char[,] ReadMap(string path)
        {
            string[] file = File.ReadAllLines(path);
            char[,] map = new char[file[0].Length, file.Length];
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    map[x, y] = file[y][x];
                }
            }
            return map;
        }
        private static void DrawMap(char[,] map, ConsoleColor color, ConsoleColor treasureColor)
        {
            ConsoleColor defaultcolor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    if (map[x, y] == 'X')
                    {
                        Console.ForegroundColor = treasureColor;
                        Console.Write(map[x, y]);
                        Console.ForegroundColor = color;
                        continue;
                    }
                    if (map[x, y] == 'A' | map[x, y] == 'D' | map[x, y] == 'H')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(map[x, y]);
                        Console.ForegroundColor = color;
                        continue;
                    }
                    if (map[x, y] == '@')
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(map[x, y]);
                        Console.ForegroundColor = color;
                        continue;
                    }
                    Console.Write(map[x, y]);

                }
                Console.WriteLine();
            }

        }

        static int GetY(char[,] map, int mapX)
        {
            bool correctY = false;
            int y = 1;
            while (correctY == false)
            {
                Random rand = new Random();
                y = rand.Next(0, map.GetLength(1));
                if (map[mapX, y] != '-' & map[mapX, y] != '|' & map[mapX, y] != 'T')
                {
                    correctY = true;

                }

            }
            return y;
        }
        static void ChooseEnemyCount(List<Enemy> enemies, string numberOfEnemies)
        {
            switch (numberOfEnemies)
            {
                case "2":
                    for (int i = 0; i < 4; i++)
                    {
                        enemies.RemoveAt(2);
                    }
                    break;
                case "4":
                    for (int i = 0; i < 2; i++)
                    {
                        enemies.RemoveAt(4);
                    }
                    break;
                case "6":

                    break;

            }
        }
        static void FightWithEnemy(List<Fighter> fighters, ref bool playerIsDead)
        {
            Fighter player = fighters[0];
            Random randomEnemy = new Random();
            if (fighters.Count <= 1)
            {
                fighters.Add(new Fighter("Браго", int.MaxValue / 500, 0, int.MaxValue / 500, "Это Браго"));
                fighters.Add(new Fighter("Мурад", randomEnemy.Next(0, int.MaxValue / 500),
                    randomEnemy.Next(0, 11),
            randomEnemy.Next(0, int.MaxValue / 500), "Загадочный и непостижимый"));
            }

            int enemyFighterNumber = randomEnemy.Next(1, fighters.Count);
            Fighter enemyFighter = fighters[enemyFighterNumber];
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Противник догнал вас, но вы готовы дать ему отпор!\n\n" +
                "Вашим соперником окажется кто-то из нижеперечисленных бойцов\n");

            for (int i = 1; i < fighters.Count; i++)
            {
                Console.Write(i + " ");
                fighters[i].ShowFighterStats();
                Console.WriteLine();
            }

            Console.WriteLine("Нажмите любую клавишу, чтобы встретиться со своей судьбой");
            Console.ReadKey();
            Console.WriteLine($"Вашим сопреником оказался {enemyFighter.Name}");
            enemyFighter.ShowFighterStats();
            Thread.Sleep(1500);
            Console.WriteLine("Битва началась");
            while (player.Health > 0 & enemyFighter.Health > 0)
            {
                Thread.Sleep(150);
                enemyFighter.TakeDamage(player.Damage);
                player.TakeDamage(enemyFighter.Damage);
                player.ShowRoundStatistic(player.Health, enemyFighter.Damage);
                enemyFighter.ShowRoundStatistic(enemyFighter.Health, player.Damage);
                Console.WriteLine(new string('-', 70));
            }

            if (player.Health <= 0)
            {
                playerIsDead = true;
                Console.WriteLine("Вы проиграли");
            }
            else
            {
                playerIsDead = false;
                fighters.Remove(enemyFighter);
                Console.WriteLine("Вы победили этого противника, пока что...");


            }
            Console.WriteLine("Нажмите любую клавишу, чтобы продолжить\n" +
                "Не пугайтесь, что увидите пустую карту, после первго же вашего хода " +
                "всё вернётся в норму, если вы выжили, конечно");

            Console.ReadKey();
            Console.Clear();

        }

        static void DrawSymbolOnEmptyCell(char[,] map, char symbol)
        {

            bool SymbolLocatioinIsCorrect = false;

            while (SymbolLocatioinIsCorrect == false)
            {
                Random random = new Random();
                int x = random.Next(1, map.GetLength(0));
                int y = random.Next(1, map.GetLength(1));
                if (map[x, y] == ' ')
                {
                    map[x, y] = symbol;
                    SymbolLocatioinIsCorrect = true;
                }
            }
        }
    }

    class Enemy
    {
        private int _enemyX;
        private int _enemyY;
        private int _previousEnemyX;
        private int _previousEnemyY;

        public Enemy(int enemyX, int enemyY)
        {
            _enemyX = enemyX;
            _enemyY = enemyY;
            _previousEnemyX = enemyX;
            _previousEnemyY = enemyY;
        }
        public void EnemyBehavior(char[,] map, int playerX, int playerY)
        {
            Random rd = new Random();
            bool RightPosition = false;

            while (RightPosition == false)
            {
                _previousEnemyX = _enemyX;
                _previousEnemyY = _enemyY;
                switch (rd.Next(1, 5))
                {
                    case 1:
                        if (map[_enemyX - 1, _enemyY] != '-' & map[_enemyX - 1, _enemyY] != '_' & map[_enemyX - 1, _enemyY] != '|')
                        {
                            _enemyX--;
                            RightPosition = true;
                        }
                        break;
                    case 2:
                        if (map[_enemyX + 1, _enemyY] != '-' & map[_enemyX + 1, _enemyY] != '_' & map[_enemyX + 1, _enemyY] != '|')
                        {
                            _enemyX++;
                            RightPosition = true;
                        }
                        break;
                    case 3:
                        if (map[_enemyX, _enemyY - 1] != '-' & map[_enemyX, _enemyY - 1] != '_' & map[_enemyX, _enemyY - 1] != '|')
                        {
                            _enemyY--;
                            RightPosition = true;
                        }
                        break;
                    case 4:
                        if (map[_enemyX, _enemyY + 1] != '-' & map[_enemyX, _enemyY + 1] != '_' & map[_enemyX, _enemyY + 1] != '|')
                        {
                            _enemyY++;
                            RightPosition = true;
                        }
                        break;
                }
            }
            Console.SetCursorPosition(_enemyX, _enemyY);
            GovnoCode.Painter('!', ConsoleColor.Red);
        }
        public bool CollisionWithEnemy(int playerX, int playerY)
        {
            if (playerX == _enemyY & playerY == _enemyX || playerX == _previousEnemyY & playerY == _previousEnemyX)
            {
                return true;
            }
            return false;
        }
    }

    class Fighter
    {
        protected float _health;
        protected float _armor;
        protected float _damage;
        protected string _name;
        protected string _specialAbilities;

        public Fighter(string name, float health, float armor, float damage,
            string special = " отсутствуют")
        {
            _name = name;
            _health = health;
            _armor = armor;
            _damage = damage;
            _specialAbilities = special;
        }

        public void ShowFighterStats()
        {
            Console.WriteLine($"Боец {_name} обладает {_health} хп, " +
                $"{_armor} брони и наносит {_damage} урона. \nСпециальные способности: {_specialAbilities}");
        }
        public void TakeDamage(float damage)
        {
            float trueDamage = damage - (damage * (_armor / 10));
            _health -= trueDamage;
        }

        public float Health
        {
            get { return _health; }
            set { _health += value; }
        }
        public float Damage
        {
            get { return _damage; }
            set { _damage += value; }
        }
        public string Name
        {
            get { return _name; }
        }
        public float Armor
        {
            get { return _armor; }
            set { _armor += value; }
        }
        public void ShowRoundStatistic(float fighterHealth, float enemyFighterDamage)
        {
            Console.WriteLine($"По персонажу {_name} нанесено " +
                $"{enemyFighterDamage - (enemyFighterDamage * (_armor / 10))} " +
                $"урона, у него осталось {fighterHealth} здоровья");

        }
    }
}