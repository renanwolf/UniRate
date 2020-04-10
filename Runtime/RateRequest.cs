using System;

namespace UniRate {

    public abstract class RateRequest : IDisposable {
        
        #region <<---------- Initializers ---------->>
        
        protected RateRequest(RateManager rateManager) {
            this.RateManager = (rateManager != null ? rateManager : throw new ArgumentNullException(nameof(rateManager)));
        }
        
        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>
        
        protected RateManager RateManager { get; private set; }
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- IDisposable ---------->>
        
        /// <summary>
        /// Is request canceled?
        /// </summary>
        public bool IsDisposed { get; protected set; }
        
        /// <summary>
        /// Cancel request.
        /// </summary>
        public abstract void Dispose();
        
        #endregion <<---------- IDisposable ---------->>
    }
}