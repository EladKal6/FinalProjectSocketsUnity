using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;
    public GameObject obstaclePrefab;
    public GameObject platPrefab;
    public GameObject lavaPrefab;
    public GameObject anvilPrefab;

    public GameObject gamePrefab;
    public GameObject dodgeObsPrefab;
    public GameObject parkourPrefab;
    public GameObject lavaFloorPrefab;
    public GameObject parkPrefab;
    public GameObject ShootingCityPrefab;

    public GameObject ConsoleToGUI;

    public int port;
    public int maxPlayers;
    public int bestOf;
    public bool debug;

    public string[] minigames = { "DodgeObs", "Parkour", "LavaFloor", "Park", "ShootingCity" };
    private string lastMinigame;

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

    private void Start()
    {
        try
        {
            port = int.Parse(GetArg("-port"));
            maxPlayers = int.Parse(GetArg("-maxPlayers"));
            bestOf = int.Parse(GetArg("-bestOf"));
            debug = bool.Parse(GetArg("-debug"));
        }
        catch
        {
            port = 5555;
            maxPlayers = 2;
            bestOf = 3;
            debug = true;
        }


        if (debug)
        {
            ConsoleToGUI.SetActive(true);
        }


        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(maxPlayers, port);
    }

    // Eg:  C:\Program Files\Unity\Unity.exe -outputDir "c:\temp\output" ...
    // read the "-outputDir" command line argument
    private static string GetArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, new Vector3(0f, 1f, 0f), Quaternion.identity).GetComponent<Player>();
    }

    public Obstacle InstantiateObstacle(Transform _spawnOrigin, Vector3 _scaleMultiplier, Vector3 _positionOffsetMultiplier, Quaternion _rotation)
    {
        GameObject obs = Instantiate(obstaclePrefab, _spawnOrigin.position, _rotation);
        obs.transform.position += Vector3.Scale(obs.transform.localScale, _positionOffsetMultiplier);

        obs.transform.localScale =  Vector3.Scale(obs.transform.localScale, _scaleMultiplier);
        return obs.GetComponent<Obstacle>();
    }

    public Obstacle InstantiateLava(Transform _spawnOrigin, Vector3 _scaleMultiplier, Vector3 _positionOffsetMultiplier, Quaternion _rotation)
    {
        GameObject obs = Instantiate(lavaPrefab, _spawnOrigin.position, _rotation);
        Debug.Log("Created Lava");
        obs.transform.position += Vector3.Scale(obs.transform.localScale, _positionOffsetMultiplier);

        obs.transform.localScale = Vector3.Scale(obs.transform.localScale, _scaleMultiplier);
        return obs.GetComponent<Obstacle>();
    }

    public Obstacle InstantiatePlat(Transform _spawnOrigin, Vector3 _scaleMultiplier, Vector3 _positionOffsetMultiplier, Quaternion _rotation)
    {
        GameObject obs = Instantiate(platPrefab, _spawnOrigin.position, _rotation);
        obs.transform.position += Vector3.Scale(obs.transform.localScale, _positionOffsetMultiplier);

        obs.transform.localScale = Vector3.Scale(obs.transform.localScale, _scaleMultiplier);
        return obs.GetComponent<Obstacle>();
    }

    public Obstacle InstantiateAnvil(Transform _spawnOrigin, Vector3 _scaleMultiplier, Vector3 _positionOffsetMultiplier, Quaternion _rotation)
    {
        GameObject obs = Instantiate(anvilPrefab, _spawnOrigin.position, _rotation);
        obs.transform.position +=  _positionOffsetMultiplier;

        return obs.GetComponent<Obstacle>();
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

    public void StartMinigame(int winningClientId, string minigameName = "random")
    {
        if (minigameName == "random")
        {
            minigameName = chooseRandomMinigame();
        }

        DestroyAllObs();

        ServerSend.StartMinigame(winningClientId, minigameName);


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


        if (minigameName == "Game")
        {
            if (Player.scoreboard[winningClientId] == bestOf)
            {
                ServerSend.GameFinished(Server.clients[winningClientId].player.username);

                StartCoroutine(IStopServer());
            }
            else
            {
                TimerTillStart();
            }
            ChangeGravityTo(-9.81f * Time.fixedDeltaTime * Time.fixedDeltaTime);
            Instantiate(gamePrefab);
        }
        else if (minigameName == "DodgeObs")
        {
            ChangeGravityTo(-9.81f * Time.fixedDeltaTime * Time.fixedDeltaTime);
            Instantiate(dodgeObsPrefab);
        }
        else if (minigameName == "Parkour")
        {
            ChangeGravityTo(-5f * Time.fixedDeltaTime * Time.fixedDeltaTime);
            Instantiate(parkourPrefab);
        }
        else if (minigameName == "LavaFloor")
        {
            ChangeGravityTo(-5f * Time.fixedDeltaTime * Time.fixedDeltaTime);
            Instantiate(lavaFloorPrefab);
        }
        else if (minigameName == "Park")
        {
            ChangeGravityTo(-8f * Time.fixedDeltaTime * Time.fixedDeltaTime);
            Instantiate(parkPrefab);
        }
        else if (minigameName == "ShootingCity")
        {
            ChangeGravityTo(-8f * Time.fixedDeltaTime * Time.fixedDeltaTime);
            Instantiate(ShootingCityPrefab);
        }


        TPEveryoneToStart();
    }


    public void TPEveryoneToStart()
    {
        foreach (Client client in Server.clients.Values)
        {
            if (client != null && client.player != null)
            {
                client.player.TPToStart();
            }
        }
    }

    public void ChangeGravityTo(float gravity)
    {
        foreach (Client client in Server.clients.Values)
        {
            if (client != null && client.player != null)
            {
                client.player.gravity = gravity;
            }
        }
    }

    public void TimerTillStart()
    {
        StartCoroutine(IStartMinigame());
    }

    public IEnumerator IStartMinigame()
    {
        yield return new WaitForSeconds(10);

        Debug.Log("StartMinigame!");

        StartMinigame(-999, "random");
    }

    public string chooseRandomMinigame()
    {
        string chosenMinigame = "";
        //while (chosenMinigame != lastMinigame)
        {
            chosenMinigame = minigames[UnityEngine.Random.Range(0, 5)];
        }
        lastMinigame = chosenMinigame;
        return chosenMinigame;
    }

    public void DestroyAllObs()
    {
        Debug.Log("Destroying obs!");
        foreach(Obstacle obs in Obstacle.obstacles.Values)
        {
            Debug.Log("Destroying obstacle!");
            ServerSend.ObstacleDestroyed(obs);
            Destroy(obs.gameObject);
        }
        foreach (Obstacle plat in Obstacle.platforms.Values)
        {
            try
            {
                Debug.Log("Destroying platform!");
                ServerSend.ObstacleDestroyed(plat);
                Destroy(plat.gameObject);
            }
            catch (Exception _ex)
            {
                Debug.Log(_ex + "Couldn't destroy platform!");
            }
        }
        foreach (Obstacle lava in Obstacle.lavas.Values)
        {
            Debug.Log("Destroying lava!");
            ServerSend.ObstacleDestroyed(lava);
            Destroy(lava.gameObject);
        }
        foreach (Obstacle anvil in Obstacle.anvils.Values)
        {
            Debug.Log("Destroying Anvil!");
            ServerSend.ObstacleDestroyed(anvil);
            Destroy(anvil.gameObject);
        }
        Obstacle.obstacles.Clear();
        Obstacle.platforms.Clear();
        Obstacle.lavas.Clear();
        Obstacle.anvils.Clear();
    }

    public IEnumerator IStopServer()
    {
        yield return new WaitForSeconds(8);

        Server.Stop();

        Application.Quit();
    }
}