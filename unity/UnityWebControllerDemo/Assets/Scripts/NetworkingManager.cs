using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class NetworkingManager : MonoBehaviour
{
    // SERIALIZED FIELDS
    [SerializeField] private string _webSocketURL = "ws://localhost:3000/";

    // PRIVATE MEMBERS
    private WebSocket _ws;

    // MonoBehaviour INTERFACE
    private void Start()
    {
        StartWebSocket();
    }

    private void OnDestroy()
    {
        CloseWebSocket();
    }

    // PRIVATE METHODS
    private void StartWebSocket()
    {
        _ws = new WebSocket(_webSocketURL);

        _ws.OnMessage += (sender, e) =>
            Debug.Log("NetworkingManager: Received message: " + e.Data);

        _ws.Connect();
        _ws.Send("{\"origin\": \"unity\", \"message\": \"connected\"}"); // TODO : Install Newtonsoft

        Debug.Log($"NetworkingManager: WebSocket connected. URL: {_webSocketURL}");
    }

    private void CloseWebSocket()
    {
        _ws.Close();
    }
}
