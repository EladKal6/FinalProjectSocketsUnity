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
            try
            {
                _packet.Write(PlayerManager.localPlayerUsername);
                _packet.Write(RSAEncryption.Encrypt(Hash128.Compute(UIManager.instance.passwordField.text).ToString()));
            }
            catch
            {
                _packet.Write("PLAYER");
                _packet.Write(RSAEncryption.Encrypt(Hash128.Compute(UIManager.instance.passwordField.text).ToString()));
            }
            SendTCPData(_packet);
        }
    }

    public static void SignUp()
    {
        using (Packet _packet = new Packet((int)ClientPacketsLobby.signUp))
        {
            _packet.Write(UIManager.instance.signUpEmailField.text);
            _packet.Write(UIManager.instance.signUpUsernameField.text);
            _packet.Write(RSAEncryption.Encrypt(Hash128.Compute(UIManager.instance.signUpPasswordField.text).ToString()));

            SendTCPData(_packet);
        }
    }

    public static void EmailCode()
    {
        using (Packet _packet = new Packet((int)ClientPacketsLobby.emailCode))
        {
            _packet.Write(int.Parse(UIManager.instance.emailCodeField.text));


            SendTCPData(_packet);
        }
    }

    public static void GameWelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPacketsLobby.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            try
            {
                _packet.Write(PlayerManager.localPlayerUsername);
            }
            catch
            {
                _packet.Write("PLAYER");
            }
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

    public static void PlayerShoot(Vector3 _facing)
    {
        using (Packet _packet = new Packet((int)ClientPacketsGame.playerShoot))
        {
            _packet.Write(_facing);

            SendTCPData(_packet);
        }
    }

    public static void HostRequest(string lobbyName, int MaxPlayers)
    {
        using (Packet _packet = new Packet((int)ClientPacketsLobby.hostRequest))
        {
            _packet.Write(lobbyName);
            _packet.Write(MaxPlayers);

            SendTCPData(_packet);
            Debug.Log("Sent!");
        }
    }

    public static void JoinRequest(string _lobbyName)
    {
        using (Packet _packet = new Packet((int)ClientPacketsLobby.joinRequest))
        {
            _packet.Write(_lobbyName);

            SendTCPData(_packet);
        }
    }

    public static void RequestLobbies()
    {
        using (Packet _packet = new Packet((int)ClientPacketsLobby.requestLobbies))
        {
            SendTCPData(_packet);
        }
        Debug.Log("requested Lobbies");
    }

    public static void ClosingLobby()
    {
        using (Packet _packet = new Packet((int)ClientPacketsLobby.closeLobby))
        {
            SendTCPData(_packet);
        }
        Debug.Log("requested Lobbies");
    }

    public static void PlayerExitingLobby()
    {
        using (Packet _packet = new Packet((int)ClientPacketsLobby.playerExitLobby))
        {
            SendTCPData(_packet);
        }
        Debug.Log("requested Lobbies");
    }

    public static void SendIntoGame()
    {
        using (Packet _packet = new Packet((int)ClientPacketsLobby.SendIntoGame))
        {
            _packet.Write(UIManager.instance.BestOfSlider.value);

            SendTCPData(_packet);
        }
        Debug.Log("sent into game");
    }
    #endregion
}