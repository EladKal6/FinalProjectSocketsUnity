using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public static Dictionary<int, Obstacle> obstacles = new Dictionary<int, Obstacle>();
    private static int nextObstacleId = 1;

    public int id;
    public Vector3 initialForce;

    private void Start()
    {
        id = nextObstacleId;
        nextObstacleId++;
        obstacles.Add(id, this);

        ServerSend.SpawnObstacle(this);

        StartCoroutine(DestroyAfterTime());
    }

    public void Initialize(float _initialForceStrength)
    {
        initialForce = Vector3.right * _initialForceStrength;
    }

    private void FixedUpdate()
    {
        transform.position += initialForce;
        ServerSend.ObstaclePosition(this);
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(10f);

        ServerSend.ObstacleDestroyed(this);

        obstacles.Remove(id);
        Destroy(gameObject);
    }
}