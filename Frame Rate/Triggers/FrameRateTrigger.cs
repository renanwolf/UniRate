using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	public class FrameRateTrigger : RateTriggerBase {

		#region <<---------- Properties and Fields ---------->>

        [SerializeField] private UnityEventFrameRate _onFrameRate;

        /// <summary>
        /// Event invoked when <see cref="FrameRateManager.onFrameRate"/> is invoked.
        /// </summary>
        public UnityEventFrameRate OnFrameRate {
            get {
                if (this._onFrameRate == null) {
                    this._onFrameRate = new UnityEventFrameRate();
                }
                return this._onFrameRate;
            }
        }

		/// <summary>
		/// Same as <see cref="FrameRateManager.CurrentFrameRate"/>.
		/// </summary>
		public int CurrentFrameRate {
			get { return this.GetRate(); }
		}

        #endregion <<---------- Properties and Fields ---------->>




		#region <<---------- RateTriggerBase ---------->>

		protected override int GetRate() {
			if (base.isApplicationQuitting) return 0;
			return FrameRateManager.Instance.CurrentFrameRate;
		}

		protected override void OnRateChangedCallback(int rate) {
            if (this._onFrameRate == null) return;
            this._onFrameRate.Invoke(rate);
        }

		protected override void StartListening() {
			FrameRateManager.Instance.onFrameRate += this.OnRateChangedCallback;
		}

		protected override void StopListening() {
			FrameRateManager.Instance.onFrameRate -= this.OnRateChangedCallback;
		}

        #endregion <<---------- RateTriggerBase ---------->>
	}
}