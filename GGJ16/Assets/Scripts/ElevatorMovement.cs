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

    public Vector3 StartPos;

    public Vector3 EndPos;

    float Timer = 0;

    float TimeToTravel = 3; 

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

    public Vector3 trueValue;

	// Use this for initialization
	void Start () {
        trueValue = transform.position; 
	}
	
	// Update is called once per frame
	void Update () {

        
            Timer += Time.deltaTime;

            if (Timer > TimeToTravel)
            {
                Timer -= TimeToTravel;
                Vector3 temp = StartPos;
                StartPos = EndPos;
                EndPos = temp; 
            }

            transform.position = Vector3.Lerp(StartPos, EndPos, Timer / TimeToTravel);
            
            if (isClient)
            {
                trueValue = Vector3.Lerp(transform.position, trueValue, Timer / TimeToTravel);
                transform.position = trueValue;
            }
	}
}
