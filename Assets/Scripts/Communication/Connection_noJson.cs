using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System.Text.RegularExpressions;
using System;
using System.Globalization;

public class Connection_noJson : MonoBehaviour
{
    WebSocket websocket;
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
        if ( message == "Connected") 
        {
            SendWebSocketMessage("ACK-Connected");
        }
        else if (message.Contains("new_instructions"))  //Reset support for next work step
        {
            this.GetComponent<MessageHandler_noJson>().NewInstructions();
            init_received = true;
            SendWebSocketMessage("ACK-new_instructions");
        }
        else if (message.Contains("version"))  //Set product version {"version": "V3.3"}
        {
            Regex rx = new Regex(@"version<(.*?)>");
            string result = rx.Match(message).Groups[1].Value;
            this.GetComponent<MessageHandler_noJson>().InitializeVersion(result);
            SendWebSocketMessage("ACK-version");
        }
        else if (message.Contains("number_steps"))  //Set number of steps
        {
            Regex rx = new Regex(@"number_steps<(.*?)>");
            string result = rx.Match(message).Groups[1].Value;
            this.GetComponent<MessageHandler_noJson>().InitializeSteps(Convert.ToInt32(result));
            SendWebSocketMessage("ACK-number_steps");
        }
        else if (message.Contains("number_points"))  //Set number of points
        {
            Regex rx = new Regex(@"number_points<(.*?)>");
            string result = rx.Match(message).Groups[1].Value;
            this.GetComponent<MessageHandler_noJson>().InitializePoints(Convert.ToInt32(result));
            SendWebSocketMessage("ACK-number_points");
        }
        else if (message.Contains("action_type"))  //Show instruction
        {
            if (init_received)  // Only execute messages if init was received
            {
                Regex rx = new Regex(@"action_type<(.*?)>");
                string action_type = rx.Match(message).Groups[1].Value;

                rx = new Regex(@"knowledge_level<(.*?)>");
                string knowledge_level_string = rx.Match(message).Groups[1].Value;
                int knowledge_level = Convert.ToInt32(knowledge_level_string);

                rx = new Regex(@"default_time<(.*?)>");
                string default_time_string = rx.Match(message).Groups[1].Value;
                double default_time = Convert.ToDouble(default_time_string);

                if (action_type == "pickItem")
                {
                    rx = new Regex(@"item_name<(.*?)>");
                    string item_name = rx.Match(message).Groups[1].Value;

                    rx = new Regex(@"color<(.*?)>");
                    string color = rx.Match(message).Groups[1].Value;

                    this.GetComponent<MessageHandler_noJson>().PickObject(
                    item_name,
                    color,
                    knowledge_level,
                    default_time
                    );
                }
                else if (action_type == "pickTool")
                {
                    rx = new Regex(@"item_name<(.*?)>");
                    string item_name = rx.Match(message).Groups[1].Value;

                    rx = new Regex(@"color<(.*?)>");
                    string color = rx.Match(message).Groups[1].Value;

                    this.GetComponent<MessageHandler_noJson>().PickTool(
                    item_name,
                    color,
                    knowledge_level,
                    default_time
                    );
                }
                else if (action_type == "mount")
                {
                    rx = new Regex(@"item_list<(.*?)>");
                    string items = rx.Match(message).Groups[1].Value;

                    rx = new Regex(@"action_list<(.*?)>");
                    string actions = rx.Match(message).Groups[1].Value;

                    rx = new Regex(@"annotation<(.*?)>");
                    string annotation = rx.Match(message).Groups[1].Value;

                    if (items.Length > 2)  // String should contain more than "[]"
                    {
                        string[] item_list = items.Split(',');

                        foreach (string item in item_list)
                        {
                            Debug.Log("Execute item " + item);
                            string item_name = item.Replace("[", "");
                            item_name = item_name.Replace("]", "");
                            item_name = item_name.Replace("'", "");
                            item_name = item_name.Replace(" ", "");
                            this.GetComponent<MessageHandler_noJson>().ShowAssemblyInfos(
                            item_name,
                            knowledge_level,
                            default_time,
                            annotation
                            );
                        }
                    }
                    if (actions.Length > 2)  // String should contain more than "[]"
                    {
                        // Debug.Log("Actions: " + actions);
                        string[] action_list = actions.Split(',');

                        foreach (string toolpoint in action_list)
                        {
                            if (toolpoint.Length > 0)
                            {
                                string toolpoint_name = toolpoint.Replace("[", "");
                                toolpoint_name = toolpoint_name.Replace("]", "");
                                toolpoint_name = toolpoint_name.Replace("'", "");
                                toolpoint_name = toolpoint_name.Replace(" ", "");
                                this.GetComponent<MessageHandler_noJson>().ShowToolUsage(
                                toolpoint_name,
                                knowledge_level,
                                default_time,
                                annotation
                                );
                            }                            
                        }
                    }
                }
                else if (action_type == "move")
                {
                    rx = new Regex(@"action_list<(.*?)>");
                    string actions = rx.Match(message).Groups[1].Value;

                    rx = new Regex(@"annotation<(.*?)>");
                    string annotation = rx.Match(message).Groups[1].Value;

                    if (actions.Length > 2)  // String should contain more than "[]"
                    {
                        string[] action_list = actions.Split(',');

                        foreach (string action in action_list)
                        {
                            if (action.Length > 0)
                            {
                                string action_name = action.Replace("[", "");
                                action_name = action_name.Replace("]", "");
                                action_name = action_name.Replace("'", "");
                                action_name = action_name.Replace(" ", "");
                                this.GetComponent<MessageHandler_noJson>().ShowMoveInstruction(
                                action_name,
                                knowledge_level,
                                default_time,
                                annotation
                                );
                            }
                        }
                    }
                }
                else if (action_type == "storeItem")
                {
                    Debug.Log("STORE");
                    rx = new Regex(@"item_name<(.*?)>");
                    string item_name = rx.Match(message).Groups[1].Value;

                    this.GetComponent<MessageHandler_noJson>().StoreObject(
                    item_name,
                    knowledge_level,
                    default_time
                    );
                }
                else if (action_type == "returnTool")
                {
                    rx = new Regex(@"item_name<(.*?)>");
                    string item_name = rx.Match(message).Groups[1].Value;

                    rx = new Regex(@"color<(.*?)>");
                    string color = rx.Match(message).Groups[1].Value;

                    this.GetComponent<MessageHandler_noJson>().ReturnTool(
                    item_name,
                    color,
                    knowledge_level,
                    default_time
                    );
                }
                else
                {
                    Debug.Log("Unknown user action: " + action_type);
                }
            }
        }
        else if (message.Contains("order_finished"))
        {
            this.GetComponent<MessageHandler_noJson>().FinishJob();
        }
        else if (message.Contains("points"))
        {
            Regex rx = new Regex(@"new_points<(.*?)>");
            string new_points = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"total_points<(.*?)>");
            string total_points = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"time_performance<(.*?)>");
            string time_performance = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"quality_performance<(.*?)>");
            string quality_performance = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"total_level<(.*?)>");
            string total_level = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"node_finished<(.*?)>");
            string node_finished = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"recipe_finished<(.*?)>");
            string recipe_finished = rx.Match(message).Groups[1].Value;

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

            rx = new Regex(@"quartile<(.*?)>");
            string quartile = rx.Match(message).Groups[1].Value;


            this.GetComponent<MessageHandler_noJson>().ParsePerformanceMessage(
                new_points: Convert.ToInt32(new_points),
                total_points: Convert.ToInt32(total_points),
                time_performance: float.Parse(time_performance, CultureInfo.InvariantCulture.NumberFormat),
                quality_performance: float.Parse(quality_performance, CultureInfo.InvariantCulture.NumberFormat),
                total_level: Convert.ToInt32(total_level),
                node_finished: node_finished,
                recipe_finished: recipe_finished,
                level_up: level_up,
                perfect_run: perfect_run,
                message_text: message_text,
                message_color_r: Convert.ToInt32(message_color_r),
                message_color_g: Convert.ToInt32(message_color_g),
                message_color_b: Convert.ToInt32(message_color_b),
                quartile: Convert.ToInt32(quartile));
        }
        else if (message.Contains("camera"))
        {
            Regex rx = new Regex(@"camera_x<(.*?)>");
            string camera_x = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"camera_y<(.*?)>");
            string camera_y = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"camera_z<(.*?)>");
            string camera_z = rx.Match(message).Groups[1].Value;

            rx = new Regex(@"camera_orthographic<(.*?)>");
            string camera_orthographic = rx.Match(message).Groups[1].Value;

            this.GetComponent<MessageHandler_noJson>().InitializeCamera(
                x: float.Parse(camera_x),
                y: float.Parse(camera_y),
                z: float.Parse(camera_z),
                ortho: float.Parse(camera_orthographic));
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
