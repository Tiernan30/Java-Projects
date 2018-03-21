using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionController : MonoBehaviour {

    /// <summary>
    /// A check if flight as an option is globally available. This should be setup at initialization
    /// </summary>
    public static bool flightGEnabled;
    /// <summary>
    /// A check if token movement as an option is globally available. This should be setup at initialization
    /// </summary>
    public static bool tokenMoveGEnabled;
    /// <summary>
    /// A check if teleportation as an option is globally available. This should be setup at initialization
    /// </summary>
    public static bool teleportGEnabled;
    /// <summary>
    /// A check if arc teleportation as an option is globally available. This should be setup at initialization
    /// </summary>
    public static bool arcTeleportGEnabled;
    /// <summary>
    /// Whether the minimap is globally available
    /// </summary>
    public static bool miniMapGEnabled;

    /// <summary>
    /// A reference to the object managing both map and minimap objects
    /// </summary>
    public Generate_Terrain map;

    public TextMesh pointbackLabel;

    //Whether flying is currently available
    private static bool enableFlying;
    public GoogleStyleMovement flyingRight;


    //Whether the token or token movement is currently available
    private static bool enableToken;
    public static bool enableTokenMove;
    public tracker_guide movementToken;

    //Whether things can be grabbed
    private bool enableGrab;
    public ControllerGrabObject controllerGrabLeft;
    public ControllerGrabObject controllerGrabRight;

    //Whether teleportation is currently available
    private static bool enableTeleport;
    public LaserPointer laserPointerRight;

    //Whether arc teleportation is currently available
    private static bool enableArcTeleport;
    public LaserPointer laserPointerRight2;
    //Whether the minimap is currently available
    public static bool enableMiniMap;
    public GameObject miniMap;

    //Whether the map is currently available
    public static bool enableMap;
    public GameObject mainMap;

    //Whether beacons/coins are currently available
    private static bool enableTasks;
    public coinBank taskBank;

    //Whether the ability to change lat/lon is avaialable
    private static bool enableNavigation;
    public Control_lat_long controlLatLonLeft;
    public control_zoom controlZoomRight;

    //Whether point backs are currently enabled
    private static bool enablePointBack;
    public pointBack pointBackLeft;

    public static bool moveEnabled = true;
    public bool boolMovement = false;
    public void toggleMovement(bool state)
    {
        moveEnabled = state;
        if (flightGEnabled)
            enableFlying = state;
        else
            enableFlying = false;

        if (teleportGEnabled)
            enableTeleport = state;
        else
            enableTeleport = false;
        if (arcTeleportGEnabled)
            enableArcTeleport = state;
        else
            enableArcTeleport = false;

        if (tokenMoveGEnabled)
            enableTokenMove = state;
        else
            enableTokenMove = false;

        flyingRight.enabled = enableFlying;
        //Debug.Log("Flight from movement set to: " + enableFlying.ToString());

        //controllerGrabLeft.ReleaseObject();
        controllerGrabRight.ReleaseObject();

        controllerGrabLeft.enabled = enableTokenMove;
        controllerGrabRight.enabled = enableTokenMove;

        laserPointerRight.enabled = enableTeleport;


        pointbackLabel.gameObject.SetActive(!state);
        taskBank.controlLabel.gameObject.SetActive(state);
        
    }

    public bool boolGrab = false;
    public void toggleGrab(bool state)
    {
        enableGrab = state;
        //controllerGrabLeft.enabled = state;
        //controllerGrabRight.enabled = state;

    }


    public bool boolFlying = false;
    public void toggleFlying(bool state)
    {
        enableFlying = state;
        flyingRight.enabled = state;
       // Debug.Log("Flight set to: " + state.ToString());
    }

    public bool boolTokenMove = false;
    public void toggleTokenMove(bool state)
    {
        enableTokenMove = state;

    }

    public bool boolToken = false;
    public void toggleToken(bool state)
    {
        enableToken = state;
        movementToken.enabled = state;
    }


    public bool boolTeleportation = false;
    public void toggleTeleportation(bool state)
    {
        enableTeleport = state;
        laserPointerRight.enabled = state;
    }

    public bool boolArcTeleportation = false;
    public void toggleArcTeleportation(bool state)
    {
        enableArcTeleport = state;
        laserPointerRight2.enabled = state;
    }



    public bool boolNavigation = false;
    public void toggleNavigation(bool state)
    {
        enableNavigation = state;
        if(!enableNavigation)
            controlLatLonLeft.indicator.changeTex('l');

        controlLatLonLeft.enabled = enableNavigation;
        if(!enableNavigation)
            controlZoomRight.indicator.changeTex('l');

        controlZoomRight.enabled = enableNavigation;
    }


    public bool boolPointBack = false;
    public void togglePointBack(bool state)
    {
        enablePointBack = state;
        pointBackLeft.enabled = state;
    }


    public bool boolMap = false;
    public void toggleMap(bool state)
    {

        enableMap = state;
        mainMap.SetActive(state);
    }


    public bool boolMinimap = false;
    public void toggleMiniMap(bool state)
    {
        if (miniMapGEnabled)
            enableMiniMap = state;
        else
            enableMiniMap = false;
        miniMap.SetActive(enableMiniMap);
    }

    void start()
    {
        mainMap = map.mainMap;
        miniMap = map.miniMap;
    }

    void Update()
    {
        if (boolFlying)
        {
            toggleFlying(!enableFlying);
            boolFlying = false;
        }
        if (boolGrab)
        {
            toggleGrab(!enableGrab);
            boolGrab = false;
        }
        if (boolTeleportation)
        {
            toggleTeleportation(!enableTeleport);
            boolTeleportation = false;
        }
        if (boolArcTeleportation)
        {
            toggleArcTeleportation(!enableArcTeleport);
            boolArcTeleportation = false;
        }
        if(boolMovement)
        {
            toggleMovement(!(enableTeleport || enableGrab || enableFlying || enableArcTeleport));
            boolMovement = false;
        }
        if (boolPointBack)
        {
            togglePointBack(!enablePointBack);
            boolPointBack = false;
        }
        if (boolNavigation)
        {
            toggleNavigation(!enableNavigation);
            boolNavigation = false;
        }
        if (boolToken)
        {
            toggleToken(!enableToken);
            boolToken = false;
        }
        if (boolTokenMove)
        {
            enableTokenMove = !enableTokenMove;
            boolTokenMove = false;
        }
        if (boolMinimap)
        {
            toggleMiniMap(!enableMiniMap);
            boolMinimap = false;
        }
        if (boolMap)
        {
            toggleMap(!enableMap);
            boolMap = false;
        }

    }

}
