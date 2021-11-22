using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;
    public GameObject obstaclePrefab;


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
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(50, 5555);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, new Vector3(0f, 0.5f, 0f), Quaternion.identity).GetComponent<Player>();
    }

    public Obstacle InstantiateObstacle(Transform _spawnOrigin, Vector3 _scaleMultiplier, Vector3 _positionOffsetMultiplier, Quaternion _rotation)
    {
        GameObject obs = Instantiate(obstaclePrefab, _spawnOrigin.position, _rotation);
        Debug.Log(obs.transform.localScale + "   " + _positionOffsetMultiplier + "   " + Vector3.Scale(obs.transform.localScale, _positionOffsetMultiplier));
        obs.transform.position += Vector3.Scale(obs.transform.localScale, _positionOffsetMultiplier);

        obs.transform.localScale =  Vector3.Scale(obs.transform.localScale, _scaleMultiplier);
        return obs.GetComponent<Obstacle>();
    }
}