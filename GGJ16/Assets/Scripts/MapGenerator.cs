using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.Networking;

public class MapGenerator : NetworkBehaviour {

    public int WidthSize = 100;

    public int HeightSize = 100;

    public List<GameObject> TileTypes = new List<GameObject>();

    public List<string> TileSets = new List<string>();

    public float Tilesize = 2.56f;

    public override void OnStartServer()
    {
        if (!isServer) return;

        //splits all the objects into something I can search with. 
        DualStore<TileDetails, GameObject> gameObjects = new DualStore<TileDetails, GameObject>(); 

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
            DualStore<TileDetails, GameObject> tileSetTiles = GetTileSetType(gameObjects, type); 

            foreach(TileType ttype in Enum.GetValues(typeof(TileType)))
            {
                level.AssignTiles(ttype, GetTileTypes(tileSetTiles, ttype)); 
            }

            tilesByTileSetType.Add(type, level); 
        }


        SpawnTiles(tilesByTileSetType); 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Retrieves the tileset from all the different tilesets. 
    /// </summary>
    /// <param name="allTiles"></param>
    /// <returns></returns>
    protected virtual DualStore<TileDetails, GameObject> GetTileset(DualStore<TileDetails, GameObject> allTiles)
    {
        //choose a tileset to play on. 
        int tileSetNo = UnityEngine.Random.Range(0, TileSets.Count);
        string tileSet = TileSets[tileSetNo];

        DualStore<TileDetails, GameObject> thisTileset = new DualStore<TileDetails, GameObject>();

        foreach (KeyValuePair<TileDetails, GameObject> kv in allTiles.KeyValuePairs)
        {
            if (kv.Key.TileSet == tileSet)
            {
                thisTileset.Add(kv.Key, kv.Value);
            }
        }

        return thisTileset; 
    }

    /// <summary>
    /// Gets all tiles of a specific set type. 
    /// </summary>
    /// <param name="allTiles"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    protected virtual DualStore<TileDetails, GameObject> GetTileSetType (DualStore<TileDetails, GameObject> allTiles, TileSetType type)
    {
        DualStore<TileDetails, GameObject> thisTileset = new DualStore<TileDetails, GameObject>();

        foreach (KeyValuePair<TileDetails, GameObject> kv in allTiles.KeyValuePairs)
        {
            if (kv.Key.Type == type)
            {
                thisTileset.Add(kv.Key, kv.Value);
            }
        }

        return thisTileset;
    }

    /// <summary>
    /// gets all the tiles of a specific tiletype. 
    /// </summary>
    /// <param name="tiles"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    protected virtual List<GameObject> GetTileTypes (DualStore<TileDetails, GameObject> tiles, TileType type)
    {
        List<GameObject> thetiles = new List<GameObject>(); 
        foreach (KeyValuePair<TileDetails, GameObject> kv in tiles.KeyValuePairs)
        {
            if (kv.Key.TileType == type)
            {
                thetiles.Add(kv.Value);
            }
        }

        return thetiles; 
    }

    /// <summary>
    /// Spawns in the tiles. 
    /// </summary>
    /// <param name="availiableTile"></param>
    protected virtual void SpawnTiles (Dictionary<TileSetType, TileLevel> availiableTile)
    {
        int dividerLevel = UnityEngine.Random.Range(0, HeightSize);

        TileType[,] map = new TileType[WidthSize, HeightSize];
        for (int xdx = 0; xdx < map.GetLength(0); xdx++)
        {
            for (int ydx = 0; ydx < map.GetLength(1); ydx++)
            {
                map[xdx, ydx] = TileType.None; 
            }
        }

        int platformNo = UnityEngine.Random.Range(1, HeightSize / 10); 

        for (int idx = 0; idx < platformNo; idx++)
        {
            int height = UnityEngine.Random.Range(0, HeightSize);
            int platformLength = UnityEngine.Random.Range(1, WidthSize);

            int platformStart = UnityEngine.Random.Range(1, platformLength);

            int lastDirection = 1; //0 is down, 1 is straight, 2 is up. 

            while (platformStart < platformLength)
            {
                if (map[platformStart, height] == TileType.None)
                {
                    float chance = UnityEngine.Random.value;
                    if (chance < 0.25f && height > 1 && lastDirection != 2)
                    {
                        //go down
                        map[platformStart, height] = TileType.RightSlope;
                        map[platformStart, height - 1] = TileType.RightSlopeCorner;
                        height--;
                        lastDirection = 0; 
                    }
                    else if (chance < 0.75f || height >= map.GetLength(1) - 1 || lastDirection == 0)
                    {
                        //stay same
                        map[platformStart, height] = TileType.Top;
                        lastDirection = 1; 
                    }
                    else
                    {
                        //go up. 
                        height++; 
                        map[platformStart, height] = TileType.LeftSlope;
                        map[platformStart, height - 1] = TileType.LeftSlopeCorner;
                        lastDirection = 2; 
                        
                    }

                   // map[platformStart, height] = TileType.Top;
                }
                platformStart++;
            }
        }

        
        InstantiateTiles(map, dividerLevel, availiableTile); 
        
    }

    /// <summary>
    /// Instantiates all the tiles. 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="dividerLevel"></param>
    /// <param name="availiableTile"></param>
    protected virtual void InstantiateTiles(TileType[,] map, int dividerLevel, Dictionary<TileSetType, TileLevel> availiableTile)
    {
        for (int xdx = 0; xdx < map.GetLength(0); xdx++)
        {
            for (int ydx = 0; ydx < map.GetLength(1); ydx++)
            {
                TileType type = map[xdx, ydx]; 
                if (type != TileType.None)
                {
                    TileSetType level = ydx > dividerLevel ? TileSetType.upperLevels : TileSetType.lowerLevels;

                    GameObject obj = availiableTile[level].GetTileType(type);
                    var tile = (GameObject) Instantiate(obj, new Vector3(xdx * Tilesize, ydx * Tilesize), new Quaternion());

                    Debug.Log(tile);
                    NetworkServer.Spawn(tile);
                }
            }
        }
    }
}
