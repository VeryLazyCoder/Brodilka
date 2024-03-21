using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrodilkaManualTesting.UserData
{
    public static class UserProvider
    {
        private static CurrentUser _currentUser;
        public static void SetCurrentUser(CurrentUser currentUser) => _currentUser = currentUser;

        public static CurrentUser GetCurrentUser() => _currentUser?? throw new NullReferenceException();
    }
}
