using System;
using UnityEngine;
using PWR.LowPowerMemoryConsumption.Internal;

namespace PWR.LowPowerMemoryConsumption {

	public class RenderRateRequest : RateRequestBase<RenderRateRequest> {

		#region <<---------- Initializers ---------->>

		public RenderRateRequest(int rateValue) : base(rateValue) { }

		public RenderRateRequest() : base() { }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- General ---------->>

		/// <summary>
		/// Start the frame rate request on <see cref="FrameRateManager"/>.
		/// </summary>
		/// <returns>Returns this instance to use as fluent interface.</returns>
		public void Start() {
			
		}

		/// <summary>
		/// Stop the render rate request on <see cref="FrameRateManager"/>.
		/// </summary>
		public void Stop() {
			
		}

		#endregion <<---------- General ---------->>
	}
}