namespace HodimBrodim
{
    public class GameMap
    {
        public MapCell[,] Map { get; }
        public int TreasuresOnTheMap { get; private set; }

        public GameMap(MapCell[,] map)
        {
            Map = map;
            CountTreasures();
        }

        public MapCell this[Point point]
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
                    Console.Write((char)currentSymbol);
                }
                Console.WriteLine();
            }
        }

        public void AddAdditionalTreasure()
        {
            this[GetEmptyPosition()] = MapCell.Treasure;
            TreasuresOnTheMap++;
        }

        public bool IsNotWall(Point position) => this[position] != MapCell.HorizontalWall && 
                                                 this[position] != MapCell.VerticalWall;

        public Point GetEmptyPosition()
        {
            var random = new Random();
            while (true)
            {
                var position = new Point(random.Next(Map.GetLength(0)),
                    random.Next(Map.GetLength(1)));

                if (this[position] == MapCell.Empty)
                    return position;
            }
        }

        private void CountTreasures()
        {
            for (var i = 0; i < Map.GetLength(0); i++)
                for (var j = 0; j < Map.GetLength(1); j++)
                    if (Map[i, j] == MapCell.Treasure)
                        TreasuresOnTheMap += 1;
        }

        private ConsoleColor GetMapObjectsColor(MapCell symbol) => symbol switch
        {
            MapCell.Treasure => ConsoleColor.Cyan,
            MapCell.ArmorBonus or MapCell.DamageBonus or MapCell.HealthBonus => ConsoleColor.DarkGreen,
            MapCell.AngryDog => ConsoleColor.Yellow,
            _ => ConsoleColor.DarkYellow
        };
    }
}   
