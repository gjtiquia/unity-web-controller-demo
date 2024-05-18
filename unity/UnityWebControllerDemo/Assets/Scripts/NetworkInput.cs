using System;
using UnityEngine;
using UnityEngine.Assertions;

public class NetworkInput : MonoBehaviour
{
    // PUBLIC GETTERS
    public bool IsLeftPressed { get; private set; } = false;
    public bool IsRightPressed { get; private set; } = false;
    public bool IsJumpPressed { get; private set; } = false;

    // SERIALIZED MEMBERS
    [SerializeField] private NetworkController _networkController;

    // MonoBehaviour INTERFACE
    private void OnValidate()
    {
        Assert.IsNotNull(_networkController);
    }

    private void Start()
    {
        _networkController.OnMessageEvent += OnMessage;
    }

    private void OnDestroy()
    {
        _networkController.OnMessageEvent -= OnMessage;
    }

    // PRIVATE METHODS
    private void OnMessage(string message)
    {
        Debug.Log($"{_networkController.Id}.NetworkInput.OnMessage: {message}");

        switch (message)
        {
            case "keydown-left": IsLeftPressed = true; break;
            case "keyup-left": IsLeftPressed = false; break;

            case "keydown-right": IsRightPressed = true; break;
            case "keyup-right": IsRightPressed = false; break;

            case "keydown-jump": IsJumpPressed = true; break;
            case "keyup-jump": IsJumpPressed = false; break;
        }
    }
}