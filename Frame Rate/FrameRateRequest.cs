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

		/// <summary>
		/// Rate value.
		/// </summary>
		public int Value {
			get { return this._value; }
			set {
				if (this._value == value) return;
				this._value = value;
				if (this.onRequestChanged != null) this.onRequestChanged(this);
			}
		}

		/// <summary>
		/// Rate type.
		/// </summary>
		public FrameRateType Type {
			get { return this._type; }
			set {
				if (this._type == value) return;
				this._type = value;
				if (this.onRequestChanged != null) this.onRequestChanged(this);
			}
		}

		/// <summary>
		/// Is valid if FPS with value ≥ 0 or FixedFPS with value ≥ 1.
		/// </summary>
		public bool IsValid {
			get { return this._value >= MinValueForType(this._type); }
		}

		/// <summary>
		/// Action invoked when <see cref="Value"/> or <see cref="Type"/> are changed.
		/// </summary>
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

		/// <summary>
		/// Add the frame request on <see cref="FrameRateManager"/>.
		/// </summary>
		/// <returns>Returns this instance to use as fluent interface.</returns>
		public FrameRateRequest Start() {
			FrameRateManager.Instance.AddRequest(this);
			return this;
		}

		/// <summary>
		/// Remove the frame request from <see cref="FrameRateManager"/>.
		/// </summary>
		public void Stop() {
			FrameRateManager.Instance.RemoveRequest(this);
		}

		#endregion <<---------- General ---------->>




		#region <<---------- Fluent Interface Pattern ---------->>

		/// <summary>
		/// Create new frame request with type FPS.
		/// </summary>
		/// <returns>Returns the new created request.</returns>
		public static FrameRateRequest FPS() {
			return new FrameRateRequest(FrameRateType.FPS);
		}

		/// <summary>
		/// Create new frame request with type FixedFPS.
		/// </summary>
		/// <returns>Returns the new created request.</returns>
		public static FrameRateRequest FixedFPS() {
			return new FrameRateRequest(FrameRateType.FixedFPS);
		}

		/// <summary>
		/// Change the rate value.
		/// </summary>
		/// <returns>Returns this instance to use as fluent interface.</returns>
		public FrameRateRequest WithRate(int requestedValue) {
			this.Value = requestedValue;
			return this;
		}		

		#endregion <<---------- Fluent Interface Pattern ---------->>
	}
}