using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();
        RSAEncryption.SetPubKeyString( _packet.ReadString());

        Debug.Log($"Message from ServerManager: {_msg}");
        Client.instance.myId = _myId;
        if (UIManager.instance.startMenu.activeSelf)
        {
            ClientSend.WelcomeReceived();
        }
        else
        {
            ClientSend.SignUp();
        }
    }

    public static void LoginOk(Packet _packet)
    {
        if (UIManager.instance.startMenu.activeSelf)
        {
            Debug.Log($"Message from ServerManager: login Successfull!");

            UIManager.instance.startMenu.SetActive(false);
            UIManager.instance.LobbyMenu.SetActive(true);
            UIManager.instance.backToLobbyMenu.SetActive(true);
            UIManager.instance.WrongUsernamePassword.SetActive(false);
        }
        else
        {
            Debug.Log($"Message from ServerManager: Sign Up Successfull!");

            UIManager.instance.startMenu.SetActive(true);
            UIManager.instance.SignUpVerificationMenu.SetActive(false);
        }
    }

    public static void LoginError(Packet _packet)
    {
        string _msg = _packet.ReadString();

        Debug.Log($"Message from ServerManager: {_msg}");
        if (UIManager.instance.startMenu.activeSelf)
        {
            UIManager.instance.ConnectButton.SetActive(true);
            UIManager.instance.WrongUsernamePassword.GetComponent<TextMeshProUGUI>().text = _msg;
            UIManager.instance.WrongUsernamePassword.SetActive(true);
            Client.instance.Disconnect();
        }
        else if(_msg == "User with that username already exists, please try a different one")
        {
            Debug.Log($"Message from ServerManager: Sign Up Not Successfull!");

            UIManager.instance.startMenu.SetActive(true);
            UIManager.instance.SignUpVerificationMenu.SetActive(false);
            UIManager.instance.WrongUsernamePassword.GetComponent<TextMeshProUGUI>().text = _msg;
            UIManager.instance.WrongUsernamePassword.SetActive(true);
            Client.instance.Disconnect();
        }

    }

    public static void GameWelcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from GameServer: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.GameWelcomeReceived();


        Debug.Log(Client.instance.tcp.socket);
        Debug.Log(Client.instance.isConnected);
        Debug.Log(Client.port);


        Debug.Log("The ip the udp instance is connecting is " + ((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(_id, username, _position, _rotation);
    }

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        bool _isRunning = _packet.ReadBool();
        bool _isFalling = _packet.ReadBool();
        bool _isJumping = _packet.ReadBool();

        GameManager.players[_id].transform.position = _position;
        if (_id != Client.instance.myId)
        {
            GameManager.players[_id].anim.SetBool("isRunning", _isRunning);
            GameManager.players[_id].anim.SetBool("isFalling", _isFalling);
            GameManager.players[_id].anim.SetBool("isJumping", _isJumping);
        }
    }

    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.players[_id].transform.rotation = _rotation;
    }

    public static void PlayerDied(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Debug.Log(_id + "Died!");
        GameManager.players[_id].Die();
    }

    public static void StartShootTimer(Packet _packet)
    {
        int _cooldown = _packet.ReadInt();

        Debug.Log("StartedShootTimer");
        ShootTimer.instance.StartShootTimer(_cooldown);
    }

    public static void ShotLine(Packet _packet)
    {
        int _id = _packet.ReadInt();

        GameManager.players[_id].gameObject.GetComponentInChildren<SimpleShoot>().Activate();

    }

    public static void PlayerHealth(Packet _packet)
    {
        int _id = _packet.ReadInt();
        float _health = _packet.ReadFloat();

        GameManager.players[_id].SetHealth(_health);
    }

    public static void PlayerRevived(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Debug.Log(_id + "Revived!");
        GameManager.players[_id].Revived();
    }

    public static void StartMinigame(Packet _packet)
    {
        int winnerId = _packet.ReadInt();

        string minigame = _packet.ReadString();

        Debug.Log("Chosen minigame: " + minigame);

        if (minigame == "Game")
        {
            UIManager.scoreboardstring = _packet.ReadString();
            UIManager.lastWinnerUsername = GameManager.players[winnerId].username;
        }


        GameManager.instance.StartMinigame(minigame);
    }

    public static void RoundWinnerUsername(Packet _packet)
    {
        GameObject wonRoundScreen = Instantiate(GameManager.instance.wonRoundPrefab);
        TextMeshProUGUI winnerText = wonRoundScreen.transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        winnerText.text = _packet.ReadString() + " won the round!";
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }


    public static void SpawnObstacle(Packet _packet)
    {
        int _obstacleId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Vector3 _scale = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        int _colorIndex = _packet.ReadInt();
        string obsType = _packet.ReadString();

        Debug.Log("Spawn Object - " + obsType);

        if (obsType == "Lava")
        {
            GameManager.instance.SpawnLavas(_obstacleId, _position, _scale, _rotation, _colorIndex);
        }
        if (obsType == "squareObstacle")
        {
            GameManager.instance.SpawnObstacle(_obstacleId, _position, _scale, _rotation, _colorIndex);
        }
        if (obsType == "Platform")
        {
            GameManager.instance.SpawnPlatform(_obstacleId, _position, _scale, _rotation, _colorIndex);
        }
        if (obsType == "Anvil")
        {
            GameManager.instance.SpawnAnvil(_obstacleId, _position, _scale, _rotation, _colorIndex);
        }

    }

    public static void ObstaclePosition(Packet _packet)
    {
        int _obstacleId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        string obsType = _packet.ReadString();

        if (obsType == "Lava")
        {
            GameManager.lavas[_obstacleId].ChangePosition(_position);
        }
        if (obsType == "squareObstacle")
        {
            GameManager.obstacles[_obstacleId].ChangePosition(_position);
        }
        if (obsType == "Platform")
        {
            GameManager.platforms[_obstacleId].ChangePosition(_position);
        }
        if (obsType == "Anvil")
        {
            GameManager.anvils[_obstacleId].ChangePosition(_position);
        }
    }

    public static void ObstacleDestroyed(Packet _packet)
    {
        int _obstacleId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        string obsType = _packet.ReadString();

        if (obsType == "Lava")
        {
            GameManager.lavas[_obstacleId].Destroy(_position, obsType);
        }
        if (obsType == "squareObstacle")
        {
            GameManager.obstacles[_obstacleId].Destroy(_position, obsType);
        }
        if (obsType == "Platform")
        {
            GameManager.platforms[_obstacleId].Destroy(_position, obsType);
        }
        if (obsType == "Anvil")
        {
            GameManager.anvils[_obstacleId].Destroy(_position, obsType);
        }
    }

    public static void PropDestroyed(Packet _packet)
    {
        string propName = _packet.ReadString();

        Debug.Log("Destroying " + propName);
        try
        {
            Destroy(GameObject.Find(GameManager.instance.GetActiveMinigame() + "(Clone)").transform.Find("ParkWorld").transform.Find(propName).gameObject);
        }
        catch (System.NullReferenceException)
        {
            Debug.LogWarning("Prop Not Found!");
        }
    }


    public static void UpdateLobbies(Packet _packet)
    {
        int lobbiesAmnt = _packet.ReadInt();
        for (int i = 0; i < lobbiesAmnt; i++)
        {
            GameManager.lobbies[_packet.ReadInt()] = new Lobby(_packet.ReadString(), _packet.ReadInt(), _packet.ReadInt());
        }
        Debug.Log($"Got {lobbiesAmnt} lobbies");

        foreach(Lobby lobby in GameManager.lobbies.Values)
        {
            UIManager.instance.InstantiateLobbyBox(lobby.lobbyName, lobby.maxPlayers, lobby.currentPlayers);
        }

    }

    public static void PlayerJoinedLobby(Packet _packet)
    {
        try
        {
            string username = _packet.ReadString();
            Debug.Log(username + " joined the lobby");
            UIManager.instance.InstantiatePlayerBox(username);
        }
        catch(Exception _ex)
        {
            Debug.Log(_ex);
        }
    }

    public static void PlayerDisconnectedLobby(Packet _packet)
    {
        try
        {
            string username = _packet.ReadString();
            Debug.Log(username + " Disconnected from the lobby");
            UIManager.instance.RemovePlayerBox(username);
        }
        catch (Exception _ex)
        {
            Debug.Log(_ex);
        }
    }

    public static void LobbyRemoved(Packet _packet)
    {
        UIManager.instance.BackToLobbyMenu();
    }

    public static void SendIntoGame(Packet _packet)
    {
        Client.instance.SendIntoGame(_packet.ReadInt()); //Game Port
    }

    public static void GameFinished(Packet _packet)
    {
        UIManager.lastWinnerUsername = _packet.ReadString();
        UIManager.instance.WinnerOverAll();

        Client.instance.SendIntoMainMenu(26950);
    }
}
