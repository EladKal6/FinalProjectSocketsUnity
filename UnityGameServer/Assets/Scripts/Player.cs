using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<int, int> scoreboard = new Dictionary<int, int>(); //<player.id, score>

    public int id;
    public string username;
    public CharacterController controller;
    public Transform shootOrigin;
    public MeshRenderer model;
    public GameObject playerObj;
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public float health;
    public float maxHealth = 100f;

    private bool alreadyWon = false;
    private bool tpToStart = false;

    public bool[] inputs;
    private float yVelocity = 0;
    public bool dead = false;

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;

        health = maxHealth;

        scoreboard.Add(_id, 0);

        inputs = new bool[5];
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }

    /// <summary>Processes player input and moves the player.</summary>
    public void FixedUpdate()
    {
        try
        {
            Vector2 _inputDirection = Vector2.zero;
            if (inputs[0])
            {
                _inputDirection.y += 1;
            }
            if (inputs[1])
            {
                _inputDirection.y -= 1;
            }
            if (inputs[2])
            {
                _inputDirection.x -= 1;
            }
            if (inputs[3])
            {
                _inputDirection.x += 1;
            }

            if (dead)
            {
                SpecMove(_inputDirection);
            }
            else
            {
                Move(_inputDirection);
            }

            if (tpToStart)
            {
                tpToStart = false;
                Revive();
                playerObj.transform.position = new Vector3(0f, 5f, 0f);
            }

            ServerSend.PlayerPosition(this);
            ServerSend.PlayerRotation(this);
        }
        catch(System.Exception _ex)
        {
            Debug.LogError("ERROR! " + _ex);
        }
    }

    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="_inputDirection"></param>
    private void Move(Vector2 _inputDirection)
    {
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= moveSpeed;

        //if dead apply gravity
        if (controller.isGrounded)
        {
            yVelocity = 0f;
            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }
        yVelocity += gravity;

        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);

        //check if the player died
        if (this.transform.position.y < -7)
        {
            Die();
        }
        if (!alreadyWon && this != null)
        {
            if (NetworkManager.instance.GetActiveMinigame() == "Parkour")
            {
                if (this.transform.position.x > 100 || OnlyPlayerAlive())
                {
                    Win();
                }
            }
            else if (NetworkManager.instance.GetActiveMinigame() == "LavaFloor" && OnlyPlayerAlive())
            {
                Win();
            }
            else if (NetworkManager.instance.GetActiveMinigame() == "DodgeObs" && OnlyPlayerAlive())
            {
                Win();
            }
            else if (NetworkManager.instance.GetActiveMinigame() == "ShootingCity" && OnlyPlayerAlive())
            {
                Win();
            }
            else if (NetworkManager.instance.GetActiveMinigame() == "Park")
            {
                if (this.transform.position.y > 20 || OnlyPlayerAlive())
                {
                    Win();
                }
            }
        }
    }

    public bool OnlyPlayerAlive()
    {
        foreach(Client client in Server.clients.Values)
        {
            if (client != null && client.player != null && client.id != id && client.player.dead == false)
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>Makes the player Crouch.</summary>
    public void Crouch()
    {

    }

    /// <summary>As Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="_inputDirection"></param>
    private void SpecMove(Vector2 _inputDirection)
    {
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        if (inputs[4])
        {
            _moveDirection.y += 2;
        }
        if (inputs[5])
        {
            _moveDirection.y -= 2;
        }

        _moveDirection *= moveSpeed;

        controller.Move(_moveDirection);
        

    }

    public void Shoot(Vector3 _viewDirection)
    {
        if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 25f))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                _hit.collider.GetComponent<Player>().TakeDamage(50f);
            }
        }
    }

    public void TakeDamage(float _damage)
    {
        if (health <= 0f)
        {
            return;
        }

        health -= _damage;
        if (health <= 0f)
        {
            Die();
        }

        ServerSend.PlayerHealth(this);
    }

    //Kill this player and turn him into a spectator
    public void Die()
    {
        dead = true;
        model.enabled = false;
        playerObj.layer = 8;
        ServerSend.PlayerDied(this.id);
    }

    //Revive this player
    public void Revive()
    {
        health = maxHealth;
        dead = false;
        model.enabled = true;
        playerObj.layer = 0;
        ServerSend.PlayerRevived(this.id);
    }

    //Kill this player and turn him into a spectator
    private void Win()
    {
        alreadyWon = true;
        Debug.Log(username + " Won!");

        scoreboard[id]++;

        StartCoroutine(IWin());
    }

    private IEnumerator IWin()
    {
        ServerSend.roundWinnerUsername(username);
        yield return new WaitForSeconds(6);

        NetworkManager.instance.StartMinigame(id, "Game");
        alreadyWon = false;
    }

    public void TPToStart()
    {
        tpToStart = true;
    }
    public bool IsRunning()
    {
        return inputs[0] || inputs[1] || inputs[2] || inputs[3];
    }

    public bool IsJumping()
    {
        return inputs[4];
    }

    public bool IsFalling()
    {
        return !controller.isGrounded;
    }

    public static string ScoreboardToString()
    {
        string S = "Scoreboard:  \n";
        foreach(Client client in Server.clients.Values)
        {
            if (client != null && client.player != null)
            {
                S += client.player.username + " -- " + scoreboard[client.player.id] + "\n";
            }
        }
        return S;
    }
}   