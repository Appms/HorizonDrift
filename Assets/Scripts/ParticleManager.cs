using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour {

	private ParticleSystem particles;

	private float trigger, boostf;
	private bool boost;

	private float velocityF = 0f;
	private float velocityF2 = 0f;

	private int qualityLevel;

	// Use this for initialization
	void Start () {
		particles = GetComponent<ParticleSystem> ();
		qualityLevel = QualitySettings.GetQualityLevel ();
	}
	
	// Update is called once per frame
	void Update () {
		if (qualityLevel > 3) {

			trigger = Input.GetAxis ("Trigger");
			boost = Input.GetButton ("Boost");
		
			boostf = boost ? 1f : 0f;
			trigger = trigger > 0f ? 1f : 0f;

			particles.startSpeed = Mathf.SmoothDamp (particles.startSpeed, 0f + 5f * trigger + 5f * boostf, ref velocityF, 1f, 1000f, Time.deltaTime);
			particles.startSize = Mathf.SmoothDamp (particles.startSize, 0f + 0.025f * trigger + 0.025f * boostf, ref velocityF2, 1f, 1000f, Time.deltaTime);
		} else gameObject.SetActive(false);
	}
}
