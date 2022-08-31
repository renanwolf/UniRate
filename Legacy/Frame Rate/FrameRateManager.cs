using System;
using UnityEngine;
using UniRate;

namespace PWR.LowPowerMemoryConsumption {

    [DisallowMultipleComponent]
	[Obsolete("OBSOLETE, use RateManager inside UniRate namespace instead.")]
	public class FrameRateManager : MonoBehaviour {

		#region <<---------- Initializers ---------->>

		protected FrameRateManager() { }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- Singleton ---------->>

        private static FrameRateManager _instance = null;

		[Obsolete("OBSOLETE, use RateManager.Instance instead.")]
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

		[Obsolete("OBSOLETE, use RateManager.Instance.MinimumUpdateRate instead.")]
		public int FallbackFrameRate {
			get => RateManager.Instance.MinimumUpdateRate;
			set => RateManager.Instance.MinimumUpdateRate = value;
		}

		[Obsolete("OBSOLETE, use RateManager.Instance.MinimumFixedUpdateRate instead.")]
		public int FallbackFixedFrameRate {
			get => RateManager.Instance.MinimumFixedUpdateRate;
			set => RateManager.Instance.MinimumFixedUpdateRate = value;
		}

		[Obsolete("OBSOLETE, use RateManager.Instance.UpdateRateChanged instead.")]
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

		[Obsolete("OBSOLETE, use RateManager.Instance.FixedUpdateRateChanged instead.")]
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

		[Obsolete("OBSOLETE, use RateManager.Instance.TargetUpdateRateChanged instead.")]
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

		[Obsolete("OBSOLETE, use RateManager.Instance.TargetFixedUpdateRateChanged instead.")]
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

		[Obsolete("OBSOLETE, use RateManager.Instance.UpdateRate.Current instead.")]
		public int FrameRate => RateManager.Instance.UpdateRate.Current;

		[Obsolete("OBSOLETE, use RateManager.Instance.FixedUpdateRate.Current instead.")]
		public int FixedFrameRate => RateManager.Instance.FixedUpdateRate.Current;

		[Obsolete("OBSOLETE, use RateManager.Instance.TargetUpdateRate instead.")]
		public int TargetFrameRate => RateManager.Instance.TargetUpdateRate;

		[Obsolete("OBSOLETE, use RateManager.Instance.TargetFixedUpdateRate instead.")]
		public int TargetFixedFrameRate => RateManager.Instance.TargetFixedUpdateRate;

		[Obsolete]
		public static bool IsSupported => true;

		private const string DefaultName = "Frame Rate Manager (OBSOLETE)";

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

			// TODO hook into new rate manager

			var rateManager = RateManager.Instance;
			rateManager.FixedUpdateRateChanged += this.OnFixedUpdateRateChanged;
			rateManager.TargetFixedUpdateRateChanged += this.OnTargetFixedUpdateRateChanged;
			rateManager.TargetUpdateRateChanged += this.OnTargetUpdateRateChanged;
			rateManager.UpdateRateChanged += this.OnUpdateRateChanged;
        }

		private void OnDestroy() {
			if (_instance != this) return;
			_instance = null;

			var rateManager = RateManager.Instance;
			if (rateManager == null) return;
			rateManager.FixedUpdateRateChanged -= this.OnFixedUpdateRateChanged;
			rateManager.TargetFixedUpdateRateChanged -= this.OnTargetFixedUpdateRateChanged;
			rateManager.TargetUpdateRateChanged -= this.OnTargetUpdateRateChanged;
			rateManager.UpdateRateChanged -= this.OnUpdateRateChanged;
		}

		#if UNITY_EDITOR
		private void OnValidate() {
			this.name = DefaultName;
		}
		private void Reset() {
			this.name = DefaultName;
		}
		#endif

        #endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- UniRate Callbacks ---------->>
		
		private void OnFixedUpdateRateChanged(RateManager manager, int rate) {
			var e = this._fixedFrameRateChanged;
			if (e == null) return;
			e(rate);
		}
		
		private void OnTargetFixedUpdateRateChanged(RateManager manager, int rate) {
			var e = this._targetFixedFrameRateChanged;
			if (e == null) return;
			e(rate);
		}
		
		private void OnTargetUpdateRateChanged(RateManager manager, int rate) {
			var e = this._targetFrameRateChanged;
			if (e == null) return;
			e(rate);
		}
		
		private void OnUpdateRateChanged(RateManager manager, int rate) {
			var e = this._frameRateChanged;
			if (e == null) return;
			e(rate);
		}
		
		#endregion <<---------- UniRate Callbacks ---------->>




		#region <<---------- Requests Management ---------->>

		[Obsolete("OBSOLETE, use RateManager.Instance.RequestUpdateRate() or RateManager.Instance.RequestFixedUpdateRate() instead.")]
		public FrameRateRequest StartRequest(FrameRateType rateType, int rateValue) {
			if (rateValue <= 0) return FrameRateRequest.Invalid;

			RateRequest request;
			if (rateType == FrameRateType.FPS) {
				request = RateManager.Instance.RequestUpdateRate(rateValue);
			}
			else {
				request = RateManager.Instance.RequestFixedUpdateRate(rateValue);
			}
			if (request == null) return FrameRateRequest.Invalid;

			return new FrameRateRequest(request);
		}
		
		[Obsolete]
		public void StopRequest(FrameRateRequest request) {
			if (request.UniRateRequest == null || request.UniRateRequest.IsDisposed) return;
			request.UniRateRequest.Dispose();
		}

		#endregion <<---------- Requests Management ---------->>
	}
}