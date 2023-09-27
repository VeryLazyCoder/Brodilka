using System.Text.RegularExpressions;

namespace HodimBrodim
{
    public class Program
    {
        private static int _mapVariant;
        private static int _numberOfEnemies;
        static void Main(string[] args)
        {
            SetGameParameters();
            GiveAdviceToPlayer();
            Console.CursorVisible = false;

            var startMoves = GameMap.GetMovesOnChoosenMap(_mapVariant);
            UserReckordsManager.LoadReckords(_mapVariant);
            var wannaPlay = true;
            
            while (wannaPlay)
            {
                var roundResult = new GameRound(startMoves, _numberOfEnemies).StartGame();
                var isRoundWin = roundResult.isWinResult;

                Console.Clear();
                if (isRoundWin)
                {                   
                    var userScore = roundResult.userScore;
                    Console.WriteLine($"Вы победиди за {userScore} ходов, поздравляю!!!");
                    UserReckordsManager.AddRecords(_mapVariant, userScore);
                }
                else
                    Console.WriteLine($"Вы не справились, игра окончена");

                wannaPlay = IsRestart();
            }
            Paint("СПАСИБО ВАМ ЗА ИГРУ", ConsoleColor.DarkGreen);
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
        public static void SetGameParameters()
        {
            GiveMapConfigurationInfo();
            var inputPattern = new Regex(@"\d+\s\d+");
            while (true)
            {
                try
                {
                    ConfigurateMapAccordingPlayersChoice(inputPattern);
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("Неверный ввод, никакой вам игры за такое!!!!\n" +
                        "Повторите ввод");
                }
            }
        }
        public static Point GetEmptyPosition(GameMap map)
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

        private static void ConfigurateMapAccordingPlayersChoice(Regex inputPattern)
        {
            string userInput = Console.ReadLine();
            var numbers = inputPattern.Matches(userInput);
            if (numbers.Count != 1)
                throw new Exception();
            _mapVariant = int.Parse(Convert.ToString(numbers[0]).Split(" ")[0]);
            _numberOfEnemies =  int.Parse(Convert.ToString(numbers[0]).Split(" ")[1]);
        }
        private static void GiveMapConfigurationInfo()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Введите пару чисел через пробел - размер карты и количество противников");
            Console.WriteLine("Выберите размер карты:\n1 - маленькая \n2 - средняя" +
                "\n3 - большая, \nВнимание!!! Eсли введёте что-то другое придётся играть на большой карте");
            Console.WriteLine("Количество противников: 2, 4, либо 6. Любое другое число и соперников будет 6");
        }
        private static bool IsRestart()
        {
            Console.WriteLine("Хотите улучшить результат? Нажмите 'y'");
            return Console.ReadKey().Key == ConsoleKey.Y;
        }
        public static void CloseGame()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("ZЗря вы покинули такую прекрасную игру :(");
            Environment.Exit(0);
        }
    }
}