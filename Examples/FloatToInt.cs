using System;
using UnityEngine;
using UnityEngine.Events;

namespace PWR.LowPowerMemoryConsumption.Examples {

	public class FloatToInt : MonoBehaviour {

		[Serializable]
		public class Event : UnityEvent<int> { }

		public Event IntEvent;

		public void TriggerFloat(float num) {
			if (this.IntEvent == null) return;
			this.IntEvent.Invoke(Mathf.RoundToInt(num));
		}
	}
}