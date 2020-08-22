//#define TM_PRO

using System;
using UnityEngine;
#if TM_PRO
using TMPro;
#endif

namespace PWR.LowPowerMemoryConsumption {

    #if TM_PRO
    [RequireComponent(typeof(TMP_InputField))]
    #endif
    [Obsolete("OBSOLETE, use RateRequestTMPInputFieldComponent instead.")]
    public class PowerProfileTMPInputField : PowerProfileComponentDelayedRelease {

        #region <<---------- Properties and Fields ---------->>
        
        #if TM_PRO
        private TMP_InputField _inputField;
        #endif

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
            #if TM_PRO
            this._inputField = this.GetComponent<TMP_InputField>();
            #else
            Debug.LogWarning("[" + this.GetType().Name + "] '#define TM_PRO' is not enabled on script, remove comment on first line of file 'PowerProfileTMPInputField.cs'", this);
            #endif
        }

        #if TM_PRO
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
        #endif
        
        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- Callbacks ---------->>

        protected virtual void OnInputFieldFocusChanged(bool isFocused) {
            if (isFocused) {
                this.RetainNow();
                return;
            }
            this.ReleaseNowOrDelayed();
        }
        
        #if TM_PRO
        protected virtual void OnInputFieldValueChangedOrEndEdit(string value) {
            this.IsInputFieldFocused = this._inputField.isFocused;
        }
        #endif
        
        #endregion <<---------- Callbacks ---------->>
    }
}