using System;

namespace UniRate {

    public abstract class RateRequest : IDisposable {

        #region <<---------- Initializers ---------->>

        protected RateRequest(RateManager rateManager) {
            this._rateManager = rateManager != null ? rateManager : throw new ArgumentNullException(nameof(rateManager));
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        protected RateManager RateManager => this._rateManager;
        private readonly RateManager _rateManager;

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

        protected abstract void Dispose(bool disposingManagedResources);

        #endregion <<---------- IDisposable ---------->>
    }
}