using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : Controller
{
    Rigidbody2D rb = null;

    float axisMovementDeadzone = 0.01f;

    protected new void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    protected new void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        HandleInput();
    }

    void HandleInput()
    {
        float horizontalAxis = Input.GetAxis("Horizontal_" + teamNumber);
        float verticalAxis = Input.GetAxis("Vertical_" + teamNumber);

        if (Mathf.Abs(horizontalAxis) > axisMovementDeadzone || Mathf.Abs(verticalAxis) > axisMovementDeadzone)
        {
            Move(new Vector2(horizontalAxis, verticalAxis));
        }
    }

    void Move(Vector2 movement)
    {
        Vector2 targetPosition = new Vector2(transform.position.x, transform.position.y) + movement * movementSpeed * Time.deltaTime;
        rb.MovePosition(targetPosition);

        float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        Quaternion quart = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, quart, Time.deltaTime * rotationSpeed);
    }

}
