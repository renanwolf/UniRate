using System;
using UnityEngine;

namespace PWR.LowPowerMemoryConsumption {

    [Obsolete]
    public abstract class PowerProfileComponentDelayedRelease : PowerProfileComponent {
        
        #region <<---------- Properties and Fields ---------->>
        
        [SerializeField] protected float delayedReleaseSeconds = 2f;

        private float _timeReleaseRequested;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>
        
        protected virtual void Awake() {
            this.SetReleaseRequestedNever();
        }

        protected virtual void Update() {
            if (!this.IsDelayedReleaseEnabled()) return;
            if (this.ElapsedTimeReleaseRequested() < this.delayedReleaseSeconds) return;
            this.ReleaseNow();
        }

        protected virtual void FixedUpdate() {
            if (!this.IsDelayedReleaseEnabled()) return;
            if (this.ElapsedTimeReleaseRequested() < this.delayedReleaseSeconds) return;
            this.ReleaseNow();
        }
        
        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- General ---------->>

        private void SetReleaseRequestedNever() {
            this._timeReleaseRequested = float.PositiveInfinity;
        }

        protected float ElapsedTimeReleaseRequested() {
            return Time.realtimeSinceStartup - this._timeReleaseRequested;
        }

        protected bool IsDelayedReleaseEnabled() {
            return this.delayedReleaseSeconds > 0f;
        }
        
        protected override void RetainNow() {
            this.SetReleaseRequestedNever();
            base.RetainNow();
        }

        protected override void ReleaseNow() {
            if (this.isRetainedByMe) {
                this.SetReleaseRequestedNever();
            }
            base.ReleaseNow();
        }

        protected void ReleaseNowOrDelayed() {
            if (!this.IsDelayedReleaseEnabled()) {
                this.ReleaseNow();
                return;
            }
            if (!this.isRetainedByMe) return;
            this._timeReleaseRequested = Time.realtimeSinceStartup;
        }
        
        #endregion <<---------- General ---------->>
    }
}