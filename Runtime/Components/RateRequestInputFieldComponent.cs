using UnityEngine;
using UnityEngine.UI;

namespace UniRate {

    [RequireComponent(typeof(InputField))]
    public class RateRequestInputFieldComponent : RateRequestComponent {

        #region <<---------- Properties and Fields ---------->>

        private InputField _inputField;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>

        protected override void Awake() {
            base.Awake();
            this._inputField = this.GetComponent<InputField>();
        }

        private void OnEnable() {
            this._inputField.onEndEdit.AddListener(this.OnInputFieldEndEdit);
            this._inputField.onValueChanged.AddListener(this.OnInputFieldValueChanged);
            this.ShouldActivateRequests = this._inputField.isFocused;
        }

        private void Update() {
            this.ShouldActivateRequests = this._inputField.isFocused;
            this.StopRequestsIfDelayed();
        }

        protected override void OnDisable() {
            this._inputField.onEndEdit.RemoveListener(this.OnInputFieldEndEdit);
            this._inputField.onValueChanged.RemoveListener(this.OnInputFieldValueChanged);
            base.OnDisable();
        }

        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- Callbacks ---------->>

        private void OnInputFieldEndEdit(string value) {
            this.ShouldActivateRequests = false;
        }

        private void OnInputFieldValueChanged(string value) {
            this.ShouldActivateRequests = true;
        }

        #endregion <<---------- Callbacks ---------->>
    }
}