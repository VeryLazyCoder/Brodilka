namespace HodimBrodim
{
    public class GameRound
    {
        public Player Player { get; private set; }
        public GameMap Map { get; private set; }

        private int _startMoves;
        private readonly int _initialEnemisCount;
        private Dictionary<char, Action<Player, GameMap>> _actionsOnCollision = new()
        {
            ['X'] = (player, map) =>
            {
                player.AddTreasure();
                map[player.Position] = ' ';
                player.RaiseStats();
            },
            ['A'] = (player, map) =>
            {
                player.ChangeArmorFor(3);
                map[player.Position] = ' ';
            },
            ['D'] = (player, map) =>
            {
                player.ChangeDamageFor(player.Damage / 3);
                map[player.Position] = ' ';
            },
            ['H'] = (player, map) =>
            {
                player.ChangeHealthFor(player.Health / 3);
                map[player.Position] = ' ';
            },
            ['@'] = (player, map) => player.MovesAvailable -= 10,
            [' '] = (player, map) => { },
        };
        private List<IEnemy> _enemies;
        private readonly HashSet<ConsoleKey> _validKeys = new()
        {
            ConsoleKey.W,
            ConsoleKey.A,
            ConsoleKey.D,
            ConsoleKey.S,
            ConsoleKey.Spacebar,
        };
        private RandomEventsHandler _eventHandler;

        public GameRound(int startMoves, int enemyCount)
        {
            _startMoves = startMoves;
            _initialEnemisCount = enemyCount;
            Map = new GameMap();
            _enemies = GetEnemies(enemyCount);
            Player = new Player(Map.GetEmptyPosition(), _startMoves);
            _eventHandler = new();
        }

        public (int userScore, bool isWinResult) StartGameLoop()
        {
            Console.Clear();
            while (true)
            {
                DisplayGameObjects();

                var pressedKey = Console.ReadKey(true);
                if (pressedKey.Key == ConsoleKey.R)
                    RecordsRepository.ShowRecordsTable();
                if (!IsValidTurn(pressedKey))
                    continue;

                Player.Move(pressedKey, Map);
                MoveEnemies();

                _eventHandler.TryRaiseEvent(this);
                _actionsOnCollision[Map[Player.Position]].Invoke(Player, Map);

                if (Player.TreasureCount == Map.TreasuresOnTheMap ||
                    (_enemies.Count == 0 && _initialEnemisCount != 0))
                    break;

                if (Player.MovesAvailable <= 0 || Player.PlayerIsDead == true)
                    return (-1, false);
            }
            return (_startMoves - Player.MovesAvailable, true);
        }
        public void AddAdditionalEnemy() => 
            _enemies.Add(new CommomEnemy(Map.GetEmptyPosition(), Map));

        private void DisplayGameObjects()
        {
            Player.ShowPlayerStatistic();
            Map.DrawMap();
            DisplayEnemies();
            DisplayCharacter();
        }
        private void MoveEnemies()
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].Move(Player.Position);
                if (_enemies[i].CollisionWithPlayer(Player.Position))
                {
                    Player.FightWithEnemy();
                    _enemies.RemoveAt(i);
                    Map.DrawMap();
                    break;
                }
            }
        }
        private void DisplayEnemies() =>
            _enemies.ForEach(enemy => enemy.Display());
        private List<IEnemy> GetEnemies(int enemyCount)
        {
            var enemies = new List<IEnemy>();

            for (int i = 0; i < enemyCount; i++)
            {
                if (i == 2)
                    enemies.Add(GetEnemy(new Point(Map.Map.GetLength(0) - 2, Map.Map.GetLength(1) - 2)));
                else if (i == 3)
                    enemies.Add(GetEnemy(new(1, 1)));
                else
                    enemies.Add(GetEnemy(Map.GetEmptyPosition()));
            }
            return enemies;
        }
        private IEnemy GetEnemy(Point point) => 
            new Random().Next(3) == 0? new SmartEnemy(point, Map) : new CommomEnemy(point, Map);
        private void DisplayCharacter()
        {
            Console.SetCursorPosition(Player.Position.X, Player.Position.Y);
            Program.Paint('T', ConsoleColor.Green);
        }
        private bool IsValidTurn(ConsoleKeyInfo pressedKey)
        {
            if (pressedKey.Key == ConsoleKey.Escape)           
                Program.CloseGame();
            
            return _validKeys.Contains(pressedKey.Key);
        }
    }
}
