using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	public class TargetFixedFrameRateTrigger : RateTriggerBase {

		#region <<---------- Properties and Fields ---------->>

        [SerializeField] private UnityEventFrameRate _onTargetFixedFrameRate;

        /// <summary>
        /// Event invoked when <see cref="FrameRateManager.onTargetFixedFrameRate"/> is invoked.
        /// </summary>
        public UnityEventFrameRate OnTargetFixedFrameRate {
            get {
                if (this._onTargetFixedFrameRate == null) {
                    this._onTargetFixedFrameRate = new UnityEventFrameRate();
                }
                return this._onTargetFixedFrameRate;
            }
        }

		/// <summary>
		/// Same as <see cref="FrameRateManager.TargetFixedFrameRate"/>.
		/// </summary>
		public int TargetFixedFrameRate {
			get { return this.GetRate(); }
		}

        #endregion <<---------- Properties and Fields ---------->>




		#region <<---------- RateTriggerBase ---------->>

		protected override int GetRate() {
			if (base.isApplicationQuitting) return 0;
			return FrameRateManager.Instance.TargetFixedFrameRate;
		}

		protected override void OnRateChangedCallback(int rate) {
            if (this._onTargetFixedFrameRate == null) return;
            this._onTargetFixedFrameRate.Invoke(rate);
        }

		protected override void StartListening() {
			FrameRateManager.Instance.onTargetFixedFrameRate += this.OnRateChangedCallback;
		}

		protected override void StopListening() {
			FrameRateManager.Instance.onTargetFixedFrameRate -= this.OnRateChangedCallback;
		}

        #endregion <<---------- RateTriggerBase ---------->>
	}
}