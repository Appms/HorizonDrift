using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameStart : MonoBehaviour {

	GameObject MainCamera;
	GameObject AerialCamera;
	GameObject EndCamera;
	float TimerAerial;
	public float TimerMain;

	bool inputStart;
	public bool started = false;
	public bool controls = false;
	AudioSource[] music;

	bool stopPressing = true;

	// Use this for initialization
	void Start () {
		MainCamera = GameObject.Find("Camera");
		AerialCamera = GameObject.Find("CameraStart");
		EndCamera = GameObject.Find("CameraEnd");
		MainCamera.SetActive(false);
		AerialCamera.SetActive(true);
		EndCamera.GetComponent<Camera> ().enabled = false;
		TimerAerial = 5f;
		TimerMain = 5f;

		foreach(GameObject o in GameObject.FindGameObjectsWithTag("Opponent")){
			o.GetComponent<IA>().enabled = false;
			o.GetComponent<HoverPoints>().PositionOffset = 0f;
		}
		GameObject.Find("Player").GetComponent<Pilot>().enabled = false;
		GameObject.Find("Player").GetComponent<IA>().enabled = false;
		GameObject.Find("Player").GetComponent<HoverPoints>().PositionOffset = 0f;

		music = GetComponents<AudioSource> ();
		music [1].enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		inputStart = Input.GetButton ("Start");
		AerialCamera.transform.LookAt(GameObject.Find("Track").transform.position);
		AerialCamera.transform.RotateAround(GameObject.Find("Track").transform.position, Vector3.up, 10f * Time.deltaTime);
		if (!inputStart) {
			stopPressing = true;
		}
		else if (!controls) {
			controls = true;
			stopPressing = false;
		} else if (stopPressing) {
			controls = false;
			started = true;
		}
		if (started){ // ON YOUR MARKS!
			AerialCamera.GetComponent<AudioListener>().enabled = false;
			MainCamera.GetComponent<AudioListener>().enabled = true;
			music[0].volume -= Time.deltaTime;
			music[1].enabled = true;
			MainCamera.SetActive(true);
			AerialCamera.SetActive(false);
			if(TimerMain > 0){
				TimerMain -= Time.deltaTime;
				//if(TimerMain <= 3)Debug.Log((int)TimerMain + 1);

				foreach(GameObject o in GameObject.FindGameObjectsWithTag("Opponent")){
					o.GetComponent<HoverPoints>().PositionOffset = 0.5f / (1f + Mathf.Pow(TimerMain, 2f));
				}
				GameObject.Find("Player").GetComponent<HoverPoints>().PositionOffset = 0.5f / (1f + Mathf.Pow(TimerMain, 2f));

			} else{ // RACE START!
				GameObject.Find ("RaceManager").GetComponent<PositionManager>().countTime = true;
				music[0].enabled = false;
				foreach(GameObject o in GameObject.FindGameObjectsWithTag("Opponent")){
					o.GetComponent<IA>().enabled = true;
					o.GetComponent<HoverPoints>().PositionOffset = 0.5f;
				}
				GameObject.Find("Player").GetComponent<Pilot>().enabled = true;
				GameObject.Find("Player").GetComponent<HoverPoints>().PositionOffset = 0.5f;
				this.enabled = false;
			}
		}
	}
}
