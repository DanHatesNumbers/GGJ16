﻿using UnityEngine;
using System.Collections;

public class RandomRotationOnSpawn : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
        this.transform.Rotate(0,0,Random.Range(0, 359.99f));
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    void OnCollisionEnter2D(Collision2D Col)
    {
        if((Col.gameObject.name == "LavaBottom") || (Col.gameObject.name == "KillZ"))
        {
            Destroy(this.gameObject);
        }

    }
}
