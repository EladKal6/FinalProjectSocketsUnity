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
    public float initialeForceStrengthOffset;
    public bool stopSpawning = false;
    public float spawnTime;
    public float spawnDelay;

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("SpawnObject", spawnTime, spawnDelay);
    }

    public Vector3 RandomPositionOffsetMultiplier(Vector3 _scaleMultiplier)
    {
        float yWiggleRoom = (1 - _scaleMultiplier.y) / 2;
        float zWiggleRoom = (1 - _scaleMultiplier.z) / 2;
        float randomYWiggleRoom, randomZWiggleRoom;
        float randomYIndex = Random.Range(0f,1f);
        float randomZIndex = Random.Range(0f,1f);
        if(randomYIndex < 0.4f){randomYWiggleRoom = Random.Range(-yWiggleRoom, -yWiggleRoom / 2);}
        else if (randomYIndex > 0.6f){randomYWiggleRoom = Random.Range(yWiggleRoom / 2, yWiggleRoom);}
        else{randomYWiggleRoom = Random.Range(-yWiggleRoom / 2, yWiggleRoom / 2);}
        if(randomZIndex < 0.4f){randomZWiggleRoom = Random.Range(-zWiggleRoom, -zWiggleRoom / 2);}
        else if (randomZIndex > 0.6f){randomZWiggleRoom = Random.Range(yWiggleRoom / 2, yWiggleRoom);}
        else{randomZWiggleRoom = Random.Range(-zWiggleRoom / 2, zWiggleRoom / 2);}
        return new Vector3(0f, randomYWiggleRoom, randomZWiggleRoom);
    }

    public void SpawnObject()
    {
        Quaternion _rotation = Quaternion.Euler(new Vector3(Random.Range(0f, 5f), Random.Range(0f, 25f)));
        Vector3 scaleMultiplier = new Vector3(Random.Range(0.5f, 1f), Random.Range(0.3f, 0.8f), Random.Range(0.1f, 0.9f));

        Vector3 positionOffsetMultiplier = RandomPositionOffsetMultiplier(scaleMultiplier);
        NetworkManager.instance.InstantiateObstacle(transform, scaleMultiplier, positionOffsetMultiplier, _rotation)
        .Initialize(initialeForceStrength + Random.Range(-initialeForceStrengthOffset, initialeForceStrengthOffset));
        if (stopSpawning)
        {
            CancelInvoke("SpawnObject");
        }
    }
}
