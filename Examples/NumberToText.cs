using System;
using UnityEngine;
using UnityEngine.Events;

namespace PWR.LowPowerMemoryConsumption.Examples {

	public class NumberToText : MonoBehaviour {

		[Serializable]
		public class Event : UnityEvent<string> { }

		public string prefix = null;

		public string suffix = null;

		public string FormatFloat = "0.00";

		public string FormatInt = "0";

		public Event TextEvent;

		public void TriggerNumber(float num) {
			if (this.TextEvent == null) return;
			this.TextEvent.Invoke(this.prefix + num.ToString(this.FormatFloat) + this.suffix);
		}

		public void TriggerNumber(int num) {
			if (this.TextEvent == null) return;
			this.TextEvent.Invoke(this.prefix + num.ToString(this.FormatInt) + this.suffix);
		}
	}
}