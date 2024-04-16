namespace HodimBrodim
{
    public class MapCreator
    {
        private static MapCell[,] _map;
        public static GameMap CreateMap(int availableMovesOnMap)
        {
            var file = File.ReadAllLines(GetMapPathAccordingChosenMoves(availableMovesOnMap));
            _map = new MapCell[file[0].Length, file.Length];

            for (var x = 0; x < _map.GetLength(0); x++)
            for (var y = 0; y < _map.GetLength(1); y++)
                _map[x, y] = GetMapCell(file[y][x]);

            DrawSymbolOnEmptyCell(MapCell.ArmorBonus);
            DrawSymbolOnEmptyCell(MapCell.DamageBonus);
            DrawSymbolOnEmptyCell(MapCell.HealthBonus);

            return new GameMap(_map);
        }

        private static void DrawSymbolOnEmptyCell(MapCell element)
        {
            while (true)
            {
                var random = new Random();
                var x = random.Next(1, _map.GetLength(0));
                var y = random.Next(1, _map.GetLength(1));
                if (_map[x, y] != MapCell.Empty) continue;
                _map[x, y] = element;
                break;
            }
        }

        private static string GetMapPathAccordingChosenMoves(int availableMoves) =>
            availableMoves switch
            {
                120 => "little.txt",
                160 => "qwerty.txt",
                _ => "bigmap.txt"
            };

        private static MapCell GetMapCell(char symbol) => symbol switch
        {
            '-' => MapCell.HorizontalWall,
            '|' => MapCell.VerticalWall,
            '@' => MapCell.AngryDog,
            'H' => MapCell.HealthBonus,
            'A' => MapCell.ArmorBonus,
            'D' => MapCell.DamageBonus,
            ' ' => MapCell.Empty,
            'X' => MapCell.Treasure
        };
    }
}
