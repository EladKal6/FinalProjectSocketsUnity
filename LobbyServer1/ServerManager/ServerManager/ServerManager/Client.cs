using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ServerManager
{
    class Client
    {
        public static int dataBufferSize = 4096;

        public int id;
        public int emailCode;
        public User user;
        public TCP tcp;

        public Client(int _clientId)
        {
            id = _clientId;
            tcp = new TCP(id);
        }

        public class TCP
        {
            public TcpClient socket;

            private readonly int id;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public TCP(int _id)
            {
                id = _id;
            }

            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                ServerSend.Welcome(id, "Welcome to the server!");
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error sending data to player {id} via TCP: {_ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0)
                    {
                        Console.WriteLine("byte length is zero tcp");
                        Server.clients[id].Disconnect();
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    receivedData.Reset(HandleData(_data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error receiving TCP data: {_ex}");
                    Server.clients[id].Disconnect();
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;

                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Server.packetHandlers[_packetId](id, _packet);
                        }
                    });

                    _packetLength = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (_packetLength <= 1)
                {
                    return true;
                }

                return false;
            }

            public void Disconnect()
            {
                socket.Close();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }

        public void Disconnect()
        {
            try
            {
                Console.WriteLine($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");

                if (Server.lobbies.ContainsKey(id))
                {
                    //kicks out everyone from the lobby
                    foreach (Client client in Server.lobbies[id].clients)
                    {
                        if (client != null && client.id != id)
                        {
                            ServerSend.RemoveLobby(client.id);
                        }
                    }
                    Console.WriteLine($"Removed {Server.clients[id].user.gameusername}'s Lobby");
                    Server.lobbies.Remove(id);
                }

                foreach (Lobby lobby in Server.lobbies.Values)
                {
                    if (lobby.RemoveClient(id))
                    {
                        foreach (Client client in lobby.clients)
                        {
                            if (client != null && client.id != id)
                            {
                                ServerSend.PlayerDisconnectedLobby(client.id, Server.clients[id].user.gameusername);
                            }
                        }
                    }
                }

                user = null;

                tcp.Disconnect();
            }
            catch (Exception _ex)
            {
                Console.WriteLine("Error disconnecting " + _ex);
            }
        }
    }
}
