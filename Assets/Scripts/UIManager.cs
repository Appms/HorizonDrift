using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

	private RawImage boostOverlay;
	private RawImage dotPositions;
	private bool boost;
	private float boostF;
	private float velocityF = 0f;
	private Rigidbody player;
	private Text[] uiElements;
	private Pilot pilot;

	private Canvas[] UIs;
	private GameObject raceManager;

	public int finalPosition = 1;
	private GameStart gameStart;

	private Image[] introImages;	
	// Use this for initialization
	void Start () {
		boostOverlay = GameObject.Find("BoostOverlay").GetComponent<RawImage>();
		//dotPositions = GameObject.Find("Positions").GetComponent<RawImage>();
		player = GameObject.Find ("Player").GetComponent<Rigidbody> ();
		uiElements = GetComponentsInChildren<Text> ();
		pilot = GameObject.Find ("Player").GetComponent<Pilot> ();

		raceManager = GameObject.Find ("RaceManager");
		gameStart = raceManager.GetComponent<GameStart> ();
		UIs = GetComponentsInChildren<Canvas> ();

		introImages = UIs [1].GetComponentsInChildren<Image> ();
	}
	
	// Update is called once per frame
	void Update () {
		boost = Input.GetButton("Boost");

		boostF = boost && pilot.blackmagic > 0f && !raceManager.GetComponent<GameStart>().enabled ? 1f : 0f;

		Color alpha = new Color(boostOverlay.color.r, boostOverlay.color.g, boostOverlay.color.b, Mathf.SmoothDamp (boostOverlay.color.a, 0f + 0.3f * boostF, ref velocityF, 1f, 1000f, Time.deltaTime));

		boostOverlay.color = alpha;

		Rect uvRect = new Rect (boostOverlay.uvRect);

		uvRect.x += 1f * Time.deltaTime;
		uvRect.y += 0.7f * Time.deltaTime;

		//boostOverlay.uvRect = uvRect;

		//dotPositions.texture = new Texture ();

		uiElements[4].text = Mathf.RoundToInt(player.velocity.magnitude * 10f).ToString();
		uiElements[5].text = Mathf.RoundToInt(player.velocity.magnitude * 10f).ToString();

		uiElements [6].text = "POS: " + raceManager.GetComponent<PositionManager> ().position;
		uiElements [7].text = "POS: " + raceManager.GetComponent<PositionManager> ().position;

		uiElements [8].text = "LAP: " + pilot.currentLap.ToString ();
		uiElements [9].text = "LAP: " + pilot.currentLap.ToString ();

		uiElements[12].text = "POSITION: " + finalPosition;
		uiElements[13].text = "POSITION: " + finalPosition;

		uiElements[14].text = "TIME: " + AdaptTime(raceManager.GetComponent<PositionManager> ().time);
		uiElements[15].text = "TIME: " + AdaptTime(raceManager.GetComponent<PositionManager> ().time);

		uiElements [10].text = gameStart.TimerMain <= 3 && gameStart.TimerMain > 0 ? ((int)gameStart.TimerMain + 1).ToString() : "";
		uiElements [11].text = gameStart.TimerMain <= 3 && gameStart.TimerMain > 0 ? ((int)gameStart.TimerMain + 1).ToString() : "";

		if (!raceManager.GetComponent<GameStart>().started) {
			if(raceManager.GetComponent<GameStart>().controls){
				introImages[0].enabled = false;
				introImages[1].enabled = true;
				uiElements[2].enabled = false;
				uiElements[3].enabled = false;
			}
			else{
				introImages[0].enabled = true;
				introImages[1].enabled = false;
			}
			UIs [1].enabled = true;
			UIs [2].enabled = false;
			UIs [3].enabled = false;
		}
		else if (raceManager.GetComponent<GameEnd>().enabled) {
			UIs [1].enabled = false;
			UIs [3].enabled = true;
			UIs [2].enabled = false;
		}
		else {
			UIs [1].enabled = false;
			UIs [3].enabled = false;
			UIs [2].enabled = true;
		}
	}

	private string AdaptTime(float time){
		string adaptation = "";
		int minutes = Mathf.FloorToInt(time/60f);
		int seconds = Mathf.RoundToInt(time - Mathf.FloorToInt(time/60f) * 60f);

		adaptation += minutes;
		adaptation += ":";
		if (seconds > 9)
			adaptation += seconds;
		else
			adaptation += "0" + seconds;

		return adaptation;
	}
}
