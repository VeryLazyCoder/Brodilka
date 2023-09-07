using HodimBrodim;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HodimBrodim
{
    public class GameMap
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

        public char this[Point point]
        {
            get => Map[point.X, point.Y];
            set => Map[point.X, point.Y] = value;
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
        public bool IsEmptyCell(Point position) => this[position] != '|' && this[position] != '-';
    }
}
