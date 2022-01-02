using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public int maxPlayers;
    public int current;

    public void Initialize(int _maxPlayers, int _current)
    {
        maxPlayers = _maxPlayers;
        current = _current;
    }
}
