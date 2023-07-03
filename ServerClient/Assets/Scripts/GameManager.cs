using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, ObstacleManager> obstacles = new Dictionary<int, ObstacleManager>();
    public static Dictionary<int, ObstacleManager> platforms = new Dictionary<int, ObstacleManager>();
    public static Dictionary<int, ObstacleManager> lavas = new Dictionary<int, ObstacleManager>();
    public static Dictionary<int, ObstacleManager> anvils = new Dictionary<int, ObstacleManager>();
    public static Dictionary<int, Lobby> lobbies = new Dictionary<int, Lobby>();


    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject obstaclePrefab;
    public GameObject platPrefab;
    public GameObject lavaPrefab;
    public GameObject AnvilPrefab;

    public GameObject gamePrefab;
    public GameObject dodgeObsPrefab;
    public GameObject parkourPrefab;
    public GameObject lavaFloorPrefab;
    public GameObject ParkPrefab;
    public GameObject ShootingCityPrefab;


    public GameObject wonRoundPrefab;

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

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if (_id == Client.instance.myId)
        {
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, _rotation);
        }

        _player.GetComponent<PlayerManager>().Initialize(_id, _username);
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }

    public void SpawnObstacle(int _id, Vector3 _position, Vector3 _scale, Quaternion _rotation, int _colorIndex)
    {
        Debug.Log("Spawning obs");
        GameObject _obstacle = Instantiate(obstaclePrefab, _position, _rotation);
        _obstacle.transform.localScale = _scale;
        _obstacle.GetComponent<ObstacleManager>().Initialize(_id, _colorIndex);
        obstacles.Add(_id, _obstacle.GetComponent<ObstacleManager>());
    }

    public void SpawnPlatform(int _id, Vector3 _position, Vector3 _scale, Quaternion _rotation, int _colorIndex)
    {
        Debug.Log("Spawning platform");

        GameObject _plaform = Instantiate(platPrefab, _position, _rotation);
        _plaform.transform.localScale = _scale;
        _plaform.GetComponent<ObstacleManager>().Initialize(_id, _colorIndex);
        platforms.Add(_id, _plaform.GetComponent<ObstacleManager>());
    }

    public void SpawnLavas(int _id, Vector3 _position, Vector3 _scale, Quaternion _rotation, int _colorIndex)
    {
        Debug.Log("Spawning lava");

        GameObject _lava = Instantiate(lavaPrefab, _position, _rotation);
        _lava.transform.localScale = _scale;
        _lava.GetComponent<ObstacleManager>().Initialize(_id, _colorIndex);
        lavas.Add(_id, _lava.GetComponent<ObstacleManager>());
    }

    public void SpawnAnvil(int _id, Vector3 _position, Vector3 _scale, Quaternion _rotation, int _colorIndex)
    {
        Debug.Log("Spawning obs");
        GameObject _obstacle = Instantiate(AnvilPrefab, _position, _rotation);
        _obstacle.transform.localScale = _scale;
        _obstacle.GetComponent<ObstacleManager>().Initialize(_id, _colorIndex);
        anvils.Add(_id, _obstacle.GetComponent<ObstacleManager>());
    }

    public void InstantiateShotLine(Vector3 originPoint, Vector3 endPoint)
    {
        Debug.Log(originPoint + "  " + endPoint);

        //For creating line renderer object
        LineRenderer lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        //For drawing line in the world space, provide the x,y,z values
        lineRenderer.SetPosition(0, originPoint); //x,y and z position of the starting point of the line
        lineRenderer.SetPosition(1, endPoint); //x,y and z position of the end point of the line

        
    }

    public void StartMinigame(string minigame)
    {

        if (GameObject.Find("WinLoseCanvas" + "(Clone)") != null)
        {
            Destroy(GameObject.Find("WinLoseCanvas" + "(Clone)"));
        }

        if (GameObject.Find(GetActiveMinigame() + "(Clone)") != null)
        {
            Destroy(GameObject.Find(GetActiveMinigame() + "(Clone)"));
        }
        else if (GameObject.Find(GetActiveMinigame()) != null)
        {
            Destroy(GameObject.Find(GetActiveMinigame()));
        }
        else
        {
            Debug.LogWarning("Should not happen -  Could not find " + GetActiveMinigame() + "(Clone)");
        }

        if (minigame == "Game")
        {
            foreach (PlayerManager player in GameManager.players.Values)
            {
                player.HideGun();
            }
            Instantiate(gamePrefab);
            UIManager.instance.scoreboard.text = UIManager.scoreboardstring;
        }
        else if (minigame == "DodgeObs")
        {
            Instantiate(dodgeObsPrefab);
        }
        else if (minigame == "Parkour")
        {
            Instantiate(parkourPrefab);
        }
        else if (minigame == "LavaFloor")
        {
            Instantiate(lavaFloorPrefab);
        }
        else if (minigame == "Park")
        {
            Instantiate(ParkPrefab);
        }
        else if (minigame == "ShootingCity")
        {
            Instantiate(ShootingCityPrefab);
        }
        else
        {
            Debug.LogWarning("Couldn't find minigame should not happen");
        }
    }

    public string GetActiveMinigame()
    {
        if (GameObject.Find("Game(Clone)") != null || GameObject.Find("Game") != null)
        {
            return "Game";
        }
        if (GameObject.Find("Parkour(Clone)") != null)
        {
            return "Parkour";
        }
        if (GameObject.Find("LavaFloor(Clone)") != null)
        {
            return "LavaFloor";
        }
        if (GameObject.Find("DodgeObs(Clone)") != null)
        {
            return "DodgeObs";
        }
        if (GameObject.Find("Park(Clone)") != null)
        {
            return "Park";
        }
        if (GameObject.Find("ShootingCity(Clone)") != null)
        {
            return "ShootingCity";
        }
        return "---";
    }

}
