using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine; 

public class TileLevel
{
    Dictionary<TileType, List<GameObject>> _tiles;

    System.Random rand; 
    
    public TileLevel(int seed)
    {
        _tiles = new Dictionary<TileType, List<GameObject>>();
        rand = new System.Random(seed); 
    } 


    public void AssignTiles(TileType type, List<GameObject> thetiles)
    {
        if (!_tiles.ContainsKey(type))
        {
            _tiles.Add(type, thetiles); 
        }
    }

    public GameObject GetTileType (TileType type)
    {
        if (!_tiles.ContainsKey(type) || _tiles[type].Count < 1)
        {
            throw new TileNotFoundException("Cannot find tile of type " + type.ToString()); 
        }

        List<GameObject> possible = _tiles[type];

        return possible[rand.Next(possible.Count)]; 
    }
}

