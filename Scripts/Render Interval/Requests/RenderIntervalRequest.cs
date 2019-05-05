using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

    public struct RenderIntervalRequest {

		#region <<---------- Initializers ---------->>

		/// <summary>
        /// Do not create a request by initializing it, use RenderIntervalManager instead.
        /// </summary>
		public RenderIntervalRequest(int interval, RenderIntervalManager manager) {
			this._interval = interval;
			this._managerInstanceID = manager == null ? -1 : manager.GetInstanceID();
		}

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Properties and Fields ---------->>

		/// <summary>
		/// Manager instance ID.
		/// </summary>
		public int ManagerInstanceID {
			get { return this._managerInstanceID; }
		}
		private readonly int _managerInstanceID;

		/// <summary>
		/// Render interval value.
		/// </summary>
		public int Interval {
			get { return this._interval; }
		}
		private readonly int _interval;

		/// <summary>
		/// Is valid if interval value is greather or equals to <see cref="MinInterval"/>.
		/// </summary>
		public bool IsValid {
			get { return this._interval >= MinInterval; }
		}

		/// <summary>
		/// Minimum interval valid value.
		/// </summary>
		public const int MinInterval = 1;

		/// <summary>
        /// A default invalid request.
        /// </summary>
        public static readonly RenderIntervalRequest Invalid = new RenderIntervalRequest(MinInterval - 1, null);

		#endregion <<---------- Properties and Fields ---------->>
		
		
		
		
		#region <<---------- General ---------->>

		/// <summary>
		/// Find manager by its instance ID.
		/// </summary>
		public RenderIntervalManager FindManager() {
			var managers = Resources.FindObjectsOfTypeAll<RenderIntervalManager>();
			if (managers == null) return null;
			int count = managers.Length;
			if (count <= 0) return null;
			RenderIntervalManager mngr;
			for (int i = 0; i < count; i++) {
				mngr = managers[i];
				if (mngr == null || mngr.GetInstanceID() != this._managerInstanceID) continue;
				return mngr;
			}
			return null;
		}

		#endregion <<---------- General ---------->>




		#region <<---------- Legacy Support ---------->>

		// ObsoletedWarning 2019/05/04 - ObsoletedError 2019/05/04
        [System.Obsolete("RenderIntervalRequest is now immutable, this instance will never change", true)]
		public event System.Action<RenderIntervalRequest> Changed {
			add { }
			remove { }
		}

		// ObsoletedWarning 2019/05/04 - ObsoletedError 2019/05/04
        [System.Obsolete("use FindManager() instead", true)]
		public RenderIntervalManager Manager {
			get { return this.FindManager(); }
		}

		// ObsoletedWarning 2019/05/04 - ObsoletedError 2019/05/04
        [System.Obsolete("RenderIntervalRequest is now immutable, start a new request on RenderIntervalManager", true)]
		public static RenderIntervalRequest WithInterval(int interval) {
			return new RenderIntervalRequest(interval, null);
		}

		// ObsoletedWarning 2019/05/04 - ObsoletedError 2019/05/04
        [System.Obsolete("use RenderIntervalManager to stop your request", true)]
		public void Stop() { }

		// ObsoletedWarning 2019/05/04 - ObsoletedError 2019/05/04
        [System.Obsolete("use RenderIntervalManager to start a new request", true)]
		public RenderIntervalRequest Start(RenderIntervalManager manager) {
			return this;
		}

		#endregion <<---------- Legacy Support ---------->>
	}
}