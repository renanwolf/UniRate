using System;
using UnityEngine;
using PWR.LowPowerMemoryConsumption.Internal;

namespace PWR.LowPowerMemoryConsumption {

	public class FrameRateRequest : RateRequestBase<FrameRateRequest> {

		#region <<---------- Initializers ---------->>

		public FrameRateRequest(FrameRateType rateType, int rateValue) : base(rateValue) {
			this._type = rateType;
		}

		public FrameRateRequest(FrameRateType rateType) : base() {
			this._type = rateType;
		}

		public FrameRateRequest() : this(FrameRateType.FPS) { }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Properties and Fields ---------->>

		private FrameRateType _type;

		/// <summary>
		/// Rate type.
		/// </summary>
		public FrameRateType Type {
			get { return this._type; }
			set {
				if (this._type == value) return;
				this._type = value;
				this.OnChanged();
			}
		}

		/// <summary>
		/// Is valid if value is greather or equals to <see cref="RateRequestBase.MinValue"/>.
		/// </summary>
		public override bool IsValid {
			get { return this.Value >= MinValue; }
		}

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- General ---------->>

		/// <summary>
		/// Start the frame rate request on <see cref="FrameRateManager"/>.
		/// </summary>
		/// <returns>Returns this instance to use as fluent interface.</returns>
		public FrameRateRequest Start() {
			return FrameRateManager.Instance.AddRequest(this);
		}

		/// <summary>
		/// Stop the frame rate request on <see cref="FrameRateManager"/>.
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

		#endregion <<---------- Fluent Interface Pattern ---------->>
	}
}