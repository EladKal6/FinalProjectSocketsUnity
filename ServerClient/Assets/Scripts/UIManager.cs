using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject entireMenu;

    public GameObject startMenu;
    public GameObject SignUpVerificationMenu;
    public InputField ServerAddressField;
    public InputField usernameField;
    public InputField passwordField;
    public InputField signUpEmailField;
    public InputField signUpUsernameField;
    public InputField signUpPasswordField;
    public InputField emailCodeField;

    public GameObject LobbyMenu;
    public GameObject backToLobbyMenu;
    public GameObject ConnectButton;
    public GameObject WrongUsernamePassword;

    public Text winText;
    public Text loseText;
    public TextMeshPro shootTimer;
    public TextMeshPro scoreboardTitle;
    public TextMeshPro scoreboard;
    public static string scoreboardstring;
    public static string lastWinnerUsername;
    public TextMeshPro winnerOverAll;

    public Text hostLobbyName;
    public GameObject PlayerBox;
    public GameObject HostPlayerList;
    public GameObject HostLobbyMenu;
    public Slider BestOfSlider;
    public Text BestOfText;
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
        PlayerManager.localPlayerUsername = usernameField.text;
        if (ServerAddressField.text == "")
        {
            Client.ip = "127.0.0.1";
        }
        else
        {
            Client.ip = ServerAddressField.text;
        }
        Client.instance.ConnectToServer();
    }

    public void SignUp()
    {
        if (ServerAddressField.text == "")
        {
            Client.ip = "127.0.0.1";
        }
        else
        {
            Client.ip = ServerAddressField.text;
        }
        Client.instance.ConnectToServer();
    }

    public void EmailCode()
    {
        ClientSend.EmailCode();
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
        newLobbyBox.transform.Find("PlayersAmnt").GetComponent<Text>().text = currentAmnt + "/" + maxAmnt;
        newLobbyBox.GetComponent<Button>().onClick.AddListener(() => { JoinLobby(lobbyName); });
        newLobbyBox.SetActive(true);
    }

    public void ChangeValueBestOfText()
    {
        BestOfText.text = "Best Of: " + BestOfSlider.value;
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

    public void WinnerOverAll()
    {
        winnerOverAll.text = lastWinnerUsername + " WON THE GAME!";
        scoreboard.gameObject.SetActive(false);
        scoreboardTitle.gameObject.SetActive(false);
        winnerOverAll.gameObject.SetActive(true);
        StartCoroutine(AddExclamationMarks());
    }

    public IEnumerator AddExclamationMarks()
    {
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.25f);
            winnerOverAll.text += "!";
        }
    }
}