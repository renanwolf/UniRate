using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	public class TargetFrameRateTrigger : RateTriggerBase {

		#region <<---------- Properties and Fields ---------->>

        [SerializeField] private UnityEventFrameRate _onTargetFrameRate;

        /// <summary>
        /// Event invoked when <see cref="FrameRateManager.onTargetFrameRate"/> is invoked.
        /// </summary>
        public UnityEventFrameRate OnTargetFrameRate {
            get {
                if (this._onTargetFrameRate == null) {
                    this._onTargetFrameRate = new UnityEventFrameRate();
                }
                return this._onTargetFrameRate;
            }
        }

		/// <summary>
		/// Same as <see cref="FrameRateManager.TargetFrameRate"/>.
		/// </summary>
		public int TargetFrameRate {
			get { return this.GetRate(); }
		}

        #endregion <<---------- Properties and Fields ---------->>




		#region <<---------- RateTriggerBase ---------->>

		protected override int GetRate() {
			if (base.isApplicationQuitting) return 0;
			return FrameRateManager.Instance.TargetFrameRate;
		}

		protected override void OnRateChangedCallback(int rate) {
            if (this._onTargetFrameRate == null) return;
            this._onTargetFrameRate.Invoke(rate);
        }

		protected override void StartListening() {
			FrameRateManager.Instance.onTargetFrameRate += this.OnRateChangedCallback;
		}

		protected override void StopListening() {
			FrameRateManager.Instance.onTargetFrameRate -= this.OnRateChangedCallback;
		}

        #endregion <<---------- RateTriggerBase ---------->>
	}
}