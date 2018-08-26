using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	public class FrameRateRequester : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private FrameRateType _type = FrameRateType.FPS;

		[SerializeField][Range(FrameRateRequest.MinValue,120)] private int _rate = 30;

		/// <summary>
		/// Rate type.
		/// </summary>
		public FrameRateType Type {
			get { return this._type; }
			set {
				this._type = value;
				if (Application.isPlaying) this.Request.Type = value;
			}
		}

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

		private FrameRateRequest _request;
		protected FrameRateRequest Request {
			get {
				if (this._request == null) {
					this._request = new FrameRateRequest(this._type, this._rate);
				}
				return this._request;
			}
		}

		private bool _isApplicationQuitting = false;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void OnEnable() {
			this.Request.Start();
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
			this.Request.Type = this._type;
			this.Request.Value = this._rate;
		}
		protected virtual void OnReset() {
			if (!Application.isPlaying) return;
			this.Request.Type = this._type;
			this.Request.Value = this._rate;
		}
		#endif

		#endregion <<---------- MonoBehaviour ---------->>
	}
}