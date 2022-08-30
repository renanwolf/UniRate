namespace UniRate {

    public class UpdateRateRequest : RateRequest {

        #region <<---------- Initializers ---------->>

        internal UpdateRateRequest(RateManager rateManager, int updateRate) : base(rateManager) {
            this._updateRate = updateRate;
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        /// <summary>
        /// Requested update rate.
        /// </summary>
        public int UpdateRate => this._updateRate;
        private readonly int _updateRate;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- IDisposable ---------->>

        protected override void Dispose(bool disposingManagedResources) {
            if (this.IsDisposed) return;
            this.IsDisposed = true;
            if (this.RateManager == null) return;
            this.RateManager.CancelUpdateRateRequest(this);
        }

        #endregion <<---------- IDisposable ---------->>
    }
}