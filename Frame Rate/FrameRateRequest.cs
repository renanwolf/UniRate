//using System.Collections;
//using System.Collections.Generic;
using System;
using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	public class FrameRateRequest {

		#region <<---------- Initializers ---------->>

		public FrameRateRequest(FrameRateType type, int requestedValue) {
			this._value = requestedValue;
			this._type = type;
			this.onRequestChanged = null;
		}

		public FrameRateRequest(FrameRateType type) : this(type, -1) { }

		public FrameRateRequest() : this(FrameRateType.FPS) { }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Properties and Fields ---------->>

		private int _value;
		private FrameRateType _type;

		public int Value {
			get { return this._value; }
			set {
				if (this._value == value) return;
				this._value = value;
				if (this.onRequestChanged != null) this.onRequestChanged(this);
			}
		}

		public FrameRateType Type {
			get { return this._type; }
			set {
				if (this._type == value) return;
				this._type = value;
				if (this.onRequestChanged != null) this.onRequestChanged(this);
			}
		}

		public bool IsValid {
			get { return this._value >= MinValueForType(this._type); }
		}

		public Action<FrameRateRequest> onRequestChanged;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- General ---------->>

		public static int MinValueForType(FrameRateType type) {
			switch (type) {
				case FrameRateType.FPS: return 0;
				case FrameRateType.FixedFPS: return 1;
				default: return int.MaxValue;
			}
		}

		public FrameRateRequest Start() {
			FrameRateManager.Instance.AddRequest(this);
			return this;
		}

		public void Stop() {
			FrameRateManager.Instance.RemoveRequest(this);
		}

		#endregion <<---------- General ---------->>




		#region <<---------- Fluent Interface Pattern ---------->>

		public static FrameRateRequest FPS() {
			return new FrameRateRequest(FrameRateType.FPS);
		}

		public static FrameRateRequest FixedFPS() {
			return new FrameRateRequest(FrameRateType.FixedFPS);
		}

		public FrameRateRequest WithRate(int requestedValue) {
			this.Value = requestedValue;
			return this;
		}		

		#endregion <<---------- Fluent Interface Pattern ---------->>
	}
}