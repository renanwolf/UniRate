using UnityEngine;
using UnityEngine.UI;

namespace UniRate {

    [RequireComponent(typeof(ScrollRect))]
    public class RateRequestScrollRectComponent : RateRequestComponent {

        #region <<---------- Properties and Fields ---------->>

        private ScrollRect _scrollRect;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBahviour ---------->>

        protected override void Awake() {
            base.Awake();
            this._scrollRect = this.GetComponent<ScrollRect>();
        }

        private void OnEnable() {
            this._scrollRect.onValueChanged.AddListener(this.OnScrollRectValueChanged);
            this.ShouldActivateRequests = this.GetIsScrollRectMoving(this._scrollRect);
        }

        private void Update() {
            this.ShouldActivateRequests = this.GetIsScrollRectMoving(this._scrollRect);
            this.StopRequestsIfDelayed();
        }

        protected override void OnDisable() {
            this._scrollRect.onValueChanged.RemoveListener(this.OnScrollRectValueChanged);
            base.OnDisable();
        }

        #endregion <<---------- MonoBahviour ---------->>




        #region <<---------- Callbacks ---------->>

        private void OnScrollRectValueChanged(Vector2 normalizedPosition) {
            this.ShouldActivateRequests = true;
        }

        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>

        private bool GetIsScrollRectMoving(ScrollRect scrollRect) {
            var velocity = scrollRect.velocity;
            return (!this.IsFloatApproximatelyWithThreshold(velocity.x, 0f, 0.0001f) || !this.IsFloatApproximatelyWithThreshold(velocity.y, 0f, 0.0001f));
        }

        private bool IsFloatApproximatelyWithThreshold(float value, float compareValue, float threshold) {
            return (value >= (compareValue - threshold) && value <= (compareValue + threshold));
        }

        #endregion <<---------- General ---------->>
    }
}