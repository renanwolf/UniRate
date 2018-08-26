using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	public class RenderRateTrigger : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private RenderRateManager _manager;

		[SerializeField] private UnityEventRate _currentRateChangedEvent;

		[SerializeField] private UnityEventRate _targetRateChangedEvent;

		/// <summary>
        /// Event raised when current render rate changes.
        /// </summary>
		public UnityEventRate CurrentRateChangedEvent {
			get {
				if (this._currentRateChangedEvent == null) {
					this._currentRateChangedEvent = new UnityEventRate();
				}
				return this._currentRateChangedEvent;
			}
		}

		/// <summary>
        /// Event raised when target render rate changes.
        /// </summary>
		public UnityEventRate TargetRateChangedEvent {
			get {
				if (this._targetRateChangedEvent == null) {
					this._targetRateChangedEvent = new UnityEventRate();
				}
				return this._targetRateChangedEvent;
			}
		}

		/// <summary>
		/// Render rate manager to listen.
		/// </summary>
		public RenderRateManager Manager {
			get { return this._manager; }
			set {
				this._manager = value;
				this.StartListeningIfActiveEnabledPlaying();
			}
		}

		private bool _isApplicationQuitting = false;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void OnEnable() {
			this.NotifyAllRatesChanged();
			this.StartListeningIfActiveEnabledPlaying();
		}

		protected virtual void OnDisable() {
			if (this._isApplicationQuitting || this._manager == null) return;
			
			this._manager.TargetRenderRateChanged -= this.NotifyTargetRenderRateChanged;
			this._manager.RenderRateChanged -= this.NotifyCurrentRenderRateChanged;
		}

		protected virtual void OnApplicationQuit() {
            this._isApplicationQuitting = true;
        }

		#if UNITY_EDITOR
		protected virtual void OnValidate() {
			this.StartListeningIfActiveEnabledPlaying();
		}
		#endif

		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- General ---------->>

		private void NotifyAllRatesChanged() {
			this.NotifyTargetRenderRateChanged();
			this.NotifyCurrentRenderRateChanged();
		}

		protected void StartListeningIfActiveEnabledPlaying() {
			if (!this.isActiveAndEnabled || this._manager == null) return;
			#if UNITY_EDITOR
			if (!Application.isPlaying) return;
			#endif
			this._manager.TargetRenderRateChanged += this.NotifyTargetRenderRateChanged;
			this._manager.RenderRateChanged += this.NotifyCurrentRenderRateChanged;
		}

		protected void NotifyCurrentRenderRateChanged() {
			if (this._manager == null) {
				this.NotifyCurrentRenderRateChanged(null, 0);
				return;
			}
			this.NotifyCurrentRenderRateChanged(this._manager, this._manager.RenderRate);
		}
		protected virtual void NotifyCurrentRenderRateChanged(RenderRateManager manager, int rate) {
			if (this._manager != manager) {
				if (manager != null) {
					manager.RenderRateChanged -= this.NotifyCurrentRenderRateChanged;
				}
				return;
			}
			if (this._currentRateChangedEvent != null) this._currentRateChangedEvent.Invoke(rate);
		}

		protected void NotifyTargetRenderRateChanged() {
			if (this._manager == null) {
				this.NotifyTargetRenderRateChanged(null, 0);
				return;
			}
			this.NotifyTargetRenderRateChanged(this._manager, this._manager.TargetRenderRate);
		}
		protected virtual void NotifyTargetRenderRateChanged(RenderRateManager manager, int rate) {
			if (this._manager != manager) {
				if (manager != null) {
					manager.TargetRenderRateChanged -= this.NotifyTargetRenderRateChanged;
				}
				return;
			}
			if (this._targetRateChangedEvent != null) this._targetRateChangedEvent.Invoke(rate);
		}

		#endregion <<---------- General ---------->>
	}
}