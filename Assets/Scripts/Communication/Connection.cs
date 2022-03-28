using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System.Text.RegularExpressions;
using System;
using System.Globalization;

public class Connection : MonoBehaviour
{
    WebSocket websocket;

    private UserInstruction instruction = new UserInstruction();
    private OrderProperties order_properties = new OrderProperties();
    private PerformanceProperties performance_info = new PerformanceProperties();
    private bool connected = false;
    private bool retry = true;
    private bool init_received = false;
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
            Debug.Log("Message received: " + message);
            if (message.Length > 0)
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
        Debug.Log(message);
        if ( message == "Connected") 
        {
            SendWebSocketMessage("ACK-Connected");
        }
        else if (message.Contains("new_instructions"))  //Reset support for next work step
        {
            this.GetComponent<MessageHandler>().NewInstructions();
            init_received = true;
            SendWebSocketMessage("ACK-new_instructions");
        }
        else if (message.Contains("version"))  //Set product version {"version": "V3.3"}
        {
            Regex rx = new Regex(@"version<(.*?)>");
            string result = rx.Match(message).Groups[1].Value;
            this.GetComponent<MessageHandler>().InitializeVersion(result);
            SendWebSocketMessage("ACK-version");
        }
        else if (message.Contains("number_steps"))  //Set number of steps
        {
            Regex rx = new Regex(@"number_steps<(.*?)>");
            string result = rx.Match(message).Groups[1].Value;
            this.GetComponent<MessageHandler>().InitializeSteps(Convert.ToInt32(result));
            SendWebSocketMessage("ACK-number_steps");
        }
        else if (message.Contains("number_points"))  //Set number of points
        {
            Regex rx = new Regex(@"number_points<(.*?)>");
            string result = rx.Match(message).Groups[1].Value;
            this.GetComponent<MessageHandler>().InitializePoints(Convert.ToInt32(result));
            SendWebSocketMessage("ACK-number_points");
        }
        //else if (message.Contains("action_type"))  //Show instruction
        //{
        //    if (init_received)  // Only execute messages if init was received
        //    {
        //        instruction = JsonConvert.DeserializeObject<UserInstruction>(message);
        //        if (instruction.action_type == "pickItem")
        //        {
        //            this.GetComponent<MessageHandler>().PickObject(
        //            instruction.item_name,
        //            instruction.color,
        //            instruction.knowledge_level,
        //            instruction.default_time
        //            );
        //        }
        //        else if (instruction.action_type == "pickTool")
        //        {
        //            this.GetComponent<MessageHandler>().PickTool(
        //                instruction.item_name,
        //                instruction.color,
        //                instruction.knowledge_level,
        //                instruction.default_time
        //                );
        //        }
        //        else if (instruction.action_type == "mount")
        //        {
        //            if (instruction.item_list != null)
        //            {
        //                foreach (string item in instruction.item_list)
        //                {
        //                    this.GetComponent<MessageHandler>().ShowAssemblyPosition(
        //                    item,
        //                    instruction.knowledge_level,
        //                    instruction.default_time
        //                    );
        //                }
        //            }
        //            if (instruction.action_list != null)
        //            {
        //                foreach (string toolpoint in instruction.action_list)
        //                {
        //                    this.GetComponent<MessageHandler>().ShowToolUsage(
        //                    toolpoint,
        //                    instruction.knowledge_level,
        //                    instruction.default_time
        //                    );
        //                }
        //            }
        //        }
        //        else if (instruction.action_type == "returnTool")
        //        {
        //            this.GetComponent<MessageHandler>().ReturnTool(
        //                instruction.item_name,
        //                instruction.color,
        //                instruction.knowledge_level,
        //                instruction.default_time
        //                );
        //        }
        //        else
        //        {
        //            Debug.Log("Unknown user action: " + instruction.action_type);
        //        }
        //    }
        //}
        else if (message.Contains("order_finished"))
        {
            this.GetComponent<MessageHandler>().FinishJob();
        }
        else if (message.Contains("performance"))  // {"message_color": {"r": 0, "g": 1, "b": 0}}
        {
            Regex rx = new Regex(@"total_points<(.*?)>");
            string total_points = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"time_performance<(.*?)>");
            string time_performance = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"quality_performance<(.*?)>");
            string quality_performance = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"total_level<(.*?)>");
            string total_level = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"node_finished<(.*?)>");
            string node_finished = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"level_up<(.*?)>");
            string level_up = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"perfect_run<(.*?)>");
            string perfect_run = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"message_text<(.*?)>");
            string message_text = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"message_color_r<(.*?)>");
            string message_color_r = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"message_color_g<(.*?)>");
            string message_color_g = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"message_color_b<(.*?)>");
            string message_color_b = rx.Match(message).Groups[1].Value;


            this.GetComponent<MessageHandler>().ParsePerformanceMessage(
                total_points: Convert.ToInt32(total_points),
                time_performance: float.Parse(time_performance, CultureInfo.InvariantCulture.NumberFormat),
                quality_performance: float.Parse(quality_performance, CultureInfo.InvariantCulture.NumberFormat),
                total_level: Convert.ToInt32(total_level),
                node_finished: node_finished,
                level_up: level_up,
                perfect_run: perfect_run,
                message_text: message_text,
                message_color_r: Convert.ToInt32(message_color_r),
                message_color_g: Convert.ToInt32(message_color_g),
                message_color_b: Convert.ToInt32(message_color_b));
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

    public void SendInformation(string message)
    {
        Debug.Log("Send information to server: " + message);
        SendWebSocketMessage(message);
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}
