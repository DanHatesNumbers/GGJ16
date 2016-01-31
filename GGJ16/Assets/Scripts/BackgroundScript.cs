using UnityEngine;
using System.Collections;

public class BackgroundScript : MonoBehaviour {

    public Vector2 MapSize = new Vector2(100 * 2.56f, 100 * 2.56f);

    public int DividerLevel = 1;

    public int PixelsToUnits = 1;

    public int scale = 10; 

	// Use this for initialization
	void Start () {
        Vector2 mapSize = new Vector2(MapSize.x * PixelsToUnits, MapSize.y * PixelsToUnits);
        Texture2D texture = new Texture2D((int)mapSize.x, (int)mapSize.y, TextureFormat.ARGB32, false); 

        for (int xdx = 0; xdx < mapSize.x; xdx++)
        {
            for (int ydx = 0; ydx < mapSize.y; ydx++)
            {
                float noiseVal = Mathf.PerlinNoise(xdx + 0.01f, ydx + 0.01f);
                //Debug.Log("Noise Val for " + xdx + ", " + ydx + "is " + noiseVal); 
                texture.SetPixel(xdx, ydx, new Color(noiseVal, noiseVal, noiseVal)); 

                //if (xdx % 2 != 0)
                //{
                //    texture.SetPixel(xdx, ydx, Color.white);
                //}
                //else
                //{
                //    texture.SetPixel(xdx, ydx, Color.black);
                //}
            }
        }

        texture.Apply();
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, MapSize.x, MapSize.y), new Vector2(), PixelsToUnits);

       // GetComponent<S>()material.mainTexture = texture; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    
}
