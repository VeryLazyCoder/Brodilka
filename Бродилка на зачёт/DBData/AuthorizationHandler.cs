using System.Data;
using System.Data.SqlClient;

namespace HodimBrodim.DBData
{
    public class AuthorizationHandler
    {
        private const string CONNECTION_STRING =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=Records;Integrated Security=True";

        private AuthorizationHandler()
        {
            
        }

        public static AuthorizationHandler SignUp(string name, string login, string password)
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
            CurrentUser.UserID = nextID;
            return new AuthorizationHandler();
        }

        public static AuthorizationHandler SignIn(string login, string password)
        {
            using SqlConnection connection = new SqlConnection(CONNECTION_STRING);
            connection.Open();

            var passwordHash = password.GetHashValue();
            using var command =
                new SqlCommand($"select UserID from Users where UserLogin = '{login}' and UserPassword = {passwordHash}",
                    connection);
            var result = command.ExecuteScalar()?? throw new DataException("Неверный логин или пароль"); ;

            if (result is DBNull)
                throw new DataException("Неверный логин или пароль");
            CurrentUser.UserID = (int)result;
            return new AuthorizationHandler();
        }

        public List<UserAccount> GetConnectedToUserAccounts()
        {
            using var connection = new SqlConnection(CONNECTION_STRING);
            connection.Open();

            var command = new SqlCommand($"select * from GameAccounts where UserID = {CurrentUser.UserID}", connection);
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

        public List<UserAccount> DisplayConnectedAccounts()
        {
            Console.WriteLine(string.Join("\n", GetConnectedToUserAccounts()));
            return GetConnectedToUserAccounts();
        }

        public void CreateAndChooseGameAccount(string nickName)
        {
            using var connection = new SqlConnection(CONNECTION_STRING);
            connection.Open();

            var nextID = GetNextIdFromTable("AccountID", "GameAccounts");

            var command = new SqlCommand($"insert into GameAccounts values({nextID}, {CurrentUser.UserID}, " +
                                         $"'{nickName}', 1)", connection);
            command.ExecuteNonQuery();
            new UserAccount(nextID, nickName, 1).Register();
            
        }

        
        private static int GetNextIdFromTable(string column, string tableName)
        {
            using var connection = new SqlConnection(CONNECTION_STRING);
            connection.Open();

            using var command = new SqlCommand($"select max({column}) from {tableName}", connection);

            var commandResult = command.ExecuteScalar();
            return commandResult is DBNull ? 1 : (int)commandResult + 1;
        }
    }
}
