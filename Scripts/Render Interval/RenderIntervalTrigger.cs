using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	public class RenderIntervalTrigger : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private RenderIntervalManager _manager;

		[SerializeField] private UnityEventInt _renderIntervalChanged;

		[SerializeField] private UnityEventBool _isRenderingChanged;

		/// <summary>
        /// Event raised when render interval changes.
        /// </summary>
		public UnityEventInt RenderIntervalChanged {
			get {
				if (this._renderIntervalChanged == null) {
					this._renderIntervalChanged = new UnityEventInt();
				}
				return this._renderIntervalChanged;
			}
		}

		/// <summary>
        /// Event raised when is rendering changes.
        /// </summary>
		public UnityEventBool IsRenderingChanged {
			get {
				if (this._isRenderingChanged == null) {
					this._isRenderingChanged = new UnityEventBool();
				}
				return this._isRenderingChanged;
			}
		}

		/// <summary>
		/// Render interval manager to listen.
		/// </summary>
		public RenderIntervalManager Manager {
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
			this.NotifyAllEvents();
			this.StartListeningIfActiveEnabledPlaying();
		}

		protected virtual void OnDisable() {
			if (this._isApplicationQuitting || this._manager == null) return;
			
			this._manager.RenderIntervalChanged -= this.OnRenderIntervalChanged;
			this._manager.IsRenderingChanged -= this.OnIsRenderingChanged;
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

		private void NotifyAllEvents() {
			this.OnRenderIntervalChanged();
			this.OnIsRenderingChanged();
		}

		protected void StartListeningIfActiveEnabledPlaying() {
			if (!this.isActiveAndEnabled || this._manager == null) return;
			#if UNITY_EDITOR
			if (!Application.isPlaying) return;
			#endif
			this._manager.RenderIntervalChanged += this.OnRenderIntervalChanged;
			this._manager.IsRenderingChanged += this.OnIsRenderingChanged;
		}

		protected void OnIsRenderingChanged() {
			if (this._manager == null) {
				this.OnIsRenderingChanged(null, false);
				return;
			}
			this.OnIsRenderingChanged(this._manager, this._manager.IsRendering);
		}
		protected virtual void OnIsRenderingChanged(RenderIntervalManager manager, bool isRendering) {
			if (this._manager != manager) {
				if (manager != null) {
					manager.IsRenderingChanged -= this.OnIsRenderingChanged;
				}
				return;
			}
			if (this._isRenderingChanged != null) this._isRenderingChanged.Invoke(isRendering);
		}

		protected void OnRenderIntervalChanged() {
			if (this._manager == null) {
				this.OnRenderIntervalChanged(null, RenderIntervalRequest.MinInterval);
				return;
			}
			this.OnRenderIntervalChanged(this._manager, this._manager.RenderInterval);
		}
		protected virtual void OnRenderIntervalChanged(RenderIntervalManager manager, int interval) {
			if (this._manager != manager) {
				if (manager != null) {
					manager.RenderIntervalChanged -= this.OnRenderIntervalChanged;
				}
				return;
			}
			if (this._renderIntervalChanged != null) this._renderIntervalChanged.Invoke(interval);
		}

		#endregion <<---------- General ---------->>
	}
}