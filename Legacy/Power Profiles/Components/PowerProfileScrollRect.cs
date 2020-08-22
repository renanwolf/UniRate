using System;
using UnityEngine;
using UnityEngine.UI;

namespace PWR.LowPowerMemoryConsumption {

    [RequireComponent(typeof(ScrollRect))]
    [Obsolete("OBSOLETE, use RateRequestScrollRectComponent instead.")]
    public class PowerProfileScrollRect : PowerProfileComponentDelayedRelease {
        
        #region <<---------- Properties and Fields ---------->>
        
        private ScrollRect _scrollRect;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>
        
        protected override void Awake() {
            base.Awake();
            this._scrollRect = this.GetComponent<ScrollRect>();
        }

        protected override void OnEnable() {
            //do not call base to prevent auto retain
            this._scrollRect.onValueChanged.AddListener(this.OnScrollRectValueChanged);
            this.OnScrollRectValueChanged(Vector2.zero);
        }

        protected override void OnDisable() {
            this._scrollRect.onValueChanged.RemoveListener(this.OnScrollRectValueChanged);
            base.OnDisable();
        }
        
        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- Callbacks ---------->>
        
        protected virtual void OnScrollRectValueChanged(Vector2 normalizedPosition) {
            this.RetainNow();
            this.ReleaseNowOrDelayed();
        }
        
        #endregion <<---------- Callbacks ---------->>
    }
}