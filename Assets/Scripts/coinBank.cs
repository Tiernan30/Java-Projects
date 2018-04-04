using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class coinBank : MonoBehaviour {

    public List<GameObject> tokens;
    public GameObject tokenPrefab;
    public string init_filename = "none";

    public bool useFly;
    public bool useTele;
    public bool useArcTele;
    public bool useRoom;
    public bool useMiniMap;
    public string tName;
    public int trial_number;
    public bool beaconTCoinF;
    public int block;

    public string condition = "";
    public string mapCond = "";

    public float coinXSize;
    public float CoinYSize;
    public float CoinZSize;

    public TextMesh controlLabel;

    public string result_filename;
    public string participant_name;
    public int count;
    public TextMesh counter;
    public GameObject beaconPrefab;
    public GameObject beaconPinPrefab;
    Generate_Terrain terrain;
    public int active_coin;
    public bool hasChanged;
    public FunctionController funcController;


	// Use this for initialization
	void Start () {

    active_coin = 0;

    tokens = new List<GameObject>();

    count = 0;
	if(init_filename == "none")
        {
            string mString = "";
            if (!useMiniMap)
            {
                mString = "NM";
                mapCond = "NM";
            }
            string trial_controls = "";

            if (useFly && useRoom && useTele)
            {
                trial_controls = "combined";
                controlLabel.text = "Full Control";
                condition = "combined";
            }
            else if (useFly)
            {

                trial_controls = "flight";
                controlLabel.text = "Flight";
                condition = "flight";
            }
            else if (useTele)
            {
                trial_controls = "tele";
                controlLabel.text = "Teleportation";
                condition = "teleportation";
            }
            else if (useArcTele)
            {
                trial_controls = "arcTele";
                controlLabel.text = "Arc Teleportation";
                condition = "arcteleportation";
            }
            else if (useRoom)
            {
                trial_controls = "room";
                controlLabel.text = "Room-in-Miniature";
                condition = "room";
            } 
            string cbstring = "";
            if (beaconTCoinF)
                cbstring = "Beacons";
            else
                cbstring = "Coins";
            

            init_filename = string.Format(@"Assets/Test_Files/{0}Test{4}{1}{2}{3}.txt", trial_controls, mString, trial_number, tName, cbstring);
            Debug.Log(init_filename);
            /*
            for (int i = 0; i < 10; i++)
            {
                GameObject tToken = Instantiate(tokenPrefab);
                tToken.SetActive(false);
                tToken.GetComponent<basicToken>().bank = this;
                tToken.GetComponent<basicToken>().coin_id = i.ToString();
               
                tToken.transform.position = new Vector3(-128, 128 , -128 + i);
                
                tokens.Add(tToken);
            }

        */}
     if(init_filename != null)
        {
            loadCoins(init_filename, out tokens);
        }	
	}
	
	// Update is called once per frame
	void Update () {
        counter.text = count.ToString();
        if (collect_tiles.center_changed || hasChanged )
        {
            foreach (GameObject t in tokens)
            {
                t.GetComponent<basicToken>().hasChanged = true;
            }
            hasChanged = false;
        }
        if (active_coin < tokens.Count)
        {
            if (tokens[active_coin].GetComponent<basicToken>().isGrabbed && tokens[active_coin].GetComponent<basicToken>().beaconEntered)
            {
                Debug.Log("Token " + active_coin + "Grabbed");
                active_coin++;
                funcController.toggleMovement(false);
                funcController.togglePointBack(true);

            }
            if (active_coin < tokens.Count)
                if (tokens[active_coin].activeSelf == false)
                {
                    tokens[active_coin].SetActive(true);
                    tokens[active_coin].GetComponent<basicToken>().hasChanged = true;
                }
        }
	}

    void OnApplicationQuit()
    {

        Debug.Log("Quiting: Time Scores Are:");
        if (participant_name == null)
            participant_name = "Anon";
        string outF = "Participant,Condition,Task,Repetition,Block,Fileset,Coin_ID,Coin_Time,Trial Time,Beacon_Time,Point_Back_Angle,minimap\n";
        float trial_time = 0.0f;
        float last_time = 0.0f;
        foreach (GameObject e in tokens)
        {
            string nav_Search;
            if (beaconTCoinF)
                nav_Search = "navigation";
            else
                nav_Search = "search";

            trial_time = e.GetComponent<basicToken>().time_grabbed - last_time;
            last_time = e.GetComponent<basicToken>().time_grabbed;
            string outFormat = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}\n";
            string outl = string.Format(outFormat,participant_name, condition, nav_Search, trial_number, block, tName,  e.GetComponent<basicToken>().coin_id,  e.GetComponent<basicToken>().time_grabbed, trial_time  , e.GetComponent<basicToken>().time_entered, e.GetComponent<basicToken>().point_back_angle, FunctionController.miniMapGEnabled);
           // outl = string.Format("name,{0},tokenID,{1},tokenTime,{2},beaconTime,{3},pointAngle,{4},Date,{5}\n", participant_name, e.GetComponent<basicToken>().coin_id, e.GetComponent<basicToken>().time_grabbed, e.GetComponent<basicToken>().time_entered, e.GetComponent<basicToken>().point_back_angle, System.DateTime.Now.ToString());
            Debug.Log(outl);
            outF += outl;
        }
        if (result_filename != null)
            File.AppendAllText(result_filename, outF);
    }

    void loadCoins(string filename, out List<GameObject> tokens)
    {
        
        tokens = new List<GameObject>();
        string [] fileTex = File.ReadAllLines(filename);

        foreach(string line in fileTex)
        {
            if (line.StartsWith("@coin"))
            {
                string[] e = line.Split('\t');

                GameObject tToken = Instantiate(tokenPrefab);
                tToken.transform.parent = this.transform;
                tToken.GetComponent<basicToken>().bank = this;
                tToken.GetComponent<basicToken>().lightHouseTransform = GameObject.Find("[CameraRig]").transform;
                tToken.GetComponent<basicToken>().coin_id = e[2];
                float x = float.Parse(e[4]);
                float z = float.Parse(e[6]);
                float y = float.Parse(e[8]);
                string bcid = e[9];

                tToken.GetComponent<basicToken>().offset = y;
                //RaycastHit hit;
                //Physics.Raycast(new Vector3(x, 1000 ,z), Vector3.down, out hit);
                //float y = hit.point.y +

                tToken.transform.position = new Vector3(x, y, z);
                tToken.transform.localScale = new Vector3(coinXSize, CoinYSize, CoinZSize);
                tToken.name = "coin" + tToken.GetComponent<basicToken>().coin_id;
                tToken.GetComponent<basicToken>().laserPrefab = beaconPrefab;

                if (bcid == "c")
                {
                    tToken.GetComponent<basicToken>().showCoin = true;
                    tToken.GetComponent<basicToken>().showBeacon = false;
                }                   
                else if (bcid == "b")
                {

                    tToken.GetComponent<basicToken>().showCoin = false;
                    tToken.GetComponent<basicToken>().showBeacon = true;
                }
                else if (bcid == "bc" || bcid == "cb")
                {

                    tToken.GetComponent<basicToken>().showCoin = true;
                    tToken.GetComponent<basicToken>().showBeacon = true;
                }

                    tokens.Add(tToken);
            }
            

            if (line.StartsWith("@pname") && participant_name == "anon")
            {
                string[] e = line.Split('\t');
                participant_name = e[1];
            }
            if (line.StartsWith("@outfile"))
            {
                string[] e = line.Split('\t');
                result_filename = participant_name + e[1];
            }
            if (line.StartsWith("@functions"))
            {
                string [] e =  line.Split('\t');
                string fs = e[1];

                if (fs.Contains("f"))
                {
                    FunctionController.flightGEnabled = true;
                    funcController.toggleFlying(true);
                }
                else
                {
                    funcController.toggleFlying(false);
                    FunctionController.flightGEnabled = false;
                    
                }
                if (fs.Contains("r"))
                {
                    FunctionController.enableTokenMove = true;
                    FunctionController.tokenMoveGEnabled = true;
                }
                else
                {
                    FunctionController.enableTokenMove = false;
                    FunctionController.tokenMoveGEnabled = false;
                }
                if (fs.Contains("t"))
                {
                    funcController.toggleTeleportation(true);
                    FunctionController.teleportGEnabled = true;
                }
                else
                {
                    funcController.toggleTeleportation(false);
                    FunctionController.teleportGEnabled = false;
                }
                if (fs.Contains("a"))
                {
                    funcController.toggleArcTeleportation(true);
                    FunctionController.arcTeleportGEnabled = true;
                }
                else
                {
                    funcController.toggleArcTeleportation(false);
                    FunctionController.arcTeleportGEnabled = false;
                }

               
                if (fs.Contains("m"))
                {
                    FunctionController.enableMiniMap = false;
                    funcController.boolMinimap = true;
                    FunctionController.miniMapGEnabled = true;
                }
                else
                {
                    FunctionController.enableMiniMap = true;
                    funcController.boolMinimap = true;
                    FunctionController.miniMapGEnabled = false;
                }
                if (fs.Contains("n"))
                {
                    funcController.toggleNavigation(false);
                    funcController.boolNavigation = true;
                }
                else
                {
                    funcController.toggleNavigation(true);
                    funcController.boolNavigation = true;
                }

            }
        }

    }

}
