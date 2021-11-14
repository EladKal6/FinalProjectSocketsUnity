using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public static Dictionary<int, Obstacle> obstacles = new Dictionary<int, Obstacle>();
    private static int nextObstacleId = 1;

    public int id;
    public Rigidbody rigidBody;
    public Vector3 scaleMultiplier;
    public Vector3 initialForce;

    private void Start()
    {
        id = nextObstacleId;
        nextObstacleId++;
        obstacles.Add(id, this);

        ServerSend.SpawnObstacle(this);

        rigidBody.velocity = initialForce;
        StartCoroutine(DestroyAfterTime());
    }

    public void Initialize(Vector3 _scaleMultiplier, float _initialForceStrength)
    {
        initialForce = Vector3.right * _initialForceStrength;
        scaleMultiplier = _scaleMultiplier;
    }

    private void FixedUpdate()
    {
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