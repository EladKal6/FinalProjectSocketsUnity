using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public CharacterController controller;
    public MeshRenderer model;
    public GameObject playerObj;
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;


    private bool[] inputs;
    private float yVelocity = 0;
    private bool dead = false;

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
        if (this.transform.position.y < -1)
        {
            Die();
        }

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
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

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }
    //Kill this player and turn him into a spectator
    private void Die()
    {
        dead = true;
        model.enabled = false;
        playerObj.layer = 8;
        ServerSend.PlayerDied(this.id);
    }
}   