using System;
using System.Collections.Generic;
using System.Text;

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
        }

        public static void StartLobby(int _fromClient, Packet _packet)
        {
            string _lobbyName = _packet.ReadString();
            int _maxPlayers = _packet.ReadInt();
            try
            {
                Server.lobbies.Add(_fromClient, new Lobby(_lobbyName, _maxPlayers));
            }
            catch
            {
                Console.WriteLine($"Server Created by {_fromClient} already exists!");
            }
            Server.lobbies[_fromClient].AddClient(_fromClient);

            Console.WriteLine($"Added Server - {_lobbyName}, opened by - {_fromClient} and has {_maxPlayers} max players");
        }

        public static void JoinLobby(int _fromClient, Packet _packet)
        {
            int _lobbyClient = _packet.ReadInt();
            Server.lobbies[_lobbyClient].AddClient(_fromClient);

        }

        public static void RequestLobbies(int _fromClient, Packet _packet)
        {
            ServerSend.SendLobbies(_fromClient);
        }
    }
}
