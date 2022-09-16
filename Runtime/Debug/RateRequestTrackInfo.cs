using System;
using System.Linq;

namespace UniRate.Debug {

    public readonly struct RateRequestTrackInfo {

        #region <<---------- Initializers ---------->>

        private RateRequestTrackInfo(int identifier, RateRequestType type, int value, DateTime startedTime, string stackTraceStart, bool isActive, DateTime finishedTime, string stackTraceFinish) {
            this.Identifier = identifier;
            this.Type = type;
            this.Value = value;
            this.StartedTime = startedTime;
            this.StackTraceStart = stackTraceStart;
            this.StackTraceStartFirstLine = stackTraceStart?.Split(_stackTraceSplitChars, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            this.IsActive = isActive;
            this.FinishedTime = finishedTime;
            this.StackTraceFinish = stackTraceFinish;
            this.StackTraceFinishFirstLine = stackTraceFinish?.Split(_stackTraceSplitChars, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        }

        public static RateRequestTrackInfo ForRequestDidStarted(RateRequest request, bool captureStackTrace, int skipStackTraceFrames) {
            return new RateRequestTrackInfo(
                request.GetHashCode(),
                request.Type,
                request.Value,
                DateTime.UtcNow,
                captureStackTrace ? RateDebug.GetCurrentStackTrace(skipStackTraceFrames + 1) : null,
                true,
                DateTime.MinValue,
                null
            );
        }

        public static RateRequestTrackInfo ForRequestUnknownStarted(RateRequest request) {
            return new RateRequestTrackInfo(
                request.GetHashCode(),
                request.Type,
                request.Value,
                DateTime.MinValue,
                null,
                true,
                DateTime.MinValue,
                null
            );
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Propertie and Fields ---------->>

        private static readonly char[] _stackTraceSplitChars = new[] {
            '\n'
        };

        public readonly int Identifier { get; }

        public readonly RateRequestType Type { get; }

        public readonly int Value { get; }

        public readonly DateTime StartedTime { get; }

        public readonly string StackTraceStart { get; }

        public readonly string StackTraceStartFirstLine { get; }

        public readonly bool IsActive { get; }

        public readonly DateTime FinishedTime { get; }

        public readonly string StackTraceFinish { get; }

        public readonly string StackTraceFinishFirstLine { get; }

        #endregion <<---------- Propertie and Fields ---------->>




        #region <<---------- General ---------->>

        public RateRequestTrackInfo ForRequestDidFinished(bool captureStackTrace, int skipStackTraceFrames) {
            return new RateRequestTrackInfo(
                this.Identifier,
                this.Type,
                this.Value,
                this.StartedTime,
                this.StackTraceStart,
                false,
                DateTime.UtcNow,
                captureStackTrace ? RateDebug.GetCurrentStackTrace(skipStackTraceFrames + 1) : null
            );
        }

        public bool HasStartTime() {
            return this.StartedTime > DateTime.MinValue && this.StartedTime != default;
        }

        public string GetStatusFormatted() {
            return this.IsActive ? "Active" : "Finished";
        }

        public string GetStartedTimeFormatted() {
            return this.HasStartTime() ? this.StartedTime.ToLocalTime().ToString("HH:mm:ss.fff") : "<unknown>";
        }

        public string GetFinishedTimeFormatted() {
            return this.IsActive ? "<still-active>" : this.FinishedTime.ToLocalTime().ToString("HH:mm:ss.fff");
        }

        #endregion <<---------- General ---------->>
    }
}