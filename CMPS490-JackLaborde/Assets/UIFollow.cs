using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollow : MonoBehaviour {

	public Canvas ui;
	public GameObject controller;


	// Use this for initialization
	void Start () {
		ui.transform.position = controller.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		ui.transform.position = controller.transform.position;
	}
}
