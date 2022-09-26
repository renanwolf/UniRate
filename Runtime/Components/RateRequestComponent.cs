﻿using UnityEngine;

namespace UniRate {

    public abstract class RateRequestComponent : MonoBehaviour {

        #region <<---------- Enum Preset Options ---------->>

        public enum PresetOptions {
            Ultra,
            VeryHigh,
            High,
            Medium,
            Low,
            VeryLow,
            Custom
        }

        #endregion <<---------- Enum Preset Options ---------->>




        #region <<---------- Properties and Fields ---------->>

        [SerializeField] [HideInInspector] private float _delaySecondsToStopRequests = 2f;
        private float DelaySecondsToStopRequests => this._delaySecondsToStopRequests;

        [SerializeField] [HideInInspector] private PresetOptions _renderIntervalPresetOption = PresetOptions.High;
        [SerializeField] [HideInInspector] private int _renderIntervalCustomValue = RatePreset.High.RenderInterval;

        [SerializeField] [HideInInspector] private PresetOptions _updateRatePresetOption = PresetOptions.High;
        [SerializeField] [HideInInspector] private int _updateRateCustomValue = RatePreset.High.UpdateRate;

        [SerializeField] [HideInInspector] private PresetOptions _fixedUpdateRatePresetOption = PresetOptions.High;
        [SerializeField] [HideInInspector] private int _fixedUpdateRateCustomValue = RatePreset.High.FixedUpdateRate;

        protected RateManager Manager => this._manager;
        private RateManager _manager;

        protected bool ShouldActivateRequests {
            get => this._shouldActivateRequests;
            set {
                if (this._shouldActivateRequests == value) return;
                this._shouldActivateRequests = value;
                this.OnShouldActivateRequestsChanged(this._shouldActivateRequests);
            }
        }
        private bool _shouldActivateRequests;

        private float ElapsedSecondsSinceShouldActivateRequestsIsFalse => (Time.realtimeSinceStartup - this._shouldActivateRequestsIsFalseAtRealTime);
        private float _shouldActivateRequestsIsFalseAtRealTime = float.PositiveInfinity;

        private bool IsRequesting => this._isRequesting;
        private bool _isRequesting;

        private RenderIntervalRequest _requestRenderInterval;
        private UpdateRateRequest _requestUpdateRate;
        private FixedUpdateRateRequest _requestFixedUpdateRate;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>

        protected virtual void Awake() {
            this._manager = RateManager.Instance;
            this.ShouldActivateRequests = false;
        }

        protected virtual void OnDisable() {
            this.ShouldActivateRequests = false;
            this.StopRequestsNow();
        }

#if UNITY_EDITOR
        protected virtual void OnValidate() {
            if (!Application.isPlaying || !this._shouldActivateRequests || this.Manager == null) return;
            this.StartOrRefreshRequests(this.Manager, this.GetCurrentPreset());
        }
#endif

        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- Callbacks ---------->>

        private void OnShouldActivateRequestsChanged(bool shouldActivateRequests) {
            if (shouldActivateRequests) {
                this._shouldActivateRequestsIsFalseAtRealTime = float.PositiveInfinity;
                this.StartOrRefreshRequests(this._manager, this.GetCurrentPreset());
                this._manager.ApplyTargetsIfDirty();
                return;
            }
            this._shouldActivateRequestsIsFalseAtRealTime = Time.realtimeSinceStartup;
            this.StopRequestsIfDelayed();
        }

        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>

        protected RatePreset GetCurrentPreset() {
            int renderInterval;
            switch (this._renderIntervalPresetOption) {
                case PresetOptions.Ultra: renderInterval = RatePreset.Ultra.RenderInterval; break;
                case PresetOptions.VeryHigh: renderInterval = RatePreset.VeryHigh.RenderInterval; break;
                case PresetOptions.High: renderInterval = RatePreset.High.RenderInterval; break;
                case PresetOptions.Medium: renderInterval = RatePreset.Medium.RenderInterval; break;
                case PresetOptions.Low: renderInterval = RatePreset.Low.RenderInterval; break;
                case PresetOptions.VeryLow: renderInterval = RatePreset.VeryLow.RenderInterval; break;
                default: renderInterval = this._renderIntervalCustomValue; break;
            }

            int updateRate;
            switch (this._updateRatePresetOption) {
                case PresetOptions.Ultra: updateRate = RatePreset.Ultra.UpdateRate; break;
                case PresetOptions.VeryHigh: updateRate = RatePreset.VeryHigh.UpdateRate; break;
                case PresetOptions.High: updateRate = RatePreset.High.UpdateRate; break;
                case PresetOptions.Medium: updateRate = RatePreset.Medium.UpdateRate; break;
                case PresetOptions.Low: updateRate = RatePreset.Low.UpdateRate; break;
                case PresetOptions.VeryLow: updateRate = RatePreset.VeryLow.UpdateRate; break;
                default: updateRate = this._updateRateCustomValue; break;
            }

            int fixedUpdateRate;
            switch (this._fixedUpdateRatePresetOption) {
                case PresetOptions.Ultra: fixedUpdateRate = RatePreset.Ultra.FixedUpdateRate; break;
                case PresetOptions.VeryHigh: fixedUpdateRate = RatePreset.VeryHigh.FixedUpdateRate; break;
                case PresetOptions.High: fixedUpdateRate = RatePreset.High.FixedUpdateRate; break;
                case PresetOptions.Medium: fixedUpdateRate = RatePreset.Medium.FixedUpdateRate; break;
                case PresetOptions.Low: fixedUpdateRate = RatePreset.Low.FixedUpdateRate; break;
                case PresetOptions.VeryLow: fixedUpdateRate = RatePreset.VeryLow.FixedUpdateRate; break;
                default: fixedUpdateRate = this._fixedUpdateRateCustomValue; break;
            }

            return new RatePreset(updateRate, fixedUpdateRate, renderInterval);
        }

        protected void StartOrRefreshRequests(RateManager manager, RatePreset preset) {
            if (this._requestRenderInterval == null || this._requestRenderInterval.IsDisposed) {
                this._requestRenderInterval = manager.RenderInterval.Request(preset.RenderInterval);
            }
            else if (this._requestRenderInterval.RenderInterval != preset.RenderInterval) {
                this._requestRenderInterval.Dispose();
                this._requestRenderInterval = manager.RenderInterval.Request(preset.RenderInterval);
            }

            if (this._requestUpdateRate == null || this._requestUpdateRate.IsDisposed) {
                this._requestUpdateRate = manager.UpdateRate.Request(preset.UpdateRate);
            }
            else if (this._requestUpdateRate.UpdateRate != preset.UpdateRate) {
                this._requestUpdateRate.Dispose();
                this._requestUpdateRate = manager.UpdateRate.Request(preset.UpdateRate);
            }

            if (this._requestFixedUpdateRate == null || this._requestFixedUpdateRate.IsDisposed) {
                this._requestFixedUpdateRate = manager.FixedUpdateRate.Request(preset.FixedUpdateRate);
            }
            else if (this._requestFixedUpdateRate.FixedUpdateRate != preset.FixedUpdateRate) {
                this._requestFixedUpdateRate.Dispose();
                this._requestFixedUpdateRate = manager.FixedUpdateRate.Request(preset.FixedUpdateRate);
            }

            this._isRequesting = true;
        }

        protected void StopRequestsNow() {
            this._requestRenderInterval?.Dispose();
            this._requestRenderInterval = null;

            this._requestUpdateRate?.Dispose();
            this._requestUpdateRate = null;

            this._requestFixedUpdateRate?.Dispose();
            this._requestFixedUpdateRate = null;

            this._isRequesting = false;
        }

        protected void StopRequestsIfDelayed() {
            if (this._isRequesting && !this._shouldActivateRequests && this.ElapsedSecondsSinceShouldActivateRequestsIsFalse > this._delaySecondsToStopRequests) {
                this.StopRequestsNow();
            }
        }

        #endregion <<---------- General ---------->>
    }
}