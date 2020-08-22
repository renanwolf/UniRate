using System;
using UnityEngine.Events;

namespace PWR.LowPowerMemoryConsumption {

	[Serializable]
	[Obsolete]
	public class UnityEventInt : UnityEvent<int> { }

	[Serializable]
	[Obsolete]
	public class UnityEventBool : UnityEvent<bool> { }
}