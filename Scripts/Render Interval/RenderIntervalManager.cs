using System;
using System.Collections.Generic;
using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	public class RenderIntervalManager : MonoBehaviour {

		#region <<---------- Initializers ---------->>

		protected RenderIntervalManager() { }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Properties and Fields ---------->>

		[SerializeField][Range(RenderIntervalRequest.MinInterval, 60)] private int _fallbackRenderInterval = 2;

		[SerializeField][Range(0, 60)] private int _framesBypassAtStartToPreventFlickering = 30;

		/// <summary>
		/// Render interval to set if there are no active requests.
		/// </summary>
		public int FallbackRenderInterval {
			get { return this._fallbackRenderInterval; }
			set {
				if (this._fallbackRenderInterval == value) return;
				if (value < RenderIntervalRequest.MinInterval) throw new ArgumentOutOfRangeException("FallbackRenderInterval", value, "must be greather or equals to " + RenderIntervalRequest.MinInterval);
				this._fallbackRenderInterval = value;
				this.RecalculateRenderIntervalIfPlaying();
			}
		}

		/// <summary>
		/// Current render interval.
		/// </summary>
		public int RenderInterval {
			get { return this._renderInterval; }
			private set {
				if (this._renderInterval == value) return;
				this._renderInterval = value;
				this.OnRenderIntervalChanged();
			}
		}
		private int _renderInterval;

		/// <summary>
		/// Camera component attached to this GameObject.
		/// </summary>
		public Camera CameraAttached {
			get {
				if (this._cameraAttached == null) {
					this._cameraAttached = this.GetComponent<Camera>();
				}
				return this._cameraAttached;
			}
		}
		private Camera _cameraAttached;

		/// <summary>
		/// Is rendering?
		/// </summary>
		public bool IsRendering {
			get { return this._isRendering; }
			private set {
				if (this._isRendering == value) return;
				this._isRendering = value;
				this.OnIsRenderingChanged();
			}
		}
		private bool _isRendering = false;

		/// <summary>
		/// Event raised when <see cref="RenderInterval"/> changes.
		/// </summary>
		public event Action<RenderIntervalManager, int> RenderIntervalChanged {
			add {
				this._renderIntervalChanged -= value;
				this._renderIntervalChanged += value;
			}
			remove {
				this._renderIntervalChanged -= value;
			}
		}
		private Action<RenderIntervalManager, int> _renderIntervalChanged;

		/// <summary>
		/// Event raised when <see cref="IsRendering"/> changes.
		/// </summary>
		public event Action<RenderIntervalManager, bool> IsRenderingChanged {
			add {
				this._isRenderingChanged -= value;
				this._isRenderingChanged += value;
			}
			remove {
				this._isRenderingChanged -= value;
			}
		}
		private Action<RenderIntervalManager, bool> _isRenderingChanged;

		private List<RenderIntervalRequest> _requests;

		private int _framesWithoutRendering;

		private bool _frameRendered;

		private int _updatesAfterAwake;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
			this.RecalculateRenderIntervalIfPlaying();
			this.AssertIsRenderingFlag();
			this._updatesAfterAwake = 0;
		}

		protected virtual void OnEnable() {
			this.Render();
			this.AssertIsRenderingFlag();
			this._frameRendered = false;
			this._framesWithoutRendering = 0;
		}

		protected virtual void Update() {

			//prevent flickering after first enable
			if (this._updatesAfterAwake < this._framesBypassAtStartToPreventFlickering) {
				this._updatesAfterAwake += 1;
				return;
			}

			this.AssertIsRenderingFlag();

			if (this._isRendering) {
				if (!this._frameRendered) {
					this._frameRendered = true;
					return;
				}
				this.StopRender();
				this.AssertIsRenderingFlag();
			}

			this._framesWithoutRendering += 1;
			if (this._framesWithoutRendering < this._renderInterval) {
				return;
			}

			this.Render();
			this.AssertIsRenderingFlag();
		}

		protected virtual void OnDisable() {
			this.AssertIsRenderingFlag();
		}

		#if UNITY_EDITOR
		private void OnValidate() {
			this.RecalculateRenderIntervalIfPlaying();
		}
		private void Reset() {
			this.RecalculateRenderIntervalIfPlaying();
		}
		#endif

		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- Internal Callbacks ---------->>

		protected virtual void OnIsRenderingChanged() {

			if (!this._isRendering) {
				this._framesWithoutRendering = 0;
			}
			else {
				this._frameRendered = false;
			}

			var evnt = this._isRenderingChanged;
			if (evnt == null) return;
			evnt(this, this._isRendering);
		}

		protected virtual void OnRenderIntervalChanged() {
			var evnt = this._renderIntervalChanged;
			if (evnt == null) return;
			evnt(this, this._renderInterval);
		}

		#endregion <<---------- Internal Callbacks ---------->>




		#region <<---------- Requests Management ---------->>

		/// <summary>
		/// Check if a request is added and active.
		/// </summary>
		/// <param name="request">Request to check.</param>
		/// <returns></returns>
		public bool ContainsRequest(RenderIntervalRequest request) {
			return request != null && this._requests != null && this._requests.Contains(request);
		}

		/// <summary>
		/// Add and activate a new render rate request.
		/// </summary>
		/// <param name="request">Request to add.</param>
		/// <returns>Returns the request.</returns>
		public RenderIntervalRequest AddRequest(RenderIntervalRequest request) {
			if (request == null) return null;
			if (this.ContainsRequest(request)) return request;
			if (this._requests == null) this._requests = new List<RenderIntervalRequest>();
			this._requests.Add(request);
			request.Changed += this.NotifyRequestChanged;
			this.RecalculateRenderIntervalIfPlaying();
			return request;
		}

		/// <summary>
		/// Remove and deactivate a render rate request.
		/// </summary>
		/// <param name="request">Request to remove.</param>
		public void RemoveRequest(RenderIntervalRequest request) {
			if (request == null || !this.ContainsRequest(request)) return;
			this._requests.Remove(request);
			request.Changed -= this.NotifyRequestChanged;
			this.RecalculateRenderIntervalIfPlaying();
		}

		private void NotifyRequestChanged(RenderIntervalRequest request) {
			if (request == null) return;
			if (!this.ContainsRequest(request)) {
				request.Changed -= this.NotifyRequestChanged;
				return;
			}
			this.RecalculateRenderIntervalIfPlaying();
		}

		#endregion <<---------- Requests Management ---------->>




		#region <<---------- General ---------->>
		
		protected void AssertIsRenderingFlag() {
			this.IsRendering = this.gameObject.activeInHierarchy && this.CameraAttached.enabled;
		}

		/// <summary>
		/// Render now.
		/// </summary>
		public void Render() {
			this.CameraAttached.enabled = true;
		}

		private void StopRender() {
			this.CameraAttached.enabled = false;
		}

		private void RecalculateRenderIntervalIfPlaying() {
			#if UNITY_EDITOR
			if (!Application.isPlaying) return;
			#endif

			int minValue = int.MaxValue;
			bool anyValidRequest = false;

			if (this._requests != null && this._requests.Count > 0) {
				for (int i = this._requests.Count - 1; i >= 0; i--) {
					if (this._requests[i] == null || (this._requests[i].Manager != null && this._requests[i].Manager != this)) {
						this._requests.RemoveAt(i);
						continue;
					}
					if (!this._requests[i].IsValid) continue;

					anyValidRequest = true;
					minValue = Mathf.Min(minValue, this._requests[i].Interval);
				}
			}

			if (!anyValidRequest) {
				minValue = this._fallbackRenderInterval;
			}

			this.RenderInterval = minValue;
		}

		#endregion <<---------- General ---------->>
	}
}