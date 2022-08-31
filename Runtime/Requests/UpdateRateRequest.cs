using UniRate.Internals;

namespace UniRate {

    public class UpdateRateRequest : RateRequest {

        #region <<---------- Initializers ---------->>

        internal UpdateRateRequest(RateManagerValueController controller, int updateRate) : base(controller) {
            this._updateRate = updateRate;
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        /// <summary>
        /// Requested update rate.
        /// </summary>
        public int UpdateRate => this._updateRate;
        private readonly int _updateRate;

        protected internal override int Value => this._updateRate;

        #endregion <<---------- Properties and Fields ---------->>
    }
}