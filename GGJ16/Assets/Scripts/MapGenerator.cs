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

    public GameObject SpawnPrefab;

    public int MinNumOfPlatforms = 100;

    public int MaxNumOfPlatforms = 400;

    public int MinPlatformTotalTiles = 10;

    public int numberOfSpawns = 10;

    public GameObject backgroundPrefab; 

    System.Random rand; 

    public override void OnStartServer()
    {
        if (!isServer) return;
        int seed = UnityEngine.Random.seed; 
        rand = new System.Random(seed); 

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
            TileLevel level = new TileLevel(seed);
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
        int dividerLevel = UnityEngine.Random.Range(3, HeightSize);

        TileType[,] map = GenerateMap(); 

        List<Vector2> SpawnPoints = GenerateSpawnPoints(map);

        //GenerateElevators(map, dividerLevel, availiableTile); 

        InstantiateTiles(map, dividerLevel, availiableTile, SpawnPoints); 
        
    }

    /// <summary>
    /// Generates the map. 
    /// </summary>
    /// <returns></returns>
    protected virtual TileType[,] GenerateMap()
    {
        TileType[,] map = new TileType[WidthSize, HeightSize];
        for (int xdx = 0; xdx < map.GetLength(0); xdx++)
        {
            for (int ydx = 0; ydx < map.GetLength(1); ydx++)
            {
                if (xdx == 0)
                {
                    map[xdx, ydx] = TileType.LeftWall;
                }
                else if (xdx == map.GetLength(0) - 1)
                {
                    map[xdx, ydx] = TileType.RightWall;
                }
                else if (ydx == 0)
                {
                    map[xdx, ydx] = TileType.Lava;
                }
                else
                {
                    map[xdx, ydx] = TileType.None;
                }
            }
        }



        int platformNo = UnityEngine.Random.Range(MinNumOfPlatforms, MaxNumOfPlatforms);
        int platformTileNo = 0; 

        for (int idx = 0; idx < platformNo; idx++)
        {
            int height = UnityEngine.Random.Range(0, HeightSize);
            

            int platformStart = UnityEngine.Random.Range(1, WidthSize - 1);
            //platformStart -= platformLength; 
            //if (platformStart <= 0)
            //{
            //    platformStart = 1; 
            //}
            int platformLength = UnityEngine.Random.Range(2, WidthSize - 2);

            int lastDirection = 1; //0 is down, 1 is straight, 2 is up. 

            bool skipGeneration = false; 

            if (map[platformStart, height] == TileType.None)
            {
                map[platformStart, height] = TileType.LeftEnd;
                platformTileNo++;
                platformStart++; 
            }
            else
            {
                platformNo--;
                platformStart = platformLength;
                skipGeneration = true; 
            }

            while (platformStart < platformLength - 1 && platformStart < map.GetLength(0))
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
                        platformTileNo++;
                    }
                    else if (chance < 0.75f || height >= map.GetLength(1) - 1 || lastDirection == 0)
                    {
                        //stay same
                        map[platformStart, height] = TileType.Top;
                        lastDirection = 1;
                        platformTileNo++;
                    }
                    else
                    {
                        //go up. 
                        height++;
                        map[platformStart, height] = TileType.LeftSlope;
                        map[platformStart, height - 1] = TileType.LeftSlopeCorner;
                        lastDirection = 2;
                        platformTileNo++;

                    }

                    // map[platformStart, height] = TileType.Top;
                }
                else
                {
                    map[platformStart - 1, height] = TileType.RightEnd;
                    platformStart = platformLength; 
                }

                platformStart++;
            }

            if (map[platformStart, height] == TileType.None && !skipGeneration)
            {
                map[platformStart, height] = TileType.RightEnd;
                platformTileNo++;
            }


            if (idx == platformNo - 1 && platformTileNo < MinPlatformTotalTiles)
            {
                idx--; 
            }
        }

        return map; 
    }

    /// <summary>
    /// Generates the spawnpoints used for spawning. 
    /// </summary>
    /// <param name="map"></param>
    /// <returns></returns>
    protected virtual List<Vector2> GenerateSpawnPoints(TileType[,] map)
    {
        List<Vector2> SpawnPoints = new List<Vector2>();
        //generate spawn points. 
        //int noOfSpawns = UnityEngine.Random.Range(1, WidthSize / 10);

        for (int idx = 0; idx < numberOfSpawns; idx++)
        {
            bool notadded = true;

            while (notadded)
            {
                int xdx = UnityEngine.Random.Range(1, WidthSize - 1);
                bool foundPlatform = false;
                int ydx = HeightSize - 1;
                while (foundPlatform != true && ydx > 0)
                {
                    if (map[xdx, ydx] != TileType.None)
                    {
                        bool notNear = true;
                        Vector2 thisV = new Vector2(xdx, ydx + 1); 
                        foreach (Vector2 other in SpawnPoints)
                        {
                            if (Vector2.Distance(other, thisV) < 3)
                            {
                                notNear = false; 
                            }
                        }

                        if (notNear)
                        {
                            SpawnPoints.Add(thisV);
                            
                            notadded = false;
                        }

                        foundPlatform = true;
                    }

                    ydx--;
                }
            }
        }

        return SpawnPoints; 
    }

    /// <summary>
    /// Creates an instantites the tiles. 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="dividerLevel"></param>
    /// <param name="availiableTile"></param>
    protected virtual void GenerateElevators(TileType[,] map, int dividerLevel, Dictionary<TileSetType, TileLevel> availiableTile)
    {
        List<Vector2> leftEnds = new List<Vector2>();
        List<Vector2> rightEnds = new List<Vector2>(); 

        for (int xdx = 0; xdx < map.GetLength(0); xdx++)
        {
            for (int ydx = 0; ydx < map.GetLength(1); ydx++)
            {
                TileType type = map[xdx, ydx]; 

                if (type == TileType.LeftEnd)
                {
                    leftEnds.Add(new Vector2(xdx, ydx)); 
                }
                else if (type == TileType.RightEnd)
                {
                    rightEnds.Add(new Vector2(xdx, ydx)); 
                }
            }
        }

        DualStore<Vector2, Vector2> veritcal = new DualStore<Vector2, Vector2>(); 

        foreach (Vector2 vec in rightEnds)
        {
            int xdx = (int)vec.x + 1;
            //int ydx = (int)vec.y; 

            for (int ydx = (int)vec.y - 1; ydx > 0; ydx--)
            {
                if (map[xdx, ydx] != TileType.None)
                {
                    veritcal.Add(new Vector2(xdx, vec.y), new Vector2(xdx, ydx + 1));
                    ydx = -1; 
                }
                else if (map[xdx - 1, ydx] == TileType.RightEnd)
                {
                    veritcal.Add(new Vector2(xdx, vec.y), new Vector2(xdx, ydx));
                    ydx = - 1; 
                }
                else if (map[xdx + 1, ydx] == TileType.LeftEnd)
                {
                    veritcal.Add(new Vector2(xdx, vec.y), new Vector2(xdx, ydx));
                    ydx = -1; 
                }
            }
        }

        foreach (Vector2 vec in leftEnds)
        {
            int xdx = (int)vec.x - 1;
            //int ydx = (int)vec.y; 

            for (int ydx = (int)vec.y - 1; ydx > 0; ydx--)
            {
                if (map[xdx, ydx] != TileType.None)
                {
                    veritcal.Add(new Vector2(xdx, vec.y), new Vector2(xdx, ydx + 1));
                    ydx = -1;
                }
                else if (map[xdx - 1, ydx] == TileType.RightEnd)
                {
                    veritcal.Add(new Vector2(xdx, vec.y), new Vector2(xdx, ydx));
                    ydx = -1;
                }
                else if (map[xdx + 1, ydx] == TileType.LeftEnd)
                {
                    veritcal.Add(new Vector2(xdx, vec.y), new Vector2(xdx, ydx));
                    ydx = -1;
                }
            }
        }

        foreach (KeyValuePair<Vector2, Vector2> kv in veritcal.KeyValuePairs)
        {
            TileSetType level = kv.Value.y > dividerLevel ? TileSetType.upperLevels : TileSetType.lowerLevels;

            GameObject obj = availiableTile[level].GetTileType(TileType.Platform);
            

            var tile = (GameObject)Instantiate(obj, new Vector3(kv.Key.x * Tilesize, kv.Key.y * Tilesize), new Quaternion());
            ElevatorMovement move = tile.GetComponent(typeof(ElevatorMovement)) as ElevatorMovement;
            move.EndPos = new Vector2(kv.Value.x * Tilesize, kv.Value.y * Tilesize);
            move.StartPos = new Vector2(kv.Key.x * Tilesize, kv.Key.y * Tilesize);
            move.moveDir = MovementDirection.updown; 
            //ElevatorMovement move = tile.GetComponent<ElevatorMovement>();
            
            Debug.Log(tile);
            NetworkServer.Spawn(tile);
        }

        DualStore<Vector2, Vector2> horizontal = new DualStore<Vector2, Vector2>();

        foreach (Vector2 vec in rightEnds)
        {
            int ydx = (int)vec.y; 

            for (int xdx = (int)vec.x + 1; xdx < map.GetLength(0) - 1; xdx++)
            {
                if (map[xdx, ydx] != TileType.None)
                {
                    horizontal.Add(new Vector2(vec.x, vec.y), new Vector2(xdx - 1, ydx));
                    xdx = map.GetLength(0); 
                }
            }


           
        }

        foreach (KeyValuePair<Vector2, Vector2> kv in horizontal.KeyValuePairs)
        {
            TileSetType level = kv.Value.y > dividerLevel ? TileSetType.upperLevels : TileSetType.lowerLevels;

            GameObject obj = availiableTile[level].GetTileType(TileType.Platform);


            var tile = (GameObject)Instantiate(obj, new Vector3(kv.Key.x * Tilesize, kv.Key.y * Tilesize), new Quaternion());
            ElevatorMovement move = tile.GetComponent(typeof(ElevatorMovement)) as ElevatorMovement;
            move.EndPos = new Vector2(kv.Value.x * Tilesize, kv.Value.y * Tilesize);
            move.StartPos = new Vector2(kv.Key.x * Tilesize, kv.Key.y * Tilesize);
            move.moveDir = MovementDirection.leftright; 
            //ElevatorMovement move = tile.GetComponent<ElevatorMovement>();

            Debug.Log(tile);
            NetworkServer.Spawn(tile);
        }

    }
    /// <summary>
    /// Instantiates all the tiles. 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="dividerLevel"></param>
    /// <param name="availiableTile"></param>
    protected virtual void InstantiateTiles(TileType[,] map, int dividerLevel, Dictionary<TileSetType, TileLevel> availiableTile, List<Vector2> spawnPoints)
    {
        for (int xdx = 0; xdx < map.GetLength(0); xdx++)
        {
            for (int ydx = 0; ydx < map.GetLength(1); ydx++)
            {
                TileType type = map[xdx, ydx]; 
                if (type != TileType.None && type != TileType.Lava)
                {
                    TileSetType level = ydx > dividerLevel ? TileSetType.upperLevels : TileSetType.lowerLevels;

                    GameObject obj = availiableTile[level].GetTileType(type);
                    var tile = (GameObject) Instantiate(obj, new Vector3(xdx * Tilesize, ydx * Tilesize), new Quaternion());

                    Debug.Log(tile);
                    NetworkServer.Spawn(tile);
                }
                else if (type == TileType.Lava)
                {
                    GameObject obj = availiableTile[TileSetType.lava].GetTileType(TileType.Top);
                    var tile = (GameObject)Instantiate(obj, new Vector3(xdx * Tilesize, ydx * Tilesize), new Quaternion());
                    tile.name = "Lava"; 
                    Debug.Log(tile);
                    NetworkServer.Spawn(tile);
                }

                //GameObject bck = availiableTile[TileSetType.background].GetTileType(TileType.Filler);
                //var backt = (GameObject)Instantiate(bck, new Vector3(xdx * Tilesize, ydx * Tilesize), new Quaternion());
                //backt.name = "background";
                //Debug.Log(backt);
                //NetworkServer.Spawn(backt); 
            }
        }

        foreach(Vector2 point in spawnPoints)
        {
            var spawn = (GameObject)Instantiate(SpawnPrefab, new Vector3(point.x * Tilesize, point.y * Tilesize), new Quaternion());
            spawn.name = "spawnPoint"; 
            Debug.Log(spawn); 
            NetworkServer.Spawn(spawn); 
        }

        //GameObject back = backgroundPrefab;
        
        var backobj = (GameObject)Instantiate(backgroundPrefab, new Vector3(0, 0), new Quaternion());
        BackgroundScript move = backobj.GetComponent(typeof(BackgroundScript)) as BackgroundScript;
        move.MapSize = new Vector2(WidthSize * Tilesize, HeightSize * Tilesize);
        move.DividerLevel = dividerLevel;
        Debug.Log(backobj);
        NetworkServer.Spawn(backobj); 


    }
}
