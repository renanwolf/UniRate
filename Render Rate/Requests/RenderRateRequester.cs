using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	public class RenderRateRequester : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private RenderRateManager _manager;

		[SerializeField][Range(RenderRateRequest.MinValue,120)] private int _rate = 30;

		/// <summary>
		/// Rate value.
		/// </summary>
		public int Rate {
			get { return this._rate; }
			set {
				this._rate = value;
				if (Application.isPlaying) this.Request.Value = value;
			}
		}

		public RenderRateManager Manager {
			get { return this._manager; }
			set {
				if (this._manager == value) return;
				this._manager = value;
				#if UNITY_EDITOR
				if (!Application.isPlaying) return;
				#endif
				if (this.isActiveAndEnabled) {
					this.Request.Stop();
					this.Request.Start(this._manager);
				}
			}
		}

		private RenderRateRequest _request;
		protected RenderRateRequest Request {
			get {
				if (this._request == null) {
					this._request = new RenderRateRequest(this._rate);
				}
				return this._request;
			}
		}

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
			if (!Application.isPlaying) return;
			if (this.isActiveAndEnabled) {
				this.Request.Stop();
				this.Request.Start(this._manager);
			}
			this.Request.Value = this._rate;
		}
		protected virtual void OnReset() {
			if (!Application.isPlaying) return;
			if (this.isActiveAndEnabled) {
				this.Request.Stop();
				this.Request.Start(this._manager);
			}
			this.Request.Value = this._rate;
		}
		#endif

		#endregion <<---------- MonoBehaviour ---------->>
	}
}