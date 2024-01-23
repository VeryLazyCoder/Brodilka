namespace HodimBrodim
{
    public class GameMap
    {
        public readonly char[,] Map;
        public int TreasuresOnTheMap { get; private set; }
        public GameMap(char[,] map)
        {
            Map = map;
            CountTreasures();
        }

        public char this[Point point]
        {
            get => Map[point.X, point.Y];
            set => Map[point.X, point.Y] = value;
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
