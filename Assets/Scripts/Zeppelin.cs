using UnityEngine;
using System.Collections;

public class Zeppelin : MonoBehaviour {


	void Update () {
		transform.RotateAround(GameObject.Find("Track").transform.position, Vector3.up, 2f * Time.deltaTime);
	}
}
