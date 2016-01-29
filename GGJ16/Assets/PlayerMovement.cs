using UnityEngine;
using System.Collections.Generic;
using AssemblyCSharp;
using XboxCtrlrInput;

public class PlayerMovement : MonoBehaviour {

	public GameObject Player;

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

		InputActions.Add (inputJump);
		InputActions.Add (inputMoveLeft);
		InputActions.Add (inputMoveRight);
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
}
