using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class Connectivity : MonoBehaviour
{
    static WebSocket _ws;
    static string _lastMessage = "";
    public string url;
    public CharacterController prefabPlayer;
    private Dictionary<string, CharacterController> _players = new Dictionary<string, CharacterController>();

    private void Awake()
    {
        _ws = new WebSocket(url);

        _ws.OnError += (sender, e) =>
        {
            print("ws.OnError " + e.Exception);
        };

        _ws.OnClose += (sender, e) =>
        {
            print("ws.OnClose " + e.Reason);
        };
        _ws.OnMessage += (sender, e) =>
        {
            print("ws.OnMessage " + e.Data);
            var arr = e.Data.Split('|');
            if (arr.Length != 3) return;
            // 0: id, 1: action, 2: position
            OnMessageMainThread(arr);
           

        };
        _ws.Connect();

    }

    private void OnDestroy()
    {
        _ws?.Close();
    }

    public static void Send(string state, Vector3 position) {
        string message = state + "|" + position.ToString("F8");

        if ((_ws?.IsAlive ?? false) &&  !_lastMessage.Contains(message)) {
            _lastMessage = message;
            _ws?.Send(message);
        }
    }

    void OnMessageMainThread(string[] arr) {
        MainThreadDispatcher.Instance().EnqueueAsync(() =>
        {

            if (!_players.ContainsKey(arr[0]) && arr[1] != "CLOSE")
            {
                PlayerInstantiate(arr[0], StringToVector3(arr[2]));
            }
            else {
                PlayerSync(arr[0], arr[1], StringToVector3(arr[2]));
            }
        });
    }

    void PlayerInstantiate(string id, Vector3 position) {
        var go = Instantiate(prefabPlayer, transform);
        go.transform.position = position;
        _players[id] = go;
        
    }

    void PlayerSync(string id, string action, Vector3 position) {
        switch (action) {
            case "CLOSE":
                Destroy(_players[id].gameObject);
                break;
            case "MoveDown":
            case "MoveUp":
            case "MoveLeft":
            case "MoveRight":
                _players[id].Move(action, position);
                break;
            case "Idle":
                _players[id].Idle();
                break;
            case "Attack":
                _players[id].Attack();
                break;
        }
    }
    private Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3

        return new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2])); ;
    }

}
