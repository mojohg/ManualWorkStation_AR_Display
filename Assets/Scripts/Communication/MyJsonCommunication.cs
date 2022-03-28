using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;


/// <summary>
/// The class CommunicationClass defines the json data type used for communication with the IoT Adapter. It includes all information
/// regarding picking and placing of objects as well as the returning of tools.
/// </summary>

[System.Serializable]
public class BoxProperties
{
    public string storage_name = "";
    public string item_name = "";
}


[System.Serializable]
public class ToolProperties
{
    public string storage_name = "";
    public string tool_name = "";
    public string cad_property = "";
}

[System.Serializable]
public class OrderProperties
{
    public string version = "";
    public int number_steps = 0;
    public int number_points = 0;
}

public class UserInstruction
{
    public string action_type = "";
    public string item_name = "";  // for pick operations
    public List<string> item_list = new List<string>();  // for place operations
    public List<string> action_list = new List<string>();  // for place operations
    public string color = "";
    public int knowledge_level = 1;
    public int default_time = 0;
}

[System.Serializable]
public class PerformanceProperties
{
    public int total_points = 0;
    public float time_performance = 0f;
    public float quality_performance = 0f;
    public int total_level = 0;
    public string node_finished = "False";
    public string level_up = "False";
    public string perfect_run = "False";
    public string message_text = "";
    public MessageColor message_color = new MessageColor();
}

[System.Serializable]
public class MessageColor
{
    public float r = 0f;
    public float g = 0f;
    public float b = 0f;
}
