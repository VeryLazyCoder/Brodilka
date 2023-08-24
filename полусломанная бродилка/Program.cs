using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
namespace HodimBrodim
{
    class GovnoCode
    {
        static void Main()
        {
        loop0:
            int PreviousPlayerX, PreviousPlayerY;
            bool showresult = false;
            int PreviousResult = 0;
            bool ifWin = false;
            bool EverWin = false;
            int maxWinResult = int.MaxValue;
            int maxLoseResult = 0;
            string path;
            int MovesAvailable = 300;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Выберите размер карты:\n1 - маленькая \n2 - средняя" +
                "\n3 - большая, \nВнимание!!! Eсли введёте что-то другое придётся играть на большой карте");
            string mapVariant = Console.ReadLine();
            switch (mapVariant)
            {
                case "1":
                    path = "little.txt";
                    MovesAvailable = 120;
                    break;
                case "2":
                    path = "qwerty.txt";
                    MovesAvailable = 160;
                    break;
                case "3":
                    Console.WriteLine("Извините карта на рестроврации приходите завтра");
                    Thread.Sleep(5000);
                    Console.Clear();
                    goto loop0;
                    path = "bigmap.txt";
                    break;
                default:
                    path = "bigmap.txt";
                    break;
            }

            Console.WriteLine("Выберите число противников: 2,4, либо 6\nЕсли введёте что-то другое - будете играть с 6 противниками ");
            string EnemyCount = Console.ReadLine();
            if (EnemyCount != "2" & EnemyCount != "4" & EnemyCount != "6")
            {
                EnemyCount = "6";
            }
            int StartMoves = MovesAvailable;
        loop1:

            char[,] map = ReadMap(path);
            bool wannaPlay = true;
            Console.CursorVisible = false;
            int playerX, playerY;
            PlayerSpot(map, out playerX, out playerY);
            int Treasures = 0;
            MovesAvailable = StartMoves;

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
            int EnemyX0, EnemyY0, EnemyX1, EnemyY1, EnemyX4, EnemyY4, EnemyX5, EnemyY5;
            int EnemyX2 = 1, EnemyY2 = 1;
            int EnemyX3 = map.GetLength(1) - 2, EnemyY3 = map.GetLength(0) - 2;
            int PreviousEnemyX0 = 0, PreviousEnemyY0 = 0, PreviousEnemyX1 = 0, PreviousEnemyY1 = 0, PreviousEnemyX2 = 0, PreviousEnemyY2 = 0, PreviousEnemyX3 = 0, PreviousEnemyY3 = 0;
            int PreviousEnemyX4 = 0, PreviousEnemyX5 = 0, PreviousEnemyY4 = 0, PreviousEnemyY5 = 0;
            EnemySpot(map, out EnemyX0, out EnemyY0, out EnemyX1, out EnemyY1);
            EnemySpot(map, out EnemyX4, out EnemyY4, out EnemyX5, out EnemyY5);
            if (EnemyCount == "2")
            {
                EnemyX2 = 0;
                EnemyY2 = 0;
                EnemyX3 = 0;
                EnemyY3 = 0;
                EnemyX4 = 0;
                EnemyY4 = 0;
                EnemyX5 = 0;
                EnemyY5 = 0;
            }
            if (EnemyCount == "4")
            {
                EnemyX4 = 0;
                EnemyY4 = 0;
                EnemyX5 = 0;
                EnemyY5 = 0;
            }
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0, 21);
            Console.WriteLine($"Осталось ходов: {MovesAvailable--} ");
            Console.SetCursorPosition(0, 22);
            Console.WriteLine($"Добро пожаловать в тестовую бродилку. Ваш персонаж обозначен буквой Т" +
            "\nЧтобы двигаться вверх нажмите W, вниз S, вправо D, а влево A. Также можно ходить с помощью стрелочек.\nНо берегитесь врагов, обозначенных '!' " +
            "\nДля выхода нажмите Esc");
            Console.WriteLine("По лабиринту разбросаны сокровища. Они обозначены буквой Х. " +
            "Соберите их все и сможете победить!");
            DogPainter("Берегитесь злых собак '@', они вас сильно задерживают!!!\n", ConsoleColor.Red);
            Console.WriteLine($"Счётчик сокровищ: {TreasureCount}");

            PreviousPlayerX = playerX; PreviousPlayerY = playerY;
            Task.Run(() =>
            {
                while (wannaPlay == true)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(0, 21);
                    Console.WriteLine($"Осталось ходов: {MovesAvailable} ");
                    Console.SetCursorPosition(0, 22);
                    Console.WriteLine($"Добро пожаловать в тестовую бродилку. Ваш персонаж обозначен буквой Т" +
                    "\nЧтобы двигаться вверх нажмите W, вниз S, вправо D, а влево A. Также можно ходить с помощью стрелочек.\nНо берегитесь врагов, обозначенных '!' " +
                    "\nДля выхода нажмите Esc");
                    Console.WriteLine("По лабиринту разбросаны сокровища. Они обозначены буквой Х. " +
                    "Соберите их все и сможете победить!");
                    DogPainter("Берегитесь злых собак '@', они вас сильно задерживают!!!\n", ConsoleColor.Red);
                    Console.WriteLine($"Счётчик сокровищ: {TreasureCount}");
                    Console.SetCursorPosition(0, 0);
                    DrawMap(map, ConsoleColor.DarkYellow, ConsoleColor.Cyan);
                    Console.SetCursorPosition(playerX, playerY);
                    Painter('T', ConsoleColor.Green);
                    Console.SetCursorPosition(0, 0);
                    switch (EnemyCount)
                    {
                        case "2":
                            PreviousEnemyX0 = EnemyX0;
                            PreviousEnemyY0 = EnemyY0;
                            PreviousEnemyX1 = EnemyX1;
                            PreviousEnemyY1 = EnemyY1;
                            EnemyBehavior(map, ref EnemyX0, ref EnemyY0, ref PreviousEnemyX0, ref PreviousEnemyY0);
                            EnemyBehavior(map, ref EnemyX1, ref EnemyY1, ref PreviousEnemyX1, ref PreviousEnemyY1);
                            Thread.Sleep(500);
                            break;
                        case "4":
                            PreviousEnemyX1 = EnemyX1;
                            PreviousEnemyY1 = EnemyY1;
                            PreviousEnemyX2 = EnemyX2;
                            PreviousEnemyY2 = EnemyY2;
                            PreviousEnemyX0 = EnemyX0;
                            PreviousEnemyY0 = EnemyY0;
                            PreviousEnemyY3 = EnemyY3;
                            PreviousEnemyX3 = EnemyX3;
                            EnemyBehavior(map, ref EnemyX0, ref EnemyY0, ref PreviousEnemyX0, ref PreviousEnemyY0);
                            EnemyBehavior(map, ref EnemyX1, ref EnemyY1, ref PreviousEnemyX1, ref PreviousEnemyY1);
                            EnemyBehavior(map, ref EnemyX2, ref EnemyY2, ref PreviousEnemyX2, ref PreviousEnemyY2);
                            EnemyBehavior(map, ref EnemyX3, ref EnemyY3, ref PreviousEnemyX3, ref PreviousEnemyY3);
                            Thread.Sleep(500);
                            break;
                        case "6":
                            PreviousEnemyX1 = EnemyX1;
                            PreviousEnemyY1 = EnemyY1;
                            PreviousEnemyX2 = EnemyX2;
                            PreviousEnemyY2 = EnemyY2;
                            PreviousEnemyX0 = EnemyX0;
                            PreviousEnemyY0 = EnemyY0;
                            PreviousEnemyY4 = EnemyY4;
                            PreviousEnemyX4 = EnemyX4;
                            PreviousEnemyY5 = EnemyY5;
                            PreviousEnemyX5 = EnemyX5;
                            PreviousEnemyY3 = EnemyY3;
                            PreviousEnemyX3 = EnemyX3;
                            EnemyBehavior(map, ref EnemyX0, ref EnemyY0, ref PreviousEnemyX0, ref PreviousEnemyY0);
                            EnemyBehavior(map, ref EnemyX1, ref EnemyY1, ref PreviousEnemyX1, ref PreviousEnemyY1);
                            EnemyBehavior(map, ref EnemyX2, ref EnemyY2, ref PreviousEnemyX2, ref PreviousEnemyY2);
                            EnemyBehavior(map, ref EnemyX3, ref EnemyY3, ref PreviousEnemyX3, ref PreviousEnemyY3);
                            EnemyBehavior(map, ref EnemyX4, ref EnemyY4, ref PreviousEnemyX4, ref PreviousEnemyY4);
                            EnemyBehavior(map, ref EnemyX5, ref EnemyY5, ref PreviousEnemyX5, ref PreviousEnemyY5);
                            Thread.Sleep(350);
                            break;

                    }
                    Console.SetCursorPosition(playerX, playerY);
                    Painter('T', ConsoleColor.Green);
                    Console.SetCursorPosition(0, 0);
                    if (MovesAvailable <= 0 | playerX == EnemyY0 & playerY == EnemyX0 || playerX == EnemyY1 & playerY == EnemyX1 || playerX == EnemyY2 & playerY == EnemyX2 || playerX == EnemyY3 & playerY == EnemyX3 ||
                playerX == PreviousEnemyY0 & playerY == PreviousEnemyX0 || playerX == PreviousEnemyY1 & playerY == PreviousEnemyX1 || playerX == PreviousEnemyY2 & playerY == PreviousEnemyX2 || playerX == PreviousEnemyY3 & playerY == PreviousEnemyX3 ||
                playerX == EnemyY4 & playerY == EnemyX4 || playerX == EnemyY5 & playerY == EnemyX5 || playerX == PreviousEnemyY4 & playerY == PreviousEnemyX4 || playerX == PreviousEnemyY5 & playerY == PreviousEnemyX5)
                    {
                        Console.Clear();
                        Console.WriteLine("Вы не справились :( Игра окончена");
                        wannaPlay = false;
                    }



                }

            });

            Task.Run(() =>
            {
                while (wannaPlay == true)
                {
                    Thread.Sleep(2400);
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.SetCursorPosition(0, 21);
                    Console.WriteLine($"Осталось ходов: {MovesAvailable} ");
                    Console.SetCursorPosition(0, 22);
                    Console.WriteLine($"Добро пожаловать в тестовую бродилку. Ваш персонаж обозначен буквой Т" +
                    "\nЧтобы двигаться вверх нажмите W, вниз S, вправо D, а влево A. Также можно ходить с помощью стрелочек.\nНо берегитесь врагов, обозначенных '!' " +
                    "\nДля выхода нажмите Esc");
                    Console.WriteLine("По лабиринту разбросаны сокровища. Они обозначены буквой Х. " +
                    "Соберите их все и сможете победить!");
                    DogPainter("Берегитесь злых собак '@', они вас сильно задерживают!!!\n", ConsoleColor.Red);
                    Console.WriteLine($"Счётчик сокровищ: {TreasureCount}");
                    Console.SetCursorPosition(0, 0);
                    DrawMap(map, ConsoleColor.DarkYellow, ConsoleColor.Cyan);
                    Console.SetCursorPosition(playerX, playerY);
                    Painter('T', ConsoleColor.Green);
                    Console.SetCursorPosition(0, 0);



                }

            });
            
            while (wannaPlay == true)
            {

                if (showresult == true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    if (ifWin == true)
                    {
                        Console.SetCursorPosition(40, 16);
                        Console.WriteLine($"Ваш предудущий результат {PreviousResult} ходов");
                        Console.SetCursorPosition(40, 17);
                        Console.WriteLine($"Ваш лучший результат {maxWinResult} ходов");
                    }
                    else
                    {
                        Console.SetCursorPosition(40, 16);
                        Console.WriteLine($"Ваш предудущий результат {PreviousResult} собранных сокровищ");
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


                if (MovesAvailable <= 0 | playerX == EnemyY0 & playerY == EnemyX0 || playerX == EnemyY1 & playerY == EnemyX1 || playerX == EnemyY2 & playerY == EnemyX2 || playerX == EnemyY3 & playerY == EnemyX3 ||
                playerX == PreviousEnemyY0 & playerY == PreviousEnemyX0 || playerX == PreviousEnemyY1 & playerY == PreviousEnemyX1 || playerX == PreviousEnemyY2 & playerY == PreviousEnemyX2 || playerX == PreviousEnemyY3 & playerY == PreviousEnemyX3 ||
                playerX == EnemyY4 & playerY == EnemyX4 || playerX == EnemyY5 & playerY == EnemyX5 || playerX == PreviousEnemyY4 & playerY == PreviousEnemyX4 || playerX == PreviousEnemyY5 & playerY == PreviousEnemyX5)
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
                        PreviousResult = TreasureCount;
                        ifWin = false;
                        switch (EverWin)
                        {
                            case true:
                                maxLoseResult = maxWinResult;
                                break;
                            case false:
                                if (PreviousResult >= maxLoseResult)
                                {
                                    maxLoseResult = PreviousResult;
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
                MovesAvailable--;


                if (map[playerX, playerY] == 'X')
                {
                    TreasureCount += 1;
                    map[playerX, playerY] = ' ';
                }
                if (map[playerX, playerY] == '@')
                {
                    MovesAvailable -= 10;

                }
                if (TreasureCount == Treasures)
                {
                    Console.Clear();
                    Console.WriteLine($"Вы победиди за {StartMoves - MovesAvailable} ходов, поздравляю!!!");
                    wannaPlay = false;
                    EverWin = true;
                    PreviousResult = StartMoves - MovesAvailable;
                    showresult = true;
                    ifWin = true;
                    if (PreviousResult <= maxWinResult)
                    {
                        maxWinResult = PreviousResult;
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
            if (EverWin == true)
            {
                Console.WriteLine($" Ваш лучший результат составил {maxWinResult} ходов(а)");
            }
            else if (EverWin == false)
            {
                Console.WriteLine(" Ваш лучший результат составил " + TreasureCount + " сокровищ(а)");
            }
            Console.ReadKey();
        }
        static void EnemySpot(char[,] map, out int EnemyX, out int EnemyY, out int EnemyX1, out int EnemyY1)
        {
            Random rand = new Random();
        loop3:
            int RandEnemyX = rand.Next(0, map.GetLength(0));

            int RandEnemyY = rand.Next(0, map.GetLength(1));
            if (map[RandEnemyX, RandEnemyY] != '-' & map[RandEnemyX, RandEnemyY] != '_' & map[RandEnemyX, RandEnemyY] != '|' & map[RandEnemyX, RandEnemyY] != 'T')
            {
                EnemyX = RandEnemyY;
                EnemyY = RandEnemyX;

            }
            else
            {
                goto loop3;
            }
        loop5:
            int RandEnemyX1 = rand.Next(0, map.GetLength(0));

            int RandEnemyY1 = rand.Next(0, map.GetLength(1));
            if (map[RandEnemyX1, RandEnemyY1] != '-' & map[RandEnemyX1, RandEnemyY1] != '_' & map[RandEnemyX1, RandEnemyY1] != '|' & map[RandEnemyX1, RandEnemyY1] != 'T' & map[RandEnemyX1, RandEnemyY1] != '!')
            {
                EnemyX1 = RandEnemyY1;
                EnemyY1 = RandEnemyX1;
            }
            else
            {
                goto loop5;
            }
        }
        static void EnemyBehavior(char[,] map, ref int EnemyY, ref int EnemyX, ref int PreviousEnemyY, ref int PreviousEnemyX)
        {

            Random rd = new Random();
            bool RightPosition = false;

            while (RightPosition == false)
            {

                switch (rd.Next(1, 5))
                {

                    case 1:
                        if (map[EnemyX - 1, EnemyY] != '-' & map[EnemyX - 1, EnemyY] != '_' & map[EnemyX - 1, EnemyY] != '|'
                            & map[EnemyX - 1, EnemyY] != 'X' & map[EnemyX - 1, EnemyY] != '@')
                        {

                            EnemyX--;
                            RightPosition = true;
                        }

                        break;
                    case 2:
                        if (map[EnemyX + 1, EnemyY] != '-' & map[EnemyX + 1, EnemyY] != '_' & map[EnemyX + 1, EnemyY] != '|'
                            & map[EnemyX + 1, EnemyY] != 'X' & map[EnemyX + 1, EnemyY] != '@')
                        {

                            EnemyX++;
                            RightPosition = true;
                        }

                        break;
                    case 3:
                        if (map[EnemyX, EnemyY - 1] != '-' & map[EnemyX, EnemyY - 1] != '_' & map[EnemyX, EnemyY - 1] != '|'
                            & map[EnemyX, EnemyY - 1] != 'X' & map[EnemyX, EnemyY - 1] != '@')
                        {

                            EnemyY--;
                            RightPosition = true;
                        }

                        break;
                    case 4:
                        if (map[EnemyX, EnemyY + 1] != '-' & map[EnemyX, EnemyY + 1] != '_' & map[EnemyX, EnemyY + 1] != '|'
                            & map[EnemyX, EnemyY + 1] != 'X' & map[EnemyX, EnemyY + 1] != '@')
                        {

                            EnemyY++;
                            RightPosition = true;
                        }
                        break;
                }
            }
            Console.SetCursorPosition(EnemyX, EnemyY);
            Painter('!', ConsoleColor.Red);
            Console.SetCursorPosition(PreviousEnemyX, PreviousEnemyY);
            Painter(' ', ConsoleColor.Black);
        }
        static void Painter(char entity, ConsoleColor color)
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
                ;
            }
            else if (pressedKey.Key == ConsoleKey.D)
            {
                direction[0]++;

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
    }
}