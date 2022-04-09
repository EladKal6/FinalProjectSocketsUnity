using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ServerManager
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }

            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
            Server.clients[_fromClient].user = new User(_fromClient, _username);
        }

        public static void StartLobby(int _fromClient, Packet _packet)
        {
            string _lobbyName = _packet.ReadString();
            int _maxPlayers = _packet.ReadInt();
            try
            {
                Server.lobbies.Add(_fromClient, new Lobby(_lobbyName, _maxPlayers));
                Server.lobbies[_fromClient].AddClient(_fromClient);
                Console.WriteLine($"Added Server - {_lobbyName}, opened by - {_fromClient} and has {_maxPlayers} max players");
            }
            catch
            {
                Console.WriteLine($"Server Created by {_fromClient} already exists!");
            }
        }

        public static void JoinLobby(int _fromClient, Packet _packet)
        {
            string lobbyName = _packet.ReadString();

            int lobbyHostClientID = -999;

            Console.WriteLine(lobbyName);
            foreach (int LobbyID in Server.lobbies.Keys)
            {
                if (Server.lobbies[LobbyID] != null &&
                    Server.lobbies[LobbyID].lobbyName == lobbyName)
                {
                    lobbyHostClientID = Server.lobbies[LobbyID].clients[0].id;
                }
            }

            Server.lobbies[lobbyHostClientID].AddClient(_fromClient);
            Console.WriteLine($"the lobby created by {lobbyHostClientID} contains {_fromClient}");
            foreach (Client client in Server.lobbies[lobbyHostClientID].clients)
            {
                if(client != null && client.id != _fromClient)
                {
                    ServerSend.PlayerjoinedLobby(client.id, Server.clients[_fromClient].user.username);
                }
            }

            foreach (Client client in Server.lobbies[lobbyHostClientID].clients)
            {
                if(client != null)
                {
                    ServerSend.PlayerjoinedLobby(_fromClient, client.user.username);
                }
            }
        }

        public static void RequestLobbies(int _fromClient, Packet _packet)
        {
            ServerSend.SendLobbies(_fromClient);
        }

        public static void SendIntoGame(int _fromClient, Packet _packet)
        {
            foreach(Client client in Server.lobbies[_fromClient].clients)
            {
                if (client != null)
                {
                    ServerSend.SendIntoGame(client.id);
                }
            }
            Server.lobbies.Remove(_fromClient);
        }

        public static void CloseLobby(int _fromClient, Packet _packet)
        {
            foreach (Client client in Server.lobbies[_fromClient].clients)
            {
                if (client != null && client.id != _fromClient)
                {
                    ServerSend.RemoveLobby(client.id);
                }
            }
            Console.WriteLine($"Removed {Server.clients[_fromClient].user.username}'s Lobby");
            Server.lobbies.Remove(_fromClient);
        }

        public static void PlayerExitLobby(int _fromClient, Packet _packet)
        {
            foreach (Client client in Server.lobbies[_fromClient].clients)
            {
                if (client != null)
                {
                    ServerSend.PlayerDisconnectedLobby(client.id, Server.clients[_fromClient].user.username);
                }
            }
        }
    }
}
