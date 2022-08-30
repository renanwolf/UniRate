namespace UniRate {

    public class FixedUpdateRateRequest : RateRequest {

        #region <<---------- Initializers ---------->>

        internal FixedUpdateRateRequest(RateManager rateManager, int fixedUpdateRate) : base(rateManager) {
            this._fixedUpdateRate = fixedUpdateRate;
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        /// <summary>
        /// Requested fixed update rate.
        /// </summary>
        public int FixedUpdateRate => this._fixedUpdateRate;
        private readonly int _fixedUpdateRate;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- IDisposable ---------->>

        /// <summary>
        /// Cancel request.
        /// </summary>
        public override void Dispose() {
            if (this.IsDisposed) return;
            this.IsDisposed = true;
            if (this.RateManager == null) return;
            this.RateManager.CancelFixedUpdateRateRequest(this);
        }

        #endregion <<---------- IDisposable ---------->>
    }
}