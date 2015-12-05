using UnityEngine;
using System.Collections;

public class Spinner : MonoBehaviour {

	private Transform spinner;
	private AudioSource sound;

	// Use this for initialization
	void Start () {
		spinner = GetComponent<Transform>();
		sound = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		spinner.Rotate(new Vector3(0f,0f,10f * GetComponentInParent<Rigidbody>().velocity.magnitude));
		sound.volume = 0f + GetComponentInParent<Rigidbody> ().velocity.magnitude / 100f;
		sound.pitch = 1f + GetComponentInParent<Rigidbody> ().velocity.magnitude / 100f;
	}
}
