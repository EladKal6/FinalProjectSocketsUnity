using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    public string lobbyName;
    public int maxPlayers;
    public int currentPlayers;


    public Lobby(string lobbyName, int maxPlayers, int currentPlayers)
    {
        this.lobbyName = lobbyName;
        this.maxPlayers = maxPlayers;
        this.currentPlayers = currentPlayers;
    }
}
