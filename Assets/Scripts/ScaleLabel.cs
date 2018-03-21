using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleLabel : MonoBehaviour {
    private TextMesh tMesh;
    public Generate_Terrain map;
    Vector3 basePos;

	// Use this for initialization
	void Start () {
        basePos = this.transform.localPosition;
        tMesh = this.GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
        tMesh.text = "1u:" + (map.center.collect.mRes).ToString() + "M";
        this.transform.localPosition = basePos + new Vector3(0, map.miniMap.GetComponent<miniMap>().yOffset, 0);

	}
}
