using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class ArcLaserPointer : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;
    public GameObject laserPrefab; 
    private List<GameObject> laser;
    private Transform[] laserTransform;
    private Vector3 hitPoint;
    public Terrain Terr;
    public Generate_Terrain map;

    public Transform cameraRigTransform;
    public GameObject teleportReticlePrefab;
    private GameObject reticle;
    private Transform teleportReticleTransform;
    public Transform headTransform;
    public Vector3 teleportReticleOffset;
    public LayerMask teleportMask;

    private bool shouldTeleport;
    private bool triggerDown;

    private Vector2 velocity; //constant x/y acceleration values
    private int x;
    private int y;
    private int z;
    private Vector2 teleTimeType;
    private Vector3[] points;

    OneEuroFilter<Vector3> posFilter;
    OneEuroFilter angleFilter;
    float filterFrequency = 60f;

        //TODO Add reticle to intersection, increase diameter of laser

    //instantiate a list of about 100 laserPrefabs to use in show.
    void Start()
    {
        laser = new List<GameObject>();
        //current unity version does not allow getting an element at index i from list?
       
       // GameObject[] allLasers;
         //works instead of(Instantiate(laserPrefab));
        for (int i = 0; i < 100; i++)
        {
            GameObject newlaser = Instantiate(laserPrefab);
            newlaser.transform.parent = this.transform;
            newlaser.name = "arcPiece" + i;
            laser.Add(newlaser);
        }
        reticle = Instantiate(teleportReticlePrefab);
        teleportReticleTransform = reticle.transform;
        teleTimeType = new Vector2();
        posFilter = new OneEuroFilter<Vector3>(filterFrequency);
        angleFilter = new OneEuroFilter(filterFrequency);
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
    private void Teleport()
    {
        // 1
        shouldTeleport = false;
        // 2
        reticle.SetActive(false);
        // 3
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        // 4
        difference.y = 0;
        // 5
        cameraRigTransform.position = hitPoint + difference;
        if (Logger.teleports != null)
        {
            teleTimeType.x = Time.time;
            Logger.teleports.Add(teleTimeType);

        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        

        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger) || triggerDown)
        {
            triggerDown = true;
            RaycastHit hit;
            
            //values to tweak for better range of arc/precision.
            velocity.x = 3.5f;
            velocity.y = 1.5f;


            points = arcPoints(velocity.x, velocity.y);

            int hitnum;


           
            hit = ArcedRaycast(points, out hitnum);
            //change paraments
            //ShowArcedLaser(points, hitPoint, hitnum, hit);
           if (hit.collider != null) 
            {
               // UnityEngine.Debug.Log("hit.collider is not null");
                hitPoint = hit.point;
                ShowArcedLaser(points, hitPoint, hitnum, hit);

                // End of additional input in this function
                //don't touch
                reticle.SetActive(true);
                if (hit.collider.gameObject.name == "miniMap")
                {
                  //  UnityEngine.Debug.Log("hit collides with miniMap");
                    hitPoint = hit.collider.gameObject.transform.InverseTransformPoint(hitPoint);
                    hitPoint = (hitPoint * map.map_width);
                    hitPoint.x = hitPoint.x - (Generate_Terrain.tile_width);
                    hitPoint.z = hitPoint.z - (Generate_Terrain.tile_height);

                    RaycastHit findHit;
                    Physics.Raycast(new Vector3(hitPoint.x, 3000, hitPoint.z), Vector3.down, out findHit);
                    hitPoint.y = findHit.point.y;
                    teleTimeType.y = 1;
                }
                else if (hit.collider.gameObject.GetComponent<monoMiniMap>() != null)
                {
                    monoMiniMap miniMap = hit.collider.gameObject.GetComponent<monoMiniMap>();
                    hitPoint = hit.collider.gameObject.transform.InverseTransformPoint(hitPoint);
                    hitPoint = (hitPoint * miniMap.mainMap.terrainData.size.x);

                    hitPoint.y = miniMap.mainMap.terrainData.GetHeight((int)hitPoint.x, (int)hitPoint.z);
                    teleTimeType.y = 1;
                }
                else
                {
                    teleTimeType.y = 0;
                }
                //Debug.Log(hitPoint.x + " " + hitPoint.y + " " + hitPoint.z);
               // teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                shouldTeleport = true;

            }
           // UnityEngine.Debug.Log("hit.collider is null");
        }
        else // 3
        {
            for (int i = 0; i < laser.Count; i++)
            {
                laser[i].SetActive(false);
            }
            reticle.SetActive(false);
        }
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) && shouldTeleport)
        {
            triggerDown = false;
            Teleport();
        }
    }


    Vector3 pointValues(float velocityX, float velocityY, float t, float g, float theta)
    {
        Vector3 forward = posFilter.Filter<Vector3>(transform.forward);
        
        Vector3 vector = new Vector3();
        
        vector.y = transform.position.y + (velocity.y * t * Mathf.Sin(theta) - (0.5f * g * (t*t)));

        //2*T to increase the length of the arc, made it a bit unstable at close proximity, change value
        //Longer the length becomes, the less precise the end point.
        vector.x = (transform.position.x + (forward.x * velocity.x)*1.5f*t);
        vector.z = (transform.position.z + (forward.z * velocity.x)*1.5f*t);

        //UnityEngine.Debug.Log("Vector in pointValues = " + vector);
        return vector;
    }
    //qaulitative feedback, comparison total time and preference, writeup (implementation) go full low lvl details
    Vector3[] arcPoints(float velocityX, float velocityY)
    {
        Vector3 forward = posFilter.Filter<Vector3>(transform.forward);
        
        float theta = ( Vector3.Angle(forward, Vector3.down) - 90);
        float rad = theta * Mathf.Deg2Rad;//radian conversion to stabilize occilation.
        

        theta = angleFilter.Filter(rad);
        float g = 0.06f; //change this for stronger arc
        Vector3 initial = new Vector3 {};
        initial.x = 0;
        initial.y = 0;
        initial.z = 0;
        Vector3[] P = new Vector3[100];
        for (int i = 0; i < 100; i++)
        {
            P[i] = initial;
        }
        
       
        for (int j = 0; j < 100; j++)
        {
            Vector3 pointVector = pointValues(velocityX, velocityY, j, g, theta);
            P[j] = pointVector;

           
            // UnityEngine.Debug.Log("arcPoints loop j = " + j +" arcPoints P[j] = " + P[j]);
        }
       
        
        return P;
    }
    //can't be a copy of showLaser, change functionality to go through the list
    //check generate coins to see
    //pi - pi+1 for the position and look at for showarcedlaser
    //intersection reticle, resize cylinders further away
    private void ShowArcedLaser(Vector3[] p, Vector3 hitPoint, int hitnumber, RaycastHit hit)
    {
       int pSize= p.Length - 1;
        
        for(int i = 0; i < hitnumber; i++ )
        {
            laser[i].SetActive(true); 
        }

        for(int i = hitnumber; i < pSize; i++)
        {
            laser[i].SetActive(false);
        }
        
        if (hitnumber != -1)
        {
            //UnityEngine.Debug.Log("Hitnumber != -1");


            for (int i = 0; i < hitnumber-1; i++)
            {
               // UnityEngine.Debug.Log("Getting into show loop.");
               float hitLength = (p[i] - p[i + 1]).magnitude;
                
              
                    laser[i].transform.position = Vector3.Lerp(p[i], p[i + 1], .5f);
                    
                    laser[i].transform.localScale = new Vector3(laser[i].transform.localScale.x, laser[i].transform.localScale.y,
                       hitLength );
                
            }
            for (int i = 0; i < hitnumber - 1; i++)
            {
                laser[i].transform.LookAt(p[i+1]);
            }

            laser[hitnumber-1].transform.position = Vector3.Lerp(p[hitnumber-1], hitPoint, .5f);
            laser[hitnumber-1].transform.LookAt(hitPoint);
            laser[hitnumber-1].transform.localScale = new Vector3(laser[hitnumber-1].transform.localScale.x, laser[hitnumber-1].transform.localScale.y,
                hit.distance);
            reticle.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            teleportReticleTransform.position = hitPoint;
            
        }

    }

    //insert the if statements in update for raycasts, but for each element in the list?
    //try defining a default out for hitnumber as -1, ideally if no return it returns -1.
    //call in place of physics.raycast, should work.
    //for visualization maybe change null to hit w/ no collision, then check for that collision object later
    //since if null object the reticle could be shown as an X.
    private RaycastHit ArcedRaycast(Vector3[] p, out int hitnumber)
    {
        Vector3 path;
        RaycastHit hit = new RaycastHit();
        
        for (int i = 1; i < p.Length; i++)
        {
            path = p[i - 1] - p[i];
            
            if (Physics.Raycast(p[i], path, out hit, path.magnitude))
            {
                //UnityEngine.Debug.Log("Hit = " + hit);

                hitnumber = i;
                //UnityEngine.Debug.Log("hit is not default value(null)");

                return hit;
            }

           
        }
        hitnumber = -1;
       // UnityEngine.Debug.Log("hit is default value(null)");
        return hit;
    }
}

