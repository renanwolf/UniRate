using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	public class FixedFrameRateTrigger : RateTriggerBase {

		#region <<---------- Properties and Fields ---------->>

        [SerializeField] private UnityEventFrameRate _onFixedFrameRate;

        /// <summary>
        /// Event invoked when <see cref="FrameRateManager.onFixedFrameRate"/> is invoked.
        /// </summary>
        public UnityEventFrameRate OnFixedFrameRate {
            get {
                if (this._onFixedFrameRate == null) {
                    this._onFixedFrameRate = new UnityEventFrameRate();
                }
                return this._onFixedFrameRate;
            }
        }

		/// <summary>
		/// Same as <see cref="FrameRateManager.CurrentFixedFrameRate"/>.
		/// </summary>
		public int CurrentFixedFrameRate {
			get { return this.GetRate(); }
		}

        #endregion <<---------- Properties and Fields ---------->>




		#region <<---------- RateTriggerBase ---------->>

		protected override int GetRate() {
			if (base.isApplicationQuitting) return 0;
			return FrameRateManager.Instance.CurrentFixedFrameRate;
		}

		protected override void OnRateChangedCallback(int rate) {
            if (this._onFixedFrameRate == null) return;
            this._onFixedFrameRate.Invoke(rate);
        }

		protected override void StartListening() {
			FrameRateManager.Instance.onFixedFrameRate += this.OnRateChangedCallback;
		}

		protected override void StopListening() {
			FrameRateManager.Instance.onFixedFrameRate -= this.OnRateChangedCallback;
		}

        #endregion <<---------- RateTriggerBase ---------->>
	}
}