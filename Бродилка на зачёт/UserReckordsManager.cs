using System.Data.SqlClient;

namespace HodimBrodim
{
    public class UserReckordsManager
    {
        private string _name;
        private int _score;
        private DateTime _date;

        private static int _maxID;
        private static List<UserReckordsManager> _userReckords = new List<UserReckordsManager>();
        private static string connectionString =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=Reckords;Integrated Security=True";
        private UserReckordsManager(string name, int score, DateTime date)
        {
            _name = name;
            _score = score;
            _date = date;
        }

        public static void LoadReckords(int mapID)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            var sqlQuery = $"SELECT TOP 10 * FROM Reckord where maptype = {mapID} ORDER BY Score ASC";
            using SqlCommand command = new(sqlQuery, connection);
            using SqlDataReader reader = command.ExecuteReader();

            _userReckords = new List<UserReckordsManager>();
            while (reader.Read())
            {
                string name = (string)reader["Nickname"];
                int score = (int)reader["Score"];
                DateTime date = (DateTime)reader["ScoreDate"];
                _userReckords.Add(new UserReckordsManager(name, score, date));
            }
            reader.Close();

            command.CommandText = "select max(Id) from Reckord";
            _maxID = (int)(command.ExecuteScalar() ?? 1);
        }

        private static void UpdateBase(int mapID, UserReckordsManager newRow)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            string insertQuery = $"insert into Reckord values ({_maxID + 1}, '{newRow._name}', {newRow._score}, " +
                $"'{newRow._date}', {mapID})";
            using SqlCommand insertCommand = new(insertQuery, connection);
            insertCommand.ExecuteNonQuery();

            _userReckords.Add(newRow);          
            _userReckords = _userReckords.OrderBy(x => x._score).ToList();

            _maxID++;
            connection.Close();
        }

        public static void AddRecords(int mapID, int playerScore)
        {
            ShowRecordsTable(new ConsoleKeyInfo('R', ConsoleKey.R, false, false, false));
            Console.WriteLine($"Хотите внести свой результат ({playerScore} ходов) в таблицу? (для этого нажмите 'y')");

            var userInput = Console.ReadKey(true).Key;
            if (userInput == ConsoleKey.Y)
            {
                Console.WriteLine("Введите ваше имя ");
                string nameOfPlayer = Console.ReadLine();
                UpdateBase(mapID, new UserReckordsManager(nameOfPlayer, playerScore, DateTime.Now));
            }

            Console.WriteLine("Чтобы увидеть обновлённую таблицу нажмите 'R'");
            ConsoleKeyInfo pressedKey = Console.ReadKey();
            ShowRecordsTable(pressedKey);
        }
        public static void ShowRecordsTable(ConsoleKeyInfo pressedKey)
        {
            if (pressedKey.Key == ConsoleKey.R)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Clear();
                Console.WriteLine("Таблица рекордов\n");

                foreach (var player in _userReckords)
                    Console.WriteLine($"Игрок {player._name} победил за {player._score} ходов." +
                        $" Рекорд был установлен {player._date}");

                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
