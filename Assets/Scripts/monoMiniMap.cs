using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monoMiniMap : MonoBehaviour {

    private float mapWidth;
    public float scale;
    public float yOffset;
    Terrain monoMini;
    public Terrain mainMap;
    public coinBank bank;
    public Dictionary<string, GameObject> beacons;
    public bool hasChanged = true;

    // Use this for initialization
    void Start () {
        yOffset = 0;
        mapWidth = 1;
        scale = 256;
        monoMini = this.gameObject.GetComponent<Terrain>();
        beacons = new Dictionary<string, GameObject>();
        }
	
    void LateUpdate()
    {
        
        if (hasChanged)
        {
            bank.hasChanged = true;
            hasChanged = false;

            scale = mainMap.terrainData.size.x / mapWidth;
            float [,] nHeightMap = miniMap.scaleHeightmap(
                mainMap.terrainData.GetHeights(0, 0, mainMap.terrainData.heightmapWidth, mainMap.terrainData.heightmapHeight), 
                    mainMap.terrainData.heightmapWidth,
                    monoMini.terrainData.heightmapWidth);
            monoMini.terrainData.SetHeights(0, 0, nHeightMap);
            float yheight = mainMap.terrainData.size.y / scale;
            monoMini.terrainData.size = new Vector3(mapWidth, yheight , mapWidth);


            Texture2D nTex = combineSplatTextures(
                mainMap.terrainData.splatPrototypes,
                mainMap.terrainData.GetAlphamaps(0, 0, mainMap.terrainData.alphamapWidth, mainMap.terrainData.alphamapHeight),
                mainMap.terrainData.alphamapResolution,
                mainMap.terrainData.alphamapWidth,
                mainMap.terrainData.alphamapHeight,
                mainMap.GetComponent<mapTile>().detail);

            TextureScale.Point(nTex, monoMini.terrainData.alphamapWidth, monoMini.terrainData.alphamapHeight);
            

            SplatPrototype splat = new SplatPrototype();
            splat.texture = new Texture2D(monoMini.terrainData.alphamapWidth, monoMini.terrainData.alphamapHeight, TextureFormat.RGB24, true);
            Graphics.CopyTexture(nTex, splat.texture);

            splat.tileSize = new Vector2(1, 1);

            SplatPrototype [] splats = new SplatPrototype[2];
            splats[0] = monoMini.terrainData.splatPrototypes[0];
            splats[1] = splat;

            monoMini.terrainData.splatPrototypes = splats;
            

            //miniMap.terrainData.alphamapResolution = mainMap.terrainData.alphamapResolution;
            //miniMap.terrainData.SetAlphamaps(0, 0, mainMap.terrainData.GetAlphamaps(0, 0, mainMap.terrainData.alphamapWidth, mainMap.terrainData.alphamapHeight));
            //miniMap.terrainData.splatPrototypes = mainMap.terrainData.splatPrototypes;

        }

        manageBeacons();

    }


    Texture2D combineSplatTextures(SplatPrototype [] splats, float [,,] AlphaMap, int AlphaMapResolution, int AlphaWidth, int AlphaHeight, int width)
    {
        Texture2D retTex = new Texture2D(AlphaWidth, AlphaHeight, TextureFormat.RGB24, true);
        /*
        for (int k = 0; k < splats.Length; k++)
            for (int i = 0; i<AlphaWidth; i++)
                for (int j = 0; j < AlphaHeight; j++)
                {
                    //if (AlphaMap[i, j, k]  == 1)
                    {
                        int x= (int)(i % splats[k].tileSize.x);
                        int y= (int)(j % splats[k].tileSize.y);
                        Color c = splats[k].texture.GetPixel(x, y);
                        //c = c * k;
                        retTex.SetPixel(i, j, c);
                    }               
                }
        */
        for(int k= 0; k< splats.Length; k++)
        for (int i = 0; i < splats[k].tileSize.x; i++)
            for (int j = 0; j < splats[k].tileSize.y; j++)
            {
                    Color c = splats[k].texture.GetPixel(i, j);
                    int x = i + (int)((k / width) * splats[k].tileSize.x);
                    int y = (j + (int)(((width - k - 1) % width) * splats[k].tileSize.y));
                    retTex.SetPixel(x, y, c); 
            }

                retTex.Apply();
        return retTex;
    }

	// Update is called once per frame
	void Update () {
		
	}

    void manageBeacons()
    {
        foreach (GameObject t in bank.tokens)
        {
            string tKey = t.GetComponent<basicToken>().coin_id;
            if (!beacons.ContainsKey(tKey))
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), t.GetComponent<Collider>());
                GameObject nBeacon = Instantiate(bank.beaconPinPrefab);
                nBeacon.transform.SetParent(this.transform);
                //  nBeacon.transform.localScale = new Vector3(nBeacon.transform.localScale.x * ( scaler/scale), 1, nBeacon.transform.localScale.z * ( scaler / scale));
                //  nBeacon.transform.localScale = new Vector3(nBeacon.transform.localScale.x / 2, nBeacon.transform.localScale.y / 2, nBeacon.transform.localScale.z / 2);
                nBeacon.transform.localPosition = new Vector3(0, 0, 0);
                beacons.Add(tKey, nBeacon);
            }

            if (t.GetComponent<basicToken>().laser.activeSelf == true && t.GetComponent<basicToken>().showBeacon)
            {
                beacons[tKey].SetActive(true);
                //if (hasChanged)
                {
                    Vector3 beaconPos;
                    beaconPos = tracker_guide.translateMaptoMMap(t.GetComponent<basicToken>().laserTransform.position, this);
                    beacons[tKey].transform.localPosition = beaconPos + new Vector3(0, 0.0f, 0);
                }
                //basicToken.ShowLaser(beacons[tKey], beacons[tKey].transform, beacons[tKey].transform.position, beacons[tKey].transform.position + new Vector3(0, 1, 0), 1);
            }
            else
            {
                beacons[tKey].SetActive(false);
            }

        }

        foreach (GameObject t in bank.tokens)
        {
            string tKey = t.GetComponent<basicToken>().coin_id;
            if (t.activeSelf == false)
                beacons[tKey].SetActive(false);
        }
    }

}
