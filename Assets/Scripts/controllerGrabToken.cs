using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controllerGrabToken : MonoBehaviour {


    protected SteamVR_TrackedObject trackedObj;
    protected GameObject collidingObject;
    protected GameObject objectInHand;
    
    protected SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Start()
    {
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    protected void SetCollidingObject(Collider col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObject = col.gameObject;
    }
    // 1
    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    // 2
    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    // 3
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }
    protected void GrabObject()
    {
        // 1
        objectInHand = collidingObject;
        collidingObject = null;

        if (objectInHand.GetComponent<basicToken>() != null)
        {
            objectInHand.GetComponent<basicToken>().isGrabbed = true;
        }

    }

    // 3
    // Update is called once per frame
    public void Update()
    {

        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            if (collidingObject)
            {
                GrabObject();
            }
        }

        // 2
    }
}

