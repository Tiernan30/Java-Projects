using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ArcLaserPointer : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;
    public GameObject laserPrefab; //change to list? List<GameObject> laserPrefab?
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

    //instantiate a list of about 100 laserPrefabs to use in show.
    void Start()
    {
        //list of lasers, instantiate a prefab each time laser.add(Instantiate(laserPrefab)
        for (int i = 0; i < 100; i++)
        {
            laser.Add(Instantiate(laserPrefab));
            laserTransform[i] = laser.ElementAt(i).transform;
        }
        reticle = Instantiate(teleportReticlePrefab);
        teleportReticleTransform = reticle.transform;
        teleTimeType = new Vector2();
        
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
    void Update()
    {
        

        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger) || triggerDown)
        {
            triggerDown = true;
            RaycastHit hit;

            /* Insert functionality to get the arc angle/pass that through to the show laser, and
            /pass hit point through to Teleport.
            */
            
            //these should be used in the teleport, not in the raycast?
            velocity.X = 1;
            velocity.Y = 1;


            points = arcPoints(velocity.X, velocity.Y);
            //needs own function, possibly use existing function Physics.Raycast if statement in update function in LaserPointer

            int hitnum;
            //arcedRaycast(points, out hitnum);

           
            
// End of additional input in this function

            //change paraments
            if (hit = arcedRaycast(points, out hitnum))
            {
                hitPoint = hit.point;
                showArcedLaser(points, hitPoint, hitnum);

                //don't touch
                reticle.SetActive(true);
                if (hit.collider.gameObject.name == "miniMap")
                {

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
                teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                shouldTeleport = true;

            }
        }
        else // 3
        {
            laser.SetActive(false);
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
        
        
        Vector3 vector = new Vector3();
        vector.y = Controller.position.Y + velocity.Y * 1 * Math.sin(theta) - 0.5 * g * t ^ 2;
        vector.x = Controller.position.X + Controller.position.normalized.x * velocity.X;
        vector.z = Controller.position.Z + Controller.position.normalized.z * velocity.X;

        return vector;
    }

    Vector3[] arcPoints(float velocityX, float velocityY)
    {
        float theta = (90 - Vector3.Angle(Controller.Forward, Vector3.Down));
        float g = 1;
        Vector3 initial = new Vector3 { X = 0, Y = 0, Z = 0 };
        Vector3[] P = new Vector3[100];
        for (int i = 0; i < 100; i++)
        {
            P[i] = initial;
        }
        
       
        for (int j = 0; j < 100; j++)
        {
            Vector3 pointVector = pointValues(1, 1, j, g, theta);
            P[j] = pointVector;

        }
        return P;
    }
    //can't be a copy of showLaser, change functionality to go through the list
    //check generate coins to see
    //pi - pi+1 for the position and look at for showarcedlaser
    private void showArcedLaser(Vector3[] p, Vector3 hitPoint, int hitnumber)
    {
        //replace hit.distance with hitnumber?
        laser.SetActive(true); //needs to set active the list of lasers
        if(hitnumber == null)
        {
            break;
        }
        for (int i = 0; i <= hitnumber; i++)
        {
            
            if(p[i+1] == hitPoint)
            {
                laserTransform[i].position = Vector3.Lerp(p[i], p[i + 1], .5f);
                laserTransform[i].LookAt(p[i + 1]);
                laserTransform[i].localScale = new Vector3(laserTransform[i].localScale.x, laserTransform[i].localScale.y,
                    hit.distance);
                break;
            }
            else
            {
                laserTransform[i].position = Vector3.Lerp(p[i], p[i + 1], .5f);
                laserTransform[i].LookAt(p[i + 1]);
                laserTransform[i].localScale = new Vector3(laserTransform[i].localScale.x, laserTransform[i].localScale.y,
                    hit.distance);
            }
        }

    }

    //insert the if statements in update for raycasts, but for each element in the list?
    //try defining a default out for hitnumber as -1, ideally if no return it returns -1.
    //call in place of physics.raycast, should work.
    //for visualization maybe change null to hit w/ no collision, then check for that collision object later
    //since if null object the reticle could be shown as an X.
    private RaycastHit arcedRaycast(Vector3[] p, out int hitnumber)
    {
        for (int i = 1; i < p.length; i++)
        {
            path = p[i - 1] - p[i];
            if (Physics.Raycast(p[i], path, out hit, path.length))
            {
                hitnumber = i;
                return hit;
            }

           
        }
        return null;
    }
}

