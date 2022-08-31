using System;
using UniRate.Internals;

namespace UniRate {

    public abstract class RateRequest : IDisposable {

        #region <<---------- Initializers ---------->>

        protected RateRequest(RateManagerValueController controller) {
            this._controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

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
            this.Controller?.CancelRequest(this);
        }

        #endregion <<---------- IDisposable ---------->>
    }
}