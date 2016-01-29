using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class InputAction
	{
		public Func<bool> IsTriggered;
		public Action<GameObject> PlayerAction;

		public static Action<GameObject> JumpAction = c => 
		{
			var rb = c.GetComponent<Rigidbody2D>();
			rb.AddForce(new Vector2(0f, 5f), ForceMode2D.Impulse);
		};

		public static Action<GameObject> MoveLeftAction = c => 
		{
			var rb = c.GetComponent<Rigidbody2D>();
			rb.AddForce(new Vector2(-0.2f, 0f), ForceMode2D.Impulse);
		};

		public static Action<GameObject> MoveRightAction = c => 
		{
			var rb = c.GetComponent<Rigidbody2D>();
			rb.AddForce(new Vector2(0.2f, 0f), ForceMode2D.Impulse);
		};
	}
}