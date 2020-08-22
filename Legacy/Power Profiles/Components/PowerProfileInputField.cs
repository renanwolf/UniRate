using System;
using UnityEngine;
using UnityEngine.UI;

namespace PWR.LowPowerMemoryConsumption {

    [RequireComponent(typeof(InputField))]
    [Obsolete("OBSOLETE, use RateRequestInputFieldComponent instead.")]
    public class PowerProfileInputField : PowerProfileComponentDelayedRelease {

        #region <<---------- Properties and Fields ---------->>
        
        private InputField _inputField;

        private bool IsInputFieldFocused {
            get { return this._isInputFieldFocused; }
            set {
                if (this._isInputFieldFocused == value) return;
                this._isInputFieldFocused = value;
                this.OnInputFieldFocusChanged(this._isInputFieldFocused);
            }
        }
        private bool _isInputFieldFocused;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>
        
        protected override void Awake() {
            base.Awake();
            this._inputField = this.GetComponent<InputField>();
        }

        protected override void OnEnable() {
            //do not call base to prevent auto retain
            this._inputField.onEndEdit.AddListener(this.OnInputFieldValueChangedOrEndEdit);
            this._inputField.onValueChanged.AddListener(this.OnInputFieldValueChangedOrEndEdit);
            this.IsInputFieldFocused = this._inputField.isFocused;
        }

        protected override void OnDisable() {
            this._inputField.onEndEdit.RemoveListener(this.OnInputFieldValueChangedOrEndEdit);
            this._inputField.onValueChanged.RemoveListener(this.OnInputFieldValueChangedOrEndEdit);
            
            this._isInputFieldFocused = false;
            base.OnDisable();
        }
        
        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- Callbacks ---------->>

        protected virtual void OnInputFieldFocusChanged(bool isFocused) {
            if (isFocused) {
                this.RetainNow();
                return;
            }
            this.ReleaseNowOrDelayed();
        }
        
        protected virtual void OnInputFieldValueChangedOrEndEdit(string value) {
            this.IsInputFieldFocused = this._inputField.isFocused;
        }
        
        #endregion <<---------- Callbacks ---------->>
    }
}