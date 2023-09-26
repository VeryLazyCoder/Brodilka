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
        public static int GetMovesOnChoosenMap(int mapVariant)
        {
            MapID = mapVariant;
            switch (MapID)
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
            for (int y = 0; y < Map.GetLength(1); y++)
            {
                for (int x = 0; x < Map.GetLength(0); x++)
                {
                    var currentSymbol = Map[x, y];
                    Console.ForegroundColor = GetMapObjectsColor(currentSymbol);
                    Console.Write(currentSymbol);
                }
                Console.WriteLine();
            }
        }
        public void AddOneTreasure() => TreasuresOnTheMap++;
        public bool IsNotWall(Point position) => this[position] != '|' && this[position] != '-';
        private void DrawSymbolOnEmptyCell(char symbol)
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
        private void CountTreasures()
        {
            for (int i = 0; i < Map.GetLength(0); i++)
                for (int j = 0; j < Map.GetLength(1); j++)
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
