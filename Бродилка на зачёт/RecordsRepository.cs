using System.Data.SqlClient;

namespace HodimBrodim
{
    public static class RecordsRepository
    {
        private static int _maxID;
        private static int _mapID;
        private static List<UserData> _userRecords = new();
        private const string CONNECTION_STRING =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=Reckords;Integrated Security=True";

        public static void LoadRecords(int mapID)
        {
            _mapID = mapID;
            using SqlConnection connection = new(CONNECTION_STRING);
            connection.Open();

            var sqlQuery = @$"SELECT TOP 10 * FROM Reckord where maptype = {mapID} ORDER BY Score ASC";
            using SqlCommand command = new(sqlQuery, connection);
            using var reader = command.ExecuteReader();

            _userRecords = new List<UserData>();
            while (reader.Read())
            {
                var name = (string)reader["Nickname"];
                var score = (int)reader["Score"];
                var date = (DateTime)reader["ScoreDate"];
                _userRecords.Add(new UserData(name, score, date));
            }
            reader.Close();

            command.CommandText = "select max(Id) from Reckord";
            _maxID = (int)(command.ExecuteScalar() ?? 1);
        }

        private static void UpdateBase(UserData newRow)
        {
            using SqlConnection connection = new(CONNECTION_STRING);
            connection.Open();

            var insertQuery = @$"insert into Reckord values ({_maxID + 1}, '{newRow.Name}', {newRow.Score}, " +
                              $"'{newRow.Date}', {_mapID})";
            using SqlCommand insertCommand = new(insertQuery, connection);
            insertCommand.ExecuteNonQuery();

            _userRecords.Add(newRow);          
            _userRecords = _userRecords.OrderBy(x => x.Score).ToList();

            _maxID++;
            connection.Close();
        }

        public static void OfferAddRecord(int playerScore)
        {
            ShowRecordsTable();
            Console.WriteLine($"Хотите внести свой результат ({playerScore} ходов) в таблицу? (для этого нажмите 'y')");

            if (Console.ReadKey(true).Key == ConsoleKey.Y)
                AddUserRecord(playerScore);

            Console.WriteLine("Чтобы увидеть обновлённую таблицу нажмите 'R'");
            if (Console.ReadKey(true).Key == ConsoleKey.R)
                ShowRecordsTable();
        }

        private static void AddUserRecord(int playerScore)
        {
            Console.WriteLine("Введите ваше имя ");
            var nameOfPlayer = Console.ReadLine();
            if (nameOfPlayer.Trim().Length == 0)
                nameOfPlayer = "без имени";
            UpdateBase(new UserData(nameOfPlayer, playerScore, DateTime.Now));
        }

        public static void ShowRecordsTable()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Clear();
            Console.WriteLine("Таблица рекордов\n");

            _userRecords.ForEach(user => Console.WriteLine(user));

            Console.ReadKey();
            Console.Clear();
        }
    }

    internal readonly struct UserData
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
