using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class miniMap : MonoBehaviour {

    public float scale;
    public Generate_Terrain map;
    public bool hasChanged = false;
    public float yOffset;
    Terrain mMap ;
    public coinBank bank;
    public Dictionary<string , GameObject> beacons;

    // Use this for initialization
    void Start () {
        //
        yOffset = 0;
        scale = 256;
        mMap = this.gameObject.GetComponent<Terrain>();
        beacons = new Dictionary<string, GameObject>();
    }
	void LateUpdate()
    {
        if (hasChanged)
        {
            //Debug.Log(map.name);
            float[,] heights = createHeightmap(map, map.terrains_width, map.terrains_height);
            heights = scaleHeightmap(heights, map.map_width, mMap.terrainData.heightmapWidth);
            float scaler = (float)map.map_width / (float)mMap.terrainData.heightmapWidth;
            setHeightMap(heights, mMap);
            float min = minHeight(heights);
            //Debug.Log(map.center.Terr.terrainData.size.z);
            float yheight = (mMap.terrainData.heightmapWidth / map.map_width   )* (map.center.Terr.GetComponent<collect_tiles>().terrBaseHeight / (scale * map.terrains_width));
            mMap.terrainData.size = new Vector3(mMap.terrainData.size.x, yheight , mMap.terrainData.size.z);
            int detail = (collect_tiles.zoom - 8);
            if (detail <= 0)
                detail = 1;
            mMap.heightmapPixelError = detail;
            //Debug.Log(yheight);
            yOffset = (min * yheight);
            mMap.gameObject.transform.localPosition  = new Vector3(mMap.gameObject.transform.localPosition.x, (1 - yOffset), mMap.gameObject.transform.localPosition.z);

            SplatPrototype[] splats = new SplatPrototype[2];
            splats[1] = new SplatPrototype();
            Texture2D tex = combineTextures(map, 256, 256);
            splats[1].texture = tex;
            splats[1].tileSize = new Vector2(1, 1);
            splats[0] = mMap.terrainData.splatPrototypes[0];
            mMap.terrainData.splatPrototypes = splats;
            try
            {
                UnityEditor.AssetDatabase.Refresh();
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }

            

            hasChanged = false;
        }

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

            if (t.activeSelf == true && t.GetComponent<basicToken>().showBeacon)
            {
                beacons[tKey].SetActive(true);
                //if (hasChanged)
                {
                    Vector3 beaconPos;
                    tracker_guide.translateMaptoMMap(out beaconPos, t.GetComponent<basicToken>().laserTransform.position, map);
                    beacons[tKey].transform.localPosition = beaconPos - new Vector3(0, 1.0f, 0);
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
        // Update is called once per frame
        void Update () {
        

	}
    float [,] createHeightmap(Generate_Terrain maps, int tiles_width, int tiles_height)
    {

        float[,] heights = new float[Generate_Terrain.tile_width * tiles_width, Generate_Terrain.tile_height * tiles_height];

        for(int k =0; k < tiles_width; k++)
            for(int q = 0; q < tiles_height; q++)
            {
                float[,] aHeight = maps.terrains[q, k].GetComponent<Terrain>().terrainData.GetHeights(0, 0, Generate_Terrain.tile_width, Generate_Terrain.tile_height);

                for (int i = 0; i < Generate_Terrain.tile_width; i++)
                    for (int j = 0; j < Generate_Terrain.tile_height; j++)
                    {
                        heights[i + k * Generate_Terrain.tile_width, j + q * Generate_Terrain.tile_height] = aHeight[i, j];
                    }
            }
               
        return heights;
    }

    public static float [,] scaleHeightmap(float [,] oHeights, int oSize, int nSize)
    {
        float[,] nHeights = new float[nSize, nSize];
        float scale = (float)oSize / (float)nSize;
        //Debug.Log(scale + " " + oSize + " " + nSize);
        for (int i = 0; i < nSize; i++)
            for (int j = 0; j < nSize; j++)
                nHeights[i, j] = oHeights[(int)(i * scale), (int)(j * scale)];

        return nHeights;
    }
    float minHeight(float [,] heights)
    {
        float min = float.MaxValue;
        foreach (float i in heights)
            if (i < min)
                min = i;
        return min;
    }

    void setHeightMap(float [,] heights , Terrain map)
    {
        map.terrainData.SetHeights(0, 0, heights);
    }

    private Texture2D combineTextures(Generate_Terrain map, int width, int height)
    {
        Texture2D ret = new Texture2D(width, height, TextureFormat.ARGB32, true);

        Texture2D [,] texs = new Texture2D[map.terrains_width, map.terrains_height];
        for (int i = 0; i < map.terrains_width; i++)
            for (int j = 0; j < map.terrains_height; j++)
                texs[i, j] = new Texture2D(256, 256, TextureFormat.RGB24, true);



        int piece_width = width / map.terrains_width;
        int piece_height = height / map.terrains_height;

        //Scale down each texture to the wanted size
        for (int i = 0; i < map.terrains_width; i++)
            for(int j = 0; j < map.terrains_height; j++)
            {
                try {
                    //Graphics.CopyTexture(map.terrains[i, j].GetComponent<Terrain>().terrainData.splatPrototypes[0].texture, texs[i, j]);
                    texs[i, j].LoadRawTextureData(map.terrains[i, j].GetComponent<Terrain>().terrainData.splatPrototypes[0].texture.GetRawTextureData());
                    TextureScale.Point(texs[i, j], piece_width, piece_height);
                }
                catch(System.Exception e)
                {
                    Debug.LogError(map.terrains[i, j].GetComponent<Terrain>().terrainData.splatPrototypes[0].texture.height);
                    Debug.LogError(map.terrains[i, j].GetComponent<Terrain>().terrainData.splatPrototypes[0].texture.width);
                    Debug.LogError(map.terrains[i, j].GetComponent<Terrain>().terrainData.splatPrototypes[0].texture.format);
                    Debug.LogError(e.Message);
                }
                
            }

        for (int i = 0; i < map.terrains_width; i++)
            for (int j = 0; j < map.terrains_height; j++)
                for(int k =0; k < piece_width; k++)
                    for(int q = 0; q < piece_height; q++)
                    {
                        ret.SetPixel((i * piece_width) + k, (j * piece_height) + q, (texs[i, j].GetPixel(k, q))); 
                    }
        ret.Apply();
       return ret;
    }

    
    Color InvertColour(Color ColourToInvert)
    {
        float RGBMAX = ColourToInvert.maxColorComponent;
        return new Color(RGBMAX - ColourToInvert.r, RGBMAX - ColourToInvert.g, RGBMAX - ColourToInvert.b);
    }

    Color ComplementColour(Color Col)
    {
        float h, s, v;
        Color.RGBToHSV(Col, out h, out s, out v);
        h = ((h + 0.66f) % 1);

        return Color.HSVToRGB(h, s, v);
    }

}
