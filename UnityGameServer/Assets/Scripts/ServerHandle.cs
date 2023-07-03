using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }

        Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has Recieved The Welcome!");

        Server.clients[_fromClient].SendIntoGame(_username);
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        try
        {
            bool[] _inputs = new bool[_packet.ReadInt()];
            for (int i = 0; i < _inputs.Length; i++)
            {
                _inputs[i] = _packet.ReadBool();
            }
            Quaternion _rotation = _packet.ReadQuaternion();

            Server.clients[_fromClient].player.SetInput(_inputs, _rotation);
        }
        catch (System.Exception _ex)
        {
            Debug.Log("ERROR in playermovement handle: " + _ex);
        }
    }

    public static void PlayerShoot(int _fromClient, Packet _packet)
    {
        if (CanShootTimer.canShoot)
        {
            Vector3 _shootDirection = _packet.ReadVector3();

            Debug.Log(_fromClient);

            Server.clients[_fromClient].player.Shoot(_shootDirection);
            ServerSend.ShotLine(_fromClient);
        }
    }
}