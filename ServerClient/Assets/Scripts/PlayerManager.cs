using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static string localPlayerUsername = "----";
    public int id;
    public string username;
    public float health;
    public float maxHealth = 100f;
    public bool dead;
    public SkinnedMeshRenderer model;
    public Animator anim;
    public TextMeshProUGUI worldUsername;
    public Canvas playerInfo;
    public Camera localPlayerCamera = null;
    public GameObject Gun;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        if (_id != Client.instance.myId)
        {
            SetPlayerInfo();
            SetlocalPlayerCamera();
        }
    }

    public void SetPlayerInfo()
    {
        worldUsername.text = username;
    }

    public void SetlocalPlayerCamera()
    {
        localPlayerCamera = Camera.main;
    }

    public void SetHealth(float _health)
    {
        health = _health;

        if (health <= 0)
        {
            Die();
        }
    }

    void Update()
    {
        if (localPlayerCamera != null && playerInfo != null)
        {
            playerInfo.transform.LookAt(playerInfo.transform.position + localPlayerCamera.transform.rotation * Vector3.forward, localPlayerCamera.transform.rotation * Vector3.up);
        }
    }

    public void Die()
    {
        model.enabled = false;
    }

    public void Revived()
    {
        model.enabled = true;
        SetHealth(maxHealth);
    }

    public void ShowGun()
    {
        Gun.SetActive(true);
    }

    public void HideGun()
    {
        Gun.SetActive(false);
    }
}
