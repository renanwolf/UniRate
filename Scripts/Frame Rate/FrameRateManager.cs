﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PWR.LowPowerMemoryConsumption {

	[DisallowMultipleComponent]
	public class FrameRateManager : MonoBehaviour {

		#region <<---------- Singleton ---------->>

        private static FrameRateManager _instance = null;

		/// <summary>
		/// Singleton instance.
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

		[SerializeField][Range(FrameRateRequest.MinValue,120)] private int _fallbackFrameRate = 18;

		[SerializeField][Range(FrameRateRequest.MinValue,120)] private int _fallbackFixedFrameRate = 18;

		[Space]
		[SerializeField][Range(MinNumberOfSamples,10)] private int _smoothFramesCount = 3;

		/// <summary>
		/// Number of frame rate samples to calculate <see cref="FrameRate"/>.
		/// </summary>
		public int SmoothFramesCount {
			get { return this._smoothFramesCount; }
			set {
				if (value < MinNumberOfSamples) throw new ArgumentOutOfRangeException("SmoothFramesCount", value, "must be greather or equals to " + MinNumberOfSamples);
				this._smoothFramesCount = value;
			}
		}

		/// <summary>
		/// Target frame rate to set if there are no active requests.
		/// </summary>
		public int FallbackFrameRate {
			get { return this._fallbackFrameRate; }
			set {
				if (this._fallbackFrameRate == value) return;
				if (value < FrameRateRequest.MinValue) throw new ArgumentOutOfRangeException("FallbackFrameRate", value, "must be greather or equals to " + FrameRateRequest.MinValue);
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
				if (value < FrameRateRequest.MinValue) throw new ArgumentOutOfRangeException("FallbackFrameRate", value, "must be greather or equals to " + FrameRateRequest.MinValue);
				this._fallbackFixedFrameRate = value;
				this.RecalculateTargetsRateIfPlaying();
			}
		}

		/// <summary>
		/// Event raised when <see cref="FrameRate"/> changes.
		/// </summary>
		public event Action<int> FrameRateChanged;

		/// <summary>
		/// Event raised when <see cref="FixedFrameRate"/> changes.
		/// </summary>
		public event Action<int> FixedFrameRateChanged;

		/// <summary>
		/// Event raised when <see cref="TargetFrameRate"/> changes.
		/// </summary>
		public event Action<int> TargetFrameRateChanged;

		/// <summary>
		/// Event raised when <see cref="TargetFixedFrameRate"/> changes.
		/// </summary>
		public event Action<int> TargetFixedFrameRateChanged;

		private List<int> _samplesFrameRate = new List<int>();

		private int _currentFrameRate = 0;

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

		private int _currentFixedFrameRate = 0;

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

		private int _targetFrameRate = 0;

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

		private int _targetFixedFrameRate = 0;

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
			DontDestroyOnLoad(this);

			if (!FrameRateManager.IsSupported) Debug.LogWarning("[" + typeof(FrameRateManager).Name + "] " + FrameRateManager.NotSupportedMessage, this);
			this.RecalculateTargetsRateIfPlaying();
        }

		private void Update() {

			//calculate current frame rate
			this._samplesFrameRate.Add(Mathf.RoundToInt(1.0f / Time.unscaledDeltaTime));
			int maxSamples = Mathf.Max(MinNumberOfSamples, this._smoothFramesCount);
			while(this._samplesFrameRate.Count > maxSamples) this._samplesFrameRate.RemoveAt(0);
			float sum = 0;
			for (int i = 0; i < this._samplesFrameRate.Count; i++) sum += this._samplesFrameRate[i];
			this.FrameRate = Mathf.RoundToInt( sum / (float)this._samplesFrameRate.Count );

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
			var evnt = this.FrameRateChanged;
			if (evnt == null) return;
			evnt(this._currentFrameRate);
		}

		private void OnCurrentFixedFrameRateChanged() {
			var evnt = this.FixedFrameRateChanged;
			if (evnt == null) return;
			evnt(this._currentFixedFrameRate);
		}

		private void OnTargetFrameRateChanged() {
			this.SetApplicationTargetFrameRate(FrameRateType.FPS, this._targetFrameRate);

			var evnt = this.TargetFrameRateChanged;
			if (evnt == null) return;
			evnt(this._targetFrameRate);
		}

		private void OnTargetFixedFrameRateChanged() {
			this.SetApplicationTargetFrameRate(FrameRateType.FixedFPS, this._targetFixedFrameRate);

			var evnt = this.TargetFixedFrameRateChanged;
			if (evnt == null) return;
			evnt(this._targetFixedFrameRate);
		}

		#endregion <<---------- Internal Callbacks ---------->>




		#region <<---------- Requests Management ---------->>

		/// <summary>
		/// Check if a request is added and active.
		/// </summary>
		/// <param name="request">Request to check.</param>
		/// <returns></returns>
		public bool ContainsRequest(FrameRateRequest request) {
			return request != null && this._requests != null && this._requests.Contains(request);
		}

		/// <summary>
		/// Add and activate a new frame rate request.
		/// </summary>
		/// <param name="request">Request to add.</param>
		/// <returns>Returns the request.</returns>
		public FrameRateRequest AddRequest(FrameRateRequest request) {
			if (request == null) return null;
			if (this.ContainsRequest(request)) return request;
			if (this._requests == null) this._requests = new List<FrameRateRequest>();
			this._requests.Add(request);
			request.Changed += this.NotifyRequestChanged;
			this.RecalculateTargetsRateIfPlaying();
			return request;
		}

		/// <summary>
		/// Remove and deactivate a frame rate request.
		/// </summary>
		/// <param name="request">Request to remove.</param>
		public void RemoveRequest(FrameRateRequest request) {
			if (request == null || !this.ContainsRequest(request)) return;
			this._requests.Remove(request);
			request.Changed -= this.NotifyRequestChanged;
			this.RecalculateTargetsRateIfPlaying();
		}

		private void NotifyRequestChanged(FrameRateRequest request) {
			if (request == null) return;
			if (!this.ContainsRequest(request)) {
				request.Changed -= this.NotifyRequestChanged;
				return;
			}
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

			int newTarget = FrameRateRequest.MinValue - 1;
			int newTargetFixed = FrameRateRequest.MinValue - 1;

			if (this._requests != null && this._requests.Count > 0) {
				for (int i = this._requests.Count - 1; i >= 0; i--) {
					if (this._requests[i] == null) {
						this._requests.RemoveAt(i);
						continue;
					}
					if (!this._requests[i].IsValid) continue;

					switch (this._requests[i].Type) {
						case FrameRateType.FPS:
							newTarget = Mathf.Max(newTarget, this._requests[i].Value);
						break;

						case FrameRateType.FixedFPS:
							newTargetFixed = Mathf.Max(newTargetFixed, this._requests[i].Value);
						break;
					}
				}
			}

			if (newTarget < FrameRateRequest.MinValue) {
				newTarget = this._fallbackFrameRate;
			}
			if (newTargetFixed < FrameRateRequest.MinValue) {
				newTargetFixed = this._fallbackFixedFrameRate;
			}

			this.TargetFrameRate = newTarget;
			this.TargetFixedFrameRate = newTargetFixed;
		}

		#endregion <<---------- General ---------->>




		#region <<---------- Legacy Support ---------->>

		[Obsolete("use CurrentFrameRateChanged event instead", false)] // ObsoletedWarning 2018/08/22 - ObsoletedError 20##/##/##
		public event Action<int> onFrameRate {
			add { this.FrameRateChanged += value; }
			remove { this.FrameRateChanged -= value; }
		}

		[Obsolete("use CurrentFixedFrameRateChanged event instead", false)] // ObsoletedWarning 2018/08/22 - ObsoletedError 20##/##/##
		public event Action<int> onFixedFrameRate {
			add { this.FixedFrameRateChanged += value; }
			remove { this.FixedFrameRateChanged -= value; }
		}

		[Obsolete("use TargetFrameRateChanged event instead", false)] // ObsoletedWarning 2018/08/22 - ObsoletedError 20##/##/##
		public event Action<int> onTargetFrameRate {
			add { this.TargetFrameRateChanged += value; }
			remove { this.TargetFrameRateChanged -= value; }
		}

		[Obsolete("use TargetFixedFrameRateChanged event instead", false)] // ObsoletedWarning 2018/08/22 - ObsoletedError 20##/##/##
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