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
	}
}
