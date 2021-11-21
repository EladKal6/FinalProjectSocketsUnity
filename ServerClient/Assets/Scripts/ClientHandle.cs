using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

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

        GameManager.players[_id].transform.position = _position;
    }

    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.players[_id].transform.rotation = _rotation;;
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

        GameManager.instance.SpawnObstacle(_obstacleId, _position, _scale, _rotation, _colorIndex);
    }

    public static void ObstaclePosition(Packet _packet)
    {
        int _obstacleId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        try
        {
            GameManager.obstacles[_obstacleId].ChangePosition(_position);
        }
        catch (KeyNotFoundException)
        {
            Debug.Log(_packet.UnreadLength());
            Vector3 _scale = _packet.ReadVector3();
            Quaternion _rotation = _packet.ReadQuaternion();
            int _colorIndex = _packet.ReadInt();

            GameManager.instance.SpawnObstacle(_obstacleId, _position, _scale, _rotation, _colorIndex);
        }

    }

    public static void ObstacleDestroyed(Packet _packet)
    {
        int _obstacleId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        GameManager.obstacles[_obstacleId].Destroy(_position);
    }
}
