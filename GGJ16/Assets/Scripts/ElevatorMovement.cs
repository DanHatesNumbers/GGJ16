using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElevatorMovement : MonoBehaviour {

    public Vector2 Top;

    public Vector2 Bottom;

    /// <summary>
    /// 1 is up, 2 is down. 
    /// </summary>
    int direction = 1;

    public float Speed = 1; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 currentPos = transform.position; 
        
        if (direction == 1)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + Speed);
            if (transform.position.y > Top.y)
            {
                direction = 2; 
            } 
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - Speed);
            if (transform.position.y < Bottom.y)
            {
                direction = 1;
            }
        }
	}
}
