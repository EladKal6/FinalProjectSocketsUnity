using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 26950;
    public int myId = 0;
    public TCP tcp;
    public UDP udp;

    public bool isConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new TCP();
        udp = new UDP();

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Client.instance.ConnectToServer();
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log(string.Join(Environment.NewLine, GameManager.obstacles));
        Disconnect();
    }

    public void ConnectToServer()
    {
        InitializeClientData();

        isConnected = true;
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];
            Debug.Log("Begin Connect to port " + instance.port);
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult _result)
        {
            socket.EndConnect(_result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
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
                Debug.Log($"Error sending data to server via TCP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    instance.Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception _ex)
            {
                Debug.Log("Inside Disconnect " + _ex);
                Disconnect();
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
                        packetHandlers[_packetId](_packet);
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

        private void Disconnect()
        {
            instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        public void Connect(int _localPort)
        {
            socket = new UdpClient(_localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet _packet = new Packet())
            {
                SendData(_packet);
            }
        }

        public void SendData(Packet _packet)
        {
            try
            {
                _packet.InsertInt(instance.myId);
                if (socket != null)
                {
                    socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to server via UDP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                byte[] _data = socket.EndReceive(_result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (_data.Length < 4)
                {
                    instance.Disconnect();
                    return;
                }

                HandleData(_data);
            }
            catch  (Exception _ex)
            {
                Debug.LogError(_ex);
                instance.Disconnect();
            }
        }

        private void HandleData(byte[] _data)
        {
            using (Packet _packet = new Packet(_data))
            {
                int _packetLength = _packet.ReadInt();
                _data = _packet.ReadBytes(_packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetId = _packet.ReadInt();
                    packetHandlers[_packetId](_packet);
                }
            });
        }

        private void Disconnect()
        {
            Debug.Log("making the socket null");
            instance.Disconnect();

            endPoint = null;
            socket = null;
        }
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerManagerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerManagerPackets.sendLobbies, ClientHandle.UpdateLobbies },
            { (int)ServerManagerPackets.sendJoinedPlayer, ClientHandle.PlayerJoinedLobby },
            { (int)ServerManagerPackets.SendIntoGame, ClientHandle.SendIntoGame },
            { (int)ServerManagerPackets.sendDisconnectedPlayer, ClientHandle.PlayerDisconnectedLobby },
            { (int)ServerManagerPackets.sendRemovedLobby, ClientHandle.LobbyRemoved },
            { (int)GameServerPackets.gamewelcome, ClientHandle.GameWelcome },
            { (int)GameServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            { (int)GameServerPackets.playerPosition, ClientHandle.PlayerPosition },
            { (int)GameServerPackets.playerRotation, ClientHandle.PlayerRotation },
            { (int)GameServerPackets.PlayerDied, ClientHandle.PlayerDied },
            { (int)GameServerPackets.playerDisconnected, ClientHandle.PlayerDisconnected },
            { (int)GameServerPackets.spawnObstacle, ClientHandle.SpawnObstacle },
            { (int)GameServerPackets.obstaclePosition, ClientHandle.ObstaclePosition },
            { (int)GameServerPackets.obstacleDestroyed, ClientHandle.ObstacleDestroyed }
        };
        Debug.Log("Initialized packets.");
    }

    private void Disconnect()
    {
        if (isConnected)
        {
            Debug.Log("Disconecting...");
            try
            {
                bool isHost = false;
                foreach (Transform Menu in UIManager.instance.entireMenu.transform)
                {
                    if (Menu.name == "HostLobbyMenu" && Menu.gameObject.activeSelf == true)
                    {
                        isHost = true;
                    }
                }
                if (isHost)
                {
                    Debug.Log("Closing Lobby");
                    ClientSend.ClosingLobby();
                }
                else
                {
                    Debug.Log("Exiting Lobby");
                    ClientSend.PlayerExitingLobby();
                }
            }
            catch
            {
                Debug.Log("There is no menu");
            }
            

            isConnected = false;
            tcp.socket.Close();
            try
            {
                udp.socket.Close();
            }
            catch
            {
                Debug.Log("udp didn't exist!");
            }

            Debug.Log("Disconnected from server");
        }
    }

    public void SendIntoGame(int Gameport)
    {
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log(Client.instance);
            Debug.Log(Client.instance.tcp);
            Debug.Log(Client.instance.tcp.socket);
            Debug.Log(Client.instance.isConnected);


            Debug.Log(instance.port);
    }
}
