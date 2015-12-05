using UnityEngine;
using System.Collections;

public class TextureOffseter : MonoBehaviour {

	private Material mat;

	// Use this for initialization
	void Start () {
		mat = GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
		mat.SetTextureOffset ("_MainTex", mat.GetTextureOffset("_MainTex") + new Vector2 (0f, 0.01f));
		//mat.SetFloat ("_EmissionScaleUI", GetComponentInParent<Rigidbody> ().velocity.magnitude);
	}
}
