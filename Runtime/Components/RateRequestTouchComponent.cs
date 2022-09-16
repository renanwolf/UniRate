using UnityEngine;

namespace UniRate {

    public class RateRequestTouchComponent : RateRequestComponent {

        #region <<---------- MonoBehaviour ---------->>

        private void OnEnable() {
            this.ShouldActivateRequests = this.GetHasTouchesOrClicks();
        }

        private void Update() {
            this.ShouldActivateRequests = this.GetHasTouchesOrClicks();
            this.StopRequestsIfDelayed();
        }

        #endregion <<---------- MonoBehaviour ---------->>




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