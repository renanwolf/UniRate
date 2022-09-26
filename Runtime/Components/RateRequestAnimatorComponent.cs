using UnityEngine;

namespace UniRate {

    [RequireComponent(typeof(Animator))]
    public class RateRequestAnimatorComponent : RateRequestComponent {

        #region <<---------- Properties and Fields ---------->>

        [SerializeField] [HideInInspector] private bool _activateRequestsInTransitions = true;
        [SerializeField] [HideInInspector] private string _layerName;
        [SerializeField] [HideInInspector] private string _stateName;

        private (int hash, bool hasHash) _stateInfo;
        private Animator _animator;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>

        protected override void Awake() {
            base.Awake();
            this._animator = this.GetComponent<Animator>();
            this._stateInfo = this.GetStateInfo(this._stateName);
        }

        private void OnEnable() {
            this.ShouldActivateRequests = this.GetIsAnimatorPlayingOrInTransition(
                this._animator,
                this._layerName,
                this._stateInfo.hash,
                this._stateInfo.hasHash,
                this._activateRequestsInTransitions
            );
        }

        private void Update() {
            this.ShouldActivateRequests = this.GetIsAnimatorPlayingOrInTransition(
                this._animator,
                this._layerName,
                this._stateInfo.hash,
                this._stateInfo.hasHash,
                this._activateRequestsInTransitions
            );
            this.StopRequestsIfDelayed();
        }

#if UNITY_EDITOR
        protected override void OnValidate() {
            if (Application.isPlaying) {
                this._stateInfo = this.GetStateInfo(this._stateName);
            }
            if (Application.isPlaying && !this.isActiveAndEnabled && this.Manager != null) {
                this.ShouldActivateRequests = this.GetIsAnimatorPlayingOrInTransition(
                    this._animator,
                    this._layerName,
                    this._stateInfo.hash,
                    this._stateInfo.hasHash,
                    this._activateRequestsInTransitions
                );
            }
            base.OnValidate();
        }
#endif

        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- General ---------->>

        private (int hash, bool hasHash) GetStateInfo(string stateName) {
            bool hasStateName = !string.IsNullOrEmpty(stateName);
            return (
                (hasStateName ? Animator.StringToHash(stateName) : 0),
                hasStateName
            );
        }

        private bool GetIsAnimatorInTransitionAtLayer(Animator animator, int layerIndex) {
            return animator.IsInTransition(layerIndex);
        }

        private bool GetIsAnimatorPlayingInStateAtLayer(Animator animator, int layerIndex, int stateHash, bool hasStateHash) {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            if (stateInfo.speed == 0f || stateInfo.speedMultiplier == 0f) return false;
            if (hasStateHash && stateInfo.shortNameHash != stateHash) return false;
            return (stateInfo.loop || stateInfo.normalizedTime < 1f);
        }

        private bool GetIsAnimatorPlayingOrInTransition(Animator animator, string layerName, int stateHash, bool hasStateHash, bool activateRequestsInTransitions) {
            if (!animator.isActiveAndEnabled || animator.speed == 0f) return false;

            if (!string.IsNullOrEmpty(layerName)) {
                int layerIndex = animator.GetLayerIndex(layerName);
                if (layerIndex >= 0) {
                    if (activateRequestsInTransitions && this.GetIsAnimatorInTransitionAtLayer(animator, layerIndex)) return true;
                    if (this.GetIsAnimatorPlayingInStateAtLayer(animator, layerIndex, stateHash, hasStateHash)) return true;
                }
            }
            else {
                for (int layerIndex = animator.layerCount - 1; layerIndex >= 0; layerIndex--) {
                    if (activateRequestsInTransitions && this.GetIsAnimatorInTransitionAtLayer(animator, layerIndex)) return true;
                    if (this.GetIsAnimatorPlayingInStateAtLayer(animator, layerIndex, stateHash, hasStateHash)) return true;
                }
            }

            return false;
        }

        #endregion <<---------- General ---------->>
    }
}