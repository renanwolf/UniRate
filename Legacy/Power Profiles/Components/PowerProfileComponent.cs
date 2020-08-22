using System;
using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

    [Obsolete("OBSOLETE, use RateRequestWhileEnabledComponent instead")]
    public class PowerProfileComponent : MonoBehaviour {

        #region <<---------- Properties and Fields ---------->>
        
        [SerializeField] private PowerProfile _profile;

        protected bool isRetainedByMe;
        
        #endregion <<---------- Properties and Fields ---------->>
        
        
        
        
        #region <<---------- MonoBehaviour ---------->>
        
        protected virtual void OnEnable() {
            this.RetainNow();
        }

        protected virtual void OnDisable() {
            this.ReleaseNow();
        }
        
        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- General ---------->>
        
        protected virtual void RetainNow() {
            if (this.isRetainedByMe || this._profile == null) return;
            this.isRetainedByMe = true;
            this._profile.Retain();
        }

        protected virtual void ReleaseNow() {
            if (!this.isRetainedByMe || this._profile == null) return;
            this.isRetainedByMe = false;
            this._profile.Release();
        }
        
        #endregion <<---------- General ---------->>
    }
}