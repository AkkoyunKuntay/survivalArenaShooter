using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private FloatingJoystick joystick;
    
    [Header("Movement Adjustments")]
    public float movemetSpeed;
    
    [Header("Debug")]
    [SerializeField] private Vector3 movementVector;

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        movementVector.x = joystick.Horizontal;
        movementVector.y = 0;
        movementVector.z = joystick.Vertical;
        
        rb.velocity = new Vector3(movementVector.x, movementVector.y, movementVector.z) * movemetSpeed;
    }
}
