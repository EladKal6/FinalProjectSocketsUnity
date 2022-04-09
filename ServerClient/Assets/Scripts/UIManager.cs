using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject entireMenu;

    public GameObject startMenu;
    public InputField usernameField;

    public GameObject LobbyMenu;


    public Text hostLobbyName;
    public GameObject PlayerBox;
    public GameObject HostPlayerList;
    public GameObject HostLobbyMenu;
    public GameObject PlayerPlayerList;

    public GameObject lobbyBox;
    public GameObject lobbyList;

    public Text playerLobbyName;

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
        hostLobbyName.text = lobbyNameField.text;
        Debug.Log("clicked!");
        InstantiatePlayerBox(usernameField.text);
        Debug.Log("Created!");
    }

    public void InstantiatePlayerBox(string _playerName)
    {
        GameObject newHostPlayerBox = Instantiate(PlayerBox, HostPlayerList.transform);
        GameObject newPlayerPlayerBox = Instantiate(PlayerBox, PlayerPlayerList.transform);
        Debug.Log("instantiated player box!");
        newHostPlayerBox.GetComponentInChildren<Text>().text = _playerName;
        newPlayerPlayerBox.GetComponentInChildren<Text>().text = _playerName;
        newHostPlayerBox.SetActive(true);
        newPlayerPlayerBox.SetActive(true);
    }

    public void RemovePlayerBox(string _playerName)
    {
        foreach (Transform PlayerBoxxx in HostPlayerList.transform)
        {
            if (PlayerBoxxx.gameObject.GetComponent<Text>().text == _playerName)
            {
                Destroy(PlayerBoxxx.gameObject);
            }
        }
        foreach (Transform PlayerBoxxx in PlayerPlayerList.transform)
        {
            if (PlayerBoxxx.gameObject.GetComponent<Text>().text == _playerName)
            {
                Destroy(PlayerBoxxx.gameObject);
            }
        }
    }

    public void InstantiateLobbyBox(string lobbyName, int maxAmnt, int currentAmnt)
    {
        GameObject newLobbyBox = Instantiate(lobbyBox, lobbyList.transform);
        newLobbyBox.transform.Find("LobbyName").GetComponent<Text>().text = lobbyName;
        newLobbyBox.transform.Find("PlayersAmnt").GetComponent<Text>().text = maxAmnt + "/" + currentAmnt;
        newLobbyBox.GetComponent<Button>().onClick.AddListener(() => { JoinLobby(lobbyName); });
        newLobbyBox.SetActive(true);
    }

    public void JoinLobby(string _lobbyName)
    {
        Debug.Log(_lobbyName);
        playerLobbyName.text = _lobbyName;
        ClientSend.JoinRequest(_lobbyName);
    }

    public void ShowLobbies()
    {
        ClientSend.RequestLobbies();
    }

    public void SendIntoGame()
    {
        ClientSend.SendIntoGame();
    }

    public void RemoveAllPlayerBoxes()
    {
        foreach (Transform PlayerBoxxx in HostPlayerList.transform)
        {
            if (PlayerBoxxx.GetComponentInChildren<Text>().text != "<Player Name>")
            {
                Destroy(PlayerBoxxx.gameObject);
            }
        }
        foreach (Transform PlayerBoxxx in PlayerPlayerList.transform)
        {
            if (PlayerBoxxx.GetComponentInChildren<Text>().text != "<Player Name>")
            {
                Destroy(PlayerBoxxx.gameObject);
            }
        }
    }

    public void BackToLobbyMenu()
    {
        bool isInLobby = false;
        bool isHost = false;
        foreach (Transform Menu in entireMenu.transform)
        {
            if (Menu.name == "HostLobbyMenu" && Menu.gameObject.activeSelf == true)
            {
                isHost = true;
            }

            if (Menu.name == "PlayerLobbyMenu" && Menu.gameObject.activeSelf == true)
            {
                isInLobby = true;
            }

            if (Menu.name != "BackToLobbyMenu")
            {
                Menu.gameObject.SetActive(false);
            }

        }
        if (isHost)
        {
            Debug.Log("Closing Lobby");
            ClientSend.ClosingLobby();
        }
        else if (isInLobby)
        {
            Debug.Log("Exiting Lobby");
            ClientSend.PlayerExitingLobby();
        }
        RemoveAllPlayerBoxes();
        LobbyMenu.SetActive(true);
    }
}