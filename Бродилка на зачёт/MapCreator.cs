namespace HodimBrodim
{
    public class MapCreator
    {
        private static char[,] _map;
        public static GameMap CreateMap(int availableMovesOnMap)
        {
            var file = File.ReadAllLines(GetMapPathAccordingChosenMoves(availableMovesOnMap));
            _map = new char[file[0].Length, file.Length];

            for (var x = 0; x < _map.GetLength(0); x++)
            for (var y = 0; y < _map.GetLength(1); y++)
                _map[x, y] = file[y][x];

            DrawSymbolOnEmptyCell('A');
            DrawSymbolOnEmptyCell('D');
            DrawSymbolOnEmptyCell('H');

            return new GameMap(_map);
        }

        private static void DrawSymbolOnEmptyCell(char symbol)
        {
            while (true)
            {
                var random = new Random();
                var x = random.Next(1, _map.GetLength(0));
                var y = random.Next(1, _map.GetLength(1));
                if (_map[x, y] != ' ') continue;
                _map[x, y] = symbol;
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
    }
}
