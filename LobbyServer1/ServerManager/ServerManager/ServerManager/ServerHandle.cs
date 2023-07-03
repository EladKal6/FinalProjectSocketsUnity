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
            string _password = RSAEncryption.Decrypt(_packet.ReadString());

            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
                ServerSend.LoginError(_fromClient, "You have assumed the wrong client ID");
                return;
            }
            int DBUserId = SqliteDataAccess.CheckLogin(_username, _password);
            if (DBUserId == -888)
            {
                Console.WriteLine($"User with username {_username} is already logged in!");
                ServerSend.LoginError(_fromClient, "This user is already logged in!");
                return;
            }
            else if (DBUserId != -999)
            {
                ServerSend.LoginOk(_fromClient);
                Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
                Server.clients[_fromClient].user = new User(_fromClient, _username);
            }
            else
            {
                ServerSend.LoginError(_fromClient, "Username or password are incorrect");
                return;
            }
        }

        public static void SignUp(int _fromClient, Packet _packet)
        {
            string _email = _packet.ReadString();
            string _username = _packet.ReadString();
            string _password = RSAEncryption.Decrypt(_packet.ReadString());

            Server.clients[_fromClient].user = new User(_username, _password);

            Console.WriteLine($"SOMEONE TRIED TO SIGN UP HIS USERNAME {_username} HIS PASSWORD {_password}");

            Server.clients[_fromClient].emailCode = new Random().Next(10000, 100000);
            EmailVerification.SendEmail(_email, Server.clients[_fromClient].emailCode);
        }

        public static void EmailCode(int _fromClient, Packet _packet)
        {
            int _emailCode = _packet.ReadInt();

            Console.WriteLine($"EMAIL VERIFICATION " + _emailCode);
            if (SqliteDataAccess.IsExists(Server.clients[_fromClient].user.Username))
            {
                ServerSend.LoginError(_fromClient, "User with that username already exists, please try a different one");
            }
            else if (Server.clients[_fromClient].emailCode == _emailCode)
            {
                SqliteDataAccess.SaveUser(Server.clients[_fromClient].user);
                ServerSend.LoginOk(_fromClient);
            }
            else
            {
                ServerSend.LoginError(_fromClient, "email Verification wrong");
            }
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
                    ServerSend.PlayerjoinedLobby(client.id, Server.clients[_fromClient].user.gameusername);
                }
            }

            foreach (Client client in Server.lobbies[lobbyHostClientID].clients)
            {
                if(client != null)
                {
                    ServerSend.PlayerjoinedLobby(_fromClient, client.user.gameusername);
                }
            }
        }

        public static void RequestLobbies(int _fromClient, Packet _packet)
        {
            ServerSend.SendLobbies(_fromClient);
        }

        public static void SendIntoGame(int _fromClient, Packet _packet)
        {
            int bestOf = (int)_packet.ReadFloat();

            Console.WriteLine("bestOf: " + bestOf);
            Server.StartGameServer(Server.lobbies[_fromClient].current, bestOf, false);

            System.Threading.Thread.Sleep(6000);

            if (Server.lobbies.ContainsKey(_fromClient)) // if false then user exited right after pressing "start game"
            {
                foreach (Client client in Server.lobbies[_fromClient].clients)
                {
                    if (client != null)
                    {
                        System.Threading.Thread.Sleep(1000);
                        ServerSend.SendIntoGame(client.id);
                    }
                }

                Server.currGameServerPort++;
            }


        }

        public static void CloseLobby(int _fromClient, Packet _packet)
        {
            if (Server.lobbies.ContainsKey(_fromClient))
            {
                foreach (Client client in Server.lobbies[_fromClient].clients)
                {
                    if (client != null && client.id != _fromClient)
                    {
                        ServerSend.RemoveLobby(client.id);
                    }
                }
                Console.WriteLine($"Removed {Server.clients[_fromClient].user.gameusername}'s Lobby");
                Server.lobbies.Remove(_fromClient);
            }
        }

        public static void PlayerExitLobby(int _fromClient, Packet _packet)
        {
            if (Server.lobbies.ContainsKey(_fromClient))
            {
                foreach (Client client in Server.lobbies[_fromClient].clients)
                {
                    if (client != null)
                    {
                        ServerSend.PlayerDisconnectedLobby(client.id, Server.clients[_fromClient].user.gameusername);
                    }
                }
            }
        }
    }
}
