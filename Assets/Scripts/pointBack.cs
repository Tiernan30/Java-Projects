using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointBack : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;
    public GameObject laserPrefab;
    public GameObject pointerPrefab;
    public GameObject pointer;
    //private Transform pointerTransform;
    private GameObject laser;
    private Transform laserTransform;
    public Transform cameraRigTransform;
    private bool isActive = true;
    public coinBank bank;
    private bool fullRun;


    float downTime;
    float heldTime;

    // Use this for initialization
    void Start()
    {
        laser = Instantiate(laserPrefab);
        laser.GetComponent<MeshRenderer>().material.color = Color.blue;
        laserTransform = laser.transform;

        pointer = Instantiate(pointerPrefab);
        //pointer.GetComponent<MeshRenderer>().material.color = Color.blue;
        pointer.transform.parent = this.gameObject.transform;
        pointer.transform.localPosition = new Vector3(0,0,1);
        //pointerTransform = pointer.transform;
        pointer.SetActive(false);
        downTime = Time.time;
        fullRun = false;

    }

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    /*
    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);

    }
    */
    // Update is called once per frame
    void Update()
    {

        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            isActive = !isActive;
        }

        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            downTime = Time.time;

        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            downTime = Time.time;
            foreach(Transform b in pointer.transform)
            {
                b.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }

        if (isActive && Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            pointer.SetActive(true);
            heldTime = Time.time - downTime;
            
           
                Vector3 truePath;
                if (bank.active_coin > 1)
                    truePath = bank.tokens[bank.active_coin - 2].transform.position - cameraRigTransform.position;
                else
                    truePath = new Vector3(0, 0, 0) - cameraRigTransform.position;

                Vector3 pointerPath = this.transform.forward;
                //Vector3 beamPath = hitPoint - cameraRigTransform.position;

                truePath.Normalize();
                //beamPath.Normalize();
                pointerPath.Normalize();

                float angle = Vector3.Angle(truePath, pointerPath);
            if (heldTime >= 0.5)
            {
                pointer.transform.Find("HSphere").gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
            }
            if (heldTime >= 1)
            {
                pointer.transform.Find("MSphere").gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
            }
            if (heldTime >= 1.5)
            {
                pointer.transform.Find("TSphere").gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                if (bank.active_coin > 0)
                    bank.tokens[bank.active_coin - 1].GetComponent<basicToken>().point_back_angle = angle;
                bank.funcController.toggleMovement(true);
                //fullRun = true;
                
            }
                //Debug.Log("Angle between paths: " + angle);

            



        }
        else
        {
            laser.SetActive(false);
            pointer.SetActive(false);
           // if(fullRun)
           //     bank.funcController.togglePointBack(false);
        }
    }
}
