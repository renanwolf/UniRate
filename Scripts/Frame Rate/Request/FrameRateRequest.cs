namespace PWR.LowPowerMemoryConsumption {

    public struct FrameRateRequest {

		#region <<---------- Initializers ---------->>

		/// <summary>
		/// Do not create a request by initializing it, use FrameRateManager instead.
		/// </summary>
		public FrameRateRequest(FrameRateType rateType, int rateValue) {
			this._type = rateType;
			this._rate = rateValue;
		}

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Properties and Fields ---------->>

		/// <summary>
		/// Frame rate value.
		/// </summary>
		public int Rate {
			get { return this._rate; }
		}
		private readonly int _rate;

		/// <summary>
		/// Frame rate type.
		/// </summary>
		public FrameRateType Type {
			get { return this._type; }
		}
		private readonly FrameRateType _type;

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

		/// <summary>
		/// A default invalid request.
		/// </summary>
		public static readonly FrameRateRequest Invalid = new FrameRateRequest(FrameRateType.FPS, MinRate - 1);

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- Legacy Support ---------->>

		// ObsoletedWarning 2019/05/04 - ObsoletedError 2019/05/04
		[System.Obsolete("use FrameRateManager to create and start a new request", true)]
		public static FrameRateRequest FPS() {
			return Invalid;
		}

		// ObsoletedWarning 2019/05/04 - ObsoletedError 2019/05/04
		[System.Obsolete("use FrameRateManager to create and start a new request", true)]
		public static FrameRateRequest FixedFPS() {
			return Invalid;
		}

		// ObsoletedWarning 2019/05/04 - ObsoletedError 2019/05/04
		[System.Obsolete("FrameRateRequest is now immutable, this instance will never change", true)]
		public event System.Action<FrameRateRequest> Changed {
			add { }
			remove { }
		}

		// ObsoletedWarning 2019/05/04 - ObsoletedError 2019/05/04
		[System.Obsolete("use FrameRateManager to start a new request", true)]
		public FrameRateRequest(FrameRateType rateType) : this(rateType, (MinRate - 1) ) { }

		// ObsoletedWarning 2019/05/04 - ObsoletedError 2019/05/04
		[System.Obsolete("FrameRateRequest is now immutable, stop this request and start a new one on FrameRateManager", true)]
		public FrameRateRequest WithRate(int rate) {
			return this;
		}

		// ObsoletedWarning 2019/05/04 - ObsoletedError 20##/##/##
		[System.Obsolete("use FrameRateManager to stop your request", false)]
		public void Stop() {
			FrameRateManager.Instance.RemoveRequest(this);
		}

		// ObsoletedWarning 2019/05/04 - ObsoletedError 2019/05/04
		[System.Obsolete("use FrameRateManager to start a new request", true)]
		public FrameRateRequest Start() {
			return this;
		}

		#endregion <<---------- Legacy Support ---------->>
	}
}