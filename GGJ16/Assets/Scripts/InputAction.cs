using System;
using UnityEngine;

namespace AssemblyCSharp
{
	public class InputAction
	{
		public Func<bool> IsTriggered;
		public Action<GameObject, float> PlayerAction;
	}
}
