using System;
using UnityEngine;
using UniRate.Debug;

namespace UniRate.Internals {

    public class RateManagerUpdateRateController : RateManagerValueController {

        #region <<---------- Initializers ---------->>

        public RateManagerUpdateRateController(RateManager rateManager, int minimumUpdateRate, UpdateRateMode mode) : base(rateManager, minimumUpdateRate, false, "update rate") {
            this._mode = mode;
            if (RateDebug.IsLogLevelActive(RateLogLevel.Info)) {
                RateDebug.Log(RateLogLevel.Info, $"created controller with {this.ValueName} mode {this._mode.ToString()}");
            }
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        /// <summary>
        /// Minimum update rate per seconds allowed.
        /// </summary>
        public int Minimum {
            get => this.LimitValue;
            set => this.LimitValue = value;
        }

        /// <summary>
        /// Update rate mode.
        /// </summary>
        public UpdateRateMode Mode {
            get => this._mode;
            set {
                if (this._mode == value) return;
                this._mode = value;
                this.OnModeChanged(this._mode);
            }
        }
        private UpdateRateMode _mode;

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

        /// <summary>
		/// Event raised when <see cref="Mode"/> changes.
		/// </summary>
        public event Action<RateManager, UpdateRateMode> ModeChanged {
            add {
                this._modeChanged -= value;
                this._modeChanged += value;
            }
            remove {
                this._modeChanged -= value;
            }
        }
        private Action<RateManager, UpdateRateMode> _modeChanged;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Callbacks ---------->>

        protected virtual void OnModeChanged(UpdateRateMode mode) {
            if (RateDebug.IsLogLevelActive(RateLogLevel.Info)) {
                RateDebug.Log(RateLogLevel.Info, $"{this.ValueName} mode changed to {mode.ToString()}");
            }
            this.ApplyTargetValueToUnitySettings();
            var e = this._modeChanged;
            if (e == null) return;
            e(this.RateManager, mode);
        }

        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>

        /// <summary>
        /// Create a new <see cref="UpdateRateRequest"/>.
        /// </summary>
        public UpdateRateRequest Request(int updateRate) {
            return this.BaseRequest(new UpdateRateRequest(this, updateRate), 1);
        }

        protected override void ApplyTargetValueToUnitySettings() {
            int targetFrameRate = this.Target;
            int vSyncCount;

            switch (this.Mode) {

                case UpdateRateMode.ApplicationTargetFrameRate:
                    vSyncCount = 0;
                    break;

                case UpdateRateMode.VSyncCount:
                    // UNITY DOCS https://docs.unity3d.com/ScriptReference/QualitySettings-vSyncCount.html?_ga=2.244925125.1929096148.1586530815-1497395495.1586530815
                    // If QualitySettings.vSyncCount is set to a value other than 'Don't Sync' (0), the value of Application.targetFrameRate will be ignored.
                    // QualitySettings.vSyncCount value must be 0, 1, 2, 3, or 4.
                    // QualitySettings.vSyncCount is ignored on iOS.
                    vSyncCount = Mathf.Clamp(
                        Screen.currentResolution.refreshRate / targetFrameRate,
                        1,
                        4
                    );
                    break;

                default: throw new NotImplementedException($"not handling {nameof(UpdateRateMode)}.{this.Mode.ToString()}");
            }

            if (QualitySettings.vSyncCount != vSyncCount) {
                if (RateDebug.IsLogLevelActive(RateLogLevel.Trace)) {
                    RateDebug.Log(RateLogLevel.Trace, $"setting QualitySettings.vSyncCount to {vSyncCount.ToString()}");
                }
                QualitySettings.vSyncCount = vSyncCount;
            }

            if (Application.targetFrameRate != targetFrameRate) {
                if (RateDebug.IsLogLevelActive(RateLogLevel.Trace)) {
                    RateDebug.Log(RateLogLevel.Trace, $"setting Application.targetFrameRate to {targetFrameRate.ToString()}");
                }
                Application.targetFrameRate = targetFrameRate;
            }
        }

        #endregion <<---------- General ---------->>




        #region <<---------- MonoBehaviour Executions ---------->>

        internal void ExecuteMonoBehaviourUpdate(float unscaledDeltaTime) {
            this.ApplyTargetValueToUnitySettings();
            this.Current = Mathf.RoundToInt(1f / unscaledDeltaTime);
        }

        #endregion <<---------- MonoBehaviour Executions ---------->>
    }
}