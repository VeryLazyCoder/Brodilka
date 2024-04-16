using System.Data;
using HodimBrodim;
using System.Data.SqlClient;
using BrodilkaManualTesting.PasswordExtensions;

namespace BrodilkaManualTesting
{
    internal class ManualTests
    {
        private const string CONNECTION_STRING =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=Records;Integrated Security=True";

        private static int _userID;
        private static int _accountID;

        public static void Main(string[] args)
        {
            //SignUp("Anton Andreev", "anton", "123");
            SignIn("anton", "123");
            Console.WriteLine(string.Join("\n", GetConnectedToUserAccounts()));
            CreateGameAccount("228");
            Console.WriteLine("\n\n");
            Console.WriteLine(string.Join("\n", GetConnectedToUserAccounts()));
        }

        private static void SignUp(string name, string login, string password)
        {
            using SqlConnection connection = new SqlConnection(CONNECTION_STRING);
            connection.Open();

            using var command = new SqlCommand($"select count(*) from Users where UserLogin = '{login}'", connection);
            if ((int)command.ExecuteScalar() != 0)
                throw new DataException("Такой пользователь уже зарегистрирован");

            var nextID = GetNextIdFromTable("UserID", "Users");
            var passwordHash = password.GetHashValue();  

            command.CommandText = $"insert into Users values ({nextID}, '{name}', '{login}', {passwordHash})";
            command.ExecuteNonQuery();
            _userID = nextID;
        }

        private static int GetNextIdFromTable(string column, string tableName)
        {
            using var connection = new SqlConnection(CONNECTION_STRING);
            connection.Open();

            using var command = new SqlCommand($"select max({column}) from {tableName}", connection);

            var commandResult = command.ExecuteScalar();
            return commandResult is DBNull ? 1 : (int)commandResult + 1;
        }

        private static void SignIn(string login, string password)
        {
            using SqlConnection connection = new SqlConnection(CONNECTION_STRING);
            connection.Open();

            var passwordHash = password.GetHashValue();
            using var command =
                new SqlCommand($"select UserID from Users where UserLogin = '{login}' and UserPassword = {passwordHash}",
                    connection);
            var result = command.ExecuteScalar();

            if (result is DBNull)
                throw new DataException("Неверный логин или пароль");
            _userID = (int)result;
        }

        private static List<UserAccount> GetConnectedToUserAccounts()
        {
            using var connection = new SqlConnection(CONNECTION_STRING);
            connection.Open();

            var command = new SqlCommand($"select * from GameAccounts where UserID = {_userID}", connection);
            var reader = command.ExecuteReader();
            var result = new List<UserAccount>();

            while (reader.Read())
            {
                result.Add(new UserAccount((int)reader["AccountID"], 
                    (string)reader["NickName"],
                    (int)reader["AccountLevel"]));
            }

            return result;
        }

        private static void SetGameAccount(UserAccount account)
        {
            _accountID = account.ID;
        }

        private static void CreateGameAccount(string nickName)
        {
            using var connection = new SqlConnection(CONNECTION_STRING);
            connection.Open();

            var nextID = GetNextIdFromTable("AccountID", "GameAccounts");

            var command = new SqlCommand($"insert into GameAccounts values({nextID}, {_userID}, " +
                                         $"'{nickName}', 1)", connection);
            command.ExecuteNonQuery();
        }
    }

    public class UserAccount
    {
        public UserAccount(int id, string nickName, int level)
        {
            ID = id;
            NickName = nickName;
            Level = level;
        }

        public int ID { get; }
        public string NickName { get; }
        public int Level { get; }

        public override string ToString()
        {
            return $"{NickName} уровня {Level}";
        }
    }

    public class CurrentUser
    {
        public int UserID { get; init; }

        public UserAccount Account { get; init; }
    }
}
