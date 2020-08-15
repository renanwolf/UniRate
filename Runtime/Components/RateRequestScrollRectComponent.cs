using UnityEngine;
using UnityEngine.UI;

namespace UniRate {

    [RequireComponent(typeof(ScrollRect))]
    public class RateRequestScrollRectComponent : RateRequestComponent {
        
        #region <<---------- Properties and Fields ---------->>
        
        private ScrollRect _scrollRect;

        private bool IsMoving {
            get => this._isMoving;
            set {
                if (this._isMoving == value) return;
                this._isMoving = value;
                this.OnIsMovingChanged(this._isMoving);
            }
        }
        private bool _isMoving;

        private const int StopDelayFrames = 3;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBahviour ---------->>
        
        private void Awake() {
            this.CacheManager();
            this._scrollRect = this.GetComponent<ScrollRect>();
        }

        private void OnEnable() {
            this._scrollRect.onValueChanged.AddListener(this.OnScrollRectValueChanged);
            this.IsMoving = this.GetIsMoving(this._scrollRect);
        }

        private void Update() {
            this.IsMoving = this.GetIsMoving(this._scrollRect);
            if (this._isMoving || !this.IsRequesting || this.ElapsedFramesSinceRequestsStarted <= StopDelayFrames) return;
            this.StopRequests();
        }

        private void OnDisable() {
            this._scrollRect.onValueChanged.RemoveListener(this.OnScrollRectValueChanged);
            this._isMoving = false;
            this.StopRequests();
        }

        #if UNITY_EDITOR

        private void OnValidate() {
            if (!Application.isPlaying || !this.isActiveAndEnabled || this.Manager == null || !this.IsRequesting) return;
            this.StartRequests(this.Manager, this.GetCurrentPreset());
        }

        #endif
        
        #endregion <<---------- MonoBahviour ---------->>




        #region <<---------- Callbacks ---------->>

        private void OnIsMovingChanged(bool isMoving) {
            if (isMoving) {
                this.StartRequests(this.Manager, this.GetCurrentPreset());
                this.Manager.ApplyTargetsIfDirty();
                return;
            }
            if (!this.IsRequesting || this.ElapsedFramesSinceRequestsStarted <= StopDelayFrames) return;
            this.StopRequests();
        }
        
        private void OnScrollRectValueChanged(Vector2 normalizedPosition) {
            this.IsMoving = true;
        }
        
        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>
        
        private bool GetIsMoving(ScrollRect scrollRect) {
            var velocity = scrollRect.velocity;
            return (!this.IsFloatApproximatelyWithThreshold(velocity.x, 0f, 0.0001f) || !this.IsFloatApproximatelyWithThreshold(velocity.y, 0f, 0.0001f));
        }

        private bool IsFloatApproximatelyWithThreshold(float value, float compareValue, float threshold) {
			return (value >= (compareValue - threshold) && value <= (compareValue + threshold));
		}
        
        #endregion <<---------- General ---------->>
    }
}