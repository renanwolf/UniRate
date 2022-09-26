using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UniRate.Debug {

    public static class RateRequestTracker {

#if UNITY_EDITOR
        #region <<---------- Properties and Fields ---------->>

        public static bool IsEnabled {
            get => _isEnabled;
            set {
                if (_isEnabled == value) return;
                _isEnabled = value;
                OnIsEnabledChanged(_isEnabled);
            }
        }
        private static bool _isEnabled;

        public static bool IsStackTraceEnabled { get; set; }

        private static readonly Dictionary<int, RateRequestTrackInfo> _infos = new Dictionary<int, RateRequestTrackInfo>();

        public static event Action InfosChanged {
            add {
                _infosChanged -= value;
                _infosChanged += value;
            }
            remove {
                _infosChanged -= value;
            }
        }
        private static Action _infosChanged;

        #endregion <<---------- Properties and Fields ---------->>
#endif




#if UNITY_EDITOR
        #region <<---------- Callbacks ---------->>

        private static void OnIsEnabledChanged(bool isEnabled) {
            if (RateDebug.IsLogLevelActive(RateLogLevel.Info)) {
                RateDebug.Log(RateLogLevel.Info, $"requests tracker is {(isEnabled ? "enabled" : "disabled")}");
            }
        }

        private static void OnInfosChanged() {
            var e = _infosChanged;
            if (e == null) return;
            e();
        }

        #endregion <<---------- Callbacks ---------->>
#endif




#if UNITY_EDITOR
        #region <<---------- General ---------->>

        public static void GetInfos(List<RateRequestTrackInfo> list) {
            list.Clear();
            list.AddRange(_infos.Values);
        }

        #endregion <<---------- General ---------->>
#endif




        #region <<---------- Requests Report ---------->>

        [Conditional("UNITY_EDITOR")]
        internal static void ReportStarted(RateRequest request, int skipStackTraceFrames) {
#if UNITY_EDITOR
            if (!_isEnabled) return;
            var trackInfo = RateRequestTrackInfo.ForRequestDidStarted(request, IsStackTraceEnabled, skipStackTraceFrames + 1);
            _infos[trackInfo.Identifier] = trackInfo;
            OnInfosChanged();
#endif
        }

        [Conditional("UNITY_EDITOR")]
        internal static void ReportFinished(RateRequest request, int skipStackTraceFrames) {
#if UNITY_EDITOR
            int identifier = request.GetHashCode();
            if (!_infos.TryGetValue(identifier, out var trackInfo)) {
                if (!_isEnabled) return;
                trackInfo = RateRequestTrackInfo.ForRequestUnknownStarted(request);
            }
            trackInfo = trackInfo.ForRequestDidFinished(IsStackTraceEnabled, skipStackTraceFrames + 1);
            _infos[trackInfo.Identifier] = trackInfo;
            OnInfosChanged();
#endif
        }

        #endregion <<---------- Requests Report ---------->>
    }
}