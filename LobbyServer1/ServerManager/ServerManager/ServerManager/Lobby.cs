using System;
using System.Collections.Generic;
using System.Text;

namespace ServerManager
{
    class Lobby
    {
        public string lobbyName { get; }
        public int maxPlayers { get; }
        public Client[] clients { get; }
        public int current { get; set; }
        public Lobby(string lobbyName, int _maxPlayers)
        {
            this.lobbyName = lobbyName;
            this.maxPlayers = _maxPlayers;
            clients = new Client[maxPlayers];
            current = 0;
        }

        private int IndexOfFirstNullClient()
        {
            for (int i = 0; i < maxPlayers; i++)
            {
                if (clients[i] == null)
                {
                    return i;
                }
            }
            return -999;
        }

        public void AddClient(int _clientID)
        {
            if (current == maxPlayers)
            {
                Console.WriteLine("Lobby Full!");
                return;
            }
            clients[IndexOfFirstNullClient()] = Server.clients[_clientID];
            current++;
        }

        //removes the client if he is in the lobby,  returns true or false according to if he was in the lobby
        public bool RemoveClient(int clientId)
        {
            for (int i = 0; i < maxPlayers; i++)
            {
                if (clients[i] != null && clients[i].id == clientId)
                {
                    clients[i] = null;
                    current--;
                    return true;
                }
            }
            return false;
        }
    }
}
