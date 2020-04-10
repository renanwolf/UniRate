using UnityEngine;
using UnityEngine.UI;

namespace UniRate.Examples {

    public class RateStatusComponent : MonoBehaviour {
        
        #region <<---------- Properties and Fields ---------->>
        
        [SerializeField] private Text _textUpdateRate;
        [SerializeField] private Text _textFixedUpdateRate;
        [SerializeField] private Text _textRenderInterval;

        [Space]
        [SerializeField] private Toggle _toggleModeTargetFrameRate;
        [SerializeField] private Toggle _toggleModeVSyncCount;
        [SerializeField] private Toggle _toggleModeThrottleEndOfFrame;

        [Space]
        [SerializeField] private Slider _sliderUpdateRate;
        [SerializeField] private Slider _sliderFixedUpdateRate;
        [SerializeField] private Slider _sliderRenderInterval;

        private RateManager _rateManager;

        private UpdateRateRequest _updateRateRequest;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>

        private void OnEnable() {
            this._rateManager = RateManager.Instance;

            this.ApplyTogglesUpdateRateMode(this._rateManager.UpdateRateMode);
            this._rateManager.UpdateRateModeChanged += this.OnUpdateRateModeChanged;

            this._toggleModeTargetFrameRate.onValueChanged.AddListener(this.OnToggleModeTargetFrameRateValueChanged);
            this._toggleModeVSyncCount.onValueChanged.AddListener(this.OnToggleModeVSyncCountValueChanged);
            this._toggleModeThrottleEndOfFrame.onValueChanged.AddListener(this.OnToggleModeThrottleEndOfFrameValueChanged);

            this.ApplyTextUpdateRate(this._rateManager.UpdateRate, this._rateManager.TargetUpdateRate);

            this.ApplySliderUpdateRate(this._rateManager.TargetUpdateRate);

            this._rateManager.UpdateRateChanged += this.OnUpdateRateChanged;
            this._rateManager.TargetUpdateRateChanged += this.OnTargetUpdateRateChanged;

            this._sliderUpdateRate.onValueChanged.AddListener(this.OnSliderUpdateRateChanged);
            this._sliderFixedUpdateRate.onValueChanged.AddListener(this.OnSliderFixedUpdateRateChanged);
            this._sliderRenderInterval.onValueChanged.AddListener(this.OnSliderRenderIntervalChanged);
        }

        private void OnDisable() {
            if (this._rateManager == null) return;

            this._rateManager.UpdateRateModeChanged -= this.OnUpdateRateModeChanged;

            this._toggleModeTargetFrameRate.onValueChanged.RemoveListener(this.OnToggleModeTargetFrameRateValueChanged);
            this._toggleModeVSyncCount.onValueChanged.RemoveListener(this.OnToggleModeVSyncCountValueChanged);
            this._toggleModeThrottleEndOfFrame.onValueChanged.RemoveListener(this.OnToggleModeThrottleEndOfFrameValueChanged);

            this._rateManager.UpdateRateChanged -= this.OnUpdateRateChanged;
            this._rateManager.TargetUpdateRateChanged -= this.OnTargetUpdateRateChanged;

            this._sliderUpdateRate.onValueChanged.RemoveListener(this.OnSliderUpdateRateChanged);
            this._sliderFixedUpdateRate.onValueChanged.RemoveListener(this.OnSliderFixedUpdateRateChanged);
            this._sliderRenderInterval.onValueChanged.RemoveListener(this.OnSliderRenderIntervalChanged);

            this._updateRateRequest?.Dispose();
            this._updateRateRequest = null;
        }
        
        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- Callbacks ---------->>
        
        private void OnUpdateRateModeChanged(RateManager manager, UpdateRateMode updateRateMode) {
            this.ApplyTogglesUpdateRateMode(updateRateMode);
        }

        private void OnTargetUpdateRateChanged(RateManager manager, int targetUpdateRate) {
            this.ApplyTextUpdateRate(manager.UpdateRate, targetUpdateRate);
        }

        private void OnUpdateRateChanged(RateManager manager, int updateRate) {
            this.ApplyTextUpdateRate(updateRate, manager.TargetUpdateRate);
        }

        private void OnToggleModeTargetFrameRateValueChanged(bool isOn) {
            if (!isOn || this._rateManager == null) return;
            this._rateManager.UpdateRateMode = UpdateRateMode.ApplicationTargetFrameRate;
        }

        private void OnToggleModeVSyncCountValueChanged(bool isOn) {
            if (!isOn || this._rateManager == null) return;
            this._rateManager.UpdateRateMode = UpdateRateMode.VSyncCount;
        }

        private void OnToggleModeThrottleEndOfFrameValueChanged(bool isOn) {
            if (!isOn || this._rateManager == null) return;
            this._rateManager.UpdateRateMode = UpdateRateMode.ThrottleEndOfFrame;
        }

        private void OnSliderUpdateRateChanged(float value) {
            if (this._rateManager == null) return;
            this._updateRateRequest?.Dispose();
            this._updateRateRequest = this._rateManager.CreateUpdateRateRequest(Mathf.RoundToInt(value));
        }

        private void OnSliderFixedUpdateRateChanged(float value) {
            if (this._rateManager == null) return;
            
        }

        private void OnSliderRenderIntervalChanged(float value) {
            if (this._rateManager == null) return;
            
        }
        
        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>
        
        private void ApplyTextUpdateRate(int rate, int target) {
            this._textUpdateRate.text = $"Update: {rate.ToString("000")} / {target.ToString("000")}";
        }

        private void ApplySliderUpdateRate(int target) {
            this._sliderUpdateRate.value = target;
        }

        private void ApplyTogglesUpdateRateMode(UpdateRateMode updateRateMode) {
            this._toggleModeTargetFrameRate.isOn = (updateRateMode == UpdateRateMode.ApplicationTargetFrameRate);
            this._toggleModeVSyncCount.isOn = (updateRateMode == UpdateRateMode.VSyncCount);
            this._toggleModeThrottleEndOfFrame.isOn = (updateRateMode == UpdateRateMode.ThrottleEndOfFrame);
        }
        
        #endregion <<---------- General ---------->>
    }
}