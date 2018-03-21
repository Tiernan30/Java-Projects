using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beaconScript : MonoBehaviour {

    private GameObject lController;
    private GameObject rController;
   

	// Use this for initialization
	void Start () {
        lController = GameObject.Find("Controller (left)");
        rController = GameObject.Find("Controller (right)");
        Physics.IgnoreCollision(this.GetComponent<Collider>(), lController.GetComponent<Collider>());
        Physics.IgnoreCollision(this.GetComponent<Collider>(), rController.GetComponent<Collider>());

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
