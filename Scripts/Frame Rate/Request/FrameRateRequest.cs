using System;
using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	public class FrameRateRequest {

		#region <<---------- Initializers ---------->>

		public FrameRateRequest(FrameRateType rateType, int rateValue) {
			this._type = rateType;
			this._rate = rateValue;
		}

		public FrameRateRequest(FrameRateType rateType) : this(rateType, (MinRate - 1) ) {
			this._type = rateType;
		}

		public FrameRateRequest() : this(FrameRateType.FPS) { }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Static Creators ---------->>

		/// <summary>
		/// Create a new frame rate request with type FPS.
		/// </summary>
		/// <returns>Returns the new created request.</returns>
		public static FrameRateRequest FPS() {
			return new FrameRateRequest(FrameRateType.FPS);
		}

		/// <summary>
		/// Create a new frame rate request with type FixedFPS.
		/// </summary>
		/// <returns>Returns the new created request.</returns>
		public static FrameRateRequest FixedFPS() {
			return new FrameRateRequest(FrameRateType.FixedFPS);
		}		

		#endregion <<---------- Static Creators ---------->>




		#region <<---------- Properties and Fields ---------->>

		/// <summary>
		/// Frame rate value.
		/// </summary>
		public int Rate {
			get { return this._rate; }
			set {
				if (this._rate == value) return;
				this._rate = value;
				this.OnChanged();
			}
		}
		private int _rate;

		/// <summary>
		/// Frame rate type.
		/// </summary>
		public FrameRateType Type {
			get { return this._type; }
			set {
				if (this._type == value) return;
				this._type = value;
				this.OnChanged();
			}
		}
		private FrameRateType _type;

		/// <summary>
		/// Event raised when this request changes.
		/// </summary>
		public event Action<FrameRateRequest> Changed {
			add {
				this._changed -= value;
				this._changed += value;
			}
			remove {
				this._changed -= value;
			}
		}
		private Action<FrameRateRequest> _changed;

		/// <summary>
		/// Is valid if rate value is greather or equals to <see cref="MinRate"/>.
		/// </summary>
		public bool IsValid {
			get { return this._rate >= MinRate; }
		}

		/// <summary>
		/// Minimum frame rate valid value.
		/// </summary>
		public const int MinRate = 1;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- General ---------->>

		protected virtual void OnChanged() {
			var evnt = this._changed;
			if (evnt == null) return;
			evnt(this);
		}

		/// <summary>
		/// Start the frame rate request on <see cref="FrameRateManager"/>.
		/// </summary>
		/// <returns>Returns this instance to use chaining pattern.</returns>
		public FrameRateRequest Start() {
			return FrameRateManager.Instance.AddRequest(this);
		}

		/// <summary>
		/// Stop the frame rate request on <see cref="FrameRateManager"/>.
		/// </summary>
		public void Stop() {
			FrameRateManager.Instance.RemoveRequest(this);
		}

		/// <summary>
		/// Set the rate value.
		/// </summary>
		/// <returns>Returns this instance to use chaining pattern.</returns>
		public FrameRateRequest WithRate(int rate) {
			this.Rate = rate;
			return this;
		}

		#endregion <<---------- General ---------->>
	}
}