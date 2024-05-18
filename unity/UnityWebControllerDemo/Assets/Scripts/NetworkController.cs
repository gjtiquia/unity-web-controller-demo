using System;
using UnityEngine;

public class NetworkController : MonoBehaviour
{
    // PUBLIC EVENTS
    public Action<string> OnMessageEvent;

    // PUBLIC GETTERS
    public string Id => _id;

    // SERIALIZED MEMBERS
    [SerializeField] private string _id;

    // PUBLIC METHODS
    public void Initialize(string id)
    {
        _id = id;
    }

    public void OnMessage(string message)
    {
        OnMessageEvent?.Invoke(message);
    }
}