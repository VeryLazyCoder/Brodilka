namespace HodimBrodim
{
    public class GameRound
    {
        public Player Player { get; }
        public GameMap Map { get; }
        public bool? IsWon { get; private set; }
        public int UserScore { get; private set; }

        private readonly int _startMoves;
        private readonly int _initialEnemiesCount;
        private readonly HashSet<ConsoleKey> _validKeys = new()
        {
            ConsoleKey.W,
            ConsoleKey.A,
            ConsoleKey.D,
            ConsoleKey.S,
            ConsoleKey.Spacebar,
        };
        private readonly Dictionary<char, Action<Player, GameMap>> _actionsOnCollision = new()
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
        private readonly List<IEnemy> _enemies;
        private readonly RandomEventsHandler _eventHandler;
        private ConsoleKey _pressedKey;

        public GameRound(int startMoves, int enemyCount)
        {
            _startMoves = startMoves;
            _initialEnemiesCount = enemyCount;
            Map = MapCreator.CreateMap(startMoves);
            _enemies = GetEnemies(enemyCount);
            Player = new Player(Map.GetEmptyPosition(), _startMoves);
            _eventHandler = new RandomEventsHandler();
        }

        public void StartGameLoop()
        {
            Console.Clear();
            while (IsWon == null)
            {
                DisplayGameObjects();

                _pressedKey = Console.ReadKey(true).Key;

                if (IsRelevantTurn())
                    ChangeGameState();

                SetRoundResultIfGameIsOver();
            }
        }
        public void AddAdditionalEnemy() =>
            _enemies.Add(new CommonEnemy(Map.GetEmptyPosition(), Map));

        private void SetRoundResultIfGameIsOver()
        {
            if (IsVictoryAchieved())
                (IsWon, UserScore) = (true, _startMoves - Player.MovesAvailable);

            if (Player.IsDead)
                (IsWon, UserScore) = (false, -1);
        }
        private bool IsVictoryAchieved()
        {
            return Player.TreasureCount == Map.TreasuresOnTheMap ||
                (_enemies.Count == 0 && _initialEnemiesCount != 0);
        }
        private void ChangeGameState()
        {
            Player.Move(_pressedKey, Map);
            MoveEnemies();
            _eventHandler.TryRaiseEvent(this);
            _actionsOnCollision[Map[Player.Position]].Invoke(Player, Map);
        }
        private void DisplayRecordsIfNecessary()
        {
            if (_pressedKey == ConsoleKey.R)
                RecordsRepository.ShowRecordsTable();
        }
        private void DisplayGameObjects()
        {
            DisplayRecordsIfNecessary();
            Player.ShowPlayerStatistic();
            Map.DrawMap();
            DisplayEnemies();
            DisplayCharacter();
        }
        private void MoveEnemies()
        {
            for (var i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].Move(Player.Position);
                if (!_enemies[i].CollisionWithPlayer(Player.Position)) 
                    continue;
                Player.FightWithEnemy();
                _enemies.RemoveAt(i);
                Map.DrawMap();
                break;
            }
        }
        private void DisplayEnemies() =>
            _enemies.ForEach(enemy => enemy.Display());
        private List<IEnemy> GetEnemies(int enemyCount)
        {
            var enemies = new List<IEnemy>();

            for (var i = 0; i < enemyCount; i++)
            {
                switch (i)
                {
                    case 2:
                        enemies.Add(GetEnemy(new Point(Map.Map.GetLength(0) - 2, 
                            Map.Map.GetLength(1) - 2)));
                        break;
                    case 3:
                        enemies.Add(GetEnemy(new Point(1, 1)));
                        break;
                    default:
                        enemies.Add(GetEnemy(Map.GetEmptyPosition()));
                        break;
                }
            }
            return enemies;
        }
        private IEnemy GetEnemy(Point point) =>
            new Random().Next(3) == 0 ? new SmartEnemy(point, Map) : new CommonEnemy(point, Map);
        private void DisplayCharacter()
        {
            Console.SetCursorPosition(Player.Position.X, Player.Position.Y);
            Program.Paint('T', ConsoleColor.Green);
        }
        private bool IsRelevantTurn()
        {
            if (_pressedKey == ConsoleKey.Escape)
                Program.CloseGame();

            return _validKeys.Contains(_pressedKey);
        }
    }
}
