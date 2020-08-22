using System;
using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

    [Obsolete("OBSOLETE, use RateRequestTouchComponent instead")]
    public class PowerProfileTouches : PowerProfileComponentDelayedRelease {
        
        #region <<---------- Properties and Fields ---------->>
        
        protected bool HasTouches {
            get { return this._hasTouches; }
            private set {
                if (this._hasTouches == value) return;
                this._hasTouches = value;
                this.OnHasTouchesChanged(this._hasTouches);
            }
        }
        private bool _hasTouches;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>

        protected override void OnEnable() {
            //do not call base to prevent auto retain
            this.HasTouches = this.GetTouchCount() > 0;
        }
        
        protected override void Update() {
            this.HasTouches = this.GetTouchCount() > 0;
            base.Update();
        }

        protected override void FixedUpdate() {
            this.HasTouches = this.GetTouchCount() > 0;
            base.FixedUpdate();
        }

        protected override void OnDisable() {
            this._hasTouches = false;
            base.OnDisable();
        }
        
        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- Callbacks ---------->>
        
        protected virtual void OnHasTouchesChanged(bool hasTouches) {
            if (hasTouches) {
                this.RetainNow();
                return;
            }
            this.ReleaseNowOrDelayed();
        }
        
        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>
        
        private int GetTouchCount() {
            #if UNITY_EDITOR
            if (Input.GetMouseButton(0)) return 1;
            #endif
            return Input.touchCount;
        }
        
        #endregion <<---------- General ---------->>
    }
}