using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MovementController : MonoBehaviour
{
    // SERIALIZED MEMBERS
    [Header("Settings")]
    [SerializeField] private float _jumpForce;

    [Header("References")]
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private NetworkInput _networkInput;

    // PRIVATE MEMBERS
    private MovementInput _input = MovementInput.Create();
    private bool _isJumping = false;

    // MonoBehaviour INTERFACE
    private void OnValidate()
    {
        Assert.IsNotNull(_rigidbody);
        Assert.IsNotNull(_networkInput);
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
        input.IsJumpPressed = Input.GetKey(KeyCode.Space) || _networkInput.IsJumpPressed;
        input.IsLeftPressed = Input.GetKey(KeyCode.A);
        input.IsRightPressed = Input.GetKey(KeyCode.D);

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

        // TODO : Left right movement
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
