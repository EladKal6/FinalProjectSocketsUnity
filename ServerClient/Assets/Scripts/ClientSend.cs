using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPacketsLobby.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }

    public static void PlayerMovement(bool[] _inputs)
    {
        using (Packet _packet = new Packet((int)ClientPacketsGame.playerMovement))
        {
            _packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }
            _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);

            SendUDPData(_packet);
        }
    }

    public static void HostRequest()
    {
        using (Packet _packet = new Packet((int)ClientPacketsLobby.hostRequest))
        {
            _packet.Write(Client.instance.myId);

            SendTCPData(_packet);
        }
    }

    public static void JoinRequest(int _lobbyClient)
    {
        using (Packet _packet = new Packet((int)ClientPacketsLobby.joinRequest))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(_lobbyClient);

            SendTCPData(_packet);
        }
    }

    public static void RequestLobbies()
    {
        using (Packet _packet = new Packet((int)ClientPacketsLobby.requestLobbies))
        {
            _packet.Write(Client.instance.myId);

            SendTCPData(_packet);
        }
    }
    #endregion
}