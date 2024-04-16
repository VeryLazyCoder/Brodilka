using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HodimBrodim.DBData
{
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

        public void Register()
        {
            CurrentUser.Account = this;
        }
    }
}
