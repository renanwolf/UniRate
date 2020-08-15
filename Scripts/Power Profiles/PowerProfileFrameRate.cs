using System;
using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

    [CreateAssetMenuAttribute(menuName="PWR/Power Profiles/Frame Rate",order=1001)]
    [Obsolete]
    public class PowerProfileFrameRate : PowerProfile {

        #region <<---------- Properties and Fields ---------->>

        [SerializeField] private FrameRateType _type;

        [SerializeField][Range(FrameRateRequest.MinRate, 120)] private int _rate = 30;

        /// <summary>
		/// Frame rate type.
		/// </summary>
		public FrameRateType Type {
			get { return this._type; }
			set {
				this._type = value;
				if (!Application.isPlaying || this._request.Type == this._type) return;
				this.OnIsRetainedChanged(this.IsRetained);
			}
		}

		/// <summary>
		/// Frame rate value.
		/// </summary>
		public int Rate {
			get { return this._rate; }
			set {
				this._rate = value;
				if (!Application.isPlaying || this._request.Rate == this._rate) return;
				this.OnIsRetainedChanged(this.IsRetained);
			}
		}

        private FrameRateRequest _request;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- SriptableObject ---------->>
        
        #if UNITY_EDITOR

        protected virtual void OnValidate() {
            if (!Application.isPlaying) return;
            this.OnIsRetainedChanged(this.IsRetained);
        }

        protected virtual void Reset() {
            if (!Application.isPlaying) return;
            this.OnIsRetainedChanged(this.IsRetained);
        }

        #endif
        
        #endregion <<---------- SriptableObject ---------->>




        #region <<---------- Callbacks ---------->>
        
        protected override void OnIsRetainedChanged(bool isRetained) {
            FrameRateManager.Instance.StopRequest(this._request);
			if (!isRetained) {
				this._request = FrameRateRequest.Invalid;
				return;
			}
			this._request = FrameRateManager.Instance.StartRequest(this._type, this._rate);
        }
        
        #endregion <<---------- Callbacks ---------->>
    }
}