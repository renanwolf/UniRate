using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace UniRate {

    [DisallowMultipleComponent]
    public sealed class RateManager : MonoBehaviour {
        
        #region <<---------- Initializers ---------->>
        
        private RateManager() { }
        
        #endregion <<---------- Initializers ---------->>




        #region <<---------- Instance ---------->>
        
        /// <summary>
        /// The singleton instance of <see cref="RateManager"/>.
        /// </summary>
        public static RateManager Instance {
            get {
                #if UNITY_EDITOR
                if (!Application.isPlaying) {
                    return _instance;
                }
                #endif
                if (!IsApplicationQuitting && _instance == null) {
                    _instance = UnityObject.FindObjectOfType<RateManager>();
                    if (_instance == null) {
                        var go = new GameObject();
                        _instance = go.AddComponent<RateManager>();
                    }
                }
                return _instance;
            }
        }
        private static RateManager _instance;
        
        #endregion <<---------- Instance ---------->>




        #region <<---------- Properties and Fields ---------->>

        [Space]
        [SerializeField] private UpdateRateMode _updateRateMode = UpdateRateMode.ApplicationTargetFrameRate;
        [SerializeField] private int _fallbackUpdateRate = 20;

        /// <summary>
        /// Target update rate per seconds.
        /// </summary>
        public int TargetUpdateRate {
            get => this._targetUpdateRate;
            private set {
                if (this._targetUpdateRate == value) return;
                this._targetUpdateRate = value;
                this.OnTargetUpdateRateChanged(this._targetUpdateRate);
            }
        }
        private int _targetUpdateRate;

        /// <summary>
        /// Update rate mode.
        /// </summary>
        public UpdateRateMode UpdateRateMode {
            get => this._updateRateMode;
            set {
                if (this._updateRateMode == value) return;
                this._updateRateMode = value;
                this.OnUpdateRateModeChanged(this._updateRateMode);
            }
        }

        /// <summary>
        /// Current update rate per seconds.
        /// </summary>
        public int UpdateRate {
            get => this._updateRate;
            private set {
                if (this._updateRate == value) return;
                this._updateRate = value;
                this.OnUpdateRateChanged(this._updateRate);
            }
        }
        private int _updateRate;

        /// <summary>
        /// Fallback update rate per seconds when there are no active requests.
        /// </summary>
        public int FallbackUpdateRate {
            get => this._fallbackUpdateRate;
            set {
                if (this._fallbackUpdateRate == value) return;
                this._fallbackUpdateRate = value;
                this.OnFallbackUpdateRateChanged(this._fallbackUpdateRate);
            }
        }

        /// <summary>
		/// Event raised when <see cref="TargetUpdateRate"/> changes.
		/// </summary>
        public event Action<RateManager, int> TargetUpdateRateChanged {
            add {
                this._targetUpdateRateChanged -= value;
                this._targetUpdateRateChanged += value;
            }
            remove {
                this._targetUpdateRateChanged -= value;
            }
        }
        private Action<RateManager, int> _targetUpdateRateChanged;

        /// <summary>
		/// Event raised when <see cref="UpdateRateMode"/> changes.
		/// </summary>
        public event Action<RateManager, UpdateRateMode> UpdateRateModeChanged {
            add {
                this._updateRateModeChanged -= value;
                this._updateRateModeChanged += value;
            }
            remove {
                this._updateRateModeChanged -= value;
            }
        }
        private Action<RateManager, UpdateRateMode> _updateRateModeChanged;

        /// <summary>
		/// Event raised when <see cref="UpdateRate"/> changes.
		/// </summary>
        public event Action<RateManager, int> UpdateRateChanged {
            add {
                this._updateRateChanged -= value;
                this._updateRateChanged += value;
            }
            remove {
                this._updateRateChanged -= value;
            }
        }
        private Action<RateManager, int> _updateRateChanged;

        private Coroutine _coroutineThrottleEndOfFrame;

        private readonly HashSet<UpdateRateRequest> _updateRateRequests = new HashSet<UpdateRateRequest>();
        
        internal static bool IsApplicationQuitting { get; private set; }
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>
        
        private void Awake() {
            
            if (_instance == null) {
                _instance = this;
            }
            else if (_instance != this) {
                Debug.LogWarning($"[{nameof(RateManager)}] trying to create another instance at '{this.gameObject.scene.name}/{this.gameObject.name}', destroying it", this);
                Destroy(this);
                return;
            }

            this.gameObject.name = nameof(RateManager);
            this.transform.SetParent(null, false);
            DontDestroyOnLoad(this);
        }

        private void OnEnable() {
            this.ApplyTargetUpdateRate(this._updateRateRequests, this._fallbackUpdateRate);
        }

        private void Update() {
            this.UpdateRate = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
            this.ApplyUpdateRateUnitySettings(this._updateRateMode, this._targetUpdateRate);
        }

        private void OnApplicationQuit() {
            IsApplicationQuitting = true;
        }
        
        private void OnDestroy() {
			if (_instance != this) return;
			_instance = null;
		}

        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- Callbacks ---------->>
        
        private void OnTargetUpdateRateChanged(int targetUpdateRate) {
            this.ApplyUpdateRateSettings(this._updateRateMode, targetUpdateRate);
            var e = this._targetUpdateRateChanged;
            if (e == null) return;
            e(this, targetUpdateRate);
        }

        private void OnUpdateRateModeChanged(UpdateRateMode updateRateMode) {
            this.ApplyUpdateRateSettings(updateRateMode, this._targetUpdateRate);
            var e = this._updateRateModeChanged;
            if (e == null) return;
            e(this, updateRateMode);
        }
        
        private void OnUpdateRateChanged(int updateRate) {
            var e = this._updateRateChanged;
            if (e == null) return;
            e(this, updateRate);
        }

        private void OnFallbackUpdateRateChanged(int fallbackUpdateRate) {
            this.ApplyTargetUpdateRate(this._updateRateRequests, fallbackUpdateRate);
        }

        private void OnUpdateRateRequestsChanged(IEnumerable<UpdateRateRequest> requests) {
            this.ApplyTargetUpdateRate(requests, this._fallbackUpdateRate);
        }

        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>

        /// <summary>
        /// Create a new <see cref="UpdateRateRequest"/>.
        /// </summary>
        public UpdateRateRequest CreateUpdateRateRequest(int updateRate) {
            if (Debug.isDebugBuild) {
                Debug.Log($"[{nameof(RateManager)}] creating update rate request {updateRate.ToString()}");
            }
            var request = new UpdateRateRequest(this, updateRate);
            this._updateRateRequests.Add(request);
            this.OnUpdateRateRequestsChanged(this._updateRateRequests);
            return request;
        }

        internal void CancelUpdateRateRequest(UpdateRateRequest request) {
            if (request == null) return;
            if (Debug.isDebugBuild) {
                Debug.Log($"[{nameof(RateManager)}] removing update rate request {request.UpdateRate.ToString()}");
            }
            if (!this._updateRateRequests.Remove(request)) return;
            this.OnUpdateRateRequestsChanged(this._updateRateRequests);
        }
        
        private void ApplyUpdateRateUnitySettings(UpdateRateMode updateRateMode, int targetUpdateRate) {
            switch (updateRateMode) {
                
                case UpdateRateMode.ApplicationTargetFrameRate:
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = targetUpdateRate;
                    break;
                
                case UpdateRateMode.VSyncCount:
                    // UNITY DOCS https://docs.unity3d.com/ScriptReference/QualitySettings-vSyncCount.html?_ga=2.244925125.1929096148.1586530815-1497395495.1586530815
                    // If QualitySettings.vSyncCount is set to a value other than 'Don't Sync' (0), the value of Application.targetFrameRate will be ignored.
                    // QualitySettings.vSyncCount value must be 0, 1, 2, 3, or 4.
                    // QualitySettings.vSyncCount is ignored on iOS.
                    QualitySettings.vSyncCount = Mathf.Clamp(
                        Mathf.FloorToInt((float)Screen.currentResolution.refreshRate / (float)targetUpdateRate),
                        0,
                        4
                    );
                    Application.targetFrameRate = targetUpdateRate;
                    break;
                
                case UpdateRateMode.ThrottleEndOfFrame:
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = 9999;
                    break;
            }
        }

        private void ApplyUpdateRateSettings(UpdateRateMode updateRateMode, int targetUpdateRate) {
            bool isThrottleEndOfFrame = (updateRateMode == UpdateRateMode.ThrottleEndOfFrame);

            if (!isThrottleEndOfFrame && this._coroutineThrottleEndOfFrame != null) {
                this.StopCoroutine(this._coroutineThrottleEndOfFrame);
                this._coroutineThrottleEndOfFrame = null;
            }

            this.ApplyUpdateRateUnitySettings(updateRateMode, targetUpdateRate);

            if (isThrottleEndOfFrame && this._coroutineThrottleEndOfFrame == null) {
                this._coroutineThrottleEndOfFrame = this.StartCoroutine(this.ThrottleEndOfFrame());
            }
        }

        private void ApplyTargetUpdateRate(IEnumerable<UpdateRateRequest> requests, int fallback) {
            var target = int.MinValue;
            bool anyRequest = false;
            foreach (var request in requests) {
                if (request == null || request.IsDisposed) continue;
                anyRequest = true;
                target = Mathf.Max(target, request.UpdateRate);
            }
            this.TargetUpdateRate = (anyRequest ? target : fallback);
        }

        private IEnumerator ThrottleEndOfFrame() {
            // https://blogs.unity3d.com/pt/2019/06/03/precise-framerates-in-unity/
            var currentFrameTime = Time.realtimeSinceStartup;
            while (true) {
                yield return new WaitForEndOfFrame();
                currentFrameTime += (1f / (float)this._targetUpdateRate);
                var t = Time.realtimeSinceStartup;
                var sleepTime = (currentFrameTime - t - 0.01f);
                if (sleepTime > 0) {
                    Thread.Sleep((int)(sleepTime * 1000));
                }
                while (t < currentFrameTime) {
                    t = Time.realtimeSinceStartup;
                }
            }
        }
        
        #endregion <<---------- General ---------->>
    }
}