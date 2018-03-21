using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicToken : MonoBehaviour {

    public TextMesh counter;
    public static int count;
    public float offset;

    public GameObject laserPrefab;
    public GameObject laser;
    public Transform laserTransform;
    private Vector3 hitPoint;

    public bool isGrabbed= false;
    public bool beaconEntered = false;
    public bool showBeacon;
    public bool showCoin;

    public coinBank bank;
    public float time_grabbed;
    public float time_entered;

    /// <summary>
    /// The angle guessed between this coin and the one before it.
    /// </summary>
    public float point_back_angle = 361;

    public string coin_id;
    public bool hasChanged = false;

    public Transform lightHouseTransform;

    // Use this for initialization
    void Start () {

        laser = Instantiate(laserPrefab);
        laser.transform.parent = this.transform;
        laserTransform = laser.transform;
        laser.name = "beacon" + coin_id;

        this.gameObject.GetComponent<Rigidbody>().maxDepenetrationVelocity = 0;

        count = 0;

        //showBeacon = true;
        //showCoin = true;

        //if (showBeacon)
        //{
            Vector2 disp = Random.insideUnitCircle;
            //disp = new Vector2(disp.x * 25, disp.y * 25);
            disp = new Vector2(0, 0);
            Vector3 start = new Vector3(this.transform.position.x + disp.x, this.transform.position.y, this.transform.position.z + disp.y);
            Vector3 end = start + new Vector3(0, 1000, 0);
            ShowLaser(laser, laserTransform, start, end, 1000);
            laser.SetActive(false);
        //}
        
        this.gameObject.SetActive(false);
        this.gameObject.GetComponent<Renderer>().enabled = false;


    }

    void onEnable()
    {
        moveOffGround();
    }


    // Update is called once per frame
    void Update() {

        if (!FunctionController.moveEnabled)
        {
            laser.SetActive(false);
        }
        else
        {



            if (hasChanged)
            {
                moveOffGround();
                hasChanged = false;

            }

            if (showBeacon)
                laser.SetActive(true);
            else
                laser.SetActive(false);


            if (showCoin)
                this.gameObject.GetComponent<Renderer>().enabled = true;
            else
                this.gameObject.GetComponent<Renderer>().enabled = false;


            if (showBeacon && hasChanged)
            {
                //Vector2 disp = Random.insideUnitCircle;
                //disp = new Vector2(disp.x * 25, disp.y * 25);
                Vector3 start = laser.transform.position;//new Vector3(this.transform.position.x + disp.x, this.transform.position.y, this.transform.position.z + disp.y);
                Vector3 end = start + new Vector3(0, 1000, 0);

                ShowLaser(laser, laserTransform, start, end, 1000);
                hasChanged = false;
            }

            if (lightHouseTransform.position.x != -128 && lightHouseTransform.position.y != 128 && lightHouseTransform.position.z != -128)
            {
                if (showBeacon && !beaconEntered)
                {
                    Vector2 beaconHeart = new Vector2(laser.transform.position.x, laser.transform.position.z);
                    Vector2 lhHeart = new Vector2(lightHouseTransform.position.x, lightHouseTransform.position.z);
                    if (Vector2.Distance(beaconHeart, lhHeart) < (laser.GetComponent<Renderer>().bounds.size.x) / 2)
                    {
                        beaconEntered = true;
                        time_entered = Time.time;
                    }
                }
                if (showCoin && !isGrabbed)
                {
                    if (Mathf.Abs(lightHouseTransform.position.x - this.transform.position.x) < (this.GetComponent<Renderer>().bounds.size.x) / 2
                      && Mathf.Abs(lightHouseTransform.position.y - this.transform.position.y) < (this.GetComponent<Renderer>().bounds.size.y) / 2
                      && Mathf.Abs(lightHouseTransform.position.z - this.transform.position.z) < (this.GetComponent<Renderer>().bounds.size.z) / 2
                      )
                    {
                        isGrabbed = true;
                    }

                }



                if (isGrabbed)
                    time_grabbed = Time.time;

                if (isGrabbed || !showCoin)
                {
                    this.gameObject.GetComponent<Renderer>().enabled = false;
                    isGrabbed = true;

                }
                if (beaconEntered || !showBeacon)
                {

                    laser.SetActive(false);
                    beaconEntered = true;
                }

                if (isGrabbed && beaconEntered)
                {

                    bank.count++;
                    this.gameObject.SetActive(false);
                }
            }
            else
            {
                moveOffGround();
            }
        }
	}

    public static void ShowLaser(GameObject laser,Transform laserTransform ,Vector3 startPoint, Vector3 hitPoint, float distance)
    {
        laser.SetActive(true);
        laserTransform.position = Vector3.Lerp(startPoint, hitPoint, .5f);
        laserTransform.LookAt(hitPoint);
        laserTransform.Rotate(new Vector3(90,0,0));
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, distance,
            laserTransform.localScale.z);
    }

    private void moveOffGround()
    {
        RaycastHit hit;
        string[] layers = { "Ignore Raycast" };
        int mask = LayerMask.GetMask(layers);
        mask = ~mask;
        Physics.Raycast(this.transform.position + new Vector3(0, 1000, 0), Vector3.down, out hit, 3000f, mask);

        this.transform.position = new Vector3(this.transform.position.x, hit.point.y + offset + (this.GetComponent<Renderer>().bounds.size.x/2), this.transform.position.z);
        this.gameObject.GetComponent<Renderer>().enabled = true;

    }
}
