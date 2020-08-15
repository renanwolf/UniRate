using UnityEngine;

namespace UniRate {

    public class RateRequestTouchComponent : RateRequestComponent {
        
        #region <<---------- Properties and Fields ---------->>
        
        private bool IsTouching {
            get => this._isTouching;
            set {
                if (this._isTouching == value) return;
                this._isTouching = value;
                this.OnIsTouchingChanged(this._isTouching);
            }
        }
        private bool _isTouching;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>
        
        private void Awake() {
            this.CacheManager();
        }

        private void OnEnable() {
            this.IsTouching = this.GetHasTouchesOrClicks();
        }

        private void Update() {
            this.IsTouching = this.GetHasTouchesOrClicks();
            if (this._isTouching || !this.IsRequesting || this.ElapsedSecondsSinceRequestsStarted <= this.DelaySecondsToStopRequests) return;
            this.StopRequests();
        }

        private void OnDisable() {
            this._isTouching = false;
            this.StopRequests();
        }

        #if UNITY_EDITOR

        private void OnValidate() {
            if (!Application.isPlaying || !this.isActiveAndEnabled || this.Manager == null || !this.IsRequesting) return;
            this.StartRequests(this.Manager, this.GetCurrentPreset());
        }

        #endif
        
        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- Callbacks ---------->>
        
        private void OnIsTouchingChanged(bool isTouching) {
            if (isTouching) {
                this.StartRequests(this.Manager, this.GetCurrentPreset());
                this.Manager.ApplyTargetsIfDirty();
                return;
            }
            if (!this.IsRequesting || this.ElapsedSecondsSinceRequestsStarted <= this.DelaySecondsToStopRequests) return;
            this.StopRequests();
        }
        
        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>
        
        private bool GetHasTouchesOrClicks() {
            return (
                Input.touchCount > 0
                || Input.GetMouseButton(0) // left click
                || Input.GetMouseButton(1) // right click
                || Input.GetMouseButton(2) // middle click
            );
        }
        
        #endregion <<---------- General ---------->>
    }
}