using UniRate.Internals;

namespace UniRate {

    public class FixedUpdateRateRequest : RateRequest {

        #region <<---------- Initializers ---------->>

        internal FixedUpdateRateRequest(RateManagerValueController controller, int fixedUpdateRate) : base(RateRequestType.FixedUpdateRate, controller) {
            this._fixedUpdateRate = fixedUpdateRate;
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        /// <summary>
        /// Requested fixed update rate.
        /// </summary>
        public int FixedUpdateRate => this._fixedUpdateRate;
        private readonly int _fixedUpdateRate;

        protected internal override int Value => this._fixedUpdateRate;

        #endregion <<---------- Properties and Fields ---------->>
    }
}