using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    public GameObject laserPrefab;
    private GameObject laser;
    private Transform laserTransform;
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

    private Vector2 teleTimeType;

    void Start()
    {
        laser = Instantiate(laserPrefab);
        laserTransform = laser.transform;

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
    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }
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
        if(Logger.teleports != null)
        {
            teleTimeType.x = Time.time;
            Logger.teleports.Add(teleTimeType);

        }

    }
    // Update is called once per frame
    void Update () {
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Trigger) || triggerDown)
        {
            triggerDown = true;
            RaycastHit hit;

            // 2
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 1000))
            {
                hitPoint = hit.point;
                ShowLaser(hit);

                reticle.SetActive(true);
                if (hit.collider.gameObject.name == "miniMap")
                {
                    
                    hitPoint = hit.collider.gameObject.transform.InverseTransformPoint(hitPoint);
                    hitPoint = (hitPoint * map.map_width);
                    hitPoint.x = hitPoint.x - (Generate_Terrain.tile_width);
                    hitPoint.z = hitPoint.z  - (Generate_Terrain.tile_height);

                    RaycastHit findHit;
                    Physics.Raycast(new Vector3(hitPoint.x, 3000, hitPoint.z), Vector3.down, out findHit);
                    hitPoint.y = findHit.point.y;
                    teleTimeType.y = 1;
                }
                else if(hit.collider.gameObject.GetComponent<monoMiniMap>() != null)
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
        if (Controller.GetPressUp (SteamVR_Controller.ButtonMask.Trigger) && shouldTeleport)
        {
            triggerDown = false;
            Teleport();
        }
    }
}
