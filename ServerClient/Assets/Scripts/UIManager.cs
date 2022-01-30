using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public InputField usernameField;


    public InputField lobbyNameField;
    public InputField maxPlayersField;

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

    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        usernameField.interactable = false;
        Client.instance.ConnectToServer();
    }

    public void HostLobby()
    {
        ClientSend.HostRequest(lobbyNameField.text, int.Parse(maxPlayersField.text));
        Debug.Log("clicked!");
    }

    public void JoinLobby(int _lobbyClient)
    {
        ClientSend.JoinRequest(_lobbyClient);
    }

    public void ShowLobbies()
    {
        ClientSend.RequestLobbies();
    }
}