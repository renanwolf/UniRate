using System;
using System.Collections.Generic;
using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	public class RenderRateManager : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

		[SerializeField][Range(RenderRateRequest.MinValue,120)] private int _fallbackRenderRate = 18;

		/// <summary>
		/// Target render rate to set if there are no active requests.
		/// </summary>
		public int FallbackRenderRate {
			get { return this._fallbackRenderRate; }
			set {
				if (this._fallbackRenderRate == value) return;
				if (value < RenderRateRequest.MinValue) throw new ArgumentOutOfRangeException("FallbackRenderRate", value, "must be greather or equals to " + RenderRateRequest.MinValue);
				this._fallbackRenderRate = value;
				this.RecalculateTargetsRateIfPlaying();
			}
		}

		/// <summary>
		/// Target render rate per second.
		/// </summary>
		public int TargetRenderRate {
			get {
				if (this._renderOnEverySeconds <= 0f) return int.MaxValue;
				return Mathf.RoundToInt(1f / this._renderOnEverySeconds);
			}
			private set {
				var newRenderSeconds = value <= 0 ? float.PositiveInfinity : (1f / (float)value);
				if (this._renderOnEverySeconds == newRenderSeconds) return;
				this._renderOnEverySeconds = newRenderSeconds;
				this.OnTargetRenderRateChanged(value);
			}
		}

		private int _currentRenderRate;

		/// <summary>
		/// Current render rate per second.
		/// </summary>
		public int RenderRate {
			get { return this._currentRenderRate; }
			private set {
				if (this._currentRenderRate == value) return;
				this._currentRenderRate = value;
				this.OnCurrentRenderRateChanged();
			}
		}

		private Camera _attachedCamera;

		/// <summary>
		/// Camera component attached to this GameObject.
		/// </summary>
		public Camera AttachedCamera {
			get {
				if (this._attachedCamera == null) {
					this._attachedCamera = this.GetComponent<Camera>();
				}
				return this._attachedCamera;
			}
		}

		private bool _isOnRenderProcess = false;

		/// <summary>
		/// Is on render process?
		/// </summary>
		public bool IsRendering {
			get { return this._isOnRenderProcess; }
			private set {
				if (this._isOnRenderProcess == value) return;
				this._isOnRenderProcess = value;
				this.OnIsRenderingChanged();
			}
		}

		/// <summary>
		/// Event raised when <see cref="RenderRate"/> changes.
		/// </summary>
		public event Action<RenderRateManager, int> RenderRateChanged;

		/// <summary>
		/// Event raised when <see cref="TargetRenderRate"/> changes.
		/// </summary>
		public event Action<RenderRateManager, int> TargetRenderRateChanged;

		/// <summary>
		/// Event raised when <see cref="IsRendering"/> changes.
		/// </summary>
		public event Action<RenderRateManager, bool> IsRenderingChanged;

		private float _renderOnEverySeconds;

		private float _renderDeltaTime = float.PositiveInfinity;

		private float _renderProcessStart;

		private float _renderFinish;

		private float ElapsedSinceRenderProcessStart {
			get { return Time.realtimeSinceStartup - this._renderProcessStart; }
		}

		private float ElapsedSinceRendeFinish {
			get { return Time.realtimeSinceStartup - this._renderFinish; }
		}

		private List<RenderRateRequest> _requests;

		private bool _firstUpdateAfterEnable;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
			this.RecalculateTargetsRateIfPlaying();
		}

		protected virtual void OnEnable() {
			this._firstUpdateAfterEnable = true;
			this.Render();
		}

		protected virtual void Update() {
			
			if (this._firstUpdateAfterEnable) {
				this._firstUpdateAfterEnable = false;
			}
			else {
				if (this._isOnRenderProcess && this.ElapsedSinceRendeFinish >= 0f) {
					this.OnStopRenderProcess();
				}
				if (!this._isOnRenderProcess && this.ElapsedSinceRenderProcessStart >= this._renderOnEverySeconds) {
					this.OnStartRenderProcess();
				}
			}

			this.RenderRate = Mathf.RoundToInt(1f / this._renderDeltaTime);
		}

		protected virtual void OnDisable() {
			this.AssertIsOnRenderProcessFlag();
		}

		#if UNITY_EDITOR
		private void OnValidate() {
			this.RecalculateTargetsRateIfPlaying();
		}
		private void Reset() {
			this.RecalculateTargetsRateIfPlaying();
		}
		#endif

		#endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- Camera Messages ---------->>

		protected virtual void OnPreRender() {
			this.OnBeginRender();
		}

		protected virtual void OnPostRender() {
			this.OnFinishRender();
		}

		#endregion <<---------- Camera Messages ---------->>




		#region <<---------- Internal Callbacks ---------->>

		protected virtual void OnTargetRenderRateChanged(int value) {
			var evnt = this.TargetRenderRateChanged;
			if (evnt == null) return;
			evnt(this, value);
		}

		protected virtual void OnIsRenderingChanged() {
			var evnt = this.IsRenderingChanged;
			if (evnt == null) return;
			evnt(this, this._isOnRenderProcess);
		}

		protected virtual void OnCurrentRenderRateChanged() {
			var evnt = this.RenderRateChanged;
			if (evnt == null) return;
			evnt(this, this._currentRenderRate);
		}

		#endregion <<---------- Internal Callbacks ---------->>




		#region <<---------- Requests Management ---------->>

		/// <summary>
		/// Check if a request is added and active.
		/// </summary>
		/// <param name="request">Request to check.</param>
		/// <returns></returns>
		public bool ContainsRequest(RenderRateRequest request) {
			return request != null && this._requests != null && this._requests.Contains(request);
		}

		/// <summary>
		/// Add and activate a new render rate request.
		/// </summary>
		/// <param name="request">Request to add.</param>
		/// <returns>Returns the request.</returns>
		public RenderRateRequest AddRequest(RenderRateRequest request) {
			if (request == null) return null;
			if (this.ContainsRequest(request)) return request;
			if (this._requests == null) this._requests = new List<RenderRateRequest>();
			this._requests.Add(request);
			request.Changed += this.NotifyRequestChanged;
			this.RecalculateTargetsRateIfPlaying();
			return request;
		}

		/// <summary>
		/// Remove and deactivate a render rate request.
		/// </summary>
		/// <param name="request">Request to remove.</param>
		public void RemoveRequest(RenderRateRequest request) {
			if (request == null || !this.ContainsRequest(request)) return;
			this._requests.Remove(request);
			request.Changed -= this.NotifyRequestChanged;
			this.RecalculateTargetsRateIfPlaying();
		}

		private void NotifyRequestChanged(RenderRateRequest request) {
			if (request == null) return;
			if (!this.ContainsRequest(request)) {
				request.Changed -= this.NotifyRequestChanged;
				return;
			}
			this.RecalculateTargetsRateIfPlaying();
		}

		#endregion <<---------- Requests Management ---------->>




		#region <<---------- General ---------->>
		
		protected void AssertIsOnRenderProcessFlag() {
			this.IsRendering = this.gameObject.activeInHierarchy && this.AttachedCamera.enabled;
		}

		private void OnStartRenderProcess() {
			float realTime = Time.realtimeSinceStartup;
			this._renderDeltaTime = Mathf.Max(0.00001f, realTime - this._renderProcessStart);
			this._renderProcessStart = realTime;

			this._renderFinish = float.PositiveInfinity;

			this.AttachedCamera.enabled = true;
			this.AssertIsOnRenderProcessFlag();
		}

		private void OnBeginRender() {

		}

		private void OnFinishRender() {
			this._renderFinish = Time.realtimeSinceStartup;
		}

		private void OnStopRenderProcess() {
			this.AttachedCamera.enabled = false;
			this.AssertIsOnRenderProcessFlag();
		}

		/// <summary>
		/// Render now.
		/// </summary>
		public void Render() {
			if (!this.isActiveAndEnabled) return;
			//this._lastRenderRealtime = Time.realtimeSinceStartup - this._renderOnEverySeconds;
			this.OnStartRenderProcess();
		}

		private void RecalculateTargetsRateIfPlaying() {
			#if UNITY_EDITOR
			if (!Application.isPlaying) return;
			#endif

			int newTarget = RenderRateRequest.MinValue - 1;

			if (this._requests != null && this._requests.Count > 0) {
				for (int i = this._requests.Count - 1; i >= 0; i--) {
					if (this._requests[i] == null || (this._requests[i].Manager != null && this._requests[i].Manager != this)) {
						this._requests.RemoveAt(i);
						continue;
					}
					if (!this._requests[i].IsValid) continue;

					newTarget = Mathf.Max(newTarget, this._requests[i].Value);
				}
			}

			if (newTarget < RenderRateRequest.MinValue) {
				newTarget = this._fallbackRenderRate;
			}

			this.TargetRenderRate = newTarget;
		}

		#endregion <<---------- General ---------->>
	}
}