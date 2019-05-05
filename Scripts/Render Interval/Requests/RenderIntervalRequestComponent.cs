using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	public class RenderIntervalRequestComponent : MonoBehaviour {

        #region <<---------- Initializers ---------->>

        protected RenderIntervalRequestComponent() { }

        #endregion <<---------- Initializers ---------->>




		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private RenderIntervalManager _manager;

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
		/// Render interval manager to start the request.
		/// </summary>
		public RenderIntervalManager Manager {
			get { return this._manager; }
			set {
				if (this._manager == value) return;
				this._manager = value;
				if (!Application.isPlaying || !this.IsRequestValuesDifferentFromFields()) return;
				this.AssertCurrentRequestRunning(this.isActiveAndEnabled);
			}
		}
		
		private RenderIntervalRequest _request;

		private bool _isApplicationQuitting = false;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

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
			if (this._manager == null) return;
			this._manager.StopRequest(this._request);
			if (!running) {
				this._request = RenderIntervalRequest.Invalid;
				return;
			}
			this._request = this._manager.StartRequest(this._interval);
		}

		private bool IsRequestValuesDifferentFromFields() {
			if (this._request.Interval != this._interval) return true;
			int myManagerID = this._manager == null ? -1 : this._manager.GetInstanceID();
			return this._request.ManagerInstanceID != myManagerID;
		}

		#endregion <<---------- General ---------->>
	}
}