namespace HodimBrodim
{
    public class GameRound
    {
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
                player.ChangeArmor(3);
                map[player.Position] = ' ';
            },
            ['D'] = (player, map) =>
            {
                player.ChangeDamage(player.Damage / 3);
                map[player.Position] = ' ';
            },
            ['H'] = (player, map) =>
            {
                player.ChangeHealth(player.Health / 3);
                map[player.Position] = ' ';
            },
            ['@'] = (player, map) => player.MovesAvailable -= 10,
            [' '] = (player, map) => { },
        };
        private Player _player;
        private GameMap _map;
        private List<IEnemy> _enemies;
        private readonly HashSet<ConsoleKey> _validKeys = new()
        {
            ConsoleKey.W,
            ConsoleKey.A,
            ConsoleKey.D,
            ConsoleKey.S,
            ConsoleKey.Spacebar,
        };

        public GameRound(int startMoves, int enemyCount)
        {
            _startMoves = startMoves;
            _initialEnemisCount = enemyCount;
            _map = new GameMap();
            _enemies = GetEnemies(enemyCount);
            _player = new Player(Program.GetEmptyPosition(_map), _startMoves);
        }

        public (int userScore, bool isWinResult) StartGame()
        {
            Console.Clear();
            while (true)
            {
                DisplayGameObjects();

                ConsoleKeyInfo pressedKey = Console.ReadKey();
                PlayerInfo.ShowRecordsTable(pressedKey);

                if (!IsValidTurn(pressedKey))
                    continue;

                _player.Move(pressedKey, _map);
                MoveEnemies();

                RandomEvents.InvokeEvent(_map, _player, _enemies);
                _actionsOnCollision[_map[_player.Position]].Invoke(_player, _map);

                if (_player.TreasureCount == _map.TreasuresOnTheMap ||
                    (_enemies.Count == 0 && _initialEnemisCount != 0))
                    break;

                if (_player.MovesAvailable <= 0 || _player.PlayerIsDead == true)
                    return (-1, false);
            }
            return (_startMoves - _player.MovesAvailable, true);
        }

        private void DisplayGameObjects()
        {
            _player.ShowPlayerStatistic();
            _map.DrawMap();
            DisplayCharacter();
            DisplayEnemies();
        }
        private void MoveEnemies()
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].Move(_player.Position);
                if (_enemies[i].CollisionWithPlayer(_player.Position))
                {
                    _player.FightWithEnemy();
                    _enemies.RemoveAt(i);
                    _map.DrawMap();
                    break;
                }
            }
        }
        private void DisplayEnemies() =>
            _enemies.ForEach(enemy => enemy.Display());
        private List<IEnemy> GetEnemies(int enemyCount)
        {
            var enemies = new List<IEnemy>();
            //if (enemyCount <= 0 || enemyCount >= 10)
            //    throw new ArgumentOutOfRangeException(nameof(enemyCount));

            for (int i = 0; i < enemyCount; i++)
            {
                if (i == 2)
                    enemies.Add(GetEnemy(new Point(_map.Map.GetLength(0) - 2, _map.Map.GetLength(1) - 2)));
                else if (i == 3)
                    enemies.Add(GetEnemy(new(1, 1)));
                else
                    enemies.Add(GetEnemy(Program.GetEmptyPosition(_map)));
            }
            return enemies;
        }
        private IEnemy GetEnemy(Point point) => 
            new Random().Next(3) == 0? new SmartEnemy(point, _map) : new CommomEnemy(point, _map);
        private void DisplayCharacter()
        {
            Console.SetCursorPosition(_player.Position.X, _player.Position.Y);
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
