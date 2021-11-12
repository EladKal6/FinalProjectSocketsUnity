using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnerScript : MonoBehaviour
{
    //IF THIS STARTS BUGGING UNCOMMENT THE NEXT LINES
    //private MeshFilter a;
    //private MeshRenderer b;
    //private BoxCollider c;
    //THE BUGS MAY BE CAUSED BECAUSE THE PROJECT DOESN'T REFRENCE THESE CLASSES

    public GameObject obs;
    private GameObject CurrentObs;
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
        CurrentObs = Instantiate(obs, transform.position, transform.rotation);
        CurrentObs.transform.localScale = new Vector3(Random.Range(0.1f, 1f), Random.Range(1, 5), Random.Range(1, 20));
        if (stopSpawning)
        {
            CancelInvoke("SpawnObject");
        }
    }
}
