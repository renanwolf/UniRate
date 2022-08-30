namespace UniRate {

    public class RenderIntervalRequest : RateRequest {

        #region <<---------- Initializers ---------->>

        internal RenderIntervalRequest(RateManager rateManager, int renderInterval) : base(rateManager) {
            this._renderInterval = renderInterval;
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        /// <summary>
        /// Requested render interval.
        /// </summary>
        public int RenderInterval => this._renderInterval;
        private readonly int _renderInterval;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- IDisposable ---------->>

        /// <summary>
        /// Cancel request.
        /// </summary>
        public override void Dispose() {
            if (this.IsDisposed) return;
            this.IsDisposed = true;
            if (this.RateManager == null) return;
            this.RateManager.CancelRenderIntervalRequest(this);
        }

        #endregion <<---------- IDisposable ---------->>
    }
}