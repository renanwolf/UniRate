using System;

namespace PWR.LowPowerMemoryConsumption {

    [Obsolete]
    public struct RenderIntervalRequest {

		#region <<---------- Initializers ---------->>

		public RenderIntervalRequest(UniRate.RenderIntervalRequest request) {
			this._interval = 0;
			this._managerInstanceID = -1;
			this._request = request;
			if (request == null) return;
			this._interval = request.RenderInterval;
		}

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Properties and Fields ---------->>

		public UniRate.RenderIntervalRequest UniRateRequest => this._request;
		private readonly UniRate.RenderIntervalRequest _request;

		public int ManagerInstanceID => this._managerInstanceID;
		private readonly int _managerInstanceID;

		public int Interval => this._interval;
		private readonly int _interval;

		public bool IsValid => (this._interval >= MinInterval && this._request != null);

		public const int MinInterval = 1;

		public static readonly RenderIntervalRequest Invalid = new RenderIntervalRequest(null);

		#endregion <<---------- Properties and Fields ---------->>
		
		
		
		
		#region <<---------- General ---------->>

		public RenderIntervalManager FindManager() => null;

		#endregion <<---------- General ---------->>
	}
}