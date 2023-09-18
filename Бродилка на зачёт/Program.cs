using System.Drawing;
using System.Text.RegularExpressions;

namespace HodimBrodim
{
    public class Program
    {
        private static int _startMoves;
        private static Dictionary<char, Action<Player, GameMap>> _actionsOnCollision = new()
        {
            ['X'] = (player, map) =>
            {
                player.AddTreasure();
                map[player.Position] = ' ';
                player.Health /= 10;
                player.Damage /= 10;
            },
            ['A'] = (player, map) =>
            {
                player.Armor = 3;
                map[player.Position] = ' ';
            },
            ['D'] = (player, map) =>
            {
                player.Damage /= 3;
                map[player.Position] = ' ';
            },
            ['H'] = (player, map) =>
            {
                player.Health /= 3;
                map[player.Position] = ' ';
            },
            ['@'] = (player, map) => player.MovesAvailable -= 10,
            [' '] = (player, map) => { },
        };
        private static int[] _playersChoice;
        private static string[] _agreement = { "да", "lf", "fl", "ад", };
        static void Main(string[] args)
        {
            _playersChoice = RecieveFromPlayerGameParametres();
            GiveAdviceToPlayer();
            Console.CursorVisible = false;
            bool wannaPlay = true;
            
            while (wannaPlay)
            {
                var userScore = StartGame(out bool isWin);
                Console.Clear();
                if (isWin)
                {                   
                    Console.WriteLine($"Вы победиди за {userScore} ходов, поздравляю!!!");
                    PlayerInfo.AddRecords(_playersChoice[0], userScore);
                }
                else
                    Console.WriteLine($"Вы не справились, игра окончена");
                wannaPlay = IsRestart();
            }
            Paint("СПАСИБО ВАМ ЗА ИГРУ", ConsoleColor.DarkGreen);
        }

        public static void DisplayEnemies(List<IEnemy> enemies)
        {
            foreach (var enemy in enemies)
                enemy.Display();
        }
        public static void Paint(char symbol, ConsoleColor color)
        {
            ConsoleColor defaultcolor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(symbol);
            Console.ForegroundColor = defaultcolor;
        }
        public static void Paint(string stringToWrite, ConsoleColor color)
        {
            ConsoleColor defaultcolor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(stringToWrite);
            Console.ForegroundColor = defaultcolor;
        }
        public static void GiveAdviceToPlayer()
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
        public static List<IEnemy> GetEnemies(GameMap map, int enemyCount)
        {
            var enemies = new List<IEnemy>();
            //if (enemyCount <= 0 || enemyCount >= 10)
            //    throw new ArgumentOutOfRangeException(nameof(enemyCount));

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
        public static IEnemy GetEnemy(Point point, GameMap map)
        {
            var random = new Random();
            if (random.Next(3) == 0)
                return new SmartEnemy(point, map);
            else
                return new CommomEnemy(point, map);
        }
        public static Point GetPosition(GameMap map)
        {
            var rand = new Random();
            while (true)
            {
                var position = new Point(rand.Next(0, map.Map.GetLength(0)),
                    rand.Next(0, map.Map.GetLength(1)));

                if (map[position] == ' ')
                    return position;
            }
        }
        private static int StartGame(out bool win)
        {
            _startMoves = GameMap.GetMapSize(_playersChoice[0]);
            PlayerInfo.Initialize(_playersChoice[0]);
            GameMap map = new();
            var enemies = GetEnemies(map, _playersChoice[1]);
            var player = new Player(map, GetPosition(map), _startMoves);
            Console.Clear();

            while (true)
            {
                player.ShowPlayerStatistic();
                map.DrawMap(ConsoleColor.DarkYellow, ConsoleColor.Cyan);
                Console.SetCursorPosition(player.Position.X, player.Position.Y);
                Paint('T', ConsoleColor.Green);
                DisplayEnemies(enemies);
                ConsoleKeyInfo pressedKey = Console.ReadKey();
                PlayerInfo.ShowRecordsTable(pressedKey);
                player.Move(pressedKey, map.Map);
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
                RandomEvents.InvokeEvent(map, player, enemies);
                _actionsOnCollision[map[player.Position]].Invoke(player, map);

                if (player.TreasureCount == map.TreasuresOnTheMap ||
                    (enemies.Count == 0 && _playersChoice[1] != 0))
                    break;
                if (player.MovesAvailable <= 0 || player.PlayerIsDead == true)
                {
                    win = false;
                    return -1;
                }
            }
            win = true;
            return _startMoves - player.MovesAvailable;
        }
        private static bool IsRestart()
        {
            Console.WriteLine("Хотите улучшить результат? да/нет");
            string answer = Console.ReadLine();
            return _agreement.Contains(answer);
        }
    }
}