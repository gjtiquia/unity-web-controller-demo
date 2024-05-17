using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    // SERIALIZED MEMBERS
    [SerializeField] private float _jumpForce;

    // PRIVATE MEMBERS
    private Rigidbody2D _rigidbody;
    private MovementInput _input = MovementInput.Create();
    private bool _isJumping = false;

    // MonoBehaviour INTERFACE
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        PollInput();
        ApplyForces();
        UpdateState();
    }

    // PRIVATE METHODS
    private void PollInput()
    {
        // Create empty input struct
        var input = MovementInput.Create();

        // Populate input struct
        input.IsJumpPressed = Input.GetKey(KeyCode.Space);

        // Save input struct
        _input = input;
    }

    private void ApplyForces()
    {
        if (_input.IsJumpPressed && !_isJumping)
        {
            _rigidbody.AddForce(_jumpForce * Vector2.up, ForceMode2D.Impulse);
            _isJumping = true;
        }
    }

    private void UpdateState()
    {
        // if not moving vertically (within a 0.01f threshold), then allow jumping
        // this also nicely allows double jump 😂😂😂
        if (Mathf.Abs(_rigidbody.velocity.y) <= 0.01f)
        {
            _isJumping = false;
        }
    }
}

public struct MovementInput
{
    public bool IsLeftPressed;
    public bool IsRightPressed;
    public bool IsJumpPressed;

    public static MovementInput Create()
    {
        return new()
        {
            IsLeftPressed = false,
            IsRightPressed = false,
            IsJumpPressed = false,
        };
    }
}
