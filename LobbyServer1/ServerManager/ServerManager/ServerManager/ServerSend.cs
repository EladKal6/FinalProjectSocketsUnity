using System;
using System.Collections.Generic;
using System.Text;

namespace ServerManager
{
    class ServerSend
    {
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_toClient].tcp.SendData(_packet);
        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
        private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.clients[i].tcp.SendData(_packet);
                }
            }
        }

        #region Packets
        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerManagerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SendLobbies(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerManagerPackets.sendLobbies))
            {
                _packet.Write(Server.lobbies.Count);

                foreach (KeyValuePair<int, Lobby> entry in Server.lobbies)
                {
                    _packet.Write(entry.Key);
                    _packet.Write(entry.Value.lobbyName);
                    _packet.Write(entry.Value.maxPlayers);
                    _packet.Write(entry.Value.current);
                }
                SendTCPData(_toClient, _packet);
            }
        }

        //lets a client know that another client joined the lobby
        public static void PlayerjoinedLobby(int _toClient, string _clientName)
        {
            using (Packet _packet = new Packet((int)ServerManagerPackets.sendJoinedPlayer))
            {
                _packet.Write(_clientName); 

                SendTCPData(_toClient, _packet);
            }
        }

        public static void PlayerDisconnectedLobby(int _toClient, string _clientName)
        {
            using (Packet _packet = new Packet((int)ServerManagerPackets.sendDisconnectedPlayer))
            {
                _packet.Write(_clientName);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void RemoveLobby(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerManagerPackets.sendRemovedLobby))
            {
                SendTCPData(_toClient, _packet);
            }
        }

        public static void SendIntoGame(int _toClient)
        {
            using (Packet _packet = new Packet((int)ServerManagerPackets.SendIntoGame))
            {
                SendTCPData(_toClient, _packet);
            }
        }
        #endregion
    }
}
