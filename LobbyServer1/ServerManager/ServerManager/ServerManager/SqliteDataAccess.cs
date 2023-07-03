using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace ServerManager
{
    public class SqliteDataAccess
    {
        public static List<User> LoadUsers()
        {
            using(IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<User>("select * from User", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SaveUser(User user)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
               cnn.Execute("insert into User (Username, Password) values (@Username, @Password)", user);
            }
        }

        public static int CheckLogin(string _username, string _password)
        {
            List<User> users = LoadUsers();

            foreach (Client client in Server.clients.Values)
            {
                if (client != null && client.user != null && client.user.gameusername == _username)
                {
                    return -888;
                }
            }
            foreach (User user in users)
            {
                if (user.Username == _username && user.Password == _password)
                {
                    return (int)user.Id;
                }
            }

            return -999;
        }

        public static bool IsExists(string _username)
        {
            List<User> users = LoadUsers();

            foreach (User user in users)
            {
                if (user.Username == _username)
                {
                    return true;
                }
            }

            return false;
        }
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
