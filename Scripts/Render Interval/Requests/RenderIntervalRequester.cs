using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	[DisallowMultipleComponent]
	public class RenderIntervalRequester : MonoBehaviour {

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
				if (Application.isPlaying) this.Request.Interval = value;
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
				this.OnManagerChanged();
			}
		}
		
		protected RenderIntervalRequest Request {
			get {
				if (this._request == null) {
					this._request = new RenderIntervalRequest(this._interval);
				}
				return this._request;
			}
		}
		private RenderIntervalRequest _request;

		private bool _isApplicationQuitting = false;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void OnEnable() {
			this.Request.Start(this._manager);
		}

		protected virtual void OnDisable() {
			if (this._isApplicationQuitting) return;
			this.Request.Stop();
		}

		protected virtual void OnApplicationQuit() {
			this._isApplicationQuitting = true;
		}

		#if UNITY_EDITOR
		protected virtual void OnValidate() {
			this.OnManagerChanged();
			if (!Application.isPlaying) return;
			this.Request.Interval = this._interval;
		}
		protected virtual void OnReset() {
			this.OnManagerChanged();
			if (!Application.isPlaying) return;
			this.Request.Interval = this._interval;
		}
		#endif

		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- Internal Callbacks ---------->>

		protected virtual void OnManagerChanged() {
			#if UNITY_EDITOR
			if (!Application.isPlaying) return;
			#endif
			if (!this.isActiveAndEnabled) return;
			this.Request.Stop();
			this.Request.Start(this._manager);
		}

		#endregion <<---------- Internal Callbacks ---------->>
	}
}