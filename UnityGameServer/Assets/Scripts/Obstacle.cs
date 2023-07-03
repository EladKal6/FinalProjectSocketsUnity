using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public static Dictionary<int, Obstacle> obstacles = new Dictionary<int, Obstacle>();
    public static Dictionary<int, Obstacle> platforms = new Dictionary<int, Obstacle>();
    public static Dictionary<int, Obstacle> lavas = new Dictionary<int, Obstacle>();
    public static Dictionary<int, Obstacle> anvils = new Dictionary<int, Obstacle>();
    private static int nextLavaId = 1;
    private static int nextObstacleId = 1;
    private static int nextPlatformId = 1;
    private static int nextAnvilId = 1;

    public int id;
    public int colorIndex;
    public string type;
    private Vector3 initialForce;
    public float timeToLive;

    private bool isGrounded = false;

    private void Start()
    {
        colorIndex = Random.Range(0, 3);
        Debug.Log(this.gameObject.name);
        if (this.gameObject.name == "Lava(Clone)")
        {
            id = nextLavaId;
            nextLavaId++;
            Debug.Log("Created Lava");
            type = "Lava";
            lavas.Add(id, this);
        }
        if (this.gameObject.name == "squareObstacle(Clone)")
        {
            id = nextObstacleId;
            nextObstacleId++;
            Debug.Log("Created squareObstacle");
            type = "squareObstacle";
            obstacles.Add(id, this);
        }
        if (this.gameObject.name == "MovingPlat(Clone)")
        {
            id = nextPlatformId;
            nextPlatformId++;
            Debug.Log("Created MovingPlat");
            type = "Platform";
            platforms.Add(id, this);
        }
        if (this.gameObject.name == "Anvil(Clone)")
        {
            id = nextAnvilId;
            nextAnvilId++;
            Debug.Log("Created Anvil");
            type = "Anvil";
            anvils.Add(id, this);
        }
        Debug.Log(type);
        ServerSend.SpawnObstacle(this);

        StartCoroutine(DestroyAfterTime());
    }

    public void Initialize(float _initialForceStrength, Vector3 direction)
    {
        initialForce = direction * _initialForceStrength;
    }

    private void FixedUpdate()
    {
        if (type != "Anvil")
        {
            transform.position += initialForce * Time.deltaTime;
        }
        
        ServerSend.ObstaclePosition(this);
    }

    void OnCollisionEnter(Collision collider)
    {
        try
        {
            if (type == "Anvil" && !isGrounded)
            {
                if (collider.gameObject.tag == "Player")
                {
                    if (collider.gameObject.GetComponent<Player>().dead == false)
                    {
                        collider.gameObject.GetComponent<Player>().Die();
                    }
                }
                if (collider.gameObject.tag == "Ground")
                {
                    Vector3 direction = transform.position - collider.gameObject.transform.position;
                    if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x) && Mathf.Abs(direction.y) > Mathf.Abs(direction.z))
                    {
                        isGrounded = true;
                    }
                }
                else
                {
                    Destroy(collider.transform.parent.gameObject);
                    ServerSend.PropDestroyed(collider.transform.parent.name);
                }
            }
            else if(type == "Lava")
            {
                if (collider.gameObject.tag == "Player")
                {
                    if (collider.gameObject.GetComponent<Player>().dead == false)
                    {
                        collider.gameObject.GetComponent<Player>().Die();
                    }
                }
            }
        }
        catch (System.Exception _ex)
        {
            Debug.LogError("ERROR " + _ex);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (type == "Lava")
        {
            if (other.gameObject.tag == "Player")
            {
                if (other.gameObject.GetComponent<Player>().dead == false)
                {
                    other.gameObject.GetComponent<Player>().Die();
                }
            }
        }
        else if (type == "Anvil")
        {
            Destroy(other.transform.parent.gameObject);
            ServerSend.PropDestroyed(other.transform.parent.name);
        }
    }


    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(timeToLive);

        ServerSend.ObstacleDestroyed(this);

        if (type == "Lava")
        {
            lavas.Remove(id);
        }
        if (type == "squareObstacle")
        {
            obstacles.Remove(id);
        }
        if (type == "MovingPlat")
        {
            platforms.Remove(id);
        }
        if (type == "Anvil")
        {
            anvils.Remove(id);
        }

        Destroy(gameObject);
    }
}