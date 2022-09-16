using UniRate.Internals;

namespace UniRate {

    public class RenderIntervalRequest : RateRequest {

        #region <<---------- Initializers ---------->>

        internal RenderIntervalRequest(RateManagerValueController controller, int renderInterval) : base(RateRequestType.RenderInterval, controller) {
            this._renderInterval = renderInterval;
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        /// <summary>
        /// Requested render interval.
        /// </summary>
        public int RenderInterval => this._renderInterval;
        private readonly int _renderInterval;

        protected internal override int Value => this._renderInterval;

        #endregion <<---------- Properties and Fields ---------->>
    }
}