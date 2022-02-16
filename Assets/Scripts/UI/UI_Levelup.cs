using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Levelup : MonoBehaviour {

	public GameObject levelup_text;
	private ParticleSystem particles;

	// Use this for initialization
	void Start () 
	{
		levelup_text = GameObject.Find("LevelUpText").gameObject;
		levelup_text.SetActive(false);
		particles = this.GetComponent<ParticleSystem>();

		var main = particles.main;
		main.stopAction = ParticleSystemStopAction.Callback;
	}

	public void ShowLevelUp()
    {
		if(levelup_text == null)
        {
			levelup_text = this.transform.Find("Text_Levelup").gameObject;
		}
		if(particles == null)
        {
			particles = this.GetComponent<ParticleSystem>();
		}
		levelup_text.SetActive(true);
		particles.Play();
		this.GetComponent<AudioSource>().Play();
	}

	public void OnParticleSystemStopped()
	{
		levelup_text.SetActive(false);
	}
}
