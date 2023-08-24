using System;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace HodimBrodim
{
    class Program
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
            var playersChoice = RecieveFromPlayerGameParametres();
            GiveAdviceToPlayer();
            Console.CursorVisible = false;
        loop1:     
            GameMap.GetMapSize(playersChoice[0]);
            PlayerInfo.Initialize(playersChoice[0]);
            //PlayerInfo.AddRecords(playersChoice[0], 90);
            int startMoves = MovesAvailable;
            GameMap map = new GameMap();
            var enemies = ChooseEnemyCount(map, playersChoice[1]);
            DrawBonusesForPlayer(map);
            Player player = new Player(map);            
            
            Console.Clear();
            while (true)
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
                        if (maxLoseResult > map.TreasuresOnTheMap)
                        {
                            Console.WriteLine($"Ваш лучший результат {maxLoseResult} ходов");
                        }
                        else
                        {
                            Console.WriteLine($"Ваш лучший результат {maxLoseResult} собранных сокровищ");
                        }
                    }
                }
                player.ShowPlayerStatistic();
                map.DrawMap(ConsoleColor.DarkYellow, ConsoleColor.Cyan);
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].EnemyBehavior();
                    if (enemies[i].CollisionWithEnemy(player))
                    {
                        player.FightWithEnemy();
                        enemies.RemoveAt(i);
                        map.DrawMap(ConsoleColor.DarkYellow, ConsoleColor.Cyan);
                        break;
                    }
                }
                RandomEvents.RandomEvent(map, player, enemies);
                if (map.Map[player.X, player.Y] == 'X')
                {
                    player.AddTreasure();
                    map.Map[player.X, player.Y] = ' ';
                    player.Health /= 10;
                    player.Damage /= 10;

                }
                if (map.Map[player.X, player.Y] == '@')
                {
                    MovesAvailable -= 10;
                }
                if (map.Map[player.X, player.Y] == 'D')
                {
                    player.Damage = player.Damage / 4;
                    map.Map[player.X, player.Y] = ' ';
                }
                if (map.Map[player.X, player.Y] == 'A')
                {
                    player.Armor = 2;
                    map.Map[player.X, player.Y] = ' ';
                }
                if (map.Map[player.X, player.Y] == 'H')
                {
                    player.Health = player.Health / 4;
                    map.Map[player.X, player.Y] = ' ';
                }
                if (player.TreasureCount == map.TreasuresOnTheMap || enemies.Count == 0)
                    break;
                if (MovesAvailable <= 0 || player.PlayerIsDead == true)
                {
                    Console.Clear();
                    Console.WriteLine("Вы не справились :( Игра окончена");
                    
                    Console.WriteLine("Хотите улучшить результат? да/нет");
                    string userInput = Console.ReadLine();
                    showresult = true;
                    if (userInput == "да" || userInput == "if" || userInput == "lf" ||
                        userInput == "fl" || userInput == "ад")
                    {
                        Console.Clear();
                        previousResult = player.TreasureCount;
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
                    ShowFinalResult(everWin, maxWinResult, player);
                    Environment.Exit(0);
                }
                Console.SetCursorPosition(player.X, player.Y);
                Paint('T', ConsoleColor.Green);
                ConsoleKeyInfo pressedKey = Console.ReadKey();
                PlayerInfo.ShowRecordsTable(pressedKey);
                player.Move(pressedKey, map.Map);
            }
            Console.Clear();
            Console.WriteLine($"Вы победиди за {startMoves - MovesAvailable} ходов, поздравляю!!!");          
            everWin = true;
            previousResult = startMoves - MovesAvailable;
            showresult = true;
            ifWin = true;
            if (previousResult <= maxWinResult)
                maxWinResult = previousResult;
            Console.WriteLine("Хотите улучшить результат? да/нет");
            string A = Console.ReadLine();
            if (A == "да" | A == "if" | A == "lf" | A == "fl" | A == "ад")
            {
                Console.Clear();
                goto loop1;
            }
            ShowFinalResult(everWin, maxWinResult, player);
        }

        private static void ShowFinalResult(bool everWin, int maxWinResult, Player player)
        {
            if (everWin == true)
            {
                Console.WriteLine($" Ваш лучший результат составил {maxWinResult} ходов(а)");
                PlayerInfo.AddRecords(GameMap.MapID, maxWinResult);
            }
            else
                Console.WriteLine(" Ваш лучший результат составил " + player.TreasureCount + " сокровищ(а)");
            Console.ReadKey();
        }
        private static void DrawBonusesForPlayer(GameMap map)
        {
            map.DrawSymbolOnEmptyCell('A');
            map.DrawSymbolOnEmptyCell('D');
            map.DrawSymbolOnEmptyCell('H');
        }
        public static void Paint(char symbol, ConsoleColor color)
        {
            ConsoleColor defaultcolor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(symbol);
            Console.ForegroundColor = defaultcolor;
        }
        private static void Paint(string stringToWrite, ConsoleColor color)
        {
            ConsoleColor defaultcolor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(stringToWrite);
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
            Paint("Берегитесь злых собак '@', они вас сильно задерживают!!!\n\n", ConsoleColor.Red);
            Console.WriteLine("Теперь игра не заканчивается, если вы встретились с врагом\n" +
                "Вы можете дать ему отпор, ваше здоровье, броня и атака указаны в правом верхнем углу.\n" +
                "Напомню, что каждая единица брони уменьшает входящий урон на 10%\n" +
                "С каждым собранным сокровищем ваши характеристики улучшаются\n" +
                "Кроме того на карте находятся специальные предметы (D - атака,A - защита,H - здоровье),\n " +
                "значительно повышающие ваши характеристики.\n" +
                "Их не обязательно собирать, но они сильно помогут вам в схватке с врагами\n" +
                "\nВы можете в любой момент посмотреть рекорды других игроков, для этого нажмите клавишу 'R'\n" +
                "Наконец, будьте готовы к неожиданным поворатам, увидев следующую надпись ");
            Paint("Случилось страшное!? Вращайте барабан", ConsoleColor.Blue);
            Console.WriteLine("\n\nЕсли вы готовы, нажмите любую кнопку и вперёд!");
            Console.ReadKey();
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
                    correctY = true;               
            }
            return y;
        }
        public static int[] RecieveFromPlayerGameParametres()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Введите пару чисел через пробел - размер карты и количество противников");
            Console.WriteLine("Выберите размер карты:\n1 - маленькая \n2 - средняя" +
                "\n3 - большая, \nВнимание!!! Eсли введёте что-то другое придётся играть на большой карте");
            Console.WriteLine("Количество противников: 2, 4, либо 6. Любое другое число и соперников будет 6");
            Regex magicThing = new Regex(@"\d+\s\d+");
            while (true)
            {
                try
                {
                    string userInput = Console.ReadLine();
                    var numbers = magicThing.Matches(userInput);
                    if (numbers.Count != 1)
                        throw new Exception();
                    return new int[] {int.Parse(Convert.ToString(numbers[0]).Split(" ")[0]),
                    int.Parse(Convert.ToString(numbers[0]).Split(" ")[1])};
                }
                catch (Exception)
                {
                    Console.WriteLine("Неверный ввод, никакой вам игры за такое!!!!\n" +
                        "Повторите ввод");
                }
            }
        }
        private static List<Enemy> ChooseEnemyCount(GameMap map, int enemyCount)
        {
            Random random = new Random();
            List<Enemy> enemies = new List<Enemy>();
            for (int i = 0; i < 6; i++)
            {
                int randx = random.Next(1, map.Map.GetLength(0));
                int x = randx;
                enemies.Add(new Enemy(x, GetY(map.Map, x), map));
            }
            enemies[2] = new Enemy(map.Map.GetLength(0) - 2, map.Map.GetLength(1) - 2, map);
            enemies[3] = new Enemy(1, 1, map);            
            switch (enemyCount)
            {
                case 2:
                    for (int i = 0; i < 4; i++)
                        enemies.RemoveAt(2);
                    break;
                case 4:
                    for (int i = 0; i < 2; i++)
                        enemies.RemoveAt(4);
                    break;
            }
            return enemies;
        }
    }

    class Enemy
    {
        private int _enemyX;
        private int _enemyY;
        private int _previousEnemyX;
        private int _previousEnemyY;
        private GameMap _map;
        public Enemy(int enemyX, int enemyY, GameMap map)
        {
            _enemyX = enemyX;
            _enemyY = enemyY;
            _previousEnemyX = enemyX;
            _previousEnemyY = enemyY;
            _map = map;
        }
        public void EnemyBehavior()
        {
            Random rd = new Random();
            bool rightPosition = false;

            while (rightPosition == false)
            {
                _previousEnemyX = _enemyX;
                _previousEnemyY = _enemyY;
                switch (rd.Next(1, 5))
                {
                    case 1:
                        if (_map.Map[_enemyX - 1, _enemyY] != '-' && _map.Map[_enemyX - 1, _enemyY] != '|')
                        {
                            _enemyX--;
                            rightPosition = true;
                        }
                        break;
                    case 2:
                        if (_map.Map[_enemyX + 1, _enemyY] != '-' & _map.Map[_enemyX + 1, _enemyY] != '|')
                        {
                            _enemyX++;
                            rightPosition = true;
                        }
                        break;
                    case 3:
                        if (_map.Map[_enemyX, _enemyY - 1] != '-' & _map.Map[_enemyX, _enemyY - 1] != '|')
                        {
                            _enemyY--;
                            rightPosition = true;
                        }
                        break;
                    case 4:
                        if (_map.Map[_enemyX, _enemyY + 1] != '-' & _map.Map[_enemyX, _enemyY + 1] != '|')
                        {
                            _enemyY++;
                            rightPosition = true;
                        }
                        break;
                }
            }
            Console.SetCursorPosition(_enemyX, _enemyY);
            Program.Paint('!', ConsoleColor.Red);
        }
        public bool CollisionWithEnemy(Player player)
        {
            return player.Y == _enemyY & player.X == _enemyX ||
                player.Y == _previousEnemyY & player.X == _previousEnemyX;
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
        public void ShowRoundStatistic(float enemyFighterDamage)
        {
            Console.WriteLine($"По персонажу {_name} нанесено " +
                $"{enemyFighterDamage - (enemyFighterDamage * (_armor / 10))} " +
                $"урона, у него осталось {_health} здоровья");
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
        public static void RandomEvent(GameMap map, Player player,
            List<Enemy> enemies)
        {
            Random randomChance = new Random();
            int randomEventNumber = randomChance.Next(0, _events.Count);

            if (randomChance.Next(0, 25) == 12)
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
                    case 0: // налоговая, - 5 ходов
                        Program.MovesAvailable -= 5;
                        break;
                    case 1: // новое сокровище
                        bool rightPostion = false;
                        while (rightPostion == false)
                        {
                            int x = randomChance.Next(0, map.Map.GetLength(0));
                            int y = randomChance.Next(0, map.Map.GetLength(1));
                            if (map.Map[x, y] == ' ')
                            {
                                map.Map[x, y] = 'X';
                                map.AddOneTreasure();
                                rightPostion = true;
                            }
                        }
                        break;
                    case 2:
                        Program.MovesAvailable += 5;
                        break;
                    case 3:
                        Thread.Sleep(1000);
                        player.FightWithEnemy();
                        break;
                    case 4:
                        int RandommapX = randomChance.Next(0, map.Map.GetLength(0));
                        int mapX = RandommapX;
                        enemies.Add(new Enemy(mapX, Program.GetY(map.Map, mapX), map));
                        break;
                    case 5:
                        player.Health = player.Health / 10;
                        break;
                    case 6:
                        player.Health = -15;
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
                map.DrawMap(ConsoleColor.DarkYellow, ConsoleColor.Cyan);
            }
        }
    }
    class PlayerInfo
    {
        private string _name;
        private int _score;
        private DateTime _date;

        private static int _maxID;
        private static List<PlayerInfo> playerInfo = new List<PlayerInfo>();
        private static string connectionString =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=Reckords;Integrated Security=True";
        private PlayerInfo(string name, int score, DateTime date)
        {
            _name = name;
            _score = score;
            _date = date;
        }

        public static void Initialize(int mapID)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            string sqlQuery = $"SELECT TOP 10 * FROM Reckord where maptype = {mapID} ORDER BY Score ASC";
            using SqlCommand command = new(sqlQuery, connection);
            SqlDataReader reader = command.ExecuteReader();

            playerInfo = new List<PlayerInfo>();
            while (reader.Read())
            {
                // Получение данных из результата запроса
                string name = (string)reader["Nickname"];
                int score = (int)reader["Score"];
                DateTime date = (DateTime)reader["ScoreDate"];
                playerInfo.Add(new PlayerInfo(name, score, date));
            }
            reader.Close();

            command.CommandText = "select max(Id) from Reckord";

            _maxID = command.ExecuteScalar() != null ? (int)command.ExecuteScalar() : 1;
            connection.Close();
        }

        private static void Initialize(int mapID, PlayerInfo newRow)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            string insertQuery = $"insert into Reckord values ({_maxID + 1}, '{newRow._name}', {newRow._score}, " +
                $"'{newRow._date}', {mapID})";
            using SqlCommand insertCommand = new(insertQuery, connection);
            insertCommand.ExecuteNonQuery();

            string sqlQuery = $"SELECT TOP 10 * FROM Reckord where maptype = {mapID} ORDER BY Score ASC";
            using SqlCommand command = new(sqlQuery, connection);
            SqlDataReader reader = command.ExecuteReader();

            playerInfo = new List<PlayerInfo>();
            while (reader.Read())
            {
                // Получение данных из результата запроса
                string name = (string)reader["Nickname"];
                int score = (int)reader["Score"];
                DateTime date = (DateTime)reader["ScoreDate"];
                playerInfo.Add(new PlayerInfo(name, score, date));
            }
            reader.Close();

            _maxID++;
            connection.Close();
        }

        public static void AddRecords(int mapID, int playerScore)
        {
            ShowRecordsTable(new ConsoleKeyInfo('R', ConsoleKey.R, false, false, false));
            Console.WriteLine($"Хотите внести свой результат ({playerScore} ходов) в таблицу? (для этого введите '+')");

            if (Console.ReadLine() == "+")
            {
                Console.Write("Введите ваше имя ");
                string nameOfPlayer = Console.ReadLine();
                Initialize(mapID, new PlayerInfo(nameOfPlayer, playerScore, DateTime.Now));
            }
           
            Console.WriteLine("Чтобы увидеть обновлённую таблицу нажмите 'R'");
            ConsoleKeyInfo pressedKey = Console.ReadKey();
            ShowRecordsTable(pressedKey);
        }
        public static void ShowRecordsTable(ConsoleKeyInfo pressedKey)
        {
            if (pressedKey.Key == ConsoleKey.R)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Clear();
                Console.WriteLine("Таблица рекордов\n");

                foreach (var player in playerInfo)
                    Console.WriteLine($"Игрок {player._name} победил за {player._score} ходов. Рекорд был установлен " +
                        $"{player._date}");

                Console.ReadKey();
                Program.MovesAvailable++;
                Console.Clear();
            }
        }
    }

    class GameMap
    {
        private static string _pathToMap;
        public static int MapID { get; private set; }
        public readonly char[,] Map;
        public int TreasuresOnTheMap { get; private set; }
        public GameMap()
        {
            string[] file = File.ReadAllLines(_pathToMap);
            char[,] map = new char[file[0].Length, file.Length];
            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++) 
                    map[x, y] = file[y][x];  
            Map = map;
            CountTreasures();
        }
        public static void GetMapSize(int mapVariant)
        {
            MapID = mapVariant;
            switch (MapID)
            {
                case 1:
                    _pathToMap = "little.txt";
                    Program.MovesAvailable = 120;
                    break;
                case 2:
                    _pathToMap = "qwerty.txt";
                    Program.MovesAvailable = 160;
                    break;
                default:
                    _pathToMap = "bigmap.txt";
                    break;
            }
            Program.MovesAvailable++;
        }
        public void DrawSymbolOnEmptyCell(char symbol)
        {
            while (true)
            {
                Random random = new Random();
                int x = random.Next(1, Map.GetLength(0));
                int y = random.Next(1, Map.GetLength(1));
                if (Map[x, y] == ' ')
                {
                    Map[x, y] = symbol;
                    break;
                }
            }
        }
        public void DrawMap(ConsoleColor color, ConsoleColor treasureColor)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < Map.GetLength(1); y++)
            {
                for (int x = 0; x < Map.GetLength(0); x++)
                {
                    if (Map[x, y] == 'X')
                    {
                        Console.ForegroundColor = treasureColor;
                        Console.Write(Map[x, y]);
                        Console.ForegroundColor = color;
                        continue;
                    }
                    else if (Map[x, y] == 'A' || Map[x, y] == 'D' || Map[x, y] == 'H')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(Map[x, y]);
                        Console.ForegroundColor = color;
                        continue;
                    }                  
                    else if (Map[x, y] == '@')
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(Map[x, y]);
                        Console.ForegroundColor = color;
                        continue;
                    }
                    else if (Map[x, y] == 'T')
                        Map[x, y] = ' ';
                    Console.Write(Map[x, y]);
                }
                Console.WriteLine();
            }
        }
        public void CountTreasures()
        {
            for (int i = 0; i < Map.GetLength(0); i++)
                for (int j = 0; j < Map.GetLength(1); j++)
                    if (Map[i, j] == 'X')
                        TreasuresOnTheMap += 1;
        }
        public void AddOneTreasure()
        {
            TreasuresOnTheMap++;
        }
    }

    class Player : Fighter
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool PlayerIsDead{ get;  private set; }
        private List<Fighter> _fighters;

        public int TreasureCount { get; private set; }
        public Player(GameMap map) : base("игрок", 150, 2, 25, "Хороший вопрос")
        {
            Random rand = new Random();
            while (true)
            {
                int randomEnemyX = rand.Next(0, map.Map.GetLength(0));
                int randomEnemyY = rand.Next(0, map.Map.GetLength(1));
                if (map.Map[randomEnemyX, randomEnemyY] == ' ')
                {
                    X = randomEnemyX;
                    Y = randomEnemyY;
                    map.Map[X, Y] = 'T';
                    break;
                }
            }
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
            PlayerIsDead = false;    
        }

        public void AddTreasure()
        {
            TreasureCount++;
        }
        public void Move(ConsoleKeyInfo pressedKey, char[,] map)
        {
            int[] direction = GetDirection(pressedKey);
            int nextPositionX = X + direction[0];
            int nextPositionY = Y + direction[1];
            if (map[nextPositionX, nextPositionY] != '-' & map[nextPositionX, nextPositionY] != '|')
            {
                X = nextPositionX;
                Y = nextPositionY;
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
            Console.WriteLine();
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
            while (this.Health > 0 & enemyFighter.Health > 0)
            {
                Thread.Sleep(150);
                enemyFighter.TakeDamage(Damage);
                this.TakeDamage(enemyFighter.Damage);
                this.ShowRoundStatistic (enemyFighter.Damage);
                enemyFighter.ShowRoundStatistic( Damage);
                Console.WriteLine(new string('-', 70));
            }
            if (this.Health <= 0)
            {
                this.PlayerIsDead = true;
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