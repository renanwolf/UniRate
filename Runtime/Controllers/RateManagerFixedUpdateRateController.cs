using System;
using UnityEngine;
using UniRate.Debug;

namespace UniRate.Internals {

    public class RateManagerFixedUpdateRateController : RateManagerValueController {

        #region <<---------- Initializers ---------->>

        public RateManagerFixedUpdateRateController(RateManager rateManager, int minimumFixedUpdateRate) : base(rateManager, minimumFixedUpdateRate, false, "fixed update rate") {

        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        /// <summary>
        /// Minimum fixed update rate per seconds allowed.
        /// </summary>
        public int Minimum {
            get => this.LimitValue;
            set => this.LimitValue = value;
        }

        /// <summary>
        /// Event raised when <see cref="Minimum"/> changes.
        /// </summary>
        public event Action<RateManager, int> MinimumChanged {
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
        /// Create a new <see cref="FixedUpdateRateRequest"/>.
        /// </summary>
        public FixedUpdateRateRequest Request(int fixedUpdateRate) {
            return this.BaseRequest(new FixedUpdateRateRequest(this, fixedUpdateRate), 1);
        }

        protected override void ApplyTargetValueToUnitySettings() {
            float fixedDeltaTime = 1f / (float)this.Target;
            if (fixedDeltaTime == Time.fixedDeltaTime) return;
            if (RateDebug.IsLogLevelActive(RateLogLevel.Trace)) {
                RateDebug.Log(RateLogLevel.Trace, $"setting Time.fixedDeltaTime to {fixedDeltaTime.ToString("0.0##")}");
            }
            Time.fixedDeltaTime = fixedDeltaTime;
        }

        #endregion <<---------- General ---------->>




        #region <<---------- MonoBehaviour Executions ---------->>

        internal void ExecuteMonoBehaviourFixedUpdate(float fixedUnscaledDeltaTime) {
            this.ApplyTargetValueToUnitySettings();
            this.Current = Mathf.RoundToInt(1f / fixedUnscaledDeltaTime);
        }

        #endregion <<---------- MonoBehaviour Executions ---------->>
    }
}