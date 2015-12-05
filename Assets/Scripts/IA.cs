using UnityEngine;
using System.Collections;
using System;

public class IA : MonoBehaviour {

	public Vector3 pointToReach;

	Rigidbody rigidbody;

	float respawnTime = 3f;
	float respawnElapsedTime = 0f;
	HoverPoints hoverScript;

	public bool obstacleFoundF;
	RaycastHit doubleAltGr6;
	LayerMask obstaclesLayer;
	RaycastHit barrierHitF;
	bool barrierFoundF;
	RaycastHit barrierHitR;
	bool barrierFoundR;
	RaycastHit barrierHitL;
	bool barrierFoundL;
	RaycastHit barrierHitFR;
	public bool barrierFoundFR;
	RaycastHit barrierHitFL;
	public bool barrierFoundFL;

	float capsuleCastRadius = 1f;
	LayerMask barrierLayer;

	bool opponentFoundR;
	bool opponentFoundL;
	LayerMask opponentsLayer;
	

	public GameObject[] waypointsArray;
	public int currentWaypointI = 0;
	public GameObject currentWaypoint;
	public int currentLap = 1;




	// PILOT
	float speed = 4000000f;
	float boost = 1.5f;
	float cornering = 150f;
	float tiltForce = 1000000f;
	float tiltCooldown = 0.25f;
	float powerSlamForce = 1500000f;
	float powerSlamStopClock = 0.01f;
	
	public float input_accel, input_turn;
	public float target_angle;
	public float adapted_angle;
	public bool input_hypermode;
	public bool input_tiltL, input_tiltR;
	bool powerSlam = false;
	float powerSlamTime;

	float blackmagic = 15f;
	float delayMagicRegen = 0f;

	Transform playerTransform;
	Transform modelTransform;
	
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

	float rotationZ = 0f;

	AudioSource[] collisionSounds;

	/*public enum State
	{
		Waypoint,
		Trail,
		Boost,
		PitLane
	}

	public State state; //_state
	/*public State state{
		get{
			return _state;
		}
		set{
			ExitState(_state);
			_state = value;
			EnterState(_state);
		}
	}*/

	/*public struct Personality{
		public int focus; // go to next waypoint
		public int aggro; // chance to Slam
		public int greed; // go for boosts
		public int conservative; // go for PitLanes

		public Personality(int f, int a, int g, int c){
			this.focus = f;
			this.aggro = a;
			this.greed = g;
			this.conservative = c;
		}
	}
	private Personality personality;*/

	public struct Mind{
		public float FD;	// Forward Distance
		public float LD; 	// Lateral Distance
		public float correction_curve;	// 
		public float correction_strength;	// 
		public float barrier_sight;		// Range of sight to detect walls
		public float rotationThreshold;
		public float turbo_sight;

		public Mind(float fd, float ld, float cc, float cs, float bs, float rt, float ts){
			this.FD = fd;
			this.LD = ld;
			this.correction_curve = cc;
			this.correction_strength = cs;
			this.barrier_sight = bs + UnityEngine.Random.value * 10f - 5f;
			this.rotationThreshold = rt;
			this.turbo_sight = ts;
		}
	}
	public Mind mind;
	

	void Start () {
		barrierLayer = LayerMask.GetMask("Barrier");
		//state = State.Waypoint;

		opponentsLayer = LayerMask.GetMask("Player", "Opponent");

		obstaclesLayer = LayerMask.GetMask("Player", "Opponent", "Barrier");

		//personality = new Personality(9,2,3,4);
		mind = new Mind(5f, 5f, 2f, 2000f, 20f, 90f, 120f);
		rigidbody = this.GetComponent<Rigidbody>();
		hoverScript = GetComponent<HoverPoints> ();

		waypointsArray = GameObject.FindGameObjectsWithTag("Waypoint");
		Array.Sort (waypointsArray, CompareNames);
		currentWaypoint = waypointsArray[currentWaypointI];

		playerTransform = GetComponent<Transform> ();
		modelTransform = playerTransform.FindChild("Model");

		wingRightClosed = modelTransform.FindChild("WingRightClosed");
		wingLeftClosed = modelTransform.FindChild("WingLeftClosed");
		
		wingRight = wingRightClosed.FindChild ("WingRight").transform;
		wingLeft = wingLeftClosed.FindChild ("WingLeft").transform;
		wingRightOpen = wingRightClosed.FindChild ("WingOpen").transform;
		wingLeftOpen = wingLeftClosed.FindChild ("WingOpen").transform;

		collisionSounds = GetComponents<AudioSource>();

	}
	
	int CompareNames (GameObject x, GameObject y)
	{
		return x.name.CompareTo (y.name);
	}
	

	void Update (){
		if(hoverScript.UseGravity){
			respawnElapsedTime += Time.deltaTime;
			if(respawnElapsedTime >= respawnTime){
				Respawn();
			}
		} else{
			respawnElapsedTime = 0f;
		}
	}

	void FixedUpdate () {
		Watch();
		Reason();
		Plan();
		Adapt();
		Act();
	}

	private void Watch(){
		barrierFoundF = false;
		barrierFoundR = false;
		barrierFoundL = false;

		barrierFoundF = Physics.CapsuleCast(this.transform.position + 4f * this.transform.up,
		                                    this.transform.position - 4f * this.transform.up,
		                    				capsuleCastRadius,
		                    				this.transform.forward, 
		                    				out barrierHitF,
		                    				mind.barrier_sight,
		                    				barrierLayer);
		
		barrierFoundR = Physics.CapsuleCast(this.transform.position + 4f * this.transform.up - this.transform.right,
		                                    this.transform.position - 4f * this.transform.up - this.transform.right,
		                    				capsuleCastRadius,
		                    				this.transform.right, 
		                    				out barrierHitR,
		                                    mind.barrier_sight,
		                    				barrierLayer);
		
		barrierFoundL = Physics.CapsuleCast(this.transform.position + 4f * this.transform.up + this.transform.right,
		                                    this.transform.position - 4f * this.transform.up + this.transform.right,
						                    capsuleCastRadius,
		        				            -this.transform.right, 
		                    				out barrierHitL,
		                                    mind.barrier_sight,
		                    				barrierLayer);


		barrierFoundFR = Physics.CapsuleCast(this.transform.position + 4f * this.transform.up - this.transform.right - this.transform.forward,
		                                     this.transform.position - 4f * this.transform.up - this.transform.right - this.transform.forward,
		                                    capsuleCastRadius,
		                                    this.transform.forward + this.transform.right, 
		                                    out barrierHitFR,
		                                    mind.barrier_sight,
		                                    barrierLayer);
		
		barrierFoundFL = Physics.CapsuleCast(this.transform.position + 4f * this.transform.up + this.transform.right - this.transform.forward,
		                                     this.transform.position - 4f * this.transform.up + this.transform.right - this.transform.forward,
		                                    capsuleCastRadius,
		                                    this.transform.forward - this.transform.right, 
		                                    out barrierHitFL,
		                                    mind.barrier_sight,
		                                    barrierLayer);

		opponentFoundR = Physics.SphereCast(new Ray(this.transform.position, this.transform.right), 
		                                    capsuleCastRadius / 2f, 
		                                    1f, 
		                                    opponentsLayer);
		opponentFoundL = Physics.SphereCast(new Ray(this.transform.position, -this.transform.right), 
		                                    capsuleCastRadius / 2f, 
		                                    1f, 
		                                    opponentsLayer);

		obstacleFoundF = Physics.CapsuleCast(this.transform.position + 20f * this.transform.up - this.transform.forward,
		                                     this.transform.position - 20f * this.transform.up - this.transform.forward,
		                                    4f*capsuleCastRadius,
		                                    this.transform.forward, 
		                                    out doubleAltGr6,
		                                    mind.turbo_sight,
		                                    barrierLayer);
	}


	private void Reason(){
		//state = State.Waypoint;
		currentWaypoint = waypointsArray[currentWaypointI];
		pointToReach = currentWaypoint.transform.position;
	}

	private void Plan(){


		Vector3 projectionPoint = pointToReach - (Vector3.Dot(pointToReach - this.transform.position, this.transform.up) * this.transform.up);
		input_turn = (90f - Vector3.Angle(this.transform.right, projectionPoint - this.transform.position)) / mind.rotationThreshold;

		if (input_turn > 1f) input_turn = 1f;
		if (input_turn < -1f) input_turn = -1f;
		input_accel = 1f;



	}

	private void Adapt(){
		input_accel = 1f;
		input_hypermode = false;

		float correction_input = 0f;
		float correction_accel = 1f;
		Vector3 correctionVector = Vector3.zero;

		if(!obstacleFoundF){
			input_hypermode = true;
		}
		if(barrierFoundF){
			if(barrierHitF.distance < mind.FD)
				input_accel = Mathf.Log(barrierHitF.distance + 1f)/Mathf.Log (mind.FD + 1f);
		}
		if(barrierFoundFR){
			if(barrierHitFR.distance < mind.FD){
				correction_accel = Mathf.Log(barrierHitFR.distance + 1f)/Mathf.Log (mind.FD + 1f);
				if(correction_accel < input_accel)
					input_accel = correction_accel;
			}
			correctionVector = Vector3.Cross(this.transform.up, barrierHitFR.normal);
			correction_input -= Mathf.Pow(Vector3.Angle(this.transform.forward, correctionVector) / (mind.rotationThreshold/2f) / barrierHitFR.distance, mind.correction_curve);
		}
		if(barrierFoundFL){
			if(barrierHitFL.distance < mind.FD){
				correction_accel = Mathf.Log(barrierHitFL.distance + 1f)/Mathf.Log (mind.FD + 1f);
				if(correction_accel < input_accel)
					input_accel = correction_accel;
			}
			correctionVector = -Vector3.Cross(this.transform.up, barrierHitFL.normal);
			correction_input += Mathf.Pow(Vector3.Angle(this.transform.forward, correctionVector) / (mind.rotationThreshold/2f) / barrierHitFL.distance, mind.correction_curve);
		}


		input_turn += correction_input * mind.correction_strength;

		if (input_turn > 1f) input_turn = 1f;
		if (input_turn < -1f) input_turn = -1f;
		
		
		input_tiltR = false;
		input_tiltL = false;

		if(opponentFoundR){
			input_tiltL = true;
			input_tiltR = false;
		}
		if (opponentFoundL){
			input_tiltR = true;
			input_tiltL = false;
		}

		if(barrierFoundR){
			if(barrierHitR.distance < mind.LD){
				input_tiltL = true;
				input_tiltR = false;
			} 
		}
		if(barrierFoundL){
			if(barrierHitL.distance < mind.LD){ //&& barrierHitL.distance < barrierHitR.distance){
				input_tiltL = false;
				input_tiltR = true;
			}
		}



	}

	private void Act() {
		/*if (input_hypermode)
			rigidbody.AddForce ((transform.forward) * Time.deltaTime * input_accel * speed * boost);
		else {
			rigidbody.AddForce ((transform.forward) * Time.deltaTime * input_accel * speed);
		}*/

		float localZ = modelTransform.localEulerAngles.z;

		float hyper = input_hypermode && blackmagic > 0f ? 1f : 0f;

		wingRight.position = Vector3.SmoothDamp (wingRight.position, (1f-hyper) * wingRightClosed.position + hyper * wingRightOpen.position,
		                                         ref velocityR, 0.1f, 1000f, Time.deltaTime);
		wingLeft.position = Vector3.SmoothDamp (wingLeft.position, (1f-hyper) * wingLeftClosed.position + hyper * wingLeftOpen.position,
		                                        ref velocityL, 0.1f, 1000f, Time.deltaTime);
		
		if (!input_tiltL && !input_tiltR) {
			rotationZ = Mathf.SmoothDamp(0f, 180f -localZ, ref velocityRotation, 0.25f, 1000f, Time.deltaTime);
			//rotationZ = Mathf.Lerp (0f, 180f - localZ , 10f * Time.deltaTime);
			modelTransform.RotateAround(modelTransform.position, modelTransform.forward, rotationZ);
		}
		else{
			if (input_tiltL){
				rotationZ = Mathf.SmoothDamp(0f, 205f - localZ, ref velocityRotation, 0.25f, 1000f, Time.deltaTime);
				//rotationZ = Mathf.Lerp (0f, 205f - localZ , 10f * Time.deltaTime);
				modelTransform.RotateAround(modelTransform.position, modelTransform.forward, rotationZ);
			}
			if (input_tiltR){
				rotationZ = Mathf.SmoothDamp(0f, 155f - localZ, ref velocityRotation, 0.25f, 1000f, Time.deltaTime);
				//rotationZ = Mathf.Lerp (0f, 155f - localZ , 10f * Time.deltaTime);
				modelTransform.RotateAround(modelTransform.position, modelTransform.forward, rotationZ);
			}
		}

		float useGravity = hoverScript.UseGravity ? 4f : 1f;
		
		// ACCELERATE
		if (input_hypermode && blackmagic > 0f){
			rigidbody.AddForce (transform.forward * Time.deltaTime * input_accel * speed * boost / useGravity);
			blackmagic -= Time.deltaTime;
			delayMagicRegen = 1f;
		}
		else {
			rigidbody.AddForce (transform.forward * Time.deltaTime * input_accel * speed / useGravity);
			if (delayMagicRegen <= 0f){
				if (blackmagic < 15f)
					blackmagic += Time.deltaTime * 6f / 40f;
			} else {
				delayMagicRegen -= Time.deltaTime;
			}
		}
		
		if (blackmagic < 0f)
			blackmagic = 0f;
		
		// ROTATE
		transform.Rotate (0, Time.deltaTime * cornering * input_turn, 0, Space.Self);
		
		
		// TILT
		if (input_tiltR){
			rigidbody.AddRelativeForce (Time.deltaTime * tiltForce, 0, 0);
		}
		
		if (input_tiltL){
			rigidbody.AddRelativeForce (Time.deltaTime * -tiltForce, 0, 0);
		}
	}



	//		AUXILIAR

	private Vector3 MultiplyV3(Vector3 a, Vector3 b){
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	void OnCollisionEnter(Collision collision){
		//if (other.GetComponentInParent<GameObject>().CompareTag ("Barrier") || other.GetComponentInParent<GameObject>().CompareTag ("Opponent")) {
		if(collision.collider.CompareTag ("Barrier") || collision.collider.CompareTag ("Opponent")){
			collisionSounds[Mathf.FloorToInt(UnityEngine.Random.value * collisionSounds.Length * 0.9f)].Play();
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.name == currentWaypoint.name){
			onReachWaypoint();
		}
	} 
	
	private void onReachWaypoint(){
		if (currentWaypointI >= 14 && currentWaypointI <= 34)
			currentWaypointI++;
		if (currentWaypointI == 13)
			currentWaypointI += Mathf.RoundToInt(UnityEngine.Random.value);
		currentWaypointI++;

		if (currentWaypointI == waypointsArray.Length){
			NewLap();
			currentWaypointI = 0;
		}
		currentWaypoint = waypointsArray[currentWaypointI];
	}
	
	private void NewLap(){
		currentLap++;
	}

	private void Respawn(){
		rigidbody.velocity = Vector3.zero;
		this.transform.rotation = currentWaypoint.transform.rotation; 
		this.transform.position = currentWaypoint.transform.position - currentWaypoint.transform.forward*2f;
		respawnElapsedTime = 0f;
	}

}
