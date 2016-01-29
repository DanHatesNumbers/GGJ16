using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class MapGenerator : MonoBehaviour {

    public int WidthSize = 100;

    public int HeightSize = 100;

    public List<GameObject> TileTypes = new List<GameObject>();

    public List<string> TileSets = new List<string>(); 



	// Use this for initialization
	void Start () {
        //splits all the objects into something I can search with. 
        Dictionary<TileDetails, GameObject> gameObjects = new Dictionary<TileDetails, GameObject>(); 

        foreach(GameObject obj in TileTypes)
        {
            TileDetails details = obj.GetComponent<TileDetails>();
            gameObjects.Add(details, obj); 
        }


        gameObjects = GetTileset(gameObjects);

        Dictionary<TileSetType, TileLevel> tilesByTileSetType = new Dictionary<TileSetType, TileLevel>(); 

        foreach(TileSetType type in Enum.GetValues(typeof(TileSetType)))
        {
            TileLevel level = new TileLevel(UnityEngine.Random.seed);
            Dictionary<TileDetails, GameObject> tileSetTiles = GetTileSetType(gameObjects, type); 

            foreach(TileType ttype in Enum.GetValues(typeof(TileType)))
            {
                level.AssignTiles(ttype, GetTileTypes(tileSetTiles, ttype)); 
            }
        }

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Retrieves the tileset from all the different tilesets. 
    /// </summary>
    /// <param name="allTiles"></param>
    /// <returns></returns>
    protected virtual Dictionary<TileDetails, GameObject> GetTileset(Dictionary<TileDetails, GameObject> allTiles)
    {
        //choose a tileset to play on. 
        int tileSetNo = UnityEngine.Random.Range(0, TileSets.Count);
        string tileSet = TileSets[tileSetNo];

        Dictionary<TileDetails, GameObject> thisTileset = new Dictionary<TileDetails, GameObject>();

        foreach (KeyValuePair<TileDetails, GameObject> kv in allTiles)
        {
            if (kv.Key.TileSet == tileSet)
            {
                thisTileset.Add(kv.Key, kv.Value);
            }
        }

        return thisTileset; 
    }

    /// <summary>
    /// Gets all tiles of a specific type. 
    /// </summary>
    /// <param name="allTiles"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    protected virtual Dictionary<TileDetails, GameObject> GetTileSetType (Dictionary<TileDetails, GameObject> allTiles, TileSetType type)
    {
        Dictionary<TileDetails, GameObject> thisTileset = new Dictionary<TileDetails, GameObject>();

        foreach (KeyValuePair<TileDetails, GameObject> kv in allTiles)
        {
            if (kv.Key.Type == type)
            {
                thisTileset.Add(kv.Key, kv.Value);
            }
        }

        return thisTileset;
    }

    protected virtual List<GameObject> GetTileTypes (Dictionary<TileDetails, GameObject> tiles, TileType type)
    {
        List<GameObject> thetiles = new List<GameObject>(); 
        foreach (KeyValuePair<TileDetails, GameObject> kv in tiles)
        {
            if (kv.Key.TileType == type)
            {
                thetiles.Add(kv.Value);
            }
        }

        return thetiles; 
    }
}
