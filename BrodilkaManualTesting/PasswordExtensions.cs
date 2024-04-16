using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BrodilkaManualTesting.PasswordExtensions
{
    public static class PasswordExtensions
    {
        public static int GetHashValue(this string input)
        {
            using SHA256 sha256Hash = SHA256.Create();
            // Вычисляем хеш строки
            var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToInt32(bytes, 0); ;
        }
    }
}
