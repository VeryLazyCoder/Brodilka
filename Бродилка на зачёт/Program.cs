using System.Drawing;
using System.Text.RegularExpressions;

namespace HodimBrodim
{
    public class Program
    {
        private static string[] _agreementWords = { "да", "lf", "fl", "ад", };
        static void Main(string[] args)
        {
            var playersChoice = RecieveFromPlayerGameParametres();
            var mapVariant = playersChoice.mapVariant;
            var numberOfEnemies = playersChoice.numberOfEnemies;
            GiveAdviceToPlayer();
            Console.CursorVisible = false;

            var startMoves = GameMap.GetMovesOnChoosenMap(mapVariant);
            PlayerInfo.Initialize(mapVariant);
            bool wannaPlay = true;
            
            while (wannaPlay)
            {
                var roundResult = new GameRound(startMoves).StartGame(numberOfEnemies);
                var isRoundWin = roundResult.isWinResult;

                Console.Clear();
                if (isRoundWin)
                {                   
                    var userScore = roundResult.userScore;
                    Console.WriteLine($"Вы победиди за {userScore} ходов, поздравляю!!!");
                    PlayerInfo.AddRecords(mapVariant, userScore);
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
        public static (int mapVariant, int numberOfEnemies) RecieveFromPlayerGameParametres()
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
                    return (int.Parse(Convert.ToString(numbers[0]).Split(" ")[0]),
                    int.Parse(Convert.ToString(numbers[0]).Split(" ")[1]));
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
        private static bool IsRestart()
        {
            Console.WriteLine("Хотите улучшить результат? да/нет");
            string answer = Console.ReadLine();
            return _agreementWords.Contains(answer);
        }
    }
}