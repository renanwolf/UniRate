using System;
using System.Collections.Generic;
using UniRate.Debug;

namespace UniRate.Internals {

    public abstract class RateManagerValueController {

        #region <<---------- Initializers ---------->>

        protected RateManagerValueController(RateManager rateManager, int limitValue, bool limitByMax, string valueName) {
            this._rateManager = rateManager;
            this._limitByMax = limitByMax;

            this._valueName = valueName;
            this._limitValueName = $"{(this._limitByMax ? "maximum" : "minimum")} {this._valueName}";
            this._targetValueName = $"target {this._valueName}";

            this._limitValue = limitValue;
            if (RateDebug.IsLogLevelActive(RateLogLevel.Info)) {
                RateDebug.Log(RateLogLevel.Info, $"created controller with {this._limitValueName} {this._limitValue.ToString()}");
            }

            this._requests = new HashSet<RateRequest>();
            this._isTargetValueDirty = true;
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        protected RateManager RateManager => this._rateManager;
        private readonly RateManager _rateManager;

        private readonly bool _limitByMax;

        protected string ValueName => this._valueName;
        private readonly string _valueName;

        private readonly string _limitValueName;
        private readonly string _targetValueName;

        internal int LimitValue {
            get => this._limitValue;
            set {
                if (this._limitValue == value) return;
                this._limitValue = value;
                this.OnLimitValueChanged(this._limitValue);
            }
        }
        private int _limitValue;

        /// <summary>
        /// Target rate/interval.
        /// </summary>
        public int Target {
            get => this.RecalculateTargetValueIfDirty();
            private set {
                if (this._targetValue == value) return;
                this._targetValue = value;
                this.OnTargetValueChanged(this._targetValue);
            }
        }
        private int _targetValue;

        /// <summary>
        /// Current rate/interval.
        /// </summary>
        public int Current {
            get => this._currentValue;
            protected set {
                if (this._currentValue == value) return;
                this._currentValue = value;
                this.OnCurrentValueChanged(this._currentValue);
            }
        }
        private int _currentValue;

        private readonly HashSet<RateRequest> _requests;

        private bool _isTargetValueDirty;

        /// <summary>
		/// Event raised when <see cref="LimitValue"/> changes.
		/// </summary>
        internal event Action<RateManager, int> LimitChanged {
            add {
                this._limitValueChanged -= value;
                this._limitValueChanged += value;
            }
            remove {
                this._limitValueChanged -= value;
            }
        }
        private Action<RateManager, int> _limitValueChanged;

        /// <summary>
		/// Event raised when <see cref="Target"/> changes.
		/// </summary>
        public event Action<RateManager, int> TargetChanged {
            add {
                this._targetValueChanged -= value;
                this._targetValueChanged += value;
            }
            remove {
                this._targetValueChanged -= value;
            }
        }
        private Action<RateManager, int> _targetValueChanged;

        /// <summary>
		/// Event raised when <see cref="Current"/> changes.
		/// </summary>
        public event Action<RateManager, int> CurrentChanged {
            add {
                this._currentValueChanged -= value;
                this._currentValueChanged += value;
            }
            remove {
                this._currentValueChanged -= value;
            }
        }
        private Action<RateManager, int> _currentValueChanged;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Callbacks ---------->>

        protected virtual void OnLimitValueChanged(int limitValue) {
            if (RateDebug.IsLogLevelActive(RateLogLevel.Info)) {
                RateDebug.Log(RateLogLevel.Info, $"{this._limitValueName} changed to {limitValue.ToString()}");
            }
            this._isTargetValueDirty = true;
            var e = this._limitValueChanged;
            if (e == null) return;
            e(this.RateManager, limitValue);
        }

        protected virtual void OnTargetValueChanged(int targetValue) {
            if (RateDebug.IsLogLevelActive(RateLogLevel.Debug)) {
                RateDebug.Log(RateLogLevel.Debug, $"{this._targetValueName} changed to {targetValue.ToString()}");
            }
            this.ApplyTargetValueToUnitySettings();
            var e = this._targetValueChanged;
            if (e == null) return;
            e(this._rateManager, targetValue);
        }

        protected virtual void OnCurrentValueChanged(int currentValue) {
            var e = this._currentValueChanged;
            if (e == null) return;
            e(this._rateManager, currentValue);
        }

        protected virtual void OnRequestsChanged() {
            this._isTargetValueDirty = true;
        }

        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>

        protected T BaseRequest<T>(T request, int trackerSkipStackTraceFrames) where T : RateRequest {
            if (!this._requests.Add(request)) return request;
            if (RateDebug.IsLogLevelActive(RateLogLevel.Trace)) {
                RateDebug.Log(RateLogLevel.Trace, $"creating {this._valueName} request {request.Value.ToString()}");
            }
            RateRequestTracker.ReportStarted(request, trackerSkipStackTraceFrames + 1);
            this.OnRequestsChanged();
            return request;
        }

        internal void CancelRequest(RateRequest request, int trackerSkipStackTraceFrames) {
            if (request == null) return;
            if (!this._requests.Remove(request)) return;
            if (RateDebug.IsLogLevelActive(RateLogLevel.Trace)) {
                RateDebug.Log(RateLogLevel.Trace, $"canceled {this._valueName} request {request.Value.ToString()}");
            }
            RateRequestTracker.ReportFinished(request, trackerSkipStackTraceFrames + 1);
            this.OnRequestsChanged();
        }

        private int RecalculateTargetValueIfDirty() {
            if (!this._isTargetValueDirty) return this._targetValue;
            this._isTargetValueDirty = false;
            int target = this.CalculateTargetValue(this._requests, this._limitValue);
            this.Target = target;
            return target;
        }

        private int CalculateTargetValue(IEnumerable<RateRequest> requests, int limitValue) {
            int targetValue = limitValue;
            foreach (var request in requests) {
                if (request.IsDisposed) continue;
                if (this._limitByMax) {
                    if (request.Value >= targetValue) continue;
                }
                else if (request.Value <= targetValue) {
                    continue;
                }
                targetValue = request.Value;
            }
            return targetValue;
        }

        protected abstract void ApplyTargetValueToUnitySettings();

        /// <summary>
        /// Recalculate <see cref="Target"/> and apply it now if changed, instead of waiting Unity's next update cycle.
        /// </summary>
        public void ApplyTargetIfDirty() {
            this.RecalculateTargetValueIfDirty();
        }

        #endregion <<---------- General ---------->>
    }
}