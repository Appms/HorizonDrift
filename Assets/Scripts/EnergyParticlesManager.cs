using UnityEngine;
using System.Collections;

public class EnergyParticlesManager : MonoBehaviour {

	private int qualityLevel;

	// Use this for initialization
	void Start () {
		qualityLevel = QualitySettings.GetQualityLevel ();
	}
	
	// Update is called once per frame
	void Update () {
		if (qualityLevel <= 3) {
			gameObject.SetActive(false);
		}
	}
}
