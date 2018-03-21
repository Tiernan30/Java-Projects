using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class showDirection : MonoBehaviour {
    Texture2D mTex;

    Texture2D nTex;
    Texture2D eTex;
    Texture2D sTex;
    Texture2D wTex;
    Texture2D dTex;

    Texture2D zTex;
    Texture2D ziTex;
    Texture2D zoTex;

    Texture2D fTex;
    Texture2D bTex;
    Texture2D fbTex;

    Texture2D emptyTex;

    public char default_texture;

    MeshRenderer mesh;

    // Use this for initialization
    void Start () {
        mesh = this.GetComponent<MeshRenderer>();
       // mesh.material.mainTexture = mTex;

        byte[] imageBytes = File.ReadAllBytes(@"Assets/Skins/textures/latlon.png");
        dTex = new Texture2D(256, 256);
        dTex.LoadImage(imageBytes);

        imageBytes = File.ReadAllBytes(@"Assets/Skins/textures/latlonN.png");
        nTex = new Texture2D(256, 256);
        nTex.LoadImage(imageBytes);

        imageBytes = File.ReadAllBytes(@"Assets/Skins/textures/latlonE.png");
        eTex = new Texture2D(256, 256);
        eTex.LoadImage(imageBytes);

        imageBytes = File.ReadAllBytes(@"Assets/Skins/textures/latlonS.png");
        sTex = new Texture2D(256, 256);
        sTex.LoadImage(imageBytes);

        imageBytes = File.ReadAllBytes(@"Assets/Skins/textures/latlonW.png");
        wTex = new Texture2D(256, 256);
        wTex.LoadImage(imageBytes);

        imageBytes = File.ReadAllBytes(@"Assets/Skins/textures/zoominout.png");
        zTex = new Texture2D(256, 256);
        zTex.LoadImage(imageBytes);

        imageBytes = File.ReadAllBytes(@"Assets/Skins/textures/zoomin.png");
        ziTex = new Texture2D(256, 256);
        ziTex.LoadImage(imageBytes);

        imageBytes = File.ReadAllBytes(@"Assets/Skins/textures/zoomout.png");
        zoTex = new Texture2D(256, 256);
        zoTex.LoadImage(imageBytes);

        //
        imageBytes = File.ReadAllBytes(@"Assets/Skins/textures/moveF.png");
        fTex = new Texture2D(256, 256);
        fTex.LoadImage(imageBytes);

        imageBytes = File.ReadAllBytes(@"Assets/Skins/textures/moveB.png");
        bTex = new Texture2D(256, 256);
        bTex.LoadImage(imageBytes);

        imageBytes = File.ReadAllBytes(@"Assets/Skins/textures/moveFB.png");
        fbTex = new Texture2D(256, 256);
        fbTex.LoadImage(imageBytes);

        emptyTex = new Texture2D(256, 256);
        for (int i = 0; i < 256; i++)
            for (int j = 0; j < 256; j++)
                emptyTex.SetPixel(i, j, new Color(1, 1, 1, 0));


        emptyTex.Apply();

        bTex.Apply();
        fTex.Apply();
        fbTex.Apply();

        zTex.Apply();
        ziTex.Apply();
        zoTex.Apply();

        dTex.Apply();
        nTex.Apply();
        eTex.Apply();
        sTex.Apply();
        wTex.Apply();

        changeTex(default_texture);
    }
	/// <summary>
    /// Changes given mesh to a ttexture from the list
    /// d is default for nesw
    /// n is north
    /// e is east
    /// s is south
    /// w is west
    /// i is zoom in
    /// o is zoom out
    /// z is default zoom
    /// f is forward move
    /// b is backwards move
    /// m is default move
    /// l is invisible
    /// </summary>
    /// <param name="d"></param>
    public void changeTex(char d)
    {
        try
        {
            switch (d)
            {
                case 'd':
                    mesh.material.mainTexture = dTex;
                    break;
                case 'n':
                    mesh.material.mainTexture = nTex;
                    break;
                case 'e':
                    mesh.material.mainTexture = eTex;
                    break;
                case 's':
                    mesh.material.mainTexture = sTex;
                    break;
                case 'w':
                    mesh.material.mainTexture = wTex;
                    break;
                case 'i':
                    mesh.material.mainTexture = ziTex;
                    break;
                case 'o':
                    mesh.material.mainTexture = zoTex;
                    break;
                case 'z':
                    mesh.material.mainTexture = zTex;
                    break;
                case 'f':
                    mesh.material.mainTexture = fTex;
                    break;
                case 'b':
                    mesh.material.mainTexture = bTex;
                    break;
                case 'm':
                    mesh.material.mainTexture = fbTex;
                    break;
                case 'l':
                    mesh.material.mainTexture = emptyTex;
                    break;
            }
        }
        catch (System.NullReferenceException e)
        {

        }
        
    }

	// Update is called once per frame
	void Update () {
		
	}
}
