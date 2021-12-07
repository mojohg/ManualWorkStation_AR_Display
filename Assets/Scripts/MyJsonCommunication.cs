using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// The class CommunicationClass defines the json data type used for communication with the IoT Adapter. It includes all information
/// regarding picking and placing of objects as well as the returning of tools.
/// </summary>

[System.Serializable]
public class CommunicationClass
{
    public string action_type = "";  // pickItem, pickTool, placeItem, useTool, useItemTool, turnAssembly, toolReturn, finishOrder
    public string user_name = "";
    public int user_level = 0;
    public string resource = "AR_HAP";
    public PickData pick = new PickData();
    public PlaceData place = new PlaceData();
    public ReturnData returns = new ReturnData();
    public int default_time = 0;
}

[System.Serializable]
public class PickData
{
    public ItemData pick_box = new ItemData();
    public ItemData pick_tool = new ItemData();
    public int knowledge_level = 0;
}

[System.Serializable]
public class PlaceData
{
    // public string picture_ar = "";        //selection of support data for work
    //public string version = "";         //definition of product version for verification process
    public string item_name = "";         //definition of product name to check location
    public string action_name = "";         //definition of product name to check location
    public bool finished = false;       //information from client that production process is finished
    public int knowledge_level = 0;      //support level [1,4]
}

[System.Serializable]
public class ReturnData
{
    public ItemData return_tool = new ItemData();
}

[System.Serializable]
public class ItemData
{
    public string name = "";               // Part_A
    public int level = 0;               //[1,4]
    public int number = 0;               //box range [1,8]
    public string led_color = "";         //green, yellow, red, reset
}

//[System.Serializable]
//public class StorageInformation
//{
//    public BoxProperties box = new BoxProperties();
//    public ToolProperties toolholder = new ToolProperties();
//}



// *********************************** Messages from the Server ***********************************************************

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

public class ObjectInstruction
{
    public string item_name = "";
    public string color = "";
    public int knowledge_level = 1;
    public int default_time = 0;
}

public class ToolInstruction
{
    public string tool_name = "";
    public string color = "";
    public int knowledge_level = 1;
    public int default_time = 0;
}

[System.Serializable]
public class PerformanceProperties
{
    public int total_points = 0;
    public float performance = 0f;
    public int total_level = 0;
    public bool node_finished = false;
    public bool level_up = false;
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
