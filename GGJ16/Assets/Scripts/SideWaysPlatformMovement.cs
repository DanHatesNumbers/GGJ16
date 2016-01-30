using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SidewaysPlatformMovement : MonoBehaviour {

    public Vector2 Left;

    public Vector2 Right;

    /// <summary>
    /// 1 is left, 2 is right. 
    /// </summary>
    int direction = 2;

    public float Speed = 0.125f; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 currentPos = transform.position; 
        
        if (direction == 1)
        {
            transform.position = new Vector3(transform.position.x + Speed, transform.position.y);
            if (transform.position.x > Left.x)
            {
                direction = 2; 
            } 
        }
        else
        {
            transform.position = new Vector3(transform.position.x - Speed, transform.position.y);
            if (transform.position.x < Right.x)
            {
                direction = 1;
            }
        }
	}
}
