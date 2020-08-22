using System;
using UniRate;

namespace PWR.LowPowerMemoryConsumption {

    [Obsolete]
    public struct FrameRateRequest {

        #region <<---------- Initializers ---------->>

        public FrameRateRequest(RateRequest request) {
            this._request = request;
            this._type = default(FrameRateType);
            this._rate = 0;
            if (request == null) return;
            if (request is FixedUpdateRateRequest fixedRequest) {
                this._type = FrameRateType.FixedFPS;
                this._rate = fixedRequest.FixedUpdateRate;
            }
            else if (request is UpdateRateRequest updateRequest) {
                this._type = FrameRateType.FPS;
                this._rate = updateRequest.UpdateRate;
            }
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        public RateRequest UniRateRequest => this._request;
        private readonly RateRequest _request;

        public int Rate => this._rate;
        private readonly int _rate;

        public FrameRateType Type => this._type;
        private readonly FrameRateType _type;

        public bool IsValid => (this._rate >= MinRate && this._request != null);

        public const int MinRate = 1;

        public static readonly FrameRateRequest Invalid = new FrameRateRequest(null);

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Legacy Support ---------->>

        

        #endregion <<---------- Legacy Support ---------->>
    }
}