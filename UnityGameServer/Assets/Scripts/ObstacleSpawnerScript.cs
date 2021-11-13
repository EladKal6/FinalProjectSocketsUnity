using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnerScript : MonoBehaviour
{
    //IF THIS STARTS BUGGING UNCOMMENT THE NEXT LINES
    //private MeshFilter a;
    //private MeshRenderer b;
    //private BoxCollider c;
    //THE BUGS MAY BE CAUSED BECAUSE THE PROJECT DOESN'T REFRENCE THESE CLASSES

    public GameObject obs;
    public float initialeForceStrength;
    public bool stopSpawning = false;
    public float spawnTime;
    public float spawnDelay;

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("SpawnObject", spawnTime, spawnDelay);
    }

    public void SpawnObject()
    {
        Vector3 scaleMultiplier = new Vector3(Random.Range(0.1f, 1f), Random.Range(1, 5), Random.Range(1, 20));
        Obstacle currentObs = NetworkManager.instance.InstantiateObstacle(transform);
        Debug.Log(currentObs);
        currentObs.Initialize(scaleMultiplier, initialeForceStrength);
        if (stopSpawning)
        {
            CancelInvoke("SpawnObject");
        }
    }
}
