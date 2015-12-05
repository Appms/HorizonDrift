using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class PositionManager : MonoBehaviour {

	GameObject[] opponents;
	GameObject player;
	Pilot playerPilot;

	public int position = 16;

	public float time = 0f;
	public bool countTime;


	// Use this for initialization
	void Start () {
		opponents = GameObject.FindGameObjectsWithTag("Opponent");
		player  = GameObject.Find ("Player");
		playerPilot = player.GetComponent<Pilot>();
		countTime = false;
		time = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		opponents = opponents.OrderBy(x => x.GetComponent<IA>().currentLap * (-100000f) + x.GetComponent<IA>().currentWaypointI * (-1000f) + Vector3.Distance(x.GetComponent<IA>().currentWaypoint.transform.position, x.transform.position)).ToArray ();
		position = opponents.Length + 1;
		for (int i = 0; i < opponents.Length; i++){
			if(playerPilot.currentLap * (-100000f) + playerPilot.currentWaypointI * (-1000f) + Vector3.Distance(playerPilot.currentWaypoint.transform.position, player.transform.position) <
			   opponents[i].GetComponent<IA>().currentLap * (-100000f) + opponents[i].GetComponent<IA>().currentWaypointI * (-1000f) + Vector3.Distance(opponents[i].GetComponent<IA>().currentWaypoint.transform.position, opponents[i].transform.position)){
				position = i + 1;
				break;
			}
		}
		if (countTime)
			time += Time.deltaTime;
	}
}
