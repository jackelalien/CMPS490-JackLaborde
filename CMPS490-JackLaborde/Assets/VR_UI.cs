using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VR_UI : MonoBehaviour {

	bool isActive;
	public GameObject eyeCam;
	public bool isLeftController;
	public Canvas menu;
	public List<GameObject> enemyList;


	private SteamVR_TrackedObject tracked;

	private bool thermalOn = false;

	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input ((int)tracked.index); }
	}

	void Awake()
	{
		tracked = GetComponent<SteamVR_TrackedObject> ();
			menu.enabled = false;

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
			if (Controller.GetPress (Valve.VR.EVRButtonId.k_EButton_ApplicationMenu) && isLeftController) {
				if (menu.enabled)
					menu.enabled = false;
				else
					menu.enabled = true;
			}




	}

	void OnTriggerStay(Collider other)
	{
		if (other.GetComponent<Button> ()) {
			var pointer = new PointerEventData (EventSystem.current);
			Button b = other.GetComponent<Button> ();
			ExecuteEvents.Execute (b.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
		}
		if (Controller.GetHairTriggerDown () && !isLeftController) {
			if(other.GetComponent<Button>())
			{
				var pointer = new PointerEventData (EventSystem.current);
				Button b = other.GetComponent<Button> ();
				ExecuteEvents.Execute (b.gameObject, pointer, ExecuteEvents.submitHandler);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<Button> ()) {
			var pointer = new PointerEventData (EventSystem.current);
			Button b = other.GetComponent<Button> ();
			ExecuteEvents.Execute (b.gameObject, pointer, ExecuteEvents.pointerExitHandler);
		}
	}


	//MENU INTERACTIONS
	//GET DIFFERENT BUTTONS
	public void ActivateThermalVision()
	{
		if (eyeCam.GetComponent<CameraEffect> ().m_Enable) {
			eyeCam.GetComponent<CameraEffect> ().m_Enable = false;
		} else {
			eyeCam.GetComponent<CameraEffect> ().m_Enable = true;
		}
	}

	void SpawnCombatStick()
	{
		Debug.Log ("Should spawn");
	}

	public void ActivateEnemies()
	{
		foreach (GameObject enemy in enemyList) {
			enemy.GetComponent<EnemyAI> ().enabled = true;
		}
	}
}
