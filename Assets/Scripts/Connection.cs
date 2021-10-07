using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public class Connection : MonoBehaviour
{
    WebSocket websocket;
    GameObject testcube;

    // Start is called before the first frame update
    async void Start()
    {
        testcube = GameObject.Find("Cube");
        websocket = new WebSocket("ws://localhost:8080");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        //websocket.OnMessage += (bytes) =>
        //{            
        //    var message = System.Text.Encoding.UTF8.GetString(bytes);  // get message as string
        //    Debug.Log("Message: " + message);
        //};

        // Keep sending messages at every 0.3s
        InvokeRepeating("SendHeartbeat", 0.0f, 0.3f);

        // waiting for messages
        // await websocket.Connect();
        EstablishConnection();
    }

    void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
                websocket.DispatchMessageQueue();
        #endif

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);  // get message as string
            ExecuteCommand(message);
        };
    }

    void ExecuteCommand(string command)
    {
        if(command == "Connected")
        {
            Debug.Log("Connection to websocket server established");
        }
        else
        {
            testcube.GetComponent<ObjectHandling>().ChangeMaterial(command);
        }
    }

    async void EstablishConnection()
    {
        await websocket.Connect();
    }

    async void SendHeartbeat()
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            // await websocket.Send(new byte[] { 10, 20, 30 });

            // Sending plain text
            await websocket.SendText("Connected");
        }
    }

    async void SendWebSocketMessage(string msg)
    {
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            // await websocket.Send(new byte[] { 10, 20, 30 });

            // Sending plain text
            await websocket.SendText(msg);
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}
