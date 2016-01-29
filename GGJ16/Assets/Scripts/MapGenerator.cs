using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 

public class MapGenerator : MonoBehaviour {

    public int WidthSize = 100;

    public int HeightSize = 100;

    public List<GameObject> TileTypes = new List<GameObject>();

    public List<List<GameObject>> TileSets = new List<List<GameObject>>()
    {

    }; 

	// Use this for initialization
	void Start () {

        //temp code. 
        foreach (GameObject temp in TileTypes)
        {
            Instantiate(temp); 
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
