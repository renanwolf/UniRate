using UnityEngine;

namespace UniRate {

    [RequireComponent(typeof(Animation))]
    public class RateRequestAnimationComponent : RateRequestComponent {

        #region <<---------- Properties and Fields ---------->>

        [SerializeField] [HideInInspector] private string _clipName;

        private Animation _animation;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>

        protected override void Awake() {
            base.Awake();
            this._animation = this.GetComponent<Animation>();
        }

        private void OnEnable() {
            this.ShouldActivateRequests = this.GetIsAnimationPlaying(this._animation, this._clipName);
        }

        private void Update() {
            this.ShouldActivateRequests = this.GetIsAnimationPlaying(this._animation, this._clipName);
            this.StopRequestsIfDelayed();
        }

#if UNITY_EDITOR
        protected override void OnValidate() {
            if (Application.isPlaying && this.isActiveAndEnabled && this.Manager != null) {
                this.ShouldActivateRequests = this.GetIsAnimationPlaying(this._animation, this._clipName);
            }
            base.OnValidate();
        }
#endif

        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- General ---------->>

        private bool GetIsAnimationPlaying(Animation animation, string clipName) {
            if (!animation.isActiveAndEnabled) return false;
            if (string.IsNullOrEmpty(clipName)) {
                return animation.isPlaying;
            }
            return animation.IsPlaying(clipName);
        }

        #endregion <<---------- General ---------->>
    }
}