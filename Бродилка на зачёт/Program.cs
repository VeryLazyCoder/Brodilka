using System.Data;
using System.Text.RegularExpressions;
using HodimBrodim.DBData;

namespace HodimBrodim
{
    public class Program
    {
        private static int _mapVariant;
        private static int _numberOfEnemies;

        private static void Main()
        {
            SetCurrentUser(GetAuthorizationHandler());
            Console.WriteLine(CurrentUser.Account);
            
            SetGameParameters();
            GiveAdviceToPlayer();
            Console.CursorVisible = false;

            var startMoves = GetMovesOnMapAmount();
            RecordsRepository.LoadRecords(_mapVariant);
            var wannaPlay = true;
            
            while (wannaPlay)
            {
                var gameRound = new GameRound(startMoves, _numberOfEnemies);
                gameRound.StartGameLoop();

                Console.Clear();

                if  (gameRound.IsWon == null)
                    throw new NullReferenceException();

                if (gameRound.IsWon.Value)
                {                   
                    var userScore = gameRound.UserScore;
                    Console.WriteLine($"Вы победиди за {userScore} ходов, поздравляю!!!");
                    RecordsRepository.OfferAddRecord(userScore);
                }
                else
                    Console.WriteLine($"Вы не справились, игра окончена");

                wannaPlay = IsRestart();
            }
            Paint("СПАСИБО ВАМ ЗА ИГРУ", ConsoleColor.DarkGreen);
        }

        public static void Paint(char symbol, ConsoleColor color)
        {
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(symbol);
            Console.ForegroundColor = defaultColor;
        }
        public static void Paint(string stringToWrite, ConsoleColor color)
        {
            ConsoleColor defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(stringToWrite);
            Console.ForegroundColor = defaultColor;
        }
        public static void CloseGame()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Зря вы покинули такую прекрасную игру :(");
            Environment.Exit(0);
        }

        private static void ConfigureMapAccordingPlayersChoice(Regex inputPattern)
        {
            var userInput = Console.ReadLine();
            var numbers = inputPattern.Matches(userInput);
            if (numbers.Count != 1)
                throw new Exception();
            _mapVariant = int.Parse(Convert.ToString(numbers[0]).Split(" ")[0]);
            _numberOfEnemies =  int.Parse(Convert.ToString(numbers[0]).Split(" ")[1]);
        }
        private static void GiveMapConfigurationInfo()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"Введите пару чисел через пробел - размер карты и количество противников");
            Console.WriteLine(@"Выберите размер карты:\n1 - маленькая \n2 - средняя" +
                "\n3 - большая, \nВнимание!!! Eсли введёте что-то другое придётся играть на большой карте");
            Console.WriteLine("Количество противников: 2, 4, либо 6. Любое другое число и соперников будет 6");
        }
        private static bool IsRestart()
        {
            Console.WriteLine("Хотите улучшить результат? Нажмите 'y'");
            return Console.ReadKey(true).Key == ConsoleKey.Y;
        }
        public static void GiveAdviceToPlayer()
        {
            Console.Clear();
            Console.WriteLine("Краткая справка:\n");
            Console.WriteLine(@$"Добро пожаловать в тестовую бродилку. Ваш персонаж обозначен буквой Т" +
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
                    ConfigureMapAccordingPlayersChoice(inputPattern);
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("Неверный ввод, никакой вам игры за такое!!!!\n" +
                        "Повторите ввод");
                }
            }
        }

        private static int GetMovesOnMapAmount() => _mapVariant switch
        {
            1 => 120,
            2 => 160,
            _ => 300
        };

        private static AuthorizationHandler GetAuthorizationHandler()
        {

            while (true)
            {
                try
                {
                    Console.WriteLine("1 - Авторизоваться, 2 - зарегистрироваться");
                    var result = int.Parse(Console.ReadLine());
                    if (result < 1 || result > 2)
                        throw new FormatException();
                    return result == 1 ? SignIn() : SignUp();
                }
                catch (FormatException)
                {
                    Console.WriteLine("Такой опции нет");
                    continue;
                }
                catch (DataException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
        }

        private static AuthorizationHandler SignUp()
        {
            Console.WriteLine("Введите имя");
            var name = Console.ReadLine();
            Console.WriteLine("Введите логин");
            var login = Console.ReadLine();
            Console.WriteLine("Введите и запомните свой пароль, восстановить его будет невозможно");
            var password = Console.ReadLine();

            return AuthorizationHandler.SignUp(name, login, password);
        }

        private static AuthorizationHandler SignIn()
        {
            Console.WriteLine("Введите логин");
            var login = Console.ReadLine();
            Console.WriteLine("Введите свой пароль");
            var password = Console.ReadLine();

            return AuthorizationHandler.SignIn(login, password);
        }

        private static void SetCurrentUser(AuthorizationHandler handler)
        {
            Console.WriteLine("Ваши аккаунты:");
            var accounts = handler.DisplayConnectedAccounts();
            Console.WriteLine("Выберите аккаунт под которым будете играть или введите 0, чтобы создать новый");
            var choice = int.Parse(Console.ReadLine());
            if (choice == 0)
            {
                Console.WriteLine("Введите ваш никнейм");
                var nick = Console.ReadLine();
                handler.CreateAndChooseGameAccount(nick);
            }
            else
                accounts[choice - 1].Register();
        }
    }
}