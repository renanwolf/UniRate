using System;
using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	[Obsolete("OBSOLETE, use RateRequestWhileEnabledComponent instead")]
    public class RenderIntervalRequestComponent : MonoBehaviour {

        #region <<---------- Initializers ---------->>

        protected RenderIntervalRequestComponent() { }

        #endregion <<---------- Initializers ---------->>




		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private RenderIntervalManagerPointer _managerPointer;

		[SerializeField][Range(RenderIntervalRequest.MinInterval, 60)] private int _interval = RenderIntervalRequest.MinInterval;

		/// <summary>
		/// Render interval value.
		/// </summary>
		public int Interval {
			get { return this._interval; }
			set {
				this._interval = value;
				if (!Application.isPlaying || !this.IsRequestValuesDifferentFromFields()) return;
				this.AssertCurrentRequestRunning(this.isActiveAndEnabled);
			}
		}

		/// <summary>
		/// Manager pointer.
		/// </summary>
		public RenderIntervalManagerPointer ManagerPointer {
			get {
				if (this._managerPointer == null) {
					this._managerPointer = new RenderIntervalManagerPointer();
					if (Application.isPlaying && this.IsRequestValuesDifferentFromFields()) {
						this.AssertCurrentRequestRunning(this.isActiveAndEnabled);
					}
				}
				return this._managerPointer;
			}
		}

		private RenderIntervalRequest _request;

		private bool _isApplicationQuitting = false;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
			this.ManagerPointer.Changed += (pointer) => {
				if (!Application.isPlaying || this == null || !this.IsRequestValuesDifferentFromFields()) return;
				this.AssertCurrentRequestRunning(this.isActiveAndEnabled);
			};
		}

		protected virtual void OnEnable() {
			this.AssertCurrentRequestRunning(true);
		}

		protected virtual void OnDisable() {
			if (this._isApplicationQuitting) return;
			this.AssertCurrentRequestRunning(false);
		}

		protected virtual void OnApplicationQuit() {
			this._isApplicationQuitting = true;
		}

		#if UNITY_EDITOR
		protected virtual void OnValidate() {
			if (!Application.isPlaying) return;
			this.AssertCurrentRequestRunning(this.isActiveAndEnabled);
		}
		protected virtual void OnReset() {
			if (!Application.isPlaying) return;
			this.AssertCurrentRequestRunning(this.isActiveAndEnabled);
		}
		#endif

		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- General ---------->>

		private void AssertCurrentRequestRunning(bool running) {
			var mngr = this.ManagerPointer.GetManager();
			if (mngr == null) return;
			mngr.StopRequest(this._request);
			if (!running) {
				this._request = RenderIntervalRequest.Invalid;
				return;
			}
			this._request = mngr.StartRequest(this._interval);
		}

		private bool IsRequestValuesDifferentFromFields() {
			if (this._request.Interval != this._interval) return true;
			var mngr = this.ManagerPointer.GetManager();
			int myManagerInstanceID = mngr == null ? -1 : mngr.GetInstanceID();
			return this._request.ManagerInstanceID != myManagerInstanceID;
		}

		#endregion <<---------- General ---------->>
	}
}