﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerManager
{
    class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public static Dictionary<int, Lobby> lobbies = new Dictionary<int, Lobby>();
        public static Dictionary<int, Lobby> activeLobbies = new Dictionary<int, Lobby>();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener;


        public static int GetClientIDWithUsername(string _username)
        {
            foreach(int clientID in clients.Keys)
            {
                if(clients[clientID].user != null && clients[clientID].user.username == _username)
                {
                    return clientID;
                }
            }
            Console.WriteLine("Couldn't find client with that username");
            return -999;
        }

        public static void Start(int _maxPlayers, int _port)
        {
            MaxPlayers = _maxPlayers;
            Port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            Console.WriteLine($"Server started on port {Port}.");
        }

        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPacketsLobby.welcomeReceived, ServerHandle.WelcomeReceived },
                { (int)ClientPacketsLobby.requestLobbies, ServerHandle.RequestLobbies },
                { (int)ClientPacketsLobby.hostRequest, ServerHandle.StartLobby },
                { (int)ClientPacketsLobby.joinRequest, ServerHandle.JoinLobby },
                { (int)ClientPacketsLobby.SendIntoGame, ServerHandle.SendIntoGame },
                { (int)ClientPacketsLobby.closeLobby, ServerHandle.CloseLobby },
                { (int)ClientPacketsLobby.playerExitLobby, ServerHandle.PlayerExitLobby },
            };
            Console.WriteLine("Initialized packets.");
        }
    }
}