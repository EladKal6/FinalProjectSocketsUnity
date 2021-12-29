using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace GameServer
{
    class User
    {
        public int id;
        public string username;

        public User(int _id, string _username)
        {
            id = _id;
            username = _username;
        }
    }
}