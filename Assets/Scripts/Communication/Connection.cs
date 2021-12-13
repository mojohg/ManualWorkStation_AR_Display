using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;

public class Connection : MonoBehaviour
{
    WebSocket websocket;

    private UserInstruction instruction = new UserInstruction();
    private OrderProperties order_properties = new OrderProperties();
    private bool connected = false;
    private bool retry = true;
    private IEnumerator coroutine;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        websocket = new WebSocket("ws://localhost:8080");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                retry = true;  // reset variable for next connection loss
            }            
            connected = true;
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);  // get message as string
            // Debug.Log("Message received: " + message);
            if(message.Length > 0)
            {
                ExecuteCommand(message);
                message = "";
            }
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed - " + e);
            connected = false;
        };

        // InvokeRepeating("SendHeartbeat", 0.0f, 0.3f);  // Heartbeat at every 0.3s
        EstablishConnection();
    }

    void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
                websocket.DispatchMessageQueue();
        #endif

        if (connected != true)
        {
            if (retry == true)
            {
                retry = false;
                Debug.Log("No connection -> retry after 5s");
                coroutine = RetryConnectionCoroutine();
                StartCoroutine(coroutine);
            }
        }
    }

    IEnumerator RetryConnectionCoroutine()
    {
        yield return new WaitForSeconds(5);
        EstablishConnection();
        retry = true;
    }

    void ExecuteCommand(string message)
    {
        if ( message == "Connected") 
        {
            SendWebSocketMessage("ACK-Connected");
        }
        else if (message.Contains("new_instructions"))  //Reset support for next work step
        {
            this.GetComponent<MessageHandler>().NewInstructions();
            SendWebSocketMessage("ACK-new_instructions");
        }
        else if (message.Contains("version"))  //Set product version
        {
            order_properties = JsonConvert.DeserializeObject<OrderProperties>(message);
            this.GetComponent<MessageHandler>().InitializeVersion(order_properties.version);
            SendWebSocketMessage("ACK-version");
        }
        else if (message.Contains("number_steps"))  //Set number of steps
        {
            order_properties = JsonConvert.DeserializeObject<OrderProperties>(message);
            this.GetComponent<MessageHandler>().InitializeSteps(order_properties.number_steps);
            SendWebSocketMessage("ACK-number_steps");
        }
        else if (message.Contains("number_points"))  //Set number of points
        {
            order_properties = JsonConvert.DeserializeObject<OrderProperties>(message);
            this.GetComponent<MessageHandler>().InitializePoints(order_properties.number_points);
            SendWebSocketMessage("ACK-number_points");
        }
        else if (message.Contains("action_type"))  //Show instruction
        {
            instruction = JsonConvert.DeserializeObject<UserInstruction>(message);
            if (instruction.action_type == "pickItem")
            {
                this.GetComponent<MessageHandler>().PickObject(
                instruction.item_name,
                instruction.color,
                instruction.knowledge_level,
                instruction.default_time
                );
            }
            else if (instruction.action_type == "pickTool")
            {
                this.GetComponent<MessageHandler>().PickTool(
                    instruction.item_name,
                    instruction.color,
                    instruction.knowledge_level,
                    instruction.default_time
                    );
            }
            else if (instruction.action_type == "placeItem")
            {
                this.GetComponent<MessageHandler>().ShowAssemblyPosition(
                    instruction.item_name,
                    instruction.action_name,
                    instruction.knowledge_level,
                    instruction.default_time
                    );
            }
        }
        else
        {
            Debug.Log("Unknown message type: " + message);
        }
    }

    async void EstablishConnection()
    {
        await websocket.Connect();
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

    private void SendHeartbeat()
    {
        SendWebSocketMessage("Ping");
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}
