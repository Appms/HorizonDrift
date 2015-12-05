using UnityEngine;

public class HoverPoints : MonoBehaviour
{
	public float LinearAcc = 90000f;
	public float AngularAcc = 600f;
	public float PositionOffset = 0.5f;
	public float range = 100f;

	public Transform back, front, right, left;

	private bool useGravity;

	Ray ray;
	RaycastHit poly;
	Vector3 normalP;
	float distanceP;

	Rigidbody playerRigidbody;
	Transform playerTransform;

	private LayerMask trackMask;

	void Awake()
	{
		playerRigidbody = GetComponent<Rigidbody> ();
		playerTransform = GetComponent<Transform> ();
		trackMask = LayerMask.GetMask ("Track");

	}

	void FixedUpdate()
	{
		bool okFront, okBack, okLeft, okRight;

		// FRONT
		ray.origin = front.position;
		ray.direction = -front.up;
		okFront = Physics.Raycast (ray, out poly, range, trackMask);
		normalP = poly.normal.normalized;
		distanceP = poly.distance;

		if (!useGravity) {
			Attract (normalP, distanceP);
			Rotate (normalP, distanceP, false, 1); // Bool -> Horizontal?
		}


		// BACK
		ray.origin = back.position;
		ray.direction = -back.up;
		okBack = Physics.Raycast (ray, out poly, range, trackMask);
		normalP = poly.normal.normalized;
		distanceP = poly.distance;

		if (!useGravity) {
			Attract (normalP, distanceP);
			Rotate (normalP, distanceP, false, -1); // Bool -> Horizontal?
		}


		// RIGHT
		ray.origin = right.position;
		ray.direction = -right.up;
		okRight = Physics.Raycast (ray, out poly, range, trackMask);
		normalP = poly.normal.normalized;
		distanceP = poly.distance;

		if (!useGravity) {
			Attract (normalP, distanceP);
			Rotate (normalP, distanceP, true, -1); // Bool -> Horizontal?
		}


		// LEFT
		ray.origin = left.position;
		ray.direction = -left.up;
		okLeft = Physics.Raycast (ray, out poly, range, trackMask);
		normalP = poly.normal.normalized;
		distanceP = poly.distance;

		if (!useGravity) {
			Attract (normalP, distanceP);
			Rotate (normalP, distanceP, true, 1); // Bool -> Horizontal?
		}


		if (!okFront && !okBack && !okLeft && !okRight) {
			useGravity = GetComponent<Rigidbody> ().useGravity = true;
		} else {
			useGravity = GetComponent<Rigidbody> ().useGravity = false;
		}
	}

	public bool UseGravity {
		get {
			return useGravity;
		}
	}

	void Attract(Vector3 normal, float distance){
		//playerRigidbody.AddForce (-normal * Mathf.Log(distance * 5 + 2 / 10) * LinearAcc);
		playerRigidbody.AddForce (-normal * (distance - PositionOffset) * LinearAcc);
	}

	void Rotate(Vector3 normal, float distance, bool horizontal, float side){
		if(horizontal)
			//playerRigidbody.AddTorque (playerTransform.forward * Mathf.Log(distance * 5 + 2 / 10) * side * AngularAcc);
			playerRigidbody.AddTorque (playerTransform.forward * (distance - PositionOffset) * side * AngularAcc);
		else
			//playerRigidbody.AddTorque (playerTransform.right * Mathf.Log(distance * 5 + 2 / 10) * side * AngularAcc);
			playerRigidbody.AddTorque (playerTransform.right * (distance - PositionOffset) * side * AngularAcc);

	}
}
