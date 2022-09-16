using System;
using UniRate.Internals;

namespace UniRate {

    public abstract class RateRequest : IDisposable {

        #region <<---------- Initializers ---------->>

        protected RateRequest(RateRequestType type, RateManagerValueController controller) {
            this._type = type;
            this._controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        public RateRequestType Type => this._type;
        private readonly RateRequestType _type;

        protected RateManagerValueController Controller => this._controller;
        private readonly RateManagerValueController _controller;

        protected internal abstract int Value { get; }

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- IDisposable ---------->>

        /// <summary>
        /// Is request canceled?
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// Cancel request.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposingManagedResources) {
            if (this.IsDisposed) return;
            this.IsDisposed = true;
            this.Controller?.CancelRequest(this, 2);
        }

        #endregion <<---------- IDisposable ---------->>
    }
}