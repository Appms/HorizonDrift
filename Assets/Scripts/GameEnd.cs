using UnityEngine;
using System.Collections;

public class GameEnd : MonoBehaviour {

	float EndCameraTimer = 5f;
	Vector3 EndCameraOffset;
	Transform EndCameraTarget;

	GameObject GameCamera;
	GameObject EndCamera;

	AudioListener GameCameraAudio;
	AudioListener EndCameraAudio;

	GameObject player;
	GameObject[] opponents;

	AudioSource[] music;

	// Use this for initialization
	void Start () {

		GameObject.Find ("RaceManager").GetComponent<PositionManager>().countTime = false;
		GameObject.Find ("UI").GetComponent<UIManager> ().finalPosition = GameObject.Find ("RaceManager").GetComponent<PositionManager> ().position; 

		player = GameObject.Find ("Player");
		Pilot playerPilot = player.GetComponent<Pilot>();
		IA playerIA = player.GetComponent<IA>();
		playerIA.enabled = true;
		playerIA.currentWaypointI = playerPilot.currentWaypointI;
		playerIA.currentWaypoint = playerPilot.currentWaypoint;
		playerPilot.enabled = false;

		Debug.Log("Your final Position is: " + this.GetComponentInParent<PositionManager>().position);

		GameCamera = GameObject.Find ("Camera");
		EndCamera = GameObject.Find ("CameraEnd");

		GameCameraAudio = GameCamera.GetComponent<AudioListener> ();
		EndCameraAudio = EndCamera.GetComponent<AudioListener> ();

		GameCamera.SetActive (false);

		EndCamera.SetActive (true);
		EndCamera.GetComponent<Camera> ().enabled = true;

		EndCameraTarget = player.transform;
		EndCameraOffset = new Vector3(UnityEngine.Random.value + 1f, 1f, UnityEngine.Random.value + 1f);

		music = GetComponents<AudioSource> ();

		opponents = GameObject.FindGameObjectsWithTag("Opponent");
	}
	
	// Update is called once per frame
	void Update () {
		GameCamera.SetActive (false);
		GameCameraAudio.enabled = false;
		EndCamera.SetActive (true);
		EndCameraAudio.enabled = true;
		//music [1].volume -= Time.deltaTime;
		//music [2].enabled = true;
		if(EndCameraTimer > 0f){
			EndCameraTimer -= Time.deltaTime;
		} else{
			EndCameraTimer = 5f;
			if(UnityEngine.Random.value < 0.4f){
				EndCameraTarget = player.transform;
			} else{
				EndCameraTarget = opponents[(int)(UnityEngine.Random.value * (opponents.Length - 1))].transform;
			}
			EndCameraOffset = new Vector3(UnityEngine.Random.value * 4f - 2f, 1f, UnityEngine.Random.value + 1f);
		}

		EndCamera.transform.position = EndCameraTarget.position + 
										EndCameraTarget.up * EndCameraOffset.y +
										EndCameraTarget.right * EndCameraOffset.x + 
										EndCameraTarget.forward * EndCameraOffset.z;

		EndCamera.transform.LookAt(EndCameraTarget.position);

		if (Input.GetButton ("Start")) {
			Application.LoadLevel(Application.loadedLevel);
		}

		else if (Input.GetButton ("Escape")) {
			Application.Quit();
		}
	}
}
