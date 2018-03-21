using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleStyleMovement : MonoBehaviour {
    private SteamVR_TrackedObject trackedObj;

    public showDirection indicator;

    public Transform cameraRigTransform;
    public Transform headTransform;
    private bool isActive = true;

    private float velocity = 0;
    private float maxVelocity = 2;
    private float minVelocity = 0.1f;
    private float acceleration = 0.01f;

    private Vector2 touchs;

    // Use this for initialization
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    // Use this for initialization
    void Start () {
        touchs = new Vector2();
	}

    void OnDisable()
    {
        indicator.changeTex('l');
    }

    void Update()
    {
        if (isActive)
        {
            if (Controller.GetAxis() != Vector2.zero)
            {
                if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
                    touchs.x = Time.time;
                if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
                {
                    touchs.y = Time.time;
                    if(Logger.flyTouchs != null)
                     Logger.flyTouchs.Add(touchs);
                }
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.gameObject.GetComponent<ControllerGrabObject>().iGrabbed)
            indicator.gameObject.SetActive(false);
        else
        {
            indicator.gameObject.SetActive(true);
        }

        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
            isActive = !isActive;

        if (isActive) {
           
            if (Controller.GetAxis() != Vector2.zero)
            {
                float angle = Mathf.Atan2(Controller.GetAxis().y, Controller.GetAxis().x) * 180 / Mathf.PI;

                

                if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
                {

                    if (velocity < minVelocity)
                        velocity = minVelocity;

                    velocity += acceleration;

                    if (velocity > maxVelocity)
                        velocity = maxVelocity;

                    if (angle > 0)
                    {
                        cameraRigTransform.Translate(this.transform.forward.normalized * velocity);
                    }
                    else if (angle < 0)
                    {
                        cameraRigTransform.Translate(-this.transform.forward.normalized * velocity);
                    }
                }
                else
                {
                    velocity = 0;
                    if (angle > 0)
                    {
                        indicator.changeTex('f');
                    }
                    else if (angle < 0)
                    {
                        indicator.changeTex('b');
                    }
                }

            }
            else
                indicator.changeTex('m');

        }
    }
}
