using UnityEngine;

namespace UniRate {

    public class RateRequestWhileEnabledComponent : RateRequestComponent {
        
        #region <<---------- MonoBehaviour ---------->>

        private void Awake() {
            this.CacheManager();
        }

        private void OnEnable() {
            this.StartRequests(this.Manager, this.GetCurrentPreset());
        }

        private void OnDisable() {
            this.StopRequests();
        }

        #if UNITY_EDITOR

        private void OnValidate() {
            if (!Application.isPlaying || !this.isActiveAndEnabled || this.Manager == null) return;
            this.StartRequests(this.Manager, this.GetCurrentPreset());
        }

        #endif
        
        #endregion <<---------- MonoBehaviour ---------->>
    }
}