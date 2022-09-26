namespace UniRate {

    public class RateRequestWhileEnabledComponent : RateRequestComponent {

        #region <<---------- MonoBehaviour ---------->>

        private void OnEnable() {
            this.ShouldActivateRequests = true;
        }

        #endregion <<---------- MonoBehaviour ---------->>
    }
}