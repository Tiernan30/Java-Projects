using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



/*
 * Class Generates and stores all of the terrains in the scene
 * 
 * 
 */
public class Generate_Terrain : MonoBehaviour {

    public FunctionController funcController;
    /// <summary>
    /// Width of a single tile in game units eg. 256
    /// </summary>
    public const int tile_width = 256;
    /// <summary>
    /// Height of a Single tile in game units eg. 256
    /// </summary>
    public const int tile_height = 256;

    public static string init_filename = @"Assets/sample_coins.txt";

    /// <summary>
    ///Width of the total map in game units eg 768 (256 x 3)
    /// </summary>
    public int map_width;

    /// <summary>
    ///Height of the total map in game units eg 768 (256 x 3)
    /// </summary>
    public int map_height;

    //2D list of all the terrains in the scene
    public GameObject[,] terrains;
    private TerrainData[,] terDatas;
    //The miniMap the corrisponds to those terrains

    //Describes the tracker object to be created
    public GameObject trackerPrefab;
    //A reference to the tracker object
    public GameObject posTracker;

    /// <summary>
    ///A reference to the minimap we'll use in our scene
    /// </summary>
    public GameObject miniMap;
    //And the terrain data we'll use to create
    private TerrainData mMapTerrData;

    public GameObject mainMap;
    public GameObject monoMiniMap;

    //A text mesh we're making to show how long the minimap is
    private GameObject scaleLabel;
    private GameObject oneKMLabel;

    //The basic data we'll use for all of our terrain tiles.
    private TerrainData terrainPrefab;


    /// <summary>
    ///The style of visual tiles we want to grab such as 'r' for road maps or 'a' for aerial maps
    /// </summary>
    public char map_style = 'r';

    /// <summary>
    /// Number of terrains tall (must be odd number) eg 3.
    /// </summary>
    public int terrains_height;
    private int centerY;

    /// <summary>
    /// The bank that holds all of the trials beacons and coins
    /// </summary>
    public coinBank bank;

    /// <summary>
    ///  Number of terrains wide (should be odd number) eg. 3
    /// </summary>
    public int terrains_width;
    private int centerX;

    public Material tranMat;

    public struct Center
    {
        public Terrain Terr;
        public collect_tiles collect;
    };
    public Center center;

    // Use this for initialization
    void Start () {
 
        SplatPrototype[] mSplats = new SplatPrototype[2];
        mSplats[0] = new SplatPrototype();

        Texture2D circleTex = new Texture2D(256, 256, TextureFormat.ARGB32, true);
        circleTex.LoadImage(File.ReadAllBytes(@"assets/transparent256x256.png"));
            
        circleTex.Apply();
        mSplats[0].texture = circleTex;
        mSplats[0].texture.alphaIsTransparency = true;

        mSplats[0].tileSize = new Vector2(255, 255);
        
        mSplats[1] = new SplatPrototype();
        mSplats[1].texture = new Texture2D(256, 256, TextureFormat.ARGB32, true);

        TerrainData mapTerr = new TerrainData();
        mapTerr.heightmapResolution = tile_width + 1;
        mapTerr.size = new Vector3(tile_width, 1, tile_height);
        mapTerr.SetDetailResolution(1024, 8);
        mapTerr.baseMapResolution = 1024;
        mainMap = Terrain.CreateTerrainGameObject(mapTerr);
        mainMap.AddComponent<mapTile>();
        mainMap.GetComponent<mapTile>().SetupMapTile(4, mainMap.GetComponent<Terrain>(), 367, 683, 11);
        

        funcController.mainMap = mainMap;

        TerrainData monoMiniTerr = new TerrainData();
        monoMiniTerr.heightmapResolution = mainMap.GetComponent<Terrain>().terrainData.heightmapResolution;
        monoMiniTerr.SetDetailResolution(1024, 8);
        monoMiniTerr.baseMapResolution = 1024;
        monoMiniTerr.size = new Vector3(1, 1, 1);
        monoMiniTerr.splatPrototypes = mSplats;

        monoMiniTerr.alphamapResolution = 1024;

        float[,,] splatMapAlphasMono = monoMiniTerr.GetAlphamaps(0, 0, monoMiniTerr.alphamapWidth, monoMiniTerr.alphamapHeight);

        float radius = 0.98f;
        for (int i = 0; i < monoMiniTerr.alphamapHeight; i++)
            for (int j = 0; j < monoMiniTerr.alphamapWidth; j++)
            {
                int height = monoMiniTerr.alphamapHeight;
                int width = monoMiniTerr.alphamapWidth;
                if (Mathf.Sqrt(Mathf.Pow(i - (height / 2), 2) + Mathf.Pow(j - (width / 2), 2)) > (radius * height * 0.5))
                {
                    splatMapAlphasMono[i, j, 0] = 1;
                    splatMapAlphasMono[i, j, 1] = 0;
                }
                else
                {

                    splatMapAlphasMono[i, j, 0] = 0;
                    splatMapAlphasMono[i, j, 1] = 1;
                }

            }


        monoMiniTerr.SetAlphamaps(0, 0, splatMapAlphasMono);

        monoMiniMap = Terrain.CreateTerrainGameObject(monoMiniTerr);
        monoMiniMap.name = "monoMiniMap";
        monoMiniMap.AddComponent<monoMiniMap>();
        monoMiniMap.GetComponent<monoMiniMap>().mainMap = mainMap.GetComponent<Terrain>();
        monoMiniMap.GetComponent<monoMiniMap>().bank = bank;
        monoMiniMap.transform.parent = this.gameObject.transform;
        monoMiniMap.transform.localPosition = new Vector3(-0.5f, 1.0f, -0.5f);
        monoMiniMap.GetComponent<Terrain>().detailObjectDistance = 250;
        monoMiniMap.GetComponent<Terrain>().heightmapPixelError = 2;
        //miniMap.GetComponent<Terrain>().basemapDistance = 10;

        monoMiniMap.GetComponent<Terrain>().materialType = Terrain.MaterialType.Custom;
        monoMiniMap.GetComponent<Terrain>().materialTemplate = tranMat;

        funcController.miniMap = monoMiniMap;

        mainMap.GetComponent<mapTile>().monoMini = monoMiniMap.GetComponent<monoMiniMap>();

        posTracker = Instantiate(trackerPrefab);
        posTracker.GetComponent<tracker_guide>().map = this;
        posTracker.GetComponent<tracker_guide>().monoMini = monoMiniMap.GetComponent<monoMiniMap>();
        posTracker.GetComponent<tracker_guide>().cameraRigTransform = GameObject.Find("[CameraRig]").transform;
        posTracker.GetComponent<tracker_guide>().headTransform = GameObject.Find("Camera (eye)").transform;
        posTracker.transform.parent = monoMiniMap.transform;

        funcController.movementToken = posTracker.GetComponent<tracker_guide>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void loadLatLonZ(string filename, out float lat, out float lon, out int zoom)
    {
        lat = lon = zoom = 0;
        string[] fileTex = File.ReadAllLines(filename);

        foreach (string line in fileTex)
        {
            if (line.StartsWith("@Lat"))
            {
                string[] e = line.Split('\t');
                lat = float.Parse(e[1]);
            }
            else if (line.StartsWith("@Lon")){
                string[] e = line.Split('\t');
                lon = float.Parse(e[1]);
            }
            else if (line.StartsWith("@Zoom"))
            {
                string[] e = line.Split('\t');
                zoom = int.Parse(e[1]);
            }
        }

    }
}
