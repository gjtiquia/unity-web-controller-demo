using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using WebSocketSharp;

public class NetworkManager : MonoBehaviour
{
    // SERIALIZED FIELDS
    [Header("Web Socket Settings")]
    [SerializeField] private string _webSocketURL = "ws://localhost:3000/";

    [Header("Game Settings")]
    [SerializeField] private NetworkController _playerPrefab;
    [SerializeField] private Transform _spawnPosition;

    // PRIVATE MEMBERS
    private WebSocket _ws;
    private Dictionary<string, NetworkController> _playerInstances = new();
    private ConcurrentBag<string> _textMessageBag = new();

    // MonoBehaviour INTERFACE
    private void OnValidate()
    {
        Assert.IsNotNull(_playerPrefab);
        Assert.IsNotNull(_spawnPosition);
    }

    private void Start()
    {
        Application.runInBackground = true; // Keeps Unity running even though not in focus
        StartWebSocket();
    }

    private void OnDestroy()
    {
        CloseWebSocket();
    }

    private void Update()
    {
        HandleAndClearTextMessageBag();
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
                Debug.Log($"NetworkManager: WebSocket opened. Current State: {_ws.ReadyState}");
            });
        };

        _ws.OnClose += (sender, e) =>
        {
            Debug.Log($"NetworkManager: Websocket closed. Code: {e.Code}. Reason: {e.Reason}");
        };

        _ws.OnMessage += (sender, e) =>
        {
            if (e.IsText)
            {
                // Debug.Log("NetworkManager: Received text message: " + e.Data);
                AddTextMessageToBag(e.Data); // Needed because this is not on the main thread
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
        Debug.Log($"NetworkManager: Connecting web socket to url {_webSocketURL}. Current State: {_ws.ReadyState}");
    }

    private void CloseWebSocket()
    {
        if (_ws.ReadyState == WebSocketState.Closing || _ws.ReadyState == WebSocketState.Closed)
            return;

        Debug.Log("NetworkManager: Closing web socket. Current State: " + _ws.ReadyState);
        _ws.CloseAsync();
    }

    private void AddTextMessageToBag(string textMessage)
    {
        _textMessageBag.Add(textMessage);
    }

    private void HandleAndClearTextMessageBag()
    {
        while (_textMessageBag.Count > 0)
        {
            var canTake = _textMessageBag.TryTake(out string textMessage);
            if (!canTake) continue;

            HandleTextMessage(textMessage);
        }
    }

    private void HandleTextMessage(string textMessage)
    {
        var parsedMessage = JsonUtility.FromJson<RelayMessage>(textMessage);

        var id = parsedMessage.id;
        var message = parsedMessage.message;

        // Debug.Log($"NetworkManager: Parsed text message: id: {id}, message: {message}");

        switch (message)
        {
            case "connected":
                SpawnPlayer(id);
                break;

            case "disconnected":
                DespawnPlayer(id);
                break;

            default:
                RelayMessageToPlayer(id, message);
                break;
        }
    }

    private void SpawnPlayer(string id)
    {
        if (_playerInstances.ContainsKey(id))
        {
            Debug.LogError($"NetworkManager.SpawnPlayer: Player with id {id} already spawned!");
            return;
        }

        Debug.Log($"NetworkManager.SpawnPlayer: Spawning Player {id}");

        var instance = Instantiate(_playerPrefab, _spawnPosition.position, Quaternion.identity);
        instance.Initialize(id);

        _playerInstances.Add(id, instance);
    }

    private void DespawnPlayer(string id)
    {
        if (!_playerInstances.ContainsKey(id))
        {
            Debug.LogError($"NetworkManager.DespawnPlayer: Player with id {id} does not exist!");
            return;
        }

        Debug.Log($"NetworkManager.DespawnPlayer: Despawning Player {id}");

        _playerInstances.Remove(id, out var instance);
        Destroy(instance.gameObject);
    }

    private void RelayMessageToPlayer(string id, string message)
    {
        if (!_playerInstances.ContainsKey(id))
        {
            Debug.LogError($"NetworkManager.RelayMessageToPlayer: Player with id {id} does not exist!");
            return;
        }

        var instance = _playerInstances[id];
        instance.OnMessage(message);
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