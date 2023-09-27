using System.Data.SqlClient;

namespace HodimBrodim
{
    public static class ReckordsRepository
    {
        private static int _maxID;
        private static int _mapID;
        private static List<UserData> _userReckords = new();
        private static string connectionString =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=Reckords;Integrated Security=True";
        public static void LoadReckords(int mapID)
        {
            _mapID = mapID;
            using SqlConnection connection = new(connectionString);
            connection.Open();

            var sqlQuery = $"SELECT TOP 10 * FROM Reckord where maptype = {mapID} ORDER BY Score ASC";
            using SqlCommand command = new(sqlQuery, connection);
            using SqlDataReader reader = command.ExecuteReader();

            _userReckords = new List<UserData>();
            while (reader.Read())
            {
                string name = (string)reader["Nickname"];
                int score = (int)reader["Score"];
                DateTime date = (DateTime)reader["ScoreDate"];
                _userReckords.Add(new UserData(name, score, date));
            }
            reader.Close();

            command.CommandText = "select max(Id) from Reckord";
            _maxID = (int)(command.ExecuteScalar() ?? 1);
        }

        private static void UpdateBase(UserData newRow)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            string insertQuery = $"insert into Reckord values ({_maxID + 1}, '{newRow.Name}', {newRow.Score}, " +
                $"'{newRow.Date}', {_mapID})";
            using SqlCommand insertCommand = new(insertQuery, connection);
            insertCommand.ExecuteNonQuery();

            _userReckords.Add(newRow);          
            _userReckords = _userReckords.OrderBy(x => x.Score).ToList();

            _maxID++;
            connection.Close();
        }

        public static void AddRecords(int playerScore)
        {
            ShowRecordsTable();
            Console.WriteLine($"Хотите внести свой результат ({playerScore} ходов) в таблицу? (для этого нажмите 'y')");

            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                Console.WriteLine("Введите ваше имя ");
                var nameOfPlayer = Console.ReadLine()?? "пожелавший остаться неизвестным";
                UpdateBase(new UserData(nameOfPlayer, playerScore, DateTime.Now));
            }

            Console.WriteLine("Чтобы увидеть обновлённую таблицу нажмите 'R'");
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
                ShowRecordsTable();
        }
        public static void ShowRecordsTable()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Clear();
            Console.WriteLine("Таблица рекордов\n");

            _userReckords.ForEach(user => Console.WriteLine(user));

            Console.ReadKey();
            Console.Clear();
        }
    }

    internal struct UserData
    {
        public string Name { get; init; }
        public int Score { get; init; }
        public DateTime Date { get; init; }

        public UserData(string name, int score, DateTime date)
        {
            Name = name;
            Score = score;
            Date = date;
        }

        public override string ToString()
        {
            return $"Игрок {Name} победил за {Score} ходов. Рекорд был установлен {Date}";
        }
    }
}
