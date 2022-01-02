using System;
using System.Collections.Generic;
using System.Text;
using ServerManager;

namespace ServerManager
{
    class Lobby
    {
        public int maxPlayers { get; }
        public Client[] clients { get; }
        public int current { get; }
        public Lobby(int _maxPlayers)
        {
            maxPlayers = _maxPlayers;
            clients = new Client[maxPlayers];
            current = 0;
        }

        public void AddClient(int _clientID)
        {
            if (current == maxPlayers)
            {
                Console.WriteLine("Lobby Full!");
            }
            clients[current] = Server.clients[_clientID];
        }
    }
}
