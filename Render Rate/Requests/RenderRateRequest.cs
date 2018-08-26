using System;
using UnityEngine;
using PWR.LowPowerMemoryConsumption.Internal;

namespace PWR.LowPowerMemoryConsumption {

	public class RenderRateRequest : RateRequestBase<RenderRateRequest> {

		#region <<---------- Initializers ---------->>

		public RenderRateRequest(int rateValue) : base(rateValue) { }

		public RenderRateRequest() : base() { }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Properties and Fields ---------->>

		public RenderRateManager Manager { get; private set; }

		#endregion <<---------- Properties and Fields ---------->>
		
		
		
		
		#region <<---------- General ---------->>

		/// <summary>
		/// Start the frame rate request.
		/// </summary>
		/// <param name="manager">The render rate manager to add the request.</param>
		/// <returns>Returns this instance to use as fluent interface.</returns>
		public RenderRateRequest Start(RenderRateManager manager) {
			if (manager == null) return null;
			if (this.Manager != null && this.Manager != manager) this.Stop();
			this.Manager = manager;
			return this.Manager.AddRequest(this);
		}

		/// <summary>
		/// Stop the render rate request on <see cref="Manager"/>.
		/// </summary>
		public void Stop() {
			if (this.Manager == null) return;
			this.Manager.RemoveRequest(this);
			this.Manager = null;
		}

		#endregion <<---------- General ---------->>
	}
}