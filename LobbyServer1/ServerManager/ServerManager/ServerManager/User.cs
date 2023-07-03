using System;
using System.Collections.Generic;
using System.Text;

namespace ServerManager
{
    public class User
    {
        public Int64 Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public int gameid;
        public string gameusername;

        public User(int _gameid, string _gameusername)
        {
            gameid = _gameid;
            gameusername = _gameusername;
        }

        public User(string _username, string _password)
        {
            Username = _username;
            Password = _password;
        }

        public User()
        {

        }
    }
}
