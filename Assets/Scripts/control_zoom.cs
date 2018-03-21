using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class control_zoom : MonoBehaviour {
    private SteamVR_TrackedObject trackedObj;

    public showDirection indicator;
    private bool inputLock;
    private float lockTime = 1f;
    //private collect_tiles collector;
    private bool isActive = false;

    public Generate_Terrain map;

    // Use this for initialization
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

    }

    void Start()
    {
        inputLock = false;
        indicator.changeTex('l');
        //collector = map.center.collect;
    }

    void unlockInput()
    {
        inputLock = false;
    }

    void OnDisable()
    {
        indicator.changeTex('l');
    }

    void lockInput()
    {
        inputLock = true;
        Invoke("unlockInput", lockTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.GetComponent<ControllerGrabObject>().iGrabbed)
            indicator.gameObject.SetActive(false);
        else
        {       
            indicator.gameObject.SetActive(true);
        }
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            isActive = !isActive;
            lockInput();
        }

        if (!inputLock)
            if (isActive)
            {
                if (Controller.GetAxis() != Vector2.zero)
                {
                    float angle = Mathf.Atan2(Controller.GetAxis().y, Controller.GetAxis().x) * 180 / Mathf.PI;

                    if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
                    {
                        lockInput();
                        //Debug.Log("Angle: " + angle);

                        
                        if (angle > 0)
                        {
                            map.mainMap.GetComponent<mapTile>().ChangeZoom(1);
                            //Debug.Log("Zoom In to "+ collect_tiles.zoom);
                        }
                        else if (angle < 0)
                        {

                            map.mainMap.GetComponent<mapTile>().ChangeZoom(-1);
                            //Debug.Log("Zoom Out to " + collect_tiles.zoom);
                        }
                    }
                    else
                    {
                        if (angle > 0)
                        {
                            indicator.changeTex('i');
                            //collector.watch_zoom(zoom + 1);
                            //Debug.Log("Zoom In to "+ collect_tiles.zoom);
                        }
                        else if (angle < 0)
                        {
                            indicator.changeTex('o');
                            //collector.watch_zoom(zoom - 1);
                            //Debug.Log("Zoom Out to " + collect_tiles.zoom);
                        }
                    }

                    // collector.latitude = Controller.GetAxis().y * lat_range;
                    // collector.longitude = Controller.GetAxis().x * long_range;
                }
                else
                    indicator.changeTex('z');
            }
    }
}
