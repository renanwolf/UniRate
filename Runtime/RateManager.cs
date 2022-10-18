using System;
using UnityEngine;
using UniRate.Internals;
using UniRate.Debug;

#if UNITY_2021_1_OR_NEWER
using UnityScreen = UnityEngine.Device.Screen;
#else
using UnityScreen = UnityEngine.Screen;
#endif

namespace UniRate {

    [DisallowMultipleComponent]
    public sealed class RateManager : MonoBehaviour {

        #region <<---------- Initializers ---------->>

        private RateManager() { }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Instance ---------->>

        /// <summary>
        /// The singleton instance of <see cref="RateManager"/>.
        /// </summary>
        public static RateManager Instance {
            get {
#if UNITY_EDITOR
                if (!Application.isPlaying) {
                    return _instance;
                }
#endif
                if (!IsApplicationQuitting && _instance == null) {
                    _instance = FindObjectOfType<RateManager>();
                    if (_instance == null) {
                        var go = new GameObject();
                        _instance = go.AddComponent<RateManager>();
                    }
                }
                return _instance;
            }
        }
        private static RateManager _instance;

        /// <summary>
        /// Check if the instance of  <see cref="RateManager"/> exists without creating it.
        /// </summary>
        public static bool HasInstance => _instance != null;

        #endregion <<---------- Instance ---------->>




        #region <<---------- Properties and Fields ---------->>

        [SerializeField] private UpdateRateMode _updateRateMode = UpdateRateMode.VSyncCount;
        [SerializeField] private int _minimumUpdateRate = 15;

        [Space]
        [SerializeField] private int _minimumFixedUpdateRate = 15;

        [Space]
        [SerializeField] private int _maximumRenderInterval = 3;

        /// <summary>
        /// Update rate controller.
        /// </summary>
        public RateManagerUpdateRateController UpdateRate => this._updateRateController ?? (this._updateRateController = this.CreateUpdateRateController());
        private RateManagerUpdateRateController _updateRateController;

        /// <summary>
        /// Fixed update rate controller.
        /// </summary>
        public RateManagerFixedUpdateRateController FixedUpdateRate => this._fixedUpdateRateController ?? (this._fixedUpdateRateController = this.CreateFixedUpdateRateController());
        private RateManagerFixedUpdateRateController _fixedUpdateRateController;

        /// <summary>
        /// Render interval controller.
        /// </summary>
        public RateManagerRenderIntervalController RenderInterval => this._renderIntervalController ?? (this._renderIntervalController = this.CreateRenderIntervalController());
        private RateManagerRenderIntervalController _renderIntervalController;

        internal static bool IsApplicationQuitting { get; private set; }

        private static readonly string GameObjectName = $"UniRate ({nameof(RateManager)})";

        private readonly GUIContent _guiContentText = new GUIContent();

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>

        private void Awake() {

            if (_instance == null) {
                _instance = this;
            }
            else if (_instance != this) {
                if (RateDebug.IsLogLevelActive(RateLogLevel.Warning)) {
                    RateDebug.Log(RateLogLevel.Warning, $"trying to awake another instance of {nameof(RateManager)} at '{this.gameObject.scene.name}/{this.gameObject.name}', destroying it", this.gameObject);
                }
                Destroy(this);
                return;
            }

            this.gameObject.name = GameObjectName;
            this.transform.SetParent(null, false);
            DontDestroyOnLoad(this);
        }

        private void Update() {
            this.RenderInterval.ExecuteMonoBehaviourUpdate();
            this.UpdateRate.ExecuteMonoBehaviourUpdate(Time.unscaledDeltaTime);
        }

        private void FixedUpdate() {
            this.FixedUpdateRate.ExecuteMonoBehaviourFixedUpdate(Time.fixedUnscaledDeltaTime);
        }

        private void OnApplicationQuit() {
            IsApplicationQuitting = true;
        }

        private void OnDestroy() {
            if (_instance == this) {
                _instance = null;
            }
        }

#if UNITY_EDITOR

        private void OnValidate() {
            if (!Application.isPlaying) return;
            this.RenderInterval.Maximum = this._maximumRenderInterval;
            this.UpdateRate.Minimum = this._minimumUpdateRate;
            this.UpdateRate.Mode = this._updateRateMode;
            this.FixedUpdateRate.Minimum = this._minimumFixedUpdateRate;
            this.ApplyTargetsIfDirty();
        }

        private void Reset() {
            this.gameObject.name = GameObjectName;
        }

#endif

        private void OnGUI() {
            if (!RateDebug.DisplayOnScreenData) return;

            if (RateDebug.ScreenDataVerbose) {
                this._guiContentText.text = $"UpdateMode: {this.UpdateRate.Mode.ToString()}\nUpdate/s: {this.UpdateRate.Current.ToString("000")} / {this.UpdateRate.Target.ToString("000")}\nFixedUpdate/s: {this.FixedUpdateRate.Current.ToString("000")} / {this.FixedUpdateRate.Target.ToString("000")}\n{(this.RenderInterval.IsSupported ? $"Will Render: {this.RenderInterval.WillRender.ToString()}" : "Render interval not supported")}\nRenderInterval: {this.RenderInterval.Current.ToString()} / {this.RenderInterval.Target.ToString()}\nRender/s: {this.RenderInterval.CurrentRenderRate.ToString("000")}";
            }
            else {
                this._guiContentText.text = $"Update/s: {this.UpdateRate.Current.ToString("000")} / {this.UpdateRate.Target.ToString("000")}\nFixedUpdate/s: {this.FixedUpdateRate.Current.ToString("000")} / {this.FixedUpdateRate.Target.ToString("000")}\nRenderInterval: {this.RenderInterval.Current.ToString()} / {this.RenderInterval.Target.ToString()}\nRender/s: {this.RenderInterval.CurrentRenderRate.ToString("000")}";
            }

            var safeArea = UnityScreen.safeArea;
            RateDebug.ScreenDataStyle.CalcMinMaxWidth(this._guiContentText, out float minWidth, out float maxWidth);
            var labelWidth = Mathf.Min(safeArea.width - 10, maxWidth);
            float labelSpaceH = Mathf.Min(5f, ((safeArea.width - labelWidth) / 2f));
            float labelSpaceV = Math.Max(UnityScreen.height - safeArea.height, 5f);
            var rectLine = new Rect(safeArea.x + labelSpaceH, labelSpaceV, labelWidth, RateDebug.ScreenDataStyle.CalcHeight(this._guiContentText, labelWidth));

            if (GUI.Button(rectLine, this._guiContentText, RateDebug.ScreenDataStyle)) {
                RateDebug.ScreenDataVerbose = !RateDebug.ScreenDataVerbose;
            }
        }

        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- Callbacks ---------->>

        private void OnUpdateRateControllerModeChanged(RateManager rateManager, UpdateRateMode updateRateMode) {
            this._updateRateMode = updateRateMode;
        }

        private void OnUpdateRateControllerMinimumChanged(RateManager rateManager, int minimumUpdateRate) {
            this._minimumUpdateRate = minimumUpdateRate;
        }

        private void OnFixedUpdateRateControllerMinimumChanged(RateManager rateManager, int minimumFixedUpdateRate) {
            this._minimumFixedUpdateRate = minimumFixedUpdateRate;
        }

        private void OnRenderIntervalControllerMaximumChanged(RateManager rateManager, int maximumRenderInterval) {
            this._maximumRenderInterval = maximumRenderInterval;
        }

        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>

        /// <summary>
        /// Recalculate targets and apply them now if changed, instead of waiting Unity's next update cycle.
        /// </summary>
        public void ApplyTargetsIfDirty() {
            this.RenderInterval.ApplyTargetIfDirty();
            this.UpdateRate.ApplyTargetIfDirty();
            this.FixedUpdateRate.ApplyTargetIfDirty();
        }

        /// <summary>
        /// Create a new <see cref="UpdateRateRequest"/>.
        /// </summary>
        public UpdateRateRequest RequestUpdateRate(RatePreset preset) {
            return this.UpdateRate.Request(preset.UpdateRate);
        }

        /// <summary>
        /// Create a new <see cref="FixedUpdateRateRequest"/>.
        /// </summary>
        public FixedUpdateRateRequest RequestFixedUpdateRate(RatePreset preset) {
            return this.FixedUpdateRate.Request(preset.FixedUpdateRate);
        }

        /// <summary>
        /// Create a new <see cref="FixedUpdateRateRequest"/>.
        /// </summary>
        public RenderIntervalRequest RequestRenderInterval(RatePreset preset) {
            return this.RenderInterval.Request(preset.RenderInterval);
        }

        private RateManagerUpdateRateController CreateUpdateRateController() {
            var controller = new RateManagerUpdateRateController(this, this._minimumUpdateRate, this._updateRateMode);
            controller.LimitChanged += this.OnUpdateRateControllerMinimumChanged;
            controller.ModeChanged += this.OnUpdateRateControllerModeChanged;
            return controller;
        }

        private RateManagerFixedUpdateRateController CreateFixedUpdateRateController() {
            var controller = new RateManagerFixedUpdateRateController(this, this._minimumFixedUpdateRate);
            controller.LimitChanged += this.OnFixedUpdateRateControllerMinimumChanged;
            return controller;
        }

        private RateManagerRenderIntervalController CreateRenderIntervalController() {
            var controller = new RateManagerRenderIntervalController(this, this._maximumRenderInterval);
            controller.LimitChanged += this.OnRenderIntervalControllerMaximumChanged;
            return controller;
        }

        #endregion <<---------- General ---------->>




        #region <<---------- Deprecated ---------->>

        [Obsolete("use .UpdateRate.Mode instead")]
        public UpdateRateMode UpdateRateMode {
            get => this.UpdateRate.Mode;
            set => this.UpdateRate.Mode = value;
        }

        [Obsolete("use .UpdateRate.Minimum instead")]
        public int MinimumUpdateRate {
            get => this.UpdateRate.Minimum;
            set => this.UpdateRate.Minimum = value;
        }

        [Obsolete("use .UpdateRate.Target instead")]
        public int TargetUpdateRate => this.UpdateRate.Target;

        [Obsolete("use .UpdateRate.ModeChanged instead")]
        public event Action<RateManager, UpdateRateMode> UpdateRateModeChanged {
            add {
                this.UpdateRate.ModeChanged += value;
            }
            remove {
                this.UpdateRate.ModeChanged -= value;
            }
        }

        [Obsolete("use .UpdateRate.CurrentChanged instead")]
        public event Action<RateManager, int> UpdateRateChanged {
            add {
                this.UpdateRate.CurrentChanged += value;
            }
            remove {
                this.UpdateRate.CurrentChanged -= value;
            }
        }

        [Obsolete("use .UpdateRate.TargetChanged instead")]
        public event Action<RateManager, int> TargetUpdateRateChanged {
            add {
                this.UpdateRate.TargetChanged += value;
            }
            remove {
                this.UpdateRate.TargetChanged -= value;
            }
        }

        [Obsolete("use .FixedUpdateRate.Minimum instead")]
        public int MinimumFixedUpdateRate {
            get => this.FixedUpdateRate.Minimum;
            set => this.FixedUpdateRate.Minimum = value;
        }

        [Obsolete("use .FixedUpdateRate.Target instead")]
        public int TargetFixedUpdateRate => this.FixedUpdateRate.Target;

        [Obsolete("use .FixedUpdateRate.CurrentChanged instead")]
        public event Action<RateManager, int> FixedUpdateRateChanged {
            add {
                this.FixedUpdateRate.CurrentChanged += value;
            }
            remove {
                this.FixedUpdateRate.CurrentChanged -= value;
            }
        }

        [Obsolete("use .FixedUpdateRate.TargetChanged instead")]
        public event Action<RateManager, int> TargetFixedUpdateRateChanged {
            add {
                this.FixedUpdateRate.TargetChanged += value;
            }
            remove {
                this.FixedUpdateRate.TargetChanged -= value;
            }
        }

        [Obsolete("use .RenderInterval.Maximum instead")]
        public int MaximumRenderInterval {
            get => this.RenderInterval.Maximum;
            set => this.RenderInterval.Maximum = value;
        }

        [Obsolete("use .RenderInterval.Target instead")]
        public int TargetRenderInterval => this.RenderInterval.Target;

        [Obsolete("use .RenderInterval.CurrentChanged instead")]
        public event Action<RateManager, int> RenderIntervalChanged {
            add {
                this.RenderInterval.CurrentChanged += value;
            }
            remove {
                this.RenderInterval.CurrentChanged -= value;
            }
        }

        [Obsolete("use .RenderInterval.TargetChanged instead")]
        public event Action<RateManager, int> TargetRenderIntervalChanged {
            add {
                this.RenderInterval.TargetChanged += value;
            }
            remove {
                this.RenderInterval.TargetChanged -= value;
            }
        }

        [Obsolete("use .RenderInterval.CurrentRenderRate instead")]
        public int RenderRate => this.RenderInterval.CurrentRenderRate;

        [Obsolete("use .RenderInterval.WillRender instead")]
        public bool WillRender => this.RenderInterval.WillRender;

        [Obsolete("use .RenderInterval.IsSupported instead")]
        public bool IsRenderIntervalSupported => this.RenderInterval.IsSupported;

        [Obsolete("use .UpdateRate.Request instead")]
        public UpdateRateRequest RequestUpdateRate(int updateRate) {
            return this.UpdateRate.Request(updateRate);
        }

        [Obsolete("use .FixedUpdateRate.Request instead")]
        public FixedUpdateRateRequest RequestFixedUpdateRate(int fixedUpdateRate) {
            return this.FixedUpdateRate.Request(fixedUpdateRate);
        }

        [Obsolete("use .RenderInterval.Request instead")]
        public RenderIntervalRequest RequestRenderInterval(int renderInterval) {
            return this.RenderInterval.Request(renderInterval);
        }

        #endregion <<---------- Deprecated ---------->>
    }
}