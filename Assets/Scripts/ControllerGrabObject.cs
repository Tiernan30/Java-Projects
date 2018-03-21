using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour {

    protected SteamVR_TrackedObject trackedObj;
    protected GameObject collidingObject;
    protected GameObject objectInHand;
    public tracker_guide target;
    public Transform trackerTransform;
    public bool iGrabbed;
    public Generate_Terrain map;

    Vector2 grabtimes;

    protected SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Start()
    {
        target = map.posTracker.GetComponent<tracker_guide>();
        trackerTransform = map.posTracker.transform;
        grabtimes = new Vector2();
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

        if (FunctionController.enableTokenMove)
        {
            if (objectInHand.GetComponent<tracker_guide>() != null && !iGrabbed)
            {
                target = objectInHand.GetComponent<tracker_guide>();
                grabtimes.x = Time.time;
                target.isGrabbed = true;
                iGrabbed = true;
                
            }
        }
        else
        {
            ReleaseObject();
        }
       /* 
        if(objectInHand.GetComponent<basicToken>() != null)
        {
            objectInHand.GetComponent<basicToken>().isGrabbed = true;
        }
        */
         
    }

    // 3
    protected FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }
    public void ReleaseObject()
    {
        objectInHand = null;
        if (target.isGrabbed)
        {
            grabtimes.y = Time.time;
            if (Logger.grabs != null)
            {
                //Debug.Log("Grab: " + grabtimes);
                Logger.grabs.Add(grabtimes);
            }
            target.isGrabbed = false;
        }
        iGrabbed = false;
    }
    // Update is called once per frame
    public void Update () {
        //Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip)
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            if (collidingObject)
            {
                GrabObject();
            }
        }

        // Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip)
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }

        if (target.isGrabbed && iGrabbed)
            trackerTransform.position = this.transform.position + this.transform.forward.normalized * 0.05f;
    }
}
