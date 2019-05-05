using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PWR.LowPowerMemoryConsumption {

    [DisallowMultipleComponent]
	public class FrameRateManager : MonoBehaviour {

		#region <<---------- Initializers ---------->>

		protected FrameRateManager() { }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Singleton ---------->>

        private static FrameRateManager _instance = null;

		/// <summary>
		/// Frame rate manager singleton instance.
		/// </summary>
        public static FrameRateManager Instance {
            get {
				if (_instance == null) {
					_instance = GameObject.FindObjectOfType<FrameRateManager>();
					if (_instance == null) {
						var goInstance = new GameObject();
						_instance = goInstance.AddComponent<FrameRateManager>();
					}
				}
                return _instance;
            }
        }

        #endregion <<---------- Singleton ---------->>




		#region <<---------- Properties and Fields ---------->>

		[SerializeField][Range(FrameRateRequest.MinRate, 120)] private int _fallbackFrameRate = 18;

		[SerializeField][Range(FrameRateRequest.MinRate, 120)] private int _fallbackFixedFrameRate = 18;

		/// <summary>
		/// Target frame rate to set if there are no active requests.
		/// </summary>
		public int FallbackFrameRate {
			get { return this._fallbackFrameRate; }
			set {
				if (this._fallbackFrameRate == value) return;
				if (value < FrameRateRequest.MinRate) throw new ArgumentOutOfRangeException("FallbackFrameRate", value, "must be greather or equals to " + FrameRateRequest.MinRate);
				this._fallbackFrameRate = value;
				this.RecalculateTargetsRateIfPlaying();
			}
		}

		/// <summary>
		/// Target fixed frame rate to set if there are no active requests.
		/// </summary>
		public int FallbackFixedFrameRate {
			get { return this._fallbackFixedFrameRate; }
			set {
				if (this._fallbackFixedFrameRate == value) return;
				if (value < FrameRateRequest.MinRate) throw new ArgumentOutOfRangeException("FallbackFrameRate", value, "must be greather or equals to " + FrameRateRequest.MinRate);
				this._fallbackFixedFrameRate = value;
				this.RecalculateTargetsRateIfPlaying();
			}
		}

		/// <summary>
		/// Event raised when <see cref="FrameRate"/> changes.
		/// </summary>
		public event Action<int> FrameRateChanged {
			add {
				this._frameRateChanged -= value;
				this._frameRateChanged += value;
			}
			remove {
				this._frameRateChanged -= value;
			}
		}
		private Action<int> _frameRateChanged;

		/// <summary>
		/// Event raised when <see cref="FixedFrameRate"/> changes.
		/// </summary>
		public event Action<int> FixedFrameRateChanged {
			add {
				this._fixedFrameRateChanged -= value;
				this._fixedFrameRateChanged += value;
			}
			remove {
				this._fixedFrameRateChanged -= value;
			}
		}
		private Action<int> _fixedFrameRateChanged;

		/// <summary>
		/// Event raised when <see cref="TargetFrameRate"/> changes.
		/// </summary>
		public event Action<int> TargetFrameRateChanged {
			add {
				this._targetFrameRateChanged -= value;
				this._targetFrameRateChanged += value;
			}
			remove {
				this._targetFrameRateChanged -= value;
			}
		}
		private Action<int> _targetFrameRateChanged;

		/// <summary>
		/// Event raised when <see cref="TargetFixedFrameRate"/> changes.
		/// </summary>
		public event Action<int> TargetFixedFrameRateChanged {
			add {
				this._targetFixedFrameRateChanged -= value;
				this._targetFixedFrameRateChanged += value;
			}
			remove {
				this._targetFixedFrameRateChanged -= value;
			}
		}
		private Action<int> _targetFixedFrameRateChanged;

		/// <summary>
		/// Current frames per second.
		/// </summary>
		public int FrameRate {
			get { return this._currentFrameRate; }
			private set {
				if (this._currentFrameRate == value) return;
				this._currentFrameRate = value;
				this.OnCurrentFrameRateChanged();
			}
		}
		private int _currentFrameRate = 0;

		/// <summary>
		/// Current fixed frames per second.
		/// </summary>
		public int FixedFrameRate {
			get { return this._currentFixedFrameRate; }
			private set {
				if (this._currentFixedFrameRate == value) return;
				this._currentFixedFrameRate = value;
				this.OnCurrentFixedFrameRateChanged();
			}
		}
		private int _currentFixedFrameRate = 0;

		/// <summary>
		/// Target frames per second.
		/// </summary>
		public int TargetFrameRate {
			get { return this._targetFrameRate; }
			private set {
				if (this._targetFrameRate == value) return;
				this._targetFrameRate = value;
				this.OnTargetFrameRateChanged();
			}
		}
		private int _targetFrameRate = 0;

		/// <summary>
		/// Target fixed frames per second.
		/// </summary>
		public int TargetFixedFrameRate {
			get { return this._targetFixedFrameRate; }
			private set {
				if (this._targetFixedFrameRate == value) return;
				this._targetFixedFrameRate = value;
				this.OnTargetFixedFrameRateChanged();
			}
		}
		private int _targetFixedFrameRate = 0;

		/// <summary>
		/// TargetFrameRate can only be changed if VSync is off.
		/// </summary>
		/// <value>Returns true if <see cref="QualitySettings.vSyncCount"/> is zero or less. Otherwise false.</value>
		public static bool IsSupported {
			get {
				#if UNITY_EDITOR
				return QualitySettings.vSyncCount <= 0;
				#elif UNITY_IOS
				//on iOS VSync is always on, but Application.targetFrameRate seems to work.
				return true;
				#else
				return QualitySettings.vSyncCount <= 0;
				#endif
			}
		}

		private List<FrameRateRequest> _requests;

		public const string NotSupportedMessage = "TargetFrameRate can only be changed if VSync is off.";
		private const string DefaultName = "Frame Rate Manager";
		private const int MinNumberOfSamples = 1;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

        private void Awake() {

			if (_instance == null) {
				_instance = this;
			}
			else if (_instance != this) {

				if (Debug.isDebugBuild) Debug.LogWarning("[" + typeof(FrameRateManager).Name + "] trying to create another instance, destroying it", this);

				#if UNITY_EDITOR
				if (!Application.isPlaying) {
					DestroyImmediate(this);
					return;
				}
				#endif
				Destroy(this);
				return;
			}
			
			this.name = DefaultName;
			this.transform.SetParent(null, false);
			if (Application.isPlaying) DontDestroyOnLoad(this);

			if (!FrameRateManager.IsSupported) Debug.LogWarning("[" + typeof(FrameRateManager).Name + "] " + FrameRateManager.NotSupportedMessage, this);
			this.RecalculateTargetsRateIfPlaying();
        }

		private void Update() {

			//calculate current frame rate
			this.FrameRate = Mathf.RoundToInt(1.0f / Time.unscaledDeltaTime);

			//check if application target frame rate has changed from elsewhere
			if (Application.targetFrameRate != this._targetFrameRate) {
				this.SetApplicationTargetFrameRate(FrameRateType.FPS, this._targetFrameRate);
			}
		}

		private void FixedUpdate() {
			
			//calculate fixed frame rate
			this.FixedFrameRate = Mathf.RoundToInt(1.0f / Time.fixedUnscaledDeltaTime);

			//check if application fixed frame rate has changed from elsewhere
			if (this._currentFixedFrameRate != this._targetFixedFrameRate) {
				this.SetApplicationTargetFrameRate(FrameRateType.FixedFPS, this._targetFixedFrameRate);
			}
		}

		private void OnDestroy() {
			if (_instance != this) return;
			_instance = null;
		}

		#if UNITY_EDITOR
		private void OnValidate() {
			this.name = DefaultName;
			this.RecalculateTargetsRateIfPlaying();
		}
		private void Reset() {
			this.name = DefaultName;
			this.RecalculateTargetsRateIfPlaying();
		}
		#endif

        #endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- Internal Callbacks ---------->>

		private void OnCurrentFrameRateChanged() {
			var evnt = this._frameRateChanged;
			if (evnt == null) return;
			evnt(this._currentFrameRate);
		}

		private void OnCurrentFixedFrameRateChanged() {
			var evnt = this._fixedFrameRateChanged;
			if (evnt == null) return;
			evnt(this._currentFixedFrameRate);
		}

		private void OnTargetFrameRateChanged() {
			this.SetApplicationTargetFrameRate(FrameRateType.FPS, this._targetFrameRate);

			var evnt = this._targetFrameRateChanged;
			if (evnt == null) return;
			evnt(this._targetFrameRate);
		}

		private void OnTargetFixedFrameRateChanged() {
			this.SetApplicationTargetFrameRate(FrameRateType.FixedFPS, this._targetFixedFrameRate);

			var evnt = this._targetFixedFrameRateChanged;
			if (evnt == null) return;
			evnt(this._targetFixedFrameRate);
		}

		#endregion <<---------- Internal Callbacks ---------->>




		#region <<---------- Requests Management ---------->>

		/// <summary>
		/// Check if a frame rate request is added and started.
		/// </summary>
		/// <param name="request">Request to check.</param>
		public bool HasRequest(FrameRateRequest request) {
			return this._requests != null && this._requests.Contains(request);
		}

		/// <summary>
		/// Start a new frame rate request.
		/// </summary>
		/// <param name="rateType">Frame rate type.</param>
		/// <param name="rateValue">Frame rate value.</param>
		/// <returns>Returns the new request.</returns>
		public FrameRateRequest StartRequest(FrameRateType rateType, int rateValue) {
			var request = new FrameRateRequest(rateType, rateValue);
			if (!request.IsValid) {
				throw new ArgumentOutOfRangeException("request", request, "invalid request");
			}
			if (this._requests == null) this._requests = new List<FrameRateRequest>();
			this._requests.Add(request);
			this.RecalculateTargetsRateIfPlaying();
			return request;
		}
		
		/// <summary>
		/// Stop and remove a frame rate request.
		/// </summary>
		/// <param name="request">Request to stop and remove.</param>
		public void StopRequest(FrameRateRequest request) {
			if (!this.HasRequest(request)) return;
			this._requests.Remove(request);
			this.RecalculateTargetsRateIfPlaying();
		}

		#endregion <<---------- Requests Management ---------->>




		#region <<---------- General ---------->>

		private void SetApplicationTargetFrameRate(FrameRateType type, int value) {
			switch (type) {

				case FrameRateType.FPS:
					Application.targetFrameRate = value;
				break;

				case FrameRateType.FixedFPS:
					Time.fixedDeltaTime = 1.0f / (float)value;
				break;
			}
		}

		private void RecalculateTargetsRateIfPlaying() {
			#if UNITY_EDITOR
			if (!Application.isPlaying) return;
			#endif

			int newTarget = FrameRateRequest.MinRate - 1;
			int newTargetFixed = FrameRateRequest.MinRate - 1;

			if (this._requests != null) {
				int count = this._requests.Count;
				if (count > 0) {
					FrameRateRequest request;
					for (int i = count - 1; i >= 0; i--) {
						request = this._requests[i];
						if (!request.IsValid) {
							this._requests.RemoveAt(i);
							continue;
						}
						switch (request.Type) {
							case FrameRateType.FPS:
								newTarget = Mathf.Max(newTarget, request.Rate);
							break;
							case FrameRateType.FixedFPS:
								newTargetFixed = Mathf.Max(newTargetFixed, request.Rate);
							break;
						}
					}
				}
			}

			if (newTarget < FrameRateRequest.MinRate) {
				newTarget = this._fallbackFrameRate;
			}
			if (newTargetFixed < FrameRateRequest.MinRate) {
				newTargetFixed = this._fallbackFixedFrameRate;
			}

			this.TargetFrameRate = newTarget;
			this.TargetFixedFrameRate = newTargetFixed;
		}

		#endregion <<---------- General ---------->>




		#region <<---------- Legacy Support ---------->>

		// ObsoletedWarning 2019/05/04 - ObsoletedError 2019/05/04
		[Obsolete("use StopRequest() instead", true)]
		public void RemoveRequest(FrameRateRequest request) {
			this.StopRequest(request);
		}

		// ObsoletedWarning 2019/05/04 - ObsoletedError 2019/05/04
		[Obsolete("use StartRequest() instead", true)]
		public FrameRateRequest AddRequest(FrameRateRequest request) {
			return this.StartRequest(request.Type, request.Rate);
		}

		// ObsoletedWarning 2019/05/04 - ObsoletedError 20##/##/##
		[Obsolete("use HasRequest() instead", false)]
		public bool ContainsRequest(FrameRateRequest request) {
			return this.HasRequest(request);
		}

		// ObsoletedWarning 2019/05/04 - ObsoletedError 20##/##/##
		[Obsolete("has no effect anymore", false)]
		public int SmoothFramesCount {
			get { return 0; }
			set { }
		}

		// ObsoletedWarning 2018/08/22 - ObsoletedError 2019/05/04
		[Obsolete("use CurrentFrameRateChanged event instead", true)]
		public event Action<int> onFrameRate {
			add { this.FrameRateChanged += value; }
			remove { this.FrameRateChanged -= value; }
		}

		// ObsoletedWarning 2018/08/22 - ObsoletedError 2019/05/04
		[Obsolete("use CurrentFixedFrameRateChanged event instead", true)]
		public event Action<int> onFixedFrameRate {
			add { this.FixedFrameRateChanged += value; }
			remove { this.FixedFrameRateChanged -= value; }
		}

		// ObsoletedWarning 2018/08/22 - ObsoletedError 2019/05/04
		[Obsolete("use TargetFrameRateChanged event instead", true)]
		public event Action<int> onTargetFrameRate {
			add { this.TargetFrameRateChanged += value; }
			remove { this.TargetFrameRateChanged -= value; }
		}

		// ObsoletedWarning 2018/08/22 - ObsoletedError 2019/05/04
		[Obsolete("use TargetFixedFrameRateChanged event instead", true)]
		public event Action<int> onTargetFixedFrameRate {
			add { this.TargetFixedFrameRateChanged += value; }
			remove { this.TargetFixedFrameRateChanged -= value; }
		}

		#endregion <<---------- Legacy Support ---------->>




		#region <<---------- Custom Inspector ---------->>
		#if UNITY_EDITOR
		[CustomEditor(typeof(FrameRateManager))]
		private class CustomInspector : Editor {

			public override void OnInspectorGUI() {
				this.serializedObject.Update();
				this.DrawDefaultInspector();

				if (!FrameRateManager.IsSupported) {
					EditorGUILayout.HelpBox(FrameRateManager.NotSupportedMessage, MessageType.Warning);
				}
			}
		}
		#endif
		#endregion <<---------- Custom Inspector ---------->>
	}
}