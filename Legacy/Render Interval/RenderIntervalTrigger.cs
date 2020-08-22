using System;
using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	[Obsolete]
	public class RenderIntervalTrigger : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private RenderIntervalManagerPointer _managerPointer;

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
		public RenderIntervalManagerPointer ManagerPointer {
			get {
				if (this._managerPointer == null) {
					this._managerPointer = new RenderIntervalManagerPointer();
					this.NotifyAllEventsIfActiveEnabledPlaying();
					this.StartListeningIfActiveEnabledPlaying();
				}
				return this._managerPointer;
			}
		}

		private bool _isApplicationQuitting = false;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
			this.ManagerPointer.Changed += (pointer) => {
				if (this == null) return;
				this.NotifyAllEventsIfActiveEnabledPlaying();
				this.StartListeningIfActiveEnabledPlaying();
			};
		}

		protected virtual void OnEnable() {
			this.NotifyAllEvents();
			this.StartListeningIfActiveEnabledPlaying();
		}

		protected virtual void OnDisable() {
			if (this._isApplicationQuitting || this._managerPointer == null) return;
			var mngr = this._managerPointer.GetManager();
			if (mngr == null) return;
			
			mngr.RenderIntervalChanged -= this.OnRenderIntervalChanged;
			mngr.IsRenderingChanged -= this.OnIsRenderingChanged;
		}

		protected virtual void OnApplicationQuit() {
            this._isApplicationQuitting = true;
        }

		#if UNITY_EDITOR
		protected virtual void OnValidate() {
			this.NotifyAllEventsIfActiveEnabledPlaying();
			this.StartListeningIfActiveEnabledPlaying();
		}
		#endif

		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- General ---------->>

		private void NotifyAllEventsIfActiveEnabledPlaying() {
			if (!this.isActiveAndEnabled) return;
			#if UNITY_EDITOR
			if (!Application.isPlaying) return;
			#endif
			this.NotifyAllEvents();
		}

		private void NotifyAllEvents() {
			RenderIntervalManager mngr = null;
			if (this._managerPointer != null) {
				mngr = this._managerPointer.GetManager();
			}
			this.OnRenderIntervalChanged(mngr);
			this.OnIsRenderingChanged(mngr);
		}

		protected void StartListeningIfActiveEnabledPlaying() {
			if (!this.isActiveAndEnabled || this._managerPointer == null) return;
			#if UNITY_EDITOR
			if (!Application.isPlaying) return;
			#endif
			var mngr = this._managerPointer.GetManager();
			mngr.RenderIntervalChanged += this.OnRenderIntervalChanged;
			mngr.IsRenderingChanged += this.OnIsRenderingChanged;
		}

		protected void OnIsRenderingChanged(RenderIntervalManager myManager) {
			if (myManager == null) {
				this.OnIsRenderingChanged(null, false);
				return;
			}
			this.OnIsRenderingChanged(myManager, myManager.IsRendering);
		}
		protected virtual void OnIsRenderingChanged(RenderIntervalManager manager, bool isRendering) {
			RenderIntervalManager mngr = null;
			if (this._managerPointer != null) {
				mngr = this._managerPointer.GetManager();
			}

			if (mngr == null || mngr != manager) {
				if (manager != null) {
					manager.IsRenderingChanged -= this.OnIsRenderingChanged;
				}
				return;
			}
			if (this._isRenderingChanged != null) this._isRenderingChanged.Invoke(isRendering);
		}

		protected void OnRenderIntervalChanged(RenderIntervalManager myManager) {
			if (myManager == null) {
				this.OnRenderIntervalChanged(null, RenderIntervalRequest.MinInterval);
				return;
			}
			this.OnRenderIntervalChanged(myManager, myManager.RenderInterval);
		}
		protected virtual void OnRenderIntervalChanged(RenderIntervalManager manager, int interval) {
			RenderIntervalManager mngr = null;
			if (this._managerPointer != null) {
				mngr = this._managerPointer.GetManager();
			}

			if (mngr == null || mngr != manager) {
				if (manager != null) {
					manager.RenderIntervalChanged -= this.OnRenderIntervalChanged;
				}
				return;
			}
			if (this._renderIntervalChanged != null) this._renderIntervalChanged.Invoke(interval);
		}

		#endregion <<---------- General ---------->>




		#region <<---------- Legacy Support ---------->>
		
		// ObsoletedWarning 2019/05/09 - ObsoletedError 2019/05/09
		[System.Obsolete("use ManagerPointer instead", true)]
		public RenderIntervalManager Manager {
			get { return this.ManagerPointer.ManagerByReference; }
			set {
				this.ManagerPointer.ManagerByReference = value;
				this.StartListeningIfActiveEnabledPlaying();
			}
		}
		
		#endregion <<---------- Legacy Support ---------->>
	}
}