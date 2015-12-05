using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float speed = 0.001f;
	public float gravity = -9.8f;
	public float range = 100f;

	Vector3 movement;
	public Transform back, front;
	float rotation, trigger ;
	bool tiltR, tiltL, boost;
	Rigidbody playerRigidbody;
	Ray ray;
	RaycastHit poly;
	Vector3 dir, dirB, dirF;

	float lastTiltR, lastTiltL;
	bool tiltRPressed, tiltLPressed = false;

	void Awake()
	{
		playerRigidbody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		trigger = Input.GetAxis("Trigger") / 5;
		tiltR = Input.GetButton("TiltR");
		tiltL = Input.GetButton("TiltL");
		boost = Input.GetButton("Boost");

		rotation += h*200;

		ray.origin = back.position;
		ray.direction = -back.up;
		
		Physics.Raycast (ray, out poly, range);
		
		dirB = poly.normal;

		ray.origin = front.position;
		ray.direction = -front.up;
		
		Physics.Raycast (ray, out poly, range);
		
		dirF = poly.normal;

		dir = Vector3.Normalize (dirB + dirF);

		/*ray.origin = transform.position;
		ray.direction = -transform.up;

		Physics.Raycast (ray, out poly, range);

		dir = poly.normal;*/

		//playerRigidbody.velocity = Vector3.Lerp(playerRigidbody.velocity, playerRigidbody.velocity + gravity*dirF, Time.deltaTime * 4);

		transform.up = Vector3.Lerp(transform.up, poly.normal, Time.deltaTime * 4);

		Move (h, v);
	}

	void Move (float h, float v)
	{
		transform.RotateAround (transform.position, transform.up, Time.deltaTime * rotation);

		movement.Set (0f, 0f, v);

		if(boost)
			playerRigidbody.AddRelativeForce (0, 0, Time.deltaTime * trigger * 5 * 50000);
		else {
			playerRigidbody.AddRelativeForce (0, 0, Time.deltaTime * trigger * 50000);
		}

		if (tiltR){
			playerRigidbody.AddRelativeForce (Time.deltaTime * 1000, 0, 0);
			if(!tiltRPressed){
				if (Time.time - lastTiltR < 0.25) {
					playerRigidbody.AddRelativeForce (1500, 0, 0);
				}
				else{
					lastTiltR = Time.time;
				}
				tiltRPressed = true;
			}
		}
		else
			tiltRPressed = false;

		if (tiltL){
			playerRigidbody.AddRelativeForce (Time.deltaTime * -1000, 0, 0);
			if(!tiltLPressed){
				if (Time.time - lastTiltL < 0.25) {
					playerRigidbody.AddRelativeForce (-1500, 0, 0);
				}
				else{
					lastTiltL = Time.time;
				}
				tiltLPressed = true;
			}
		}
		else
			tiltLPressed = false;

	}
	
}
