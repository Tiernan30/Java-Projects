using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDConeDrag : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;

    public GameObject preLaserPrefab;
    private GameObject preLaser;
    private Transform preLaserTransform;

    public GameObject teleportReticlePrefab;
    private GameObject reticle;
    private Transform teleportReticleTransform;
    public Vector3 teleportReticleOffset;

    private Vector3 hitPoint;
    private RaycastHit hit;
    private Vector3 grabForward;
    private Vector3 grabLoc;
    private Transform grabTrans;
    private Vector3 grabPath;
    private Vector3 feetPoint;
    private Vector3 grabRot;
    private int grabLength;
    private float cone_length;
    private float lastMovementRatio;
    private float lastRotAngle;
    private Transform localPlane;
    private GameObject planeObject;
    //public Terrain Terr;
    //public Generate_Terrain map;
    OneEuroFilter angleFilter;
    OneEuroFilter stepFilter;
    float filterFrequency = 60f;

    

    public Transform cameraRigTransform;
    public Transform headTransform;

    private bool triggerDown;

    //private Vector2 teleTimeType;

    void Start()
    {
        laser = Instantiate(laserPrefab);
        laser.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        laserTransform = laser.transform;

        preLaser = Instantiate(preLaserPrefab);
        preLaserTransform = preLaser.transform;

        reticle = Instantiate(teleportReticlePrefab);
        teleportReticleTransform = reticle.transform;

        planeObject = new GameObject();
        localPlane = planeObject.transform;

        grabLength = 800;
        cone_length = 15;
        lastMovementRatio = 0;

        reticle = Instantiate(teleportReticlePrefab);
        teleportReticleTransform = reticle.transform;
        stepFilter = new OneEuroFilter(filterFrequency);
        angleFilter = new OneEuroFilter(filterFrequency);
        //teleTimeType = new Vector2();
    }

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
    private void ShowLaser(Vector3 hitPoint, float distance)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, distance);
        reticle.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        teleportReticleTransform.position = hitPoint;
    }

    private void ShowPreLaser(Vector3 hitPoint, float distance)
    {
        preLaser.SetActive(true);
        preLaserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        preLaserTransform.LookAt(hitPoint);
        preLaserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, distance);
        reticle.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        teleportReticleTransform.position = hitPoint;
    }

    // Update is called once per frame


    void Update()
    {
        if (Controller.GetHairTriggerUp())
        {
            triggerDown = false;
            laser.SetActive(false);
            //reticle.SetActive(false);

            stepFilter = new OneEuroFilter(filterFrequency);
            angleFilter = new OneEuroFilter(filterFrequency);
            lastMovementRatio = 0;
            lastRotAngle = 0;
            return;
        }
        if (Controller.GetHairTriggerDown() && !triggerDown)
        {

            // 2
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, grabLength))
            {
                preLaser.SetActive(false);
                triggerDown = true;

                grabForward = trackedObj.transform.forward;
                grabLoc = new Vector3(headTransform.position.x, cameraRigTransform.position.y, headTransform.position.z);
                localPlane.rotation = trackedObj.transform.rotation;

                hitPoint = hit.point;

                grabPath = hitPoint - grabLoc;
            }
            else
            {
                preLaser.SetActive(false);
                triggerDown = true;

                grabForward = trackedObj.transform.forward;
                grabLoc = new Vector3(headTransform.position.x, cameraRigTransform.position.y, headTransform.position.z);
                localPlane.rotation = trackedObj.transform.rotation;
       
                hitPoint = grabLoc + trackedObj.transform.forward * grabLength;
                grabPath = hitPoint - grabLoc;
            }
        }
        if (triggerDown)
        {

            //Part 1: Movement along cone curved side
         
            //Get the angle between the original grab location and down
            float baseAngle = Vector3.Angle(grabForward, Vector3.down);
            //Get the angle between the current controller location and down
            Vector3 contForward = trackedObj.transform.forward;
            float contAngle = Vector3.Angle(contForward, Vector3.down);     
              
            //Find the difference
            float difAngle = (baseAngle - contAngle) ;

            float movementRatio = 0;
            //Prevent divide by 0 case. If we would be dividing by 0 assume no movement has occured from before
            //Determine how much of the way the new angle has moved towards up vs. the starting location
            if(baseAngle != 0)
                movementRatio = (difAngle / baseAngle);

            if(movementRatio > 0)
                movementRatio = smoothstep(0.0f, 0.9f, movementRatio);
            else
                movementRatio = smoothstep(-0.7f, 0.0f, movementRatio) - 1;

            movementRatio = stepFilter.Filter(movementRatio);

            //If its moved more then 10% of the way in one frame slow it down to 10%
            float posRatio = movementRatio - lastMovementRatio;   
            if (posRatio > 0.1)
            {
                lastMovementRatio += 0.1f;
                posRatio = 0.1f;

            }
            else if (posRatio < -0.1)
            {
                lastMovementRatio -= 0.1f;
                posRatio = -0.1f;             
            }
            else 
                lastMovementRatio = movementRatio;

            float transDist;

            //Determine how far the transform should move
            if (grabPath.magnitude > cone_length)
                transDist = posRatio * grabPath.magnitude;
            else
                transDist = posRatio * cone_length;

            //Direct that distance in the correct direction
            Vector3 movementPath = grabPath.normalized * transDist;
            
            //Determine the new location of the camera rig as moved by that path 
            Vector3 triPath = cameraRigTransform.position + movementPath;

 

            //Determine how much the controller has been rotated from its starting position

            //Find the vectors from the forward of the controller and the vector between the controller and the target point
            Vector3 pathToTarget =  (hitPoint - trackedObj.transform.position ).normalized ;
            contForward = trackedObj.transform.forward.normalized;

            //2D Math
            Vector2 cont2DForward = new Vector2(contForward.x, contForward.z);
            Vector2 pathToTarget2D = new Vector2(pathToTarget.x, pathToTarget.z);


            float forAngle =Mathf.Acos(cont2DForward.x / cont2DForward.magnitude);
            float pathAngle = Mathf.Acos(pathToTarget2D.x / pathToTarget2D.magnitude);

            
            //Effectively account for unsigned negative angles
            if (cont2DForward.y < 0)
                forAngle = (2 * (Mathf.PI)) - forAngle;
            if(pathToTarget2D.y < 0)
                pathAngle = (2 * (Mathf.PI)) - pathAngle;
              

            //Debug.Log("Forward Angle: " + forAngle + "Path Angle: " + pathAngle);
              
            float difYAngle = (Mathf.Rad2Deg * forAngle) - (Mathf.Rad2Deg * pathAngle);

            /*
            if (difYAngle > 10)
                difYAngle = 10;
            else if (difYAngle < -10)
                difYAngle = -10;
            */


            localPlane.transform.position = triPath;
            localPlane.RotateAround(hitPoint, Vector3.up, -difYAngle);
           
            //Return the orginal local rotation
            cameraRigTransform.position = localPlane.position;

           
            //Show a laser between controller and grab point
            float distance = Vector3.Distance(trackedObj.transform.position, hitPoint);
            ShowLaser(hitPoint, distance);


        }
        else // 3
        {
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, grabLength))
            {
                ShowPreLaser(hit.point, hit.distance);
            }
            else
            {
                hitPoint = trackedObj.transform.position + trackedObj.transform.forward * grabLength;
                ShowPreLaser(hitPoint, grabLength);
            }
        }
        
    }
    //Using Smoothest step by Kyle McDonald
    float smoothstep(float edge0, float edge1, float x)
    {
        // Scale, bias and saturate x to 0..1 range
        x = clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
        // Evaluate polynomial
        return (-20 * Mathf.Pow(x, 7)) + (70 * Mathf.Pow(x, 6)) - (84 * Mathf.Pow(x, 5)) + (35 * Mathf.Pow(x, 4));
        //return x * x * (3 - 2 * x);
    }

    float clamp(float x, float lowerlimit, float upperlimit)
    {
        if (x < lowerlimit)
            x = lowerlimit;
        if (x > upperlimit)
            x = upperlimit;
        return x;
    }
}
