using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public static Color[] colorsArr = 
    {
        new Color(0,0.1f,1,1),
        new Color(0,1,0.1f,1),
        new Color(1,0,0.1f,1),
    };
    public int id;

    public void Initialize(int _id, int _colorIndex)
    {
        id = _id;
        CreateRandomColor(_colorIndex);
    }

    public void CreateRandomColor(int _colorIndex)
    {
        try
        {
            this.GetComponent<Renderer>().material.color = colorsArr[_colorIndex];
        }
        catch (System.IndexOutOfRangeException)
        {
            Debug.Log(_colorIndex);
        }
    }

    public void Destroy(Vector3 _position)
    {
        transform.position.Set(_position.x, _position.y, _position.z);
        GameManager.obstacles.Remove(id);
        Destroy(gameObject);
    }

    public void ChangePosition(Vector3 _position)
    {
        transform.position = _position;
    }

    public override string ToString()
    {
        return id + "--" + transform.position;
    }
}
