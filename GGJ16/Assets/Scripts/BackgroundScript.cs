using UnityEngine;
using System.Collections;

public class BackgroundScript : MonoBehaviour {

    public Vector2 MapSize = new Vector2(1, 1);

    public int DividerLevel = 1; 

	// Use this for initialization
	void Start () {

        Texture2D texture = new Texture2D((int)MapSize.x, (int)MapSize.y); 

        for (int xdx = 0; xdx < MapSize.x; xdx++)
        {
            for (int ydx = 0; ydx < MapSize.y; ydx++)
            {
                texture.SetPixel(xdx, ydx, Color.black); 
            }
        }

        texture.Apply();
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, MapSize.x, MapSize.y), new Vector2());

       // GetComponent<S>()material.mainTexture = texture; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    
}
