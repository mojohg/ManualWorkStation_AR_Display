using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Confetti : MonoBehaviour {

	public GameObject confetti_text;  // Text is optional
	public bool keep_text = false;
	private ParticleSystem particles;

	// Use this for initialization
	void Start () 
	{
		if(confetti_text != null)
        {
			confetti_text.SetActive(false);
		}
		particles = this.GetComponent<ParticleSystem>();
		var main = particles.main;
		main.stopAction = ParticleSystemStopAction.Callback;
	}

	public void ShowConfetti()
    {
		Debug.Log("Confetti");
		if(particles == null)
        {
			Debug.Log("Particles not found");
			particles = this.GetComponent<ParticleSystem>();
		}
		if (confetti_text != null)
		{
			confetti_text.SetActive(true);
		}
		particles.Stop();
		particles.Play();
		this.GetComponent<AudioSource>().Play();
	}

	public void OnParticleSystemStopped()
	{
		if (confetti_text != null && keep_text == false)
		{
			confetti_text.SetActive(false);
		}
	}
}
