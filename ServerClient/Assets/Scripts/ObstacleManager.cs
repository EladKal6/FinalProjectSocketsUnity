using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public int id;

    public void Initialize(int _id)
    {
        id = _id;
    }

    public void Destroy(Vector3 _position)
    {
        transform.position = _position;
        GameManager.obstacles.Remove(id);
        Destroy(gameObject);
    }
}
