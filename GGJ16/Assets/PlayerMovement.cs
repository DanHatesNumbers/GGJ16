using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using XboxCtrlrInput;
using System;

public class PlayerMovement : MonoBehaviour {

	public GameObject Player;
    public GameObject Fireball;

    public float LastFireTime;
    public const float FireballCooldown = 0.5f;

    public bool FacingLeft;

	public List<InputAction> InputActions;

	public const float leftThreshold = -0.1f;
	public const float rightThreshold = 0.1f;

	// Use this for initialization
	void Start () {
		InputActions = new List<InputAction> ();
		var inputJump = new InputAction 
		{
			IsTriggered = () => Input.GetButtonDown("Jump") && Player.GetComponent<Collider2D>().IsTouchingLayers(),
			PlayerAction = InputAction.JumpAction
		};

		var inputMoveLeft = new InputAction 
		{
			IsTriggered = () => Input.GetAxis("Horizontal") < leftThreshold,
			PlayerAction = InputAction.MoveLeftAction
		};

		var inputMoveRight = new InputAction 
		{
			IsTriggered = () => Input.GetAxis("Horizontal") > rightThreshold,
			PlayerAction = InputAction.MoveRightAction
		};

        var inputFireball = new InputAction
        {
            IsTriggered = () => (Input.GetAxis("Fire1") != 0) && CanFireball(),
            PlayerAction = p => SpawnFireball()
        };

		InputActions.Add(inputJump);
		InputActions.Add(inputMoveLeft);
		InputActions.Add(inputMoveRight);
        InputActions.Add(inputFireball);
        LastFireTime = Time.time - FireballCooldown;
        FacingLeft = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		foreach (var action in InputActions) 
		{
			if(action.IsTriggered())
			{
				action.PlayerAction(Player);
			}
		}
	}

    void SpawnFireball()
    {
        var fireball = (GameObject)Instantiate(Fireball, Player.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
        var fireballVelocity = FacingLeft ? new Vector2(-5f, 0f) : new Vector2(5f, 0f);
        fireball.GetComponent<Rigidbody2D>().velocity = fireballVelocity;
        LastFireTime = Time.time;
    }

    bool CanFireball()
    {
        return LastFireTime < Time.time - FireballCooldown;
    }
}
