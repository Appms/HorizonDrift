using UnityEngine;
using System.Collections;
using System;

public class Pilot : MonoBehaviour {

	public float speed = 4000000f;
	public float boost = 1.5f;
	public float cornering = 150f;
	public float tiltForce = 1000000f;
	public float tiltCooldown = 0.25f;
	public float powerSlamForce = 1500000f;
	float powerSlamStopClock = 0.5f;

	float respawnTime = 3f;
	float respawnElapsedTime = 0f;

	float input_accel, input_turn, input_vertical;
	bool input_hypermode;
	bool input_tiltL, input_tiltR;
	bool tiltLPressed, tiltRPressed;
	float lastTiltL, lastTiltR;
	bool powerSlam = false;
	float powerSlamTime;
	Vector3 slamStop;
	float x1, x2, y1, y2, z1, z2;
	float rotationZ = 0f;
	
	public float blackmagic = 6f;
	float delayMagicRegen = 0f;
	RectTransform energyBar;

	Ray ray;
	RaycastHit poly;

	Rigidbody playerRigidbody;
	Transform playerTransform;
	Transform modelTransform;
	HoverPoints hoverScript;

	Transform wingRight;
	Transform wingLeft;
	Transform wingRightOpen;
	Transform wingLeftOpen;
	Transform wingRightClosed;
	Transform wingLeftClosed;
	private Vector3 velocityL = Vector3.zero;
	private Vector3 velocityR = Vector3.zero;

	private float velocityRotation = 0f;
	private float velocityGiroR = 0f;
	private float velocityGiroL = 0f;

	GameObject[] waypointsArray;
	public int currentWaypointI = 0;
	public GameObject currentWaypoint;
	public int currentLap = 1;

	bool right = true;

	AudioSource[] collisionSounds;

	void Awake(){
		playerRigidbody = GetComponent<Rigidbody> ();
		playerTransform = GetComponent<Transform> ();
		modelTransform = playerTransform.FindChild("Model");
		hoverScript = GetComponent<HoverPoints> ();

		wingRightClosed = modelTransform.FindChild("WingRightClosed");
		wingLeftClosed = modelTransform.FindChild("WingLeftClosed");

		wingRight = wingRightClosed.FindChild ("WingRight").transform;
		wingLeft = wingLeftClosed.FindChild ("WingLeft").transform;
		wingRightOpen = wingRightClosed.FindChild ("WingOpen").transform;
		wingLeftOpen = wingLeftClosed.FindChild ("WingOpen").transform;

		waypointsArray = GameObject.FindGameObjectsWithTag("Waypoint");
		Array.Sort (waypointsArray, CompareNames);
		currentWaypoint = waypointsArray[currentWaypointI];

		energyBar = GameObject.Find ("EnergyValue").GetComponent<RectTransform>();

		collisionSounds = GetComponents<AudioSource>();
	}

	int CompareNames (GameObject x, GameObject y)
	{
		return x.name.CompareTo (y.name);
	}

	void Update() {
		float localZ = modelTransform.localEulerAngles.z;

		float hyper = input_hypermode && blackmagic > 0f ? 1f : 0f;

		wingRight.position = Vector3.SmoothDamp (wingRight.position, (1f-hyper) * wingRightClosed.position + hyper * wingRightOpen.position,
		                                         ref velocityR, 0.1f, 1000f, Time.deltaTime);
		wingLeft.position = Vector3.SmoothDamp (wingLeft.position, (1f-hyper) * wingLeftClosed.position + hyper * wingLeftOpen.position,
		                                         ref velocityL, 0.1f, 1000f, Time.deltaTime);

		if (!tiltLPressed && !tiltRPressed) {
			rotationZ = Mathf.SmoothDamp(0f, 180f -localZ, ref velocityRotation, 0.25f, 1000f, Time.deltaTime);
			//rotationZ = Mathf.Lerp (0f, 180f - localZ , 10f * Time.deltaTime);
			modelTransform.RotateAround(modelTransform.position, modelTransform.forward, rotationZ);
		}
		else{
			if (tiltLPressed){
				rotationZ = Mathf.SmoothDamp(0f, 205f - localZ, ref velocityRotation, 0.25f, 1000f, Time.deltaTime);
				//rotationZ = Mathf.Lerp (0f, 205f - localZ , 10f * Time.deltaTime);
				modelTransform.RotateAround(modelTransform.position, modelTransform.forward, rotationZ);
			}
			if (tiltRPressed){
				rotationZ = Mathf.SmoothDamp(0f, 155f - localZ, ref velocityRotation, 0.25f, 1000f, Time.deltaTime);
				//rotationZ = Mathf.Lerp (0f, 155f - localZ , 10f * Time.deltaTime);
				modelTransform.RotateAround(modelTransform.position, modelTransform.forward, rotationZ);
			}
		}
		
		if(hoverScript.UseGravity){
			respawnElapsedTime += Time.deltaTime;
			if(respawnElapsedTime >= respawnTime){
				Respawn();
			}
		} else{
			respawnElapsedTime = 0f;
		}

		energyBar.sizeDelta = new Vector2(blackmagic * 300f / 6f, energyBar.sizeDelta.y);
	}

	void FixedUpdate () {
		// GATHERING
		input_accel = Input.GetAxis ("Trigger");
		input_turn = Input.GetAxis ("Horizontal");
		input_vertical = Input.GetAxis ("Vertical");
		input_hypermode = Input.GetButton ("Boost");
		input_tiltL = Input.GetButton ("TiltL");
		input_tiltR = Input.GetButton ("TiltR");

		float useGravity = hoverScript.UseGravity ? 4f : 1f;

		// ACCELERATE
		if (input_hypermode && blackmagic > 0f){
			playerRigidbody.AddForce (playerTransform.forward * Time.deltaTime * input_accel * speed * boost / useGravity);
			blackmagic -= Time.deltaTime;
			delayMagicRegen = 1f;
		}
		else {
			playerRigidbody.AddForce (playerTransform.forward * Time.deltaTime * input_accel * speed / useGravity);
			if (delayMagicRegen <= 0f){
				if (blackmagic < 6f)
					blackmagic += Time.deltaTime * 6f / 40f;
			} else {
				delayMagicRegen -= Time.deltaTime;
			}
		}

		if (blackmagic < 0f)
			blackmagic = 0f;

		
		// ROTATE
		if (hoverScript.UseGravity)
			playerTransform.Rotate (Time.deltaTime * cornering * input_vertical, Time.deltaTime * cornering * input_turn, 0, Space.Self);
		else	
			playerTransform.Rotate (0f, Time.deltaTime * cornering * input_turn, 0, Space.Self);
		
		
		// TILT
		if (input_tiltR) {
			playerRigidbody.AddRelativeForce (Time.deltaTime * tiltForce / useGravity, 0, 0);
			if (!tiltRPressed) {
				if (Time.time - lastTiltR < tiltCooldown && !powerSlam) {
					playerRigidbody.AddRelativeForce (powerSlamForce / useGravity, 0, 0);
					powerSlam = true;
					powerSlamTime = 0f;
				} else {
					lastTiltR = Time.time;
				}
				tiltRPressed = true;
			}
		} 
		else 
			tiltRPressed = false;
		
		if (input_tiltL){
			playerRigidbody.AddRelativeForce (Time.deltaTime * -tiltForce / useGravity, 0, 0);
			if(!tiltLPressed){
				if (Time.time - lastTiltL < tiltCooldown && !powerSlam) {
					playerRigidbody.AddRelativeForce (-powerSlamForce / useGravity, 0, 0);
					powerSlam = true;
					powerSlamTime = 0f;
				}
				else{
					lastTiltL = Time.time;
				}
				tiltLPressed = true;
			}
		}
		else
			tiltLPressed = false;
		
		// POWER SLAM CONTROL
		if (powerSlam) {
			if (powerSlamTime >= powerSlamStopClock){
				powerSlam = false;
				playerRigidbody.velocity = playerRigidbody.velocity - (Vector3.Dot(playerRigidbody.velocity, this.transform.right) * this.transform.right);
			}
			else{
				powerSlamTime += Time.deltaTime;
			}
		}
	}




	// CHECKPOINTS
	void OnTriggerEnter(Collider other){
		if (other.gameObject.name == currentWaypoint.name){
			onReachWaypoint();
		}
		else if (currentWaypointI == 14){
			if (other.gameObject.name == waypointsArray[15].name){
				currentWaypointI++;
				onReachWaypoint();
			}
		}
	}

	void OnCollisionEnter(Collision collision){
		//if (other.GetComponentInParent<GameObject>().CompareTag ("Barrier") || other.GetComponentInParent<GameObject>().CompareTag ("Opponent")) {
		if(collision.collider.CompareTag ("Barrier") || collision.collider.CompareTag ("Opponent")){
			collisionSounds[Mathf.FloorToInt(UnityEngine.Random.value * collisionSounds.Length * 0.9f)].Play();
		}
	}
	
	private void onReachWaypoint(){
		//GameObject.Find("RaceManager").GetComponent<GameEnd>().enabled = true;
		if (currentWaypointI >= 14 && currentWaypointI <= 34)
			currentWaypointI++;
		currentWaypointI++;
		if (currentWaypointI == waypointsArray.Length){
			NewLap();
			currentWaypointI = 0;
		}
		currentWaypoint = waypointsArray[currentWaypointI];
	}
	
	private void NewLap(){
		currentLap++;
		if (currentLap == 3)
			GameObject.Find("RaceManager").GetComponent<GameEnd>().enabled = true;
	}

	private void Respawn(){
		playerRigidbody.velocity = Vector3.zero;
		playerTransform.rotation = currentWaypoint.transform.rotation; 
		playerTransform.position = currentWaypoint.transform.position - currentWaypoint.transform.forward*2f;
		//PlaySound
	}
}
