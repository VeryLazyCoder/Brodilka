using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;


namespace HodimBrodim
{
    class Program
    {
        public static int MovesAvailable = 300;
        static void Main()
        {
            var playersChoice = RecieveFromPlayerGameParametres();
            
            GiveAdviceToPlayer();
            Console.CursorVisible = false;
        loop1:
            GameMap.GetMapSize(playersChoice[0]);
            PlayerInfo.Initialize(playersChoice[0]);
            int startMoves = MovesAvailable;
            GameMap map = new GameMap();
            var enemies = ChooseEnemyCount(map, playersChoice[1]);
            DrawBonusesForPlayer(map);
            Player player = new Player(map, GetPosition(map));

            Console.Clear();
            while (true)
            {
                player.ShowPlayerStatistic();
                map.DrawMap(ConsoleColor.DarkYellow, ConsoleColor.Cyan);
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].Move(player.Position);
                    if (enemies[i].CollisionWithPlayer(player.Position))
                    {
                        player.FightWithEnemy();
                        enemies.RemoveAt(i);
                        map.DrawMap(ConsoleColor.DarkYellow, ConsoleColor.Cyan);
                        break;
                    }
                }
                RandomEvents.RandomEvent(map, player, enemies);
                if (map[player.Position] == 'X')
                {
                    player.AddTreasure();
                    map[player.Position] = ' ';
                    player.Health /= 10;
                    player.Damage /= 10;
                }
                if (map[player.Position] == '@')
                    MovesAvailable -= 10;
                if (map[player.Position] == 'D')
                {
                    player.Damage = player.Damage / 4;
                    map[player.Position] = ' ';
                }
                if (map[player.Position] == 'A')
                {
                    player.Armor = 2;
                    map[player.Position] = ' ';
                }
                if (map[player.Position] == 'H')
                {
                    player.Health = player.Health / 4;
                    map[player.Position] = ' ';
                }
                if (player.TreasureCount == map.TreasuresOnTheMap || enemies.Count == 0)
                    break;
                if (MovesAvailable <= 0 || player.PlayerIsDead == true)
                {
                    Console.Clear();
                    Console.WriteLine("Вы не справились :( Игра окончена");

                    Console.WriteLine("Хотите улучшить результат? да/нет");
                    string userInput = Console.ReadLine();
                    
                    if (userInput == "да" || userInput == "if" || userInput == "lf" ||
                        userInput == "fl" || userInput == "ад")      
                        goto loop1;
                    
                    Environment.Exit(0);
                }
                Console.SetCursorPosition(player.Position.X, player.Position.Y);
                Paint('T', ConsoleColor.Green);
                ConsoleKeyInfo pressedKey = Console.ReadKey();
                PlayerInfo.ShowRecordsTable(pressedKey);
                player.Move(pressedKey, map.Map);
            }
            Console.Clear();
            Console.WriteLine($"Вы победиди за {startMoves - MovesAvailable} ходов, поздравляю!!!");
            Console.WriteLine("Хотите улучшить результат? да/нет");
            string answer = Console.ReadLine();
            if (answer == "да" | answer == "if" | answer == "lf" | answer == "fl" | answer == "ад")
            {
                Console.Clear();
                goto loop1;
            }
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
        private static List<IEnemy> ChooseEnemyCount(GameMap map, int enemyCount)
        {
            var enemies = new List<IEnemy>();
            for (int i = 0; i < enemyCount; i++)
            {
                if (i == 2)
                    enemies.Add(GetEnemy(new Point(map.Map.GetLength(0) - 2, map.Map.GetLength(1) - 2), map));
                else if (i == 3)
                    enemies.Add(GetEnemy(new(1, 1), map));
                else
                    enemies.Add(GetEnemy(GetPosition(map), map));
            }
            return enemies;
        }
        private static IEnemy GetEnemy(Point point, GameMap map)
        {
            var random = new Random();
            if (random.Next(0, 2) == 0)
                return new SmartEnemy(point, map);
            else
                return new CommomEnemy(point, map);
        }
        public static Point GetPosition(GameMap map)
        {
            var rand = new Random();
            var position = new Point();
            while (true)
            {
                position = new(rand.Next(0, map.Map.GetLength(0)),
                    rand.Next(0, map.Map.GetLength(1)));

                if (map[position] == ' ')
                    return position;
            }
        }
    }
}