using System;
using UniRate.Debug;

#if UNITY_2019_3_OR_NEWER
using UnityEngine.Rendering;
#endif

namespace UniRate.Internals {

    public class RateManagerRenderIntervalController : RateManagerValueController {

        #region <<---------- Initializers ---------->>

        public RateManagerRenderIntervalController(RateManager rateManager, int maximumRenderInterval) : base(rateManager, maximumRenderInterval, true, "render interval") {

        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        /// <summary>
        /// Is render interval supported?
        /// </summary>
        public bool IsSupported {
#if UNITY_2019_3_OR_NEWER
            get => true;
#else
            get => false;
#endif
        }

        /// <summary>
        /// Maximum render interval allowed.
        /// </summary>
        public int Maximum {
            get => this.LimitValue;
            set => this.LimitValue = value;
        }

        /// <summary>
        /// Current render rate.
        /// </summary>
        public int CurrentRenderRate {
#if UNITY_2019_3_OR_NEWER
            get => OnDemandRendering.effectiveRenderFrameRate;
#else
            get => this.RateManager.UpdateRate.Current;
#endif
        }

        /// <summary>
        /// Will current frame render?
        /// </summary>
        public bool WillRender {
#if UNITY_2019_3_OR_NEWER
            get => OnDemandRendering.willCurrentFrameRender;
#else
            get => true;
#endif
        }

#if !UNITY_2019_3_OR_NEWER
        private bool _loggedNotSupportedOnce;
#endif

        /// <summary>
        /// Event raised when <see cref="Maximum"/> changes.
        /// </summary>
        public event Action<RateManager, int> MaximumChanged {
            add {
                this.LimitChanged += value;
            }
            remove {
                this.LimitChanged -= value;
            }
        }

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- General ---------->>

        /// <summary>
        /// Create a new <see cref="RenderIntervalRequest"/>.
        /// </summary>
        public RenderIntervalRequest Request(int renderInterval) {
            var request = this.BaseRequest(new RenderIntervalRequest(this, renderInterval), 1);
#if !UNITY_2019_3_OR_NEWER
            this.LogNotSupportedOnce();
#endif
            return request;
        }

        protected override void ApplyTargetValueToUnitySettings() {
#if UNITY_2019_3_OR_NEWER
            var targetRednerInterval = this.Target;
            if (OnDemandRendering.renderFrameInterval == targetRednerInterval) return;
            if (RateDebug.IsLogLevelActive(RateLogLevel.Trace)) {
                RateDebug.Log(RateLogLevel.Trace, $"setting OnDemandRendering.renderFrameInterval to {targetRednerInterval.ToString()}");
            }
            OnDemandRendering.renderFrameInterval = targetRednerInterval;
#endif
        }

#if !UNITY_2019_3_OR_NEWER
        private void LogNotSupportedOnce() {
            if (this._loggedNotSupportedOnce) return;
            this._loggedNotSupportedOnce = true;
            if (RateDebug.IsLogLevelActive(RateLogLevel.Warning)) {
                RateDebug.Log(RateLogLevel.Warning, $"{this.ValueName} is only supported on Unity 2019.3 or newer");
            }
        }
#endif

        #endregion <<---------- General ---------->>




        #region <<---------- MonoBehaviour Executions ---------->>

        internal void ExecuteMonoBehaviourUpdate() {
            this.ApplyTargetValueToUnitySettings();
#if UNITY_2019_3_OR_NEWER
            this.Current = OnDemandRendering.renderFrameInterval;
#else
            this.CurrentValue = 1;
#endif
        }

        #endregion <<---------- MonoBehaviour Executions ---------->>
    }
}