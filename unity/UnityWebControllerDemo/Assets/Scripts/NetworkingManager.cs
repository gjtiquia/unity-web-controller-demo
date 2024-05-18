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

        _ws.OnOpen += (sender, e) =>
        {
            var message = new WebSocketMessage("connected");
            var messageJson = JsonUtility.ToJson(message);
            _ws.SendAsync(messageJson, completed: (_) =>
            {
                Debug.Log($"NetworkingManager: WebSocket opened. Current State: {_ws.ReadyState}");
            });
        };

        _ws.OnClose += (sender, e) =>
        {
            Debug.Log($"NetworkingManager: Websocket closed. Code: {e.Code}. Reason: {e.Reason}");
        };

        _ws.OnMessage += (sender, e) =>
        {
            if (e.IsText)
            {
                Debug.Log("NetworkingManager: Received text message: " + e.Data);
                OnTextMessageReceived(e.Data);
            }

            if (e.IsBinary)
                Debug.Log("NetworkingManage: Received binary message: " + e.RawData.ToString());
        };

        _ws.OnError += (sender, e) =>
        {
            Debug.LogError(e.Message);
            Debug.LogException(e.Exception);
        };

        _ws.ConnectAsync(); // Use async or else the whole main thread will be blocked. Expected state: Connecting.
        Debug.Log($"NetworkingManager: Connecting web socket to url {_webSocketURL}. Current State: {_ws.ReadyState}");
    }

    private void CloseWebSocket()
    {
        if (_ws.ReadyState == WebSocketState.Closing || _ws.ReadyState == WebSocketState.Closed)
            return;

        Debug.Log("NetworkingManager: Closing web socket. Current State: " + _ws.ReadyState);
        _ws.CloseAsync();
    }

    private void OnTextMessageReceived(string textMessage)
    {
        var parsedMessage = JsonUtility.FromJson<RelayMessage>(textMessage);

        Debug.Log($"NetworkingManager: Parsed text message: id: {parsedMessage.id}, message: {parsedMessage.message}");
    }
}

[System.Serializable]
public class WebSocketMessage
{
    public string origin;
    public string message;

    public WebSocketMessage(string message)
    {
        this.origin = "unity";
        this.message = message;
    }
}

[System.Serializable]
public class RelayMessage
{
    public string id;
    public string message;
}