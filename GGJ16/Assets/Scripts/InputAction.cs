using System;
using UnityEngine;
using UnityEngine.Audio;

namespace AssemblyCSharp
{
	public class InputAction
	{
		public Func<bool> IsTriggered;
		public Action<GameObject, float> PlayerAction;
        public AudioClip AudioClip;

        public static Action<GameObject, float> JumpAction = (c, delta) => 
		{
            var forceAmount = 13f;
            //Debug.Log(String.Format("Jump input action delta: {0}, total force: {1}", delta, forceAmount));
			var rb = c.GetComponent<Rigidbody2D>();
            rb.AddForce(new Vector2(0f, forceAmount), ForceMode2D.Impulse);
		};

        public static Action<GameObject, float> MoveLeftAction = (c, delta) => 
		{
            var forceAmount = -0.2f * delta * 100;
            Debug.Log(String.Format("Move left input action delta: {0}, total force: {1}", delta, forceAmount));
			var rb = c.GetComponent<Rigidbody2D>();
			rb.AddForce(new Vector2(forceAmount, 0f), ForceMode2D.Impulse);
            c.GetComponent<PlayerMovement>().FacingLeft = true;
		};

        public static Action<GameObject, float> MoveRightAction = (c, delta) => 
		{
            var forceAmount = 0.2f * delta * 100;
            Debug.Log(String.Format("Move right input action delta: {0}, total force: {1}", delta, forceAmount));
            var rb = c.GetComponent<Rigidbody2D>();
			rb.AddForce(new Vector2(forceAmount, 0f), ForceMode2D.Impulse);
            c.GetComponent<PlayerMovement>().FacingLeft = false;
		};
	}
}
