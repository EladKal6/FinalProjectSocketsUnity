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

            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
        }

        public static void StartLobby(int _fromClient, Packet _packet)
        {
            int _maxPlayers = _packet.ReadInt();
            Server.lobbies.Add(_fromClient, new Lobby(_maxPlayers));
            Server.lobbies[_fromClient].AddClient(_fromClient);
        }

        public static void JoinLobby(int _fromClient, Packet _packet)
        {
            int _lobbyClient = _packet.ReadInt();
            Server.lobbies[_lobbyClient].AddClient(_fromClient);

        }

        public static void ShowLobbies(int _fromClient, Packet _packet)
        {
            ServerSend.SendLobbies(_fromClient);
        }
    }
}
