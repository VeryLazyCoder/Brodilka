namespace HodimBrodim
{
    public class GameMap
    {
        private static string _pathToMap;
        public static int MapId { get; private set; }
        public readonly char[,] Map;
        public int TreasuresOnTheMap { get; private set; }
        public GameMap()
        {
            var file = File.ReadAllLines(_pathToMap);
            var map = new char[file[0].Length, file.Length];
            for (var x = 0; x < map.GetLength(0); x++)
                for (var y = 0; y < map.GetLength(1); y++)
                    map[x, y] = file[y][x];
            Map = map;

            DrawSymbolOnEmptyCell('A');
            DrawSymbolOnEmptyCell('D');
            DrawSymbolOnEmptyCell('H');
            CountTreasures();
        }

        public char this[Point point]
        {
            get => Map[point.X, point.Y];
            set => Map[point.X, point.Y] = value;
        }
        public static int GetMovesOnChosenMap(int mapVariant)
        {
            MapId = mapVariant;
            switch (MapId)
            {
                case 1:
                    _pathToMap = "little.txt";
                    return 120;
                case 2:
                    _pathToMap = "qwerty.txt";
                    return 160;
                default:
                    _pathToMap = "bigmap.txt";
                    return 300;
            }
        }
        public void DrawMap()
        {
            Console.SetCursorPosition(0, 0);
            for (var y = 0; y < Map.GetLength(1); y++)
            {
                for (var x = 0; x < Map.GetLength(0); x++)
                {
                    var currentSymbol = Map[x, y];
                    Console.ForegroundColor = GetMapObjectsColor(currentSymbol);
                    Console.Write(currentSymbol);
                }
                Console.WriteLine();
            }
        }
        public void AddAdditionalTreasure()
        {
            this[GetEmptyPosition()] = 'X';
            TreasuresOnTheMap++;
        }
        public bool IsNotWall(Point position) => this[position] != '|' && this[position] != '-';
        public Point GetEmptyPosition()
        {
            var random = new Random();
            while (true)
            {
                var position = new Point(random.Next(Map.GetLength(0)),
                    random.Next(Map.GetLength(1)));

                if (this[position] == ' ')
                    return position;
            }
        }

        private void DrawSymbolOnEmptyCell(char symbol)
        {
            while (true)
            {
                var random = new Random();
                var x = random.Next(1, Map.GetLength(0));
                var y = random.Next(1, Map.GetLength(1));
                if (Map[x, y] == ' ')
                {
                    Map[x, y] = symbol;
                    break;
                }
            }
        }
        private void CountTreasures()
        {
            for (var i = 0; i < Map.GetLength(0); i++)
                for (var j = 0; j < Map.GetLength(1); j++)
                    if (Map[i, j] == 'X')
                        TreasuresOnTheMap += 1;
        }
        private ConsoleColor GetMapObjectsColor(char symbol) => symbol switch
        {
            'X' => ConsoleColor.Cyan,
            'A' or 'D' or 'H' => ConsoleColor.DarkGreen,
            '@' => ConsoleColor.Yellow,
            _ => ConsoleColor.DarkYellow
        };
    }
}   
