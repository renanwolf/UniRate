using UnityEngine;

namespace UniRate {

    [RequireComponent(typeof(Animator))]
    public class RateRequestAnimatorComponent : RateRequestComponent {
        
        #region <<---------- Properties and Fields ---------->>

        public bool ActivateRequestsInTransitions {
            get => this._activateRequestsInTransitions;
            set {
                if (this._activateRequestsInTransitions == value) return;
                this._activateRequestsInTransitions = value;
                this.OnActivateRequestsInTransitionsChanged(this._activateRequestsInTransitions);
            }
        }
        [SerializeField][HideInInspector] private bool _activateRequestsInTransitions = true;

        public string LayerName {
            get => this._layerName;
            set {
                if (this._layerName == value) return;
                this._layerName = value;
                this.OnLayerNameChanged(this._layerName);
            }
        }
        [SerializeField][HideInInspector] private string _layerName;

        public string StateName {
            get => this._stateName;
            set {
                if (this._stateName == value) return;
                this._stateName = value;
                this.OnStateNameChanged(this._stateName);
            }
        }
        [SerializeField][HideInInspector] private string _stateName;

        private (int hash, bool hasHash) StateInfo {
            get => this._stateInfo;
            set {
                if (this._stateInfo.hasHash == value.hasHash && this._stateInfo.hash == value.hash) return;
                this._stateInfo = value;
                this.OnStateInfoChanged(this._stateInfo.hash, this._stateInfo.hasHash);
            }
        }
        private (int hash, bool hasHash) _stateInfo;

        private bool ShouldActivateRequests {
            get => this._shouldActivateRequests;
            set {
                if (this._shouldActivateRequests == value) return;
                this._shouldActivateRequests = value;
                this.OnShouldActivateRequestsChanged(this._shouldActivateRequests);
            }
        }
        private bool _shouldActivateRequests;
        
        private Animator _animator;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>
        
        private void Awake() {
            this.CacheManager();
            this._animator = this.GetComponent<Animator>();
            this.OnStateNameChanged(this._stateName);
        }

        private void OnEnable() {
            this.ShouldActivateRequests = this.GetShouldActivateRequests(
                this._animator,
                this._layerName,
                this._stateInfo.hash,
                this._stateInfo.hasHash,
                this._activateRequestsInTransitions
            );
        }

        private void Update() {
            this.ShouldActivateRequests = this.GetShouldActivateRequests(
                this._animator,
                this._layerName,
                this._stateInfo.hash,
                this._stateInfo.hasHash,
                this._activateRequestsInTransitions
            );
            if (this._shouldActivateRequests || !this.IsRequesting || this.ElapsedSecondsSinceRequestsStarted <= this.DelaySecondsToStopRequests) return;
            this.StopRequests();
        }

        private void OnDisable() {
            this._shouldActivateRequests = false;
            this.StopRequests();
        }

        #if UNITY_EDITOR

        private void OnValidate() {
            if (!Application.isPlaying || this.Manager == null) return;
            this.OnActivateRequestsInTransitionsChanged(this._activateRequestsInTransitions);
            this.OnLayerNameChanged(this._layerName);
            this.OnStateNameChanged(this._stateName);
        }

        #endif
        
        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- Callbacks ---------->>

        private void OnActivateRequestsInTransitionsChanged(bool activateRequestsInTransitions) {
            if (!this.isActiveAndEnabled) return;
            this.ShouldActivateRequests = this.GetShouldActivateRequests(
                this._animator,
                this._layerName,
                this._stateInfo.hash,
                this._stateInfo.hasHash,
                activateRequestsInTransitions
            );
        }
        
        private void OnLayerNameChanged(string layerName) {
            if (!this.isActiveAndEnabled) return;
            this.ShouldActivateRequests = this.GetShouldActivateRequests(
                this._animator,
                layerName,
                this._stateInfo.hash,
                this._stateInfo.hasHash,
                this._activateRequestsInTransitions
            );
        }
        
        private void OnStateNameChanged(string stateName) {
            bool hasStateName = !string.IsNullOrEmpty(stateName);
            this._stateInfo = (
                (hasStateName ? Animator.StringToHash(stateName) : 0),
                hasStateName
            );
        }

        private void OnStateInfoChanged(int stateHash, bool hasStateHash) {
            if (!this.isActiveAndEnabled) return;
            this.ShouldActivateRequests = this.GetShouldActivateRequests(
                this._animator,
                this._layerName,
                stateHash,
                hasStateHash,
                this._activateRequestsInTransitions
            );
        }

        private void OnShouldActivateRequestsChanged(bool shouldActivateRequests) {
            if (shouldActivateRequests) {
                this.StartRequests(this.Manager, this.GetCurrentPreset());
                this.Manager.ApplyTargetsIfDirty();
                return;
            }
            if (!this.IsRequesting || this.ElapsedSecondsSinceRequestsStarted <= this.DelaySecondsToStopRequests) return;
            this.StopRequests();
        }
        
        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>

        private bool GetIsAnimatorInTransitionAtLayer(Animator animator, int layerIndex) {
            return animator.IsInTransition(layerIndex);
        }

        private bool GetIsAnimatorPlayingInStateAtLayer(Animator animator, int layerIndex, int stateHash, bool hasStateHash) {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            if (stateInfo.speed == 0f || stateInfo.speedMultiplier == 0f) return false;
            if (!hasStateHash) return true;
            return (stateInfo.shortNameHash == stateHash);
        }

        private bool GetShouldActivateRequests(Animator animator, string layerName, int stateHash, bool hasStateHash, bool activateRequestsInTransitions) {
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