using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour {

	public float lerpSpeed = 1f;

	private Transform transform;
	private Transform player;
	private Rigidbody playerRigidbody;
	private Camera camera;
	private Vector3 velocity = Vector3.zero;
	private float velocityF = 0f;
	private float velocityF2 = 0f;
	private float velocityF3 = 0f;
	private float velocityF4 = 0f;
	private UnityStandardAssets.ImageEffects.Bloom bloom;
	private UnityStandardAssets.ImageEffects.CameraMotionBlur motionBlur;
	private UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration aberration;

	private float trigger, boostf;
	private bool boost;

	Ray ray;
	RaycastHit poly;
	float range = 10000000000f;
	bool cameraRayFound;
	LayerMask track;

	private Pilot playerPilotScript;
	private GameStart race;

	// Use this for initialization
	void Start () {
		transform = GetComponent<Transform>();
		player = GameObject.Find ("CameraPositions").GetComponent<Transform>();
		playerPilotScript = GameObject.Find ("Player").GetComponent<Pilot> ();
		track = LayerMask.GetMask("Track");
		playerRigidbody = player.GetComponentInParent<Rigidbody>();
		camera = GetComponent<Camera>();
		bloom = GetComponent<UnityStandardAssets.ImageEffects.Bloom> ();
		motionBlur = GetComponent<UnityStandardAssets.ImageEffects.CameraMotionBlur> ();
		aberration = GetComponent<UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration> ();

		race = GameObject.Find ("RaceManager").GetComponent<GameStart>();
	}
	
	// Update is called once per frame
	void Update () {
		trigger = Input.GetAxis("Trigger");
		boost = Input.GetButton("Boost");

		boostf = boost && playerPilotScript.blackmagic > 0f && !race.enabled ? 1f : 0f;
		trigger = trigger > 0f && playerPilotScript.blackmagic > 0f && !race.enabled ? 1f : 0f;

		transform.position = Vector3.SmoothDamp (transform.position, player.position, ref velocity, 0.001f, 100000f, Time.deltaTime);
		transform.rotation = player.rotation;

		camera.fieldOfView = Mathf.SmoothDamp (camera.fieldOfView, 60f + boostf * 20f + trigger * 20f, ref velocityF, 1f, 1000f, Time.deltaTime);
		bloom.bloomThreshold = Mathf.SmoothDamp (bloom.bloomThreshold, 0.8f - boostf * 0.5f, ref velocityF2, 1f, 1000f, Time.deltaTime);
		motionBlur.maxVelocity = Mathf.SmoothDamp (motionBlur.maxVelocity, 4f + boostf * 6f, ref velocityF3, 1f, 1000f, Time.deltaTime);
		aberration.chromaticAberration = Mathf.SmoothDamp (aberration.chromaticAberration, 0.2f + boostf * 20f, ref velocityF4, 1f, 1000f, Time.deltaTime);

	}
	private Vector3 MultiplyV3(Vector3 a, Vector3 b){
		return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}
}
