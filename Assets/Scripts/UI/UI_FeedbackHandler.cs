using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FeedbackHandler : MonoBehaviour 
{
    public Sprite level_1_sprite;
    public Sprite level_2_sprite;
    public Sprite level_3_sprite;
    public List<GameObject> ui_elements;
    public List<GameObject> uncompleted_steps;
    private float max_number_points;

    // Prefabs
    private GameObject prefab_popup;
    private GameObject prefab_levelup;
    private GameObject prefab_bar;

    // Elements in scene
    private GameObject popup_parent;
    private GameObject progressBar;
    private GameObject point_display;
    private GameObject levelup;
    private GameObject training_finished;



    void Awake()
    {
		foreach (Transform element in this.transform)
        {
            if (element.name != "LevelUpText" && element.name != "TrainingFinishedText")
            {
                ui_elements.Add(element.gameObject);
            }
        }
    }

	void Start () 
	{
        prefab_bar = (GameObject)Resources.Load("Prefabs/UI/bar", typeof(GameObject));
        prefab_popup = Resources.Load("Prefabs/General/Popup", typeof(GameObject)) as GameObject;
        prefab_levelup = Resources.Load("Prefabs/General/Levelup", typeof(GameObject)) as GameObject;
        point_display = FindUiElement("PointDisplay", ui_elements);
        progressBar = FindUiElement("StepDisplay", ui_elements);
        popup_parent = GameObject.Find("PopupParent");
        levelup = GameObject.Find("LevelUp");
        training_finished = GameObject.Find("TrainingFinished");
    }

    public void SetMaxPoints(int max_points)
    {
        max_number_points = max_points;
        point_display.transform.Find("Maximum").GetComponent<Text>().text = "/ " + max_points.ToString();
    }

    public void ShowPoints(int current_points)
    {
        point_display.transform.Find("Current").GetComponent<Text>().text = current_points.ToString();        
        float ratio = current_points / max_number_points;

        if (ratio > 0.8f)
        {
            point_display.transform.Find("Current").GetComponent<Text>().color = Color.green;
        }
        else if (ratio > 0.4f)
        {
            point_display.transform.Find("Current").GetComponent<Text>().color = Color.cyan;
        }
        else
        {
            point_display.transform.Find("Current").GetComponent<Text>().color = Color.yellow;
        }
    }

    public void ShowLevel(int level)
    {
        GameObject status = FindUiElement("Status", ui_elements);
        if (level == 0)
        {
            status.transform.Find("LevelName").GetComponent<Text>().text = "Introduction";
            status.GetComponent<Image>().sprite = level_1_sprite;
        }
        if (level == 1)
        {
            status.transform.Find("LevelName").GetComponent<Text>().text = "Beginner";
            status.GetComponent<Image>().sprite = level_1_sprite;
        }
        else if ( level == 2)
        {
            status.transform.Find("LevelName").GetComponent<Text>().text = "Advanced";
            status.GetComponent<Image>().sprite = level_2_sprite;
        }
        else if (level == 3)
        {            
            status.transform.Find("LevelName").GetComponent<Text>().text = "Expert";
            status.GetComponent<Image>().sprite = level_3_sprite;
        }
        else if (level == 4)
        {
            status.transform.Find("LevelName").GetComponent<Text>().text = "Expert";
            status.GetComponent<Image>().sprite = level_3_sprite;
        }
    }

	public void ShowNumberSteps(int number)
    {
        GameObject new_bar;

        for (int i = 0; i < number; i++)
        {
            new_bar = Instantiate(prefab_bar, progressBar.transform);
            new_bar.name = "Bar_" + i;
            uncompleted_steps.Add(new_bar);
        }
    }

    public void FinishStep()
    {
        uncompleted_steps[0].GetComponent<Image>().color = Color.green;
        uncompleted_steps.RemoveAt(0);
    }

    public void ShowPerformance(float performance, float performance_lower_limit, float performance_upper_limit)
    {
        GameObject slider = FindUiElement("Performance", ui_elements);
        if(slider == null)
        {
            return;
        }
        slider.GetComponent<Slider>().value = performance;
        if (performance > performance_upper_limit)
        {
            slider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.green;
        }
        else if (performance > performance_lower_limit)
        {
            slider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            slider.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.red;
        }
    }

    public GameObject FindUiElement(string name, List<GameObject> gameobject_list)
    {
        foreach (GameObject obj in gameobject_list)
        {
            if (obj.name == name)
            {
                return obj;
            }
        }
        Debug.LogWarning("Gameobject " + name + " not found");
        return null;
    }

    public void DisplayPopup(string message, float color_r, float color_g, float color_b)
    {
        GameObject go = Instantiate(prefab_popup, popup_parent.transform.position, Quaternion.identity, popup_parent.transform);
        Color col = new Color(color_r, color_g, color_b);
        go.GetComponent<UI_Popup>().Setup(message, col);
    }

    public void DisplayLevelup()
    {
        levelup.GetComponent<UI_Confetti>().ShowConfetti();
    }

    public void DisplayTrainingFinished()
    {
        training_finished.GetComponent<UI_Confetti>().ShowConfetti();
        
        GameObject.Find("Assemblies").SetActive(false);
        this.DisableFeedbackElements();
    }

    private void DisableFeedbackElements()
    {
        foreach (GameObject element in ui_elements)
        {
            element.SetActive(false);
        }
    }
}
