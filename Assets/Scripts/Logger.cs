using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Logger : MonoBehaviour {

    float updateRate = 1;
    float cTime;
    float nTime;

    float cTimeU;
    float nTimeU;

    public GameObject cameraRig;
    public GameObject controllerLeft;
    public GameObject controllerRight;
    public GameObject headCam;
    public coinBank bank;

    public string filename= "LogFile.csv";
    System.IO.FileStream logFile;

    public struct posTime
    {
        public Vector3 p;
        public Vector3 a;
        public float t;
    }

    public static List<Vector2> grabs;
    public static List<Vector2> flyTouchs;
    public static List<Vector2> teleports;
    public static List<Vector2> arcTeleports;

    public static List<posTime> rigPositions;
    public static List<posTime> leftHPositions;
    public static List<posTime> rightHPositions;




    // Use this for initialization
    void Start () {


        StartCoroutine(setup_file());

        rigPositions = new List<posTime>();
        leftHPositions = new List<posTime>();
        rightHPositions = new List<posTime>();

        grabs = new List<Vector2>();
        flyTouchs = new List<Vector2>();
        teleports = new List<Vector2>();
        arcTeleports = new List<Vector2>();
        cTime = nTime = cTimeU = nTimeU = Time.time;



	}

    IEnumerator setup_file()
    {
        bool isSetup = false;
        do
        {
            if (bank.result_filename == null || bank.participant_name == null)
                yield return null;
            else
            {
                filename = "Log" + bank.result_filename ;
                using (StreamWriter sw = File.AppendText(filename))
                {
                        sw.Write("Date,Participant_name,Event_type,start_time,end_time,x_pos,y_pos,z_pos,x_angle,y_angle,z_angle,condition,repetition,task,block,fileset,minimap\n");
                }
                // logFile = File.Open(filename, System.IO.FileMode.Append);
                isSetup = true;
            }
        } while (!isSetup);
    }
	
	void FixedUpdate()
    {
        cTimeU = Time.time;

        if (cTimeU > nTimeU)
        {
            StartCoroutine(logData(filename));
            nTimeU = cTimeU + updateRate * 10;
        }
    }
    
    // Update is called once per frame
	void Update () {

        cTime = Time.time;

        if (cTime > nTime)
        {
            posTime temp = new posTime();
            temp.p = headCam.transform.position;
            temp.a = headCam.transform.transform.forward;
            temp.t = Time.time;
            rigPositions.Add(temp);

            posTime tempR = new posTime();
            tempR.p = controllerRight.transform.position;
            tempR.a = controllerRight.transform.transform.forward;
            tempR.t = Time.time;
            rightHPositions.Add(tempR);

            posTime tempL = new posTime();
            tempL.p = controllerLeft.transform.position;
            tempL.a = controllerLeft.transform.transform.forward;
            tempL.t = Time.time;
            leftHPositions.Add(tempL);

            nTime = cTime + updateRate;
        }
    }

    void OnApplicationQuit()
    {
        if(this.enabled == true)
        logDataFull(filename);
    }

    IEnumerator logData(string filename)
    {
        using (StreamWriter sw = File.AppendText(filename))
        {
            string nav_Search;
            if (bank.beaconTCoinF)
                nav_Search = "navigation";
            else
                nav_Search = "search";

            if (!File.Exists(filename))
                sw.Write("Date,Participant_name,Event_type,start_time,end_time,x_pos,y_pos,z_pos,x_angle,y_angle,z_angle,condition,repetition,task,block,fileset,minimap\n");
            string format_string = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}\n";

            if (1 / Time.smoothDeltaTime <= 50)
                yield return null;

            foreach (posTime v in rigPositions)
            {
                sw.Write( string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "HeadPosition", v.t.ToString(), v.t.ToString(), v.p.x, v.p.y, v.p.z, v.a.x, v.a.y, v.a.z, bank.condition+bank.mapCond, bank.trial_number,nav_Search, bank.block ,bank.tName, FunctionController.miniMapGEnabled));
            }
            rigPositions.Clear();
            if (1 / Time.smoothDeltaTime <= 50)
                yield return null;

            foreach (posTime v in rightHPositions)
            {
                sw.Write( string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "RightHPosition", v.t.ToString(), v.t.ToString(), v.p.x, v.p.y, v.p.z, v.a.x, v.a.y, v.a.z, bank.condition + bank.mapCond, bank.trial_number, nav_Search, bank.block ,bank.tName, FunctionController.miniMapGEnabled));
            }
            rightHPositions.Clear();
            if (1 / Time.smoothDeltaTime <= 50)
                yield return null;

            foreach (posTime v in leftHPositions)
            {
                sw.Write( string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "LeftHPosition", v.t.ToString(), v.t.ToString(), v.p.x, v.p.y, v.p.z, v.a.x, v.a.y, v.a.z, bank.condition + bank.mapCond, bank.trial_number, nav_Search, bank.block ,bank.tName, FunctionController.miniMapGEnabled));
            }
            leftHPositions.Clear();
            if (1 / Time.smoothDeltaTime <= 50)
                yield return null;

            foreach (Vector2 t in grabs)
            {
                sw.Write( string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "Grab", t.x, t.y, "", "", "", "", "", "", bank.condition + bank.mapCond, bank.trial_number, nav_Search, bank.block ,bank.tName, FunctionController.miniMapGEnabled));
            }
            grabs.Clear();
            if (1 / Time.smoothDeltaTime <= 50)
                yield return null;
            foreach (Vector2 t in flyTouchs)
            {
                sw.Write( string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "FlyEvent", t.x, t.y, "", "", "", "", "", "", bank.condition + bank.mapCond, bank.trial_number, nav_Search, bank.block ,bank.tName, FunctionController.miniMapGEnabled));
            }
            flyTouchs.Clear();

            if (1 / Time.smoothDeltaTime <= 50)
                yield return null;
            foreach (Vector2 t in teleports)
            {
                string event_type = "";
                if (t.y == 1)
                {
                    event_type = "TeleportMM";
                }
                else
                {
                    event_type = "Teleport";
                }
                sw.Write(string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, event_type, t.x, t.x, "", "", "", "", "", "", bank.condition + bank.mapCond, bank.trial_number, nav_Search, bank.block ,bank.tName, FunctionController.miniMapGEnabled));
            }
            teleports.Clear();

            //addition for arc Teleports
            if (1 / Time.smoothDeltaTime <= 50)
                yield return null;
            foreach (Vector2 t in arcTeleports)
            {
                string event_type = "";
                if (t.y == 1)
                {
                    event_type = "ArcTeleportMM";
                }
                else
                {
                    event_type = "ArcTeleport";
                }
                sw.Write(string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, event_type, t.x, t.x, "", "", "", "", "", "", bank.condition + bank.mapCond, bank.trial_number, nav_Search, bank.block, bank.tName, FunctionController.miniMapGEnabled));
            }
            arcTeleports.Clear();
        }
    }

    void logDataFull(string filename)
    {
        using (StreamWriter sw = File.AppendText(filename))
        {
            string nav_Search;
            if (bank.beaconTCoinF)
                nav_Search = "navigation";
            else
                nav_Search = "search";

            if (!File.Exists(filename))
                sw.Write("Date,Participant_name,Event_type,start_time,end_time,x_pos,y_pos,z_pos,x_angle,y_angle,z_angle,condition,repetition,task,block,fileset,minimap\n");
            string format_string = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}\n";

            foreach (posTime v in rigPositions)
            {
                sw.Write(string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "HeadPosition", v.t.ToString(), v.t.ToString(), v.p.x, v.p.y, v.p.z, v.a.x, v.a.y, v.a.z, bank.condition + bank.mapCond, bank.trial_number, nav_Search, bank.block ,bank.tName, FunctionController.miniMapGEnabled));
            }
            rigPositions.Clear();

            foreach (posTime v in rightHPositions)
            {
                sw.Write(string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "RightHPosition", v.t.ToString(), v.t.ToString(), v.p.x, v.p.y, v.p.z, v.a.x, v.a.y, v.a.z, bank.condition + bank.mapCond, bank.trial_number, nav_Search, bank.block ,bank.tName, FunctionController.miniMapGEnabled));
            }
            rightHPositions.Clear();

            foreach (posTime v in leftHPositions)
            {
                sw.Write(string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "LeftHPosition", v.t.ToString(), v.t.ToString(), v.p.x, v.p.y, v.p.z, v.a.x, v.a.y, v.a.z, bank.condition + bank.mapCond, bank.trial_number, nav_Search, bank.block ,bank.tName, FunctionController.miniMapGEnabled));
            }
            leftHPositions.Clear();

            foreach (Vector2 t in grabs)
            {
                sw.Write(string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "Grab", t.x, t.y, "", "", "", "", "", "", bank.condition + bank.mapCond, bank.trial_number, nav_Search, bank.block ,bank.tName, FunctionController.miniMapGEnabled));
            }
            grabs.Clear();
            foreach (Vector2 t in flyTouchs)
            {
                sw.Write(string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, "FlyEvent", t.x, t.y, "", "", "", "", "", "", bank.condition + bank.mapCond, bank.trial_number, nav_Search, bank.block ,bank.tName, FunctionController.miniMapGEnabled));
            }
            flyTouchs.Clear();

            foreach (Vector2 t in teleports)
            {
                string event_type= "";
                if(t.y == 1)
                {
                    event_type = "TeleportMM";
                }
                else
                {
                    event_type = "Teleport";
                }
                sw.Write(string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, event_type, t.x, t.x, "", "", "", "", "", "", bank.condition + bank.mapCond, bank.trial_number, nav_Search, bank.block ,bank.tName, FunctionController.miniMapGEnabled));
            }
            teleports.Clear();
            foreach (Vector2 t in arcTeleports)
            {
                string event_type = "";
                if (t.y == 1)
                {
                    event_type = "ArcTeleportMM";
                }
                else
                {
                    event_type = "ArcTeleport";
                }
                sw.Write(string.Format(format_string, System.DateTime.Now.ToString(), bank.participant_name, event_type, t.x, t.x, "", "", "", "", "", "", bank.condition + bank.mapCond, bank.trial_number, nav_Search, bank.block, bank.tName, FunctionController.miniMapGEnabled));
            }
            arcTeleports.Clear();
        }
    }
}
