using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
    /// <summary>Sends a packet to a client via TCP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to a client via UDP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via TCP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
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

    /// <summary>Sends a packet to all clients via UDP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via UDP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }

    #region Packets
    /// <summary>Sends a welcome message to the given client.</summary>
    /// <param name="_toClient">The client to send the packet to.</param>
    /// <param name="_msg">The message to send.</param>
    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            Debug.Log($"sending Welcome to {Server.clients[_toClient].tcp.socket.Client.RemoteEndPoint}");
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>Tells a client to spawn a player.</summary>
    /// <param name="_toClient">The client that should spawn the player.</param>
    /// <param name="_player">The player to spawn.</param>
    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>Sends a player's updated position to all clients.</summary>
    /// <param name="_player">The player whose position to update.</param>
    public static void PlayerPosition(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position); //new position
            _packet.Write(_player.IsRunning()); //is Running?
            _packet.Write(_player.IsFalling()); //is Falling?
            _packet.Write(_player.IsJumping()); //is Jumping?

            SendUDPDataToAll(_packet);
        }
    }

    /// <summary>Sends a player's updated rotation to all clients except to himself (to avoid overwriting the local player's rotation).</summary>
    /// <param name="_player">The player whose rotation to update.</param>
    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);

            SendUDPDataToAll(_player.id, _packet);
        }
    }

    /// <summary>Tells all clients that a player has died.</summary>
    /// <param name="_playerId">The player that died.</param>
    public static void PlayerDied(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDied))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }

    public static void StartShootTimer(int _countdown)
    {
        using (Packet _packet = new Packet((int)ServerPackets.startShootTimer))
        {
            Debug.Log("Currently sending shoot timer");
            _packet.Write(_countdown);

            SendTCPDataToAll(_packet);
        }
    }

    public static void ShotLine(int _id)
    {
        using (Packet _packet = new Packet((int)ServerPackets.shotLine))
        {
            _packet.Write(_id);

            SendUDPDataToAll(_packet);
        }
    }

    public static void PlayerHealth(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerHealth))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.health);

            SendTCPDataToAll(_packet);
        }
    }

    /// <summary>Tells all clients that a player has died.</summary>
    /// <param name="_playerId">The player that died.</param>
    public static void PlayerRevived(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRevived))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }

    /// <summary>Tells all clients that a player has won.</summary>
    /// <param name="_playerId">The player that won.</param>
    public static void StartMinigame(int _winningClientId, string _minigameName)
    {
        using (Packet _packet = new Packet((int)ServerPackets.startMinigame))
        {
            _packet.Write(_winningClientId);
            _packet.Write(_minigameName);

            if (_minigameName == "Game")
            {
                _packet.Write(Player.ScoreboardToString());
            }

            SendTCPDataToAll(_packet);
        }
    }

    /// <summary>Tells all clients that a player has won.</summary>
    /// <param name="_playerId">The player that won.</param>
    public static void roundWinnerUsername(string _winningUsername)
    {
        using (Packet _packet = new Packet((int)ServerPackets.roundWinnerUsername))
        {
            _packet.Write(_winningUsername);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerDisconnected(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnObstacle(Obstacle _obstacle)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnObstacle))
        {
            _packet.Write(_obstacle.id);
            _packet.Write(_obstacle.transform.position);
            _packet.Write(_obstacle.transform.localScale);
            _packet.Write(_obstacle.transform.rotation);
            _packet.Write(_obstacle.colorIndex);
            _packet.Write(_obstacle.type);
            Debug.Log(_obstacle.type);

            SendTCPDataToAll(_packet);
        }
    }
    public static void SpawnObstacleToPlayer(int _clientId, Obstacle _obstacle)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnObstacle))
        {
            _packet.Write(_obstacle.id);
            _packet.Write(_obstacle.transform.position);
            _packet.Write(_obstacle.transform.localScale);
            _packet.Write(_obstacle.transform.rotation);
            _packet.Write(_obstacle.colorIndex);
            _packet.Write(_obstacle.type);



            Debug.Log("Spawn Obstacle");
            SendTCPData(_clientId, _packet);
        }
    }

    public static void ObstaclePosition(Obstacle _obstacle)
    {
        using (Packet _packet = new Packet((int)ServerPackets.obstaclePosition))
        {
            _packet.Write(_obstacle.id);
            _packet.Write(_obstacle.transform.position);
            _packet.Write(_obstacle.type);


            SendTCPDataToAll(_packet);
        }
    }

    public static void ObstacleDestroyed(Obstacle _obstacle)
    {
        using (Packet _packet = new Packet((int)ServerPackets.obstacleDestroyed))
        {
            _packet.Write(_obstacle.id);
            _packet.Write(_obstacle.transform.position);
            _packet.Write(_obstacle.type);


            SendTCPDataToAll(_packet);
        }
    }

    public static void PropDestroyed(string propName)
    {
        using (Packet _packet = new Packet((int)ServerPackets.propDestroyed))
        {
            _packet.Write(propName);


            SendTCPDataToAll(_packet);
        }
    }

    public static void GameFinished(string _winnerUsername)
    {
        using (Packet _packet = new Packet((int)ServerPackets.gameFinished))
        {
            _packet.Write(_winnerUsername);

            SendTCPDataToAll(_packet);
        }
    }

    #endregion
}