using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PWR.LowPowerMemoryConsumption {

    [DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	public class RenderIntervalManager : MonoBehaviour {

		#region <<---------- Initializers ---------->>

		protected RenderIntervalManager() { }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Properties and Fields ---------->>

		[SerializeField] private string _identifier;

		[SerializeField][Range(RenderIntervalRequest.MinInterval, 60)] private int _fallbackRenderInterval = 2;

		[SerializeField][Range(0, 60)] private int _framesBypassAtStartToPreventFlickering = 30;

		/// <summary>
		/// Identifier to help find this component on <see cref="Instances"/> enumerable.
		/// </summary>
		public string Identifier {
			get { return this._identifier; }
			set { this._identifier = value; }
		}

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

		/// <summary>
		/// All awaked instances.
		/// </summary>
		public static IEnumerable<RenderIntervalManager> Instances {
			get {
				if (_instances != null) return _instances;
				return Enumerable.Empty<RenderIntervalManager>();
			}
		}
		private static HashSet<RenderIntervalManager> _instances;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
			this.AddToStaticInstances();
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

		protected virtual void OnDestroy() {
			this.RemoveFromStaticInstances();
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
		/// Check if a request is added and started.
		/// </summary>
		/// <param name="request">Request to check.</param>
		public bool HasRequest(RenderIntervalRequest request) {
			return this._requests != null && this._requests.Contains(request);
		}

		/// <summary>
		/// Start a new render rate request.
		/// </summary>
		/// <param name="interval">Render interval.</param>
		/// <returns>Returns the new request.</returns>
		public RenderIntervalRequest StartRequest(int interval) {
			var request = new RenderIntervalRequest(interval, this);
			if (!request.IsValid) {
				throw new ArgumentOutOfRangeException("request", request, "invalid request");
			}
			if (this._requests == null) this._requests = new List<RenderIntervalRequest>();
			this._requests.Add(request);
			this.RecalculateRenderIntervalIfPlaying();
			return request;
		}

		/// <summary>
		/// Stop and remove a render rate request.
		/// </summary>
		/// <param name="request">Request to remove.</param>
		public void StopRequest(RenderIntervalRequest request) {
			if (!this.HasRequest(request)) return;
			this._requests.Remove(request);
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

			if (this._requests != null) {
				int count = this._requests.Count;
				if (count > 0) {
					RenderIntervalRequest request;
					int myInstanceID = this.GetInstanceID();
					for (int i = count - 1; i >= 0; i--) {
						request = this._requests[i];
						if (request.ManagerInstanceID != myInstanceID || !request.IsValid) {
							this._requests.RemoveAt(i);
							continue;
						}
						anyValidRequest = true;
						minValue = Mathf.Min(minValue, request.Interval);
					}
				}
			}

			if (!anyValidRequest) {
				minValue = this._fallbackRenderInterval;
			}

			this.RenderInterval = minValue;
		}

		private void AddToStaticInstances() {
			if (_instances == null) _instances = new HashSet<RenderIntervalManager>();
			_instances.Add(this);
		}

		private void RemoveFromStaticInstances() {
			if (_instances == null) return;
			_instances.Remove(this);
		}

		#endregion <<---------- General ---------->>




		#region <<---------- Legacy Support ---------->>

		// ObsoletedWarning 2019/05/04 - ObsoletedError 20##/##/##
		[Obsolete("use HasRequest() instead", false)]
		public bool ContainsRequest(RenderIntervalRequest request) {
			return this.HasRequest(request);
		}

		// ObsoletedWarning 2019/05/04 - ObsoletedError 2019/05/04
		[Obsolete("use StartRequest() instead", true)]
		public RenderIntervalRequest AddRequest(RenderIntervalRequest request) {
			return this.StartRequest(request.Interval);
		}

		// ObsoletedWarning 2019/05/04 - ObsoletedError 2019/05/04
		[Obsolete("use StopRequest() instead", true)]
		public void RemoveRequest(RenderIntervalRequest request) {
			this.StopRequest(request);
		}

		#endregion <<---------- Legacy Support ---------->>




		#region <<---------- Custom Inspector ---------->>
		#if UNITY_EDITOR
		[CustomEditor(typeof(RenderIntervalManager))]
		[CanEditMultipleObjects]
		private class CustomInspector : Editor {
			private RenderIntervalManager _script;
			private bool _live;
			private void OnEnable() {
				this._script = (RenderIntervalManager)this.target;
			}
			public override bool RequiresConstantRepaint() {
				return Application.isPlaying && this._live && !this.serializedObject.isEditingMultipleObjects;
			}
			public override void OnInspectorGUI() {
				this.serializedObject.Update();
				this.DrawDefaultInspector();

				if (!Application.isPlaying || this.serializedObject.isEditingMultipleObjects) return;

				EditorGUILayout.Space();
				this._live = EditorGUILayout.ToggleLeft("LIVE", this._live);
				if (!this._live) return;

				int count = this._script._requests == null ? 0 : this._script._requests.Count;
				EditorGUILayout.LabelField("Requests: " + count);
				EditorGUILayout.LabelField("Interval: " + this._script._renderInterval.ToString("000"));
			}
		}
		#endif
		#endregion <<---------- Custom Inspector ---------->>
	}
}