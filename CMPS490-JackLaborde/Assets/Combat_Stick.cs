using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Combat_Stick : MonoBehaviour {

    VRTK_InteractableObject obj;
    private Vector3 target, orig1, orig2, orig3, orig4;
    public GameObject CS1, CS2, CS3, CS4;
    float speed = 10;
    bool isReady;

	// Use this for initialization
	void Start () {
        obj = GetComponent<VRTK_InteractableObject>();
        target = new Vector3(0, 0, 0);
        orig1 = CS1.transform.position;
        orig2 = CS2.transform.position;
        orig3 = CS3.transform.position;
        orig4 = CS4.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
		if(obj.IsGrabbed() && !isReady)
        {
            float step = speed * Time.deltaTime;
            CS1.transform.position = Vector3.MoveTowards(CS1.transform.position, target, step);
            CS2.transform.position = Vector3.MoveTowards(CS2.transform.position, target, step);
            CS3.transform.position = Vector3.MoveTowards(CS3.transform.position, target, step);
            CS4.transform.position = Vector3.MoveTowards(CS4.transform.position, target, step);
            isReady = true;
        }
        else if(!obj.IsGrabbed() && isReady)
        {
            float step = speed * Time.deltaTime;
            CS1.transform.position = Vector3.MoveTowards(CS1.transform.position, orig1, step);
            CS2.transform.position = Vector3.MoveTowards(CS2.transform.position, orig2, step);
            CS3.transform.position = Vector3.MoveTowards(CS3.transform.position, orig3, step);
            CS4.transform.position = Vector3.MoveTowards(CS4.transform.position, orig4, step);
            isReady = false;
        }
	}
}
