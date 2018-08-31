using System;
using UnityEngine.Events;

namespace PWR.LowPowerMemoryConsumption {

	[Serializable]
	public class UnityEventInt : UnityEvent<int> { }

	[Serializable]
	public class UnityEventBool : UnityEvent<bool> { }
}