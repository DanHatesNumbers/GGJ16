using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public enum MovementDirection
{
    leftright, 
    updown
}

public class ElevatorMovement : NetworkBehaviour {

    public Vector2 Top;

    public Vector2 Bottom;

    public Vector2 Left;

    public Vector2 Right;

    /// <summary>
    /// 1 is up, 2 is down. 
    /// </summary>
    int direction = 2;

    public float Speed = 0.125f;

    public MovementDirection moveDir;

    public int updateSpeed = 5;

    private int currentUpdate = 0; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (isServer ) //&& currentUpdate > updateSpeed)
        {
            if (moveDir == MovementDirection.updown)
            {

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
            else if (moveDir == MovementDirection.leftright)
            {
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
            currentUpdate = 0; 
        }
        else
        {
            currentUpdate++; 
        }
	}
}
