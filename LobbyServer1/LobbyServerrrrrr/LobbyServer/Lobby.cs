using System;
using System.Collections.Generic;
using System.Text;

namespace LobbyServer
{
    class Lobby
    {
        public string lobbyName { get; }
        public int maxPlayers { get; }
        public Client[] clients { get; }
        public int current { get; }
        public Lobby(string lobbyName, int _maxPlayers)
        {
            this.lobbyName = lobbyName;
            this.maxPlayers = _maxPlayers;
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
