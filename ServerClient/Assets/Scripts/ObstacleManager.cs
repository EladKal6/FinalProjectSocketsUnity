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
        transform.position.Set(_position.x, _position.y, _position.z);
        GameManager.obstacles.Remove(id);
        Destroy(gameObject);
    }

    public void ChangePosition(Vector3 _position)
    {
        Debug.Log(id + " " + _position);
        transform.position = _position;
    }

    public override string ToString()
    {
        return id + "--" + transform.position;
    }
}
