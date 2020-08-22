using System;
using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	[Obsolete("OBSOLETE, use RateRequestWhileEnabledComponent instead")]
	public class FrameRateRequestComponent : MonoBehaviour {

		#region <<---------- Initializers ---------->>

		protected FrameRateRequestComponent() { }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private FrameRateType _type = FrameRateType.FPS;

		[SerializeField][Range(FrameRateRequest.MinRate, 120)] private int _rate = 30;

		/// <summary>
		/// Frame rate type.
		/// </summary>
		public FrameRateType Type {
			get { return this._type; }
			set {
				this._type = value;
				if (!Application.isPlaying || !this.IsRequestValuesDifferentFromFields()) return;
				this.AssertCurrentRequestRunning(this.isActiveAndEnabled);
			}
		}

		/// <summary>
		/// Frame rate value.
		/// </summary>
		public int Rate {
			get { return this._rate; }
			set {
				this._rate = value;
				if (!Application.isPlaying || !this.IsRequestValuesDifferentFromFields()) return;
				this.AssertCurrentRequestRunning(this.isActiveAndEnabled);
			}
		}

		private FrameRateRequest _request;

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
			FrameRateManager.Instance.StopRequest(this._request);
			if (!running) {
				this._request = FrameRateRequest.Invalid;
				return;
			}
			this._request = FrameRateManager.Instance.StartRequest(this._type, this._rate);
		}

		private bool IsRequestValuesDifferentFromFields() {
			return this._request.Type != this._type || this._request.Rate != this._rate;
		}

		#endregion <<---------- General ---------->>
	}
}