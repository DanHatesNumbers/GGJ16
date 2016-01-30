using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PowerUpSpawner : NetworkBehaviour {

    public List<GameObject> PowerUpRepo;
    public float SpawnRate = 5.0f;
    private float timeSinceLastSpawn = 0;

    private MapGenerator MG;

	// Use this for initialization
	void Start ()
    {
        if (PowerUpRepo.Count == 0)
        {
            Debug.Log("OI! YOU NEED TO PUT SOME POWERUPS IN THE SPAWNER!");
        }
        MG = this.GetComponent<MapGenerator>();
        if (MG == null)
        {
            Debug.Log("OMG MAP GENERATOR NOT FOUND!");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn > SpawnRate)
        {
            timeSinceLastSpawn -= SpawnRate;
            CmdSpawnPowerUp();
        }
	}

    [Command]
    void CmdSpawnPowerUp()
    {
        int spawnIndex = Random.Range(0, PowerUpRepo.Count - 1);
        Debug.Log(PowerUpRepo[spawnIndex].name);
        GameObject Spawned = GameObject.Instantiate(PowerUpRepo[spawnIndex]);
        Spawned.transform.position = new Vector3(Random.Range(3f, ((float)(MG.WidthSize - 3) * 2.56f)),
                                            Random.Range(3f, ((float)(MG.HeightSize) * 2.56f)));
        Spawned.name = PowerUpRepo[spawnIndex].name;

        NetworkServer.Spawn(Spawned);
    }
}
