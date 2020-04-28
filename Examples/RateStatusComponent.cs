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
        private FixedUpdateRateRequest _fixedUpdateRateRequest;
        private RenderIntervalRequest _renderIntervalRequest;
        
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
            this.ApplyTextFixedUpdateRate(this._rateManager.FixedUpdateRate, this._rateManager.TargetFixedUpdateRate);
            this.ApplyTextRenderInterval(this._rateManager.RenderInterval, this._rateManager.TargetRenderInterval);

            this.ApplySliderUpdateRate(this._rateManager.TargetUpdateRate);
            this.ApplySliderFixedUpdateRate(this._rateManager.TargetFixedUpdateRate);
            this.ApplySliderRenderInterval(this._rateManager.TargetRenderInterval);

            this._rateManager.UpdateRateChanged += this.OnUpdateRateChanged;
            this._rateManager.TargetUpdateRateChanged += this.OnTargetUpdateRateChanged;

            this._rateManager.FixedUpdateRateChanged += this.OnFixedUpdateRateChanged;
            this._rateManager.TargetFixedUpdateRateChanged += this.OnTargetFixedUpdateRateChanged;

            this._rateManager.RenderIntervalChanged += this.OnRenderIntervalChanged;
            this._rateManager.TargetRenderIntervalChanged += this.OnTargetRenderIntervalChanged;

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

            this._rateManager.FixedUpdateRateChanged -= this.OnFixedUpdateRateChanged;
            this._rateManager.TargetFixedUpdateRateChanged -= this.OnTargetFixedUpdateRateChanged;

            this._rateManager.RenderIntervalChanged -= this.OnRenderIntervalChanged;
            this._rateManager.TargetRenderIntervalChanged -= this.OnTargetRenderIntervalChanged;

            this._sliderUpdateRate.onValueChanged.RemoveListener(this.OnSliderUpdateRateChanged);
            this._sliderFixedUpdateRate.onValueChanged.RemoveListener(this.OnSliderFixedUpdateRateChanged);
            this._sliderRenderInterval.onValueChanged.RemoveListener(this.OnSliderRenderIntervalChanged);

            this._updateRateRequest?.Dispose();
            this._updateRateRequest = null;

            this._fixedUpdateRateRequest?.Dispose();
            this._fixedUpdateRateRequest = null;

            this._renderIntervalRequest?.Dispose();
            this._renderIntervalRequest = null;
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

        private void OnTargetFixedUpdateRateChanged(RateManager manager, int targetFixedUpdateRate) {
            this.ApplyTextFixedUpdateRate(manager.FixedUpdateRate, targetFixedUpdateRate);
        }

        private void OnFixedUpdateRateChanged(RateManager manager, int fixedUpdateRate) {
            this.ApplyTextFixedUpdateRate(fixedUpdateRate, manager.TargetFixedUpdateRate);
        }

        private void OnRenderIntervalChanged(RateManager manager, int renderInterval) {
            this.ApplyTextRenderInterval(renderInterval, manager.TargetRenderInterval);
        }

        private void OnTargetRenderIntervalChanged(RateManager manager, int targetRenderInterval) {
            this.ApplyTextRenderInterval(manager.RenderInterval, targetRenderInterval);
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
            this._updateRateRequest = this._rateManager.RequestUpdateRate(Mathf.RoundToInt(value));
        }

        private void OnSliderFixedUpdateRateChanged(float value) {
            if (this._rateManager == null) return;
            this._fixedUpdateRateRequest?.Dispose();
            this._fixedUpdateRateRequest = this._rateManager.RequestFixedUpdateRate(Mathf.RoundToInt(value));
        }

        private void OnSliderRenderIntervalChanged(float value) {
            if (this._rateManager == null) return;
            this._renderIntervalRequest?.Dispose();
            this._renderIntervalRequest = this._rateManager.RequestRenderInterval(Mathf.RoundToInt(value));
        }
        
        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>
        
        private void ApplyTextUpdateRate(int rate, int target) {
            this._textUpdateRate.text = $"Update: {rate.ToString("000")} / {target.ToString("000")}";
        }

        private void ApplyTextFixedUpdateRate(int rate, int target) {
            this._textFixedUpdateRate.text = $"Fixed Update: {rate.ToString("000")} / {target.ToString("000")}";
        }

        private void ApplyTextRenderInterval(int rate, int target) {
            this._textRenderInterval.text = $"Render Interval: {rate.ToString()} / {target.ToString()}";
        }

        private void ApplySliderUpdateRate(int target) {
            this._sliderUpdateRate.value = target;
        }

        private void ApplySliderFixedUpdateRate(int target) {
            this._sliderFixedUpdateRate.value = target;
        }

        private void ApplySliderRenderInterval(int target) {
            this._sliderRenderInterval.value = target;
        }

        private void ApplyTogglesUpdateRateMode(UpdateRateMode updateRateMode) {
            this._toggleModeTargetFrameRate.isOn = (updateRateMode == UpdateRateMode.ApplicationTargetFrameRate);
            this._toggleModeVSyncCount.isOn = (updateRateMode == UpdateRateMode.VSyncCount);
            this._toggleModeThrottleEndOfFrame.isOn = (updateRateMode == UpdateRateMode.ThrottleEndOfFrame);
        }
        
        #endregion <<---------- General ---------->>
    }
}