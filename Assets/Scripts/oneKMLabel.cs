using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oneKMLabel : MonoBehaviour {

    private TextMesh tMesh;
    public Generate_Terrain map;
    public miniMap mMap;
    GameObject kmBar;
    float baseBarLength;
    float barLength;
    Vector3 basePos;
    //GameObject myLine;

    // Use this for initialization
    void Start () {
        /*
        myLine = new GameObject();
        myLine.transform.position = this.transform.position;
        myLine.AddComponent<LineRenderer>();
        myLine.transform.parent = mMap.transform;
        */

        basePos = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.transform.localPosition.z);

        tMesh = this.GetComponent<TextMesh>();

        kmBar = Instantiate(Resources.Load<GameObject>("kmBarPrefab"));
        kmBar.name = "kmBar";
        kmBar.transform.parent = this.transform;
        kmBar.transform.localPosition = new Vector3(0.5f, 0, 0);
        baseBarLength = kmBar.transform.localScale.x;

        //tMesh = this.GetComponent<TextMesh>();

    }
	
	// Update is called once per frame
	void Update () {

	}

    void LateUpdate()
    {

        if (collect_tiles.center_changed)
        {
            //GameObject.Destroy(myLine);
            float map_width_per_unit = map.center.collect.mRes;
            float map_width = map_width_per_unit * map.map_width;

            int scale = nearestMagnitude(map_width);

            tMesh.text = scale.ToString() + " M";

            float ratio = ((float)scale / map_width);
            barLength = ratio * baseBarLength;

            //Debug.Log(barLength);

            kmBar.transform.localPosition = new Vector3(ratio/2, 0, 0);
            kmBar.transform.localScale = new Vector3(barLength, kmBar.transform.localScale.y, kmBar.transform.localScale.z);

            this.transform.localPosition = basePos +  new Vector3(0, mMap.yOffset, 0);

            /*
            Vector3 line_start = this.transform.position + new Vector3(0.5f, 0, 0);
            Vector3 line_end = line_start + new Vector3(laser_length, 0, 0);
            DrawLine(line_start, line_end , new Color(1, 0, 0));
            */
        }

    }

    int nearestMagnitude(float x)
    {
        int magnitude = 1;

        while (true) {

            if (x / magnitude < 1)
                return magnitude / 10;
            else
                magnitude *= 10;
        }
    }

    /*
    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        myLine.transform.position = start;
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        //lr.SetColors(color, color);
        lr.startColor = color;
        lr.endColor = color;
        //lr.SetWidth(0.1f, 0.1f);
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }
*/
}
