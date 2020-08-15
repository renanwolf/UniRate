#if TMPRO
using UnityEngine;
using TMPro;

namespace UniRate {

    [RequireComponent(typeof(TMP_InputField))]
    public class RateRequestTMPInputFieldComponent : RateRequestComponent {
        
        #region <<---------- Properties and Fields ---------->>
        
        private TMP_InputField _inputField;

        private bool IsFocused {
            get => this._isFocused;
            set {
                if (this._isFocused == value) return;
                this._isFocused = value;
                this.OnIsFocusedChanged(this._isFocused);
            }
        }
        private bool _isFocused;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>

        private void Awake() {
            this.CacheManager();
            this._inputField = this.GetComponent<TMP_InputField>();
        }

        private void OnEnable() {
            this._inputField.onEndEdit.AddListener(this.OnInputFieldEndEdit);
            this._inputField.onValueChanged.AddListener(this.OnInputFieldValueChanged);
            this.IsFocused = this._inputField.isFocused;
        }

        private void Update() {
            this.IsFocused = this._inputField.isFocused;
            if (this._isFocused || !this.IsRequesting || this.ElapsedSecondsSinceRequestsStarted <= this.DelaySecondsToStopRequests) return;
            this.StopRequests();
        }

        private void OnDisable() {
            this._inputField.onEndEdit.RemoveListener(this.OnInputFieldEndEdit);
            this._inputField.onValueChanged.RemoveListener(this.OnInputFieldValueChanged);
            this._isFocused = false;
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

        private void OnIsFocusedChanged(bool isFocused) {
            if (isFocused) {
                this.StartRequests(this.Manager, this.GetCurrentPreset());
                this.Manager.ApplyTargetsIfDirty();
                return;
            }
            if (!this.IsRequesting || this.ElapsedSecondsSinceRequestsStarted <= this.DelaySecondsToStopRequests) return;
            this.StopRequests();
        }
        
        private void OnInputFieldEndEdit(string value) {
            this.IsFocused = false;
        }

        private void OnInputFieldValueChanged(string value) {
            this.IsFocused = true;
        }
        
        #endregion <<---------- Callbacks ---------->>
    }
}
#endif