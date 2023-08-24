
namespace HodimBrodim
{
    class GovnoCode
    {
        public static int MovesAvailable = 300;
        static void Main()
        {
            bool showresult = false;
            int previousResult = 0;
            bool ifWin = false;
            bool everWin = false;
            int maxWinResult = int.MaxValue;
            int maxLoseResult = 0;
            string path, pathOfRecordsFile;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Выберите размер карты:\n1 - маленькая \n2 - средняя" +
                "\n3 - большая, \nВнимание!!! Eсли введёте что-то другое придётся играть на большой карте");
            string mapVariant = Console.ReadLine();
            switch (mapVariant)
            {
                case "1":
                    path = "little.txt";
                    pathOfRecordsFile = "recordsOnLittle.txt";
                    MovesAvailable = 120;
                    break;
                case "2":
                    path = "qwerty.txt";
                    pathOfRecordsFile = "recordsOnMiddle.txt";
                    MovesAvailable = 160;
                    break;
                default:
                    path = "bigmap.txt";
                    pathOfRecordsFile = "recordsOnBig.txt";
                    break;
            }
            Console.WriteLine("Выберите число противников: 2,4, либо 6" +
                "\nЕсли введёте что-то другое - будете играть с 6 противниками ");
            string enemyCount = Console.ReadLine();
            if (enemyCount != "2" & enemyCount != "4" & enemyCount != "6")
            {
                enemyCount = "6";
            }
            bool wannaExtrim = false;
            Console.WriteLine("Хотите ли вы экстрима?. Введите '+'");
            if (Console.ReadLine() == "+")
            {
                wannaExtrim = true;
            }
            
            int startMoves = MovesAvailable;
            Console.CursorVisible = false;
            GiveAdviceToPlayer();
        loop1:
            bool successfulEscape = false;
            Random random = new Random();
            List<Fighter> fighters = new List<Fighter>()
            {
                new Fighter("игрок",150,2,25,"это игрок"),
                new Fighter("Копоч", 200f + random.Next(-20,21) ,
                2 + random.Next(-1,2), 75f + random.Next(-7,8),
                "в зависимости от степени чесания головы меняет свои характеристики"),
                new Fighter("Спартак", 10f, 9.25f, 100f,"обладает сюжетной бронёй"),
                new Fighter("Голышев", 500f, 2, 25f,"обезьяна, мозгов нет, но хп много"),
                new Fighter("Ноутбук ирбис", 1000f, 8, 3.5f,"легенда, если проиграет попадёт к вам на стол." +
                "Вы точно хотите этого?")
            };
            List<PlayerInfo> playerInfo = new List<PlayerInfo>();        
            char[,] map = ReadMap(path);
            DrawSymbolOnEmptyCell(map, 'A');
            DrawSymbolOnEmptyCell(map, 'D');
            DrawSymbolOnEmptyCell(map, 'H');
            int playerX = 0, playerY = 0;
            PlayerSpot(map, ref playerX, ref playerY);
            List<Enemy> enemies = new List<Enemy>();
            for (int i = 0; i < 6; i++)
            {
                int randx = random.Next(1, map.GetLength(0));
                int x = randx;
                enemies.Add(new Enemy(x, GetY(map, x)));
            }
            enemies[2] = new Enemy(map.GetLength(0) - 2, map.GetLength(1) - 2);
            enemies[3] = new Enemy(1, 1);
            ChooseEnemyCount(enemies, enemyCount);
            bool wannaPlay = true;
            int Treasures = 0;
            MovesAvailable = startMoves;
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
                ShowPlayerStatistic(fighters, TreasureCount);
                DrawMap(map, ConsoleColor.DarkYellow, ConsoleColor.Cyan);
                bool playerKilled = false;
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].EnemyBehavior(map);
                    if (enemies[i].CollisionWithEnemy(playerY, playerX))
                    {
                        FightWithEnemy(fighters, ref playerKilled);
                        enemies.RemoveAt(i);
                        DrawMap(map, ConsoleColor.DarkYellow, ConsoleColor.Cyan);
                        break;
                    }
                }
                RandomEvents.RandomEvent(map, ref Treasures, fighters, ref playerKilled, enemies);
                if (map[playerX, playerY] == 'X')
                {
                    TreasureCount += 1;
                    map[playerX, playerY] = ' ';
                    fighters[0].Health = fighters[0].Health / 10;
                    fighters[0].Damage = fighters[0].Damage / 10;
                    if (wannaExtrim == true)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Сыграем в рулеточку?) Выберите чисто от 1 до 3");
                        int playerNumber = Convert.ToInt32(Console.ReadLine());
                        int luckeyNumber = random.Next(1, 4);
                        Console.WriteLine($"Правильным числом было {luckeyNumber}");
                        Thread.Sleep(1000);
                        if (playerNumber != luckeyNumber)
                        {
                            EscapeAfterCollectingTreasures(map, fighters, playerX,
                                playerY, ref playerKilled);
                        }
                        Console.Clear();
                    }
                }
                if (map[playerX, playerY] == '@')
                {
                    MovesAvailable -= 10;
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
                    EscapeAfterCollectingTreasures(map, fighters, playerX, playerY, ref playerKilled);
                    successfulEscape = true;
                }
                if (MovesAvailable <= 0 | playerKilled == true)
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
                    break;
                }
                if (playerKilled == false & successfulEscape == true)
                {
                    Console.Clear();
                    Console.WriteLine($"Вы победиди за {startMoves - MovesAvailable} ходов, поздравляю!!!");
                    wannaPlay = false;
                    everWin = true;
                    previousResult = startMoves - MovesAvailable;
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
                Console.SetCursorPosition(playerX, playerY);
                Painter('T', ConsoleColor.Green);
                ConsoleKeyInfo pressedKey = Console.ReadKey();
                ShowRecordsTable(pressedKey, pathOfRecordsFile, playerInfo);
                HandleInput(pressedKey, ref playerX, ref playerY, map);
            }
            Console.Clear();
            if (everWin == true)
            {
                Console.WriteLine($" Ваш лучший результат составил {maxWinResult} ходов(а)");
                AddRecords(pathOfRecordsFile, playerInfo, maxWinResult);
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
        private static void StringPainter(string entity, ConsoleColor color)
        {
            ConsoleColor defaultcolor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(entity);
            Console.ForegroundColor = defaultcolor;
        }
        private static void GiveAdviceToPlayer()
        {
            Console.Clear();
            Console.WriteLine("Краткая справка:\n");
            Console.WriteLine($"Добро пожаловать в тестовую бродилку. Ваш персонаж обозначен буквой Т" +
            "\nЧтобы двигаться вверх нажмите W, вниз S, вправо D, а влево A. " +
            "\nНо берегитесь врагов, обозначенных '!' " +
            "\nДля выхода нажмите Esc");
            Console.WriteLine("По лабиринту разбросаны сокровища. Они обозначены буквой Х. " +
            "Соберите их все и сможете победить!");
            StringPainter("Берегитесь злых собак '@', они вас сильно задерживают!!!\n\n", ConsoleColor.Red);
            Console.WriteLine("Теперь игра не заканчивается, если вы встретились с врагом\n" +
                "Вы можете дать ему отпор, ваше здоровье, броня и атака указаны в правом верхнем углу.\n" +
                "Напомню, что каждая единица брони уменьшает входящий урон на 10%\n" +
                "С каждым собранным сокровищем ваши характеристики улучшаются\n" +
                "Кроме того на карте находятся специальные предметы (D - атака,A - защита,H - здоровье),\n " +
                "значительно повышающие ваши характеристики.\n" +
                "Их не обязательно собирать, но они сильно помогут вам в схватке с врагами\n" +
                "\nВы можете в любой момент посмотреть рекорды других игроков, для этого нажмите клавишу 'R'\n" +
                "Наконец, будьте готовы к неожиданным поворатам, увидев следующую надпись ");
            StringPainter("Случилось страшное!? Вращайте барабан", ConsoleColor.Blue);
            Console.WriteLine("\n\nЕсли вы готовы, нажмите любую кнопку и вперёд!");
            Console.ReadKey();
        }
        private static void PlayerSpot(char[,] map, ref int playerY, ref int playerX)
        {
            Random rand = new Random();


            bool spotIsCorrect = false;
            while (spotIsCorrect == false)
            {
                int RandEnemyX = rand.Next(0, map.GetLength(0));
                int notRandX = RandEnemyX;
                int RandEnemyY = rand.Next(0, map.GetLength(1));
                int notRandY = RandEnemyY;
                if (map[notRandX, notRandY] == ' ')
                {
                    playerX = notRandY;
                    playerY = notRandX;
                    spotIsCorrect = true;
                    map[playerY, playerX] = 'T';
                }
            }


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
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("ZЗря вы покинули такую прекрасную игру :(");
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
        public static void DrawMap(char[,] map, ConsoleColor color, ConsoleColor treasureColor)
        {
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
                    if (map[x, y] == 'Z' | map[x, y] == 'V')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
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
                    if (map[x, y] == 'T')
                    {
                        map[x, y] = ' ';
                    }
                    Console.Write(map[x, y]);

                }
                Console.WriteLine();
            }

        }
        private static void ShowPlayerStatistic(List<Fighter> fighters, int TreasureCount)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(40, 0);
            Console.WriteLine($"Ваше здоровье {fighters[0].Health}");
            Console.SetCursorPosition(40, 1);
            Console.WriteLine($"Ваш урон составляет {fighters[0].Damage}");
            Console.SetCursorPosition(40, 2);
            Console.WriteLine($"Ваша броня  {fighters[0].Armor}");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.SetCursorPosition(0, 21);
            Console.WriteLine($"Осталось ходов: {MovesAvailable--} ");
            Console.WriteLine($"Счётчик сокровищ: {TreasureCount}");
        }
        public static int GetY(char[,] map, int mapX)
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
        private static void ChooseEnemyCount(List<Enemy> enemies, string numberOfEnemies)
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
            }
        }
        public static void FightWithEnemy(List<Fighter> fighters, ref bool playerIsDead)
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
        private static void DrawSymbolOnEmptyCell(char[,] map, char symbol)
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
        private static void EscapeAfterCollectingTreasures(char[,] map, List<Fighter> fighters, int playerX, int playerY,
            ref bool isDead)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Вы сорвали сокровище с постамента и лабиринт начал разрушаться.\n" +
                "Вас может завалить камнями, как можно скорее добегите до выхода" +
                ", обозначеного буквой V\n" +
                "Но перед этим вам необходимо подобрать символ Z, без него вы не сможете не уйти\n" +
                "С каждым вашим ходом вы будите терять здоровье, поспешите!!!");
            Console.ReadKey();
            Console.Clear();
            DrawSymbolOnEmptyCell(map, 'Z');
            DrawSymbolOnEmptyCell(map, 'V');
            bool pickZ = false;
            while (fighters[0].Health > 0)
            {
                DrawMap(map, ConsoleColor.DarkYellow, ConsoleColor.DarkGreen);
                Console.SetCursorPosition(40, 0);
                Console.WriteLine($"Ваше здоровье {fighters[0].Health}");
                Console.SetCursorPosition(playerX, playerY);
                Painter('T', ConsoleColor.Green);
                ConsoleKeyInfo pressedKey = Console.ReadKey();
                HandleInput(pressedKey, ref playerX, ref playerY, map);
                if (map[playerX, playerY] == 'Z')
                {
                    pickZ = true;
                    map[playerX, playerY] = ' ';
                    fighters[0].Health = fighters[0].Health / 3;
                }
                if (map[playerX, playerY] == 'V' & pickZ == true)
                {
                    map[playerX, playerY] = ' ';
                    break;
                }
                fighters[0].Health = -5;
            }
            if (fighters[0].Health <= 0)
            {
                isDead = true;
            }
            Console.Clear();
        }
        private static void AddRecords(string pathOfRecordsFile, List<PlayerInfo> playerInfo, int playerScore)
        {
            ShowRecordsTable(new ConsoleKeyInfo('R',ConsoleKey.R, false, false, false), pathOfRecordsFile, playerInfo);
            PlayerInfo.SplitReadingFile(pathOfRecordsFile, playerInfo);
            Console.WriteLine($"Хотите внести свой результат ({playerScore} ходов) в таблицу? (для этого введите '+')");
            if (Console.ReadLine() == "+")
            {
                Console.Write("Введите ваше имя  ");
                string nameOfPlayer = Console.ReadLine();
                playerInfo.Add(new PlayerInfo(nameOfPlayer, playerScore, DateTime.Now));
            }
            var filteredRecords = playerInfo.OrderBy(player => player.Score);
            playerInfo = filteredRecords.ToList();

            List<string> newList = new List<string>();
            foreach (var player in playerInfo)
            {
                newList.Add($"{player.Name};{player.Score};{player.Date}");
            }
            if (newList.Count > 10)
            {
                newList.RemoveAt(10);
            }
            File.WriteAllLines(pathOfRecordsFile, newList);
            
            Console.WriteLine("Чтобы увидеть обновлённую таблицу нажмите 'R'");
            ConsoleKeyInfo pressedKey = Console.ReadKey();
            playerInfo.RemoveRange(0, playerInfo.Count);
            ShowRecordsTable(pressedKey, pathOfRecordsFile, playerInfo);
        }
        private static void ShowRecordsTable(ConsoleKeyInfo pressedKey, string pathOfRecordsFile, 
            List<PlayerInfo> playerInfo)
        {
            if (pressedKey.Key == ConsoleKey.R)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Clear ();
                PlayerInfo.SplitReadingFile(pathOfRecordsFile, playerInfo);
                Console.WriteLine("Таблица рекордов\n");
                foreach (var player in playerInfo)
                {
                    Console.WriteLine($"Игрок {player.Name} победил за {player.Score} ходов. Рекорд был установлен " +
                        $"{player.Date}");
                    
                }
                playerInfo.RemoveRange(0,playerInfo.Count);
                Console.ReadKey();
                MovesAvailable++;
                Console.Clear();
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
        public void EnemyBehavior(char[,] map)
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
                        if (map[_enemyX - 1, _enemyY] != '-' & map[_enemyX - 1, _enemyY] != '|')
                        {
                            _enemyX--;
                            RightPosition = true;
                        }
                        break;
                    case 2:
                        if (map[_enemyX + 1, _enemyY] != '-' & map[_enemyX + 1, _enemyY] != '|')
                        {
                            _enemyX++;
                            RightPosition = true;
                        }
                        break;
                    case 3:
                        if (map[_enemyX, _enemyY - 1] != '-' & map[_enemyX, _enemyY - 1] != '|')
                        {
                            _enemyY--;
                            RightPosition = true;
                        }
                        break;
                    case 4:
                        if (map[_enemyX, _enemyY + 1] != '-' & map[_enemyX, _enemyY + 1] != '|')
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
            if (playerX == _enemyY & playerY == _enemyX ||
                playerX == _previousEnemyY & playerY == _previousEnemyX)
            {
                return true;
            }
            return false;
        }
    }
    class Fighter
    {
        private float _health;
        private float _armor;
        private float _damage;
        private string _name;
        private string _specialAbilities;
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
    class RandomEvents
    {
        private string _nameOfEvent;
        private string _description;
        public RandomEvents(string nameOfEvent, string description)
        {
            _nameOfEvent = nameOfEvent;
            _description = description;
        }
        private static List<RandomEvents> _events = new List<RandomEvents>()
        {
            new RandomEvents("К вам пришла налоговая","вы не в состоянии думать ни о чём, кроме налогов. " +
                "\nПропустите 5 ходов и платите налоги вовремя"),
            new RandomEvents("К вам в руки попала новая карта лабиринта","на ней обнаружилось новое сокровище"),
            new RandomEvents("Вы нашли ковёр самолёт"," у вас стало больше времени. " +
                "Получите 5 ходов"),
            new RandomEvents("На вас напали конкуренты","придётся с ними сразиться"),
            new RandomEvents("Вы протёрли свои очки и увидели нового врага","придётся перемещаться осторожнее :)"),
            new RandomEvents("Вы нашли подозрительную синюю таблетку с лекарством",
                "Приняв её вы выяснили, что здоровье увеличилось"),
            new RandomEvents("Вы нашли совсем неподозрительную красную таблетку",
                "Выпив её ваше здоровье почему-то уменьшилось"),
            new RandomEvents("Мы решили вас похвалить","Прочитайте комплимент в ваш адрес :)")
        };
        public static void RandomEvent(char[,] map, ref int treasures, List<Fighter> fighters, ref bool playerIsDead,
            List<Enemy> enemies)
        {
            Random randomChance = new Random();
            int randomEventNumber = randomChance.Next(0, _events.Count);

            if (randomChance.Next(0, 22) == 1)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("Случилось страшное!? вращайте барабан...");
                Console.ReadKey();
                Console.SetCursorPosition(0, 1);
                Console.WriteLine($"{_events[randomEventNumber]._nameOfEvent}. " +
                    $"В итоге {_events[randomEventNumber]._description}");
                Thread.Sleep(250);
                switch (randomEventNumber)
                {
                    case 0:
                        GovnoCode.MovesAvailable -= 5;
                        break;
                    case 1:
                        bool rightPostion = false;
                        while (rightPostion == false)
                        {
                            int x = randomChance.Next(0, map.GetLength(0));
                            int y = randomChance.Next(0, map.GetLength(1));
                            if (map[x, y] == ' ')
                            {
                                map[x, y] = 'X';
                                treasures++;
                                rightPostion = true;
                            }
                        }
                        break;
                    case 2:
                        GovnoCode.MovesAvailable += 5;
                        break;
                    case 3:
                        Thread.Sleep(1000);
                        GovnoCode.FightWithEnemy(fighters, ref playerIsDead);
                        break;
                    case 4:
                        int RandommapX = randomChance.Next(0, map.GetLength(0));
                        int mapX = RandommapX;
                        enemies.Add(new Enemy(mapX, GovnoCode.GetY(map, mapX)));
                        break;
                    case 5:
                        fighters[0].Health = fighters[0].Health / 10;
                        break;
                    case 6:
                        fighters[0].Health = -15;
                        break;
                    case 7:
                        string[] compliments = { "Вы лучший",
                         "Вы очаровательны сегодня и весьма умны. Продолжайте играть, но не принимайте все всерьез.\n" +
                         "Помните, что это всего лишь игра.", "Вы пышите здоровьем и ваша броня крепка." +
                         " Продолжайте в том же духе.\n" +"Игра - ваша тема!",
                            "Сударь! А может быть, сударыня! Вы умны и здоровы, а можете стать богатым. " +
                            "\nПродолжайте собирать сокровища. Мы вас любим."
                            , "Вперёд, вам по силам пройти эту игру!!!" };
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine(compliments[randomChance.Next(0, compliments.Length)]);
                        break;
                }
                Console.ReadKey();
                Console.Clear();
                GovnoCode.DrawMap(map, ConsoleColor.DarkYellow, ConsoleColor.Cyan);
            }
        }
    }
    class PlayerInfo
    {
        private string _name;
        private int _score;
        private DateTime _date;
        public PlayerInfo(string name, int score, DateTime date)
        {
            _name = name;
            _score = score;
            _date = date;
        }

        public string Name { get { return _name; } }
        public int Score { get { return _score; } }
        public DateTime Date { get { return _date; } }
        public static void SplitReadingFile(string path, List<PlayerInfo> playerInfos)
        {
            string[] fields = File.ReadAllLines(path);
            foreach (var item in fields)
            {
                string[] fields1 = item.Split(';');
                string Name = fields1[0];
                int Score = int.Parse(fields1[1]);
                DateTime Date = DateTime.Parse(fields1[2]);
                playerInfos.Add(new PlayerInfo(Name, Score, Date));
            }
            
        }


    }
}