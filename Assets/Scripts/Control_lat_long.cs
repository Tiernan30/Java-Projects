using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control_lat_long : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    //private float lat_range = 85f;
    //private float long_range = 179f;
    private collect_tiles collector;

    public Generate_Terrain map;

    public showDirection indicator;

    private bool isActive = false;
    private bool inputLock;
    private float lockTime = 1f;
    // Use this for initialization
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

    }

    void Start () {
        inputLock = false;
        //collector = map.center.collect;
        indicator.changeTex('l');
    }

    void OnDisable()
    {
        indicator.changeTex('l');
    }

    void unlockInput()
    {
        inputLock = false;
    }

    void lockInput()
    {
        inputLock = true;
        Invoke("unlockInput", lockTime);
    }

	// Update is called once per frame
	void Update () {
        if (this.gameObject.GetComponent<ControllerGrabObject>().iGrabbed)
            indicator.gameObject.SetActive(false);
        else
        {
            if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
            {
                isActive = !isActive;
            }
            indicator.gameObject.SetActive(isActive);
        }
        if (isActive)
        {
            if (!inputLock)
                if (Controller.GetAxis() != Vector2.zero)
                {

                    float angle = Mathf.Atan2(Controller.GetAxis().y, Controller.GetAxis().x) * 180 / Mathf.PI;

                    if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
                    {
                        lockInput();
                        //Debug.Log("Angle: " + angle);

                        if (angle > 45 && angle <= 135)
                        {
                            //collector.latitude += 90 / (Mathf.Pow(2, collect_tiles.zoom));

                            map.mainMap.GetComponent<mapTile>().ChangeLatLong(0,-1);
                            //Debug.Log("Up");
                        }
                        else if (angle > -135 && angle <= -45)
                        {
                            //collector.latitude -= 4 * (90 / (Mathf.Pow(2, collect_tiles.zoom)));


                            map.mainMap.GetComponent<mapTile>().ChangeLatLong(0, 1);
                            //Debug.Log("Down");
                        }
                        else if (angle > 135 && angle <= 180 || angle > -180 && angle < -135)
                        {
                            //collector.longitude -= 180 / (Mathf.Pow(2, collect_tiles.zoom));


                            map.mainMap.GetComponent<mapTile>().ChangeLatLong(-1, 0);
 

                            //Debug.Log("Left");
                        }
                        else if (angle > 0 && angle <= 45 || angle > -45 && angle < 0)
                        {
                            // collector.longitude += 4 * (180 / (Mathf.Pow(2, collect_tiles.zoom)));
                            //Debug.Log("Right");

                            map.mainMap.GetComponent<mapTile>().ChangeLatLong(1, 0);
                            
                        }
                    }
                    else
                    {
                        if (angle > 45 && angle <= 135)
                        {
                            indicator.changeTex('n');
                        }
                        else if (angle > -135 && angle <= -45)
                        {
                            indicator.changeTex('s');
                        }
                        else if (angle > 135 && angle <= 180 || angle > -180 && angle < -135)
                        {
                            indicator.changeTex('w');

                        }
                        else if (angle > 0 && angle <= 45 || angle > -45 && angle < 0)
                        {
                            indicator.changeTex('e');

                        }
                    }
                    // collector.latitude = Controller.GetAxis().y * lat_range;
                    // collector.longitude = Controller.GetAxis().x * long_range;
                }
                else
                    indicator.changeTex('d');
        }
	}
}
