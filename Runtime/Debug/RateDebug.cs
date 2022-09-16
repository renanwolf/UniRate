using System;
using System.Diagnostics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityDebug = UnityEngine.Debug;
using UnityObject = UnityEngine.Object;

namespace UniRate.Debug {

    public static class RateDebug {

        #region <<---------- Initializers ---------->>
        
        static RateDebug() {
            #if UNITY_EDITOR
            LogLevel = (IsDebugBuild ? RateLogLevel.Debug : RateLogLevel.Info);
            #else
            LogLevel = (IsDebugBuild ? RateLogLevel.Info : RateLogLevel.Warning);
            #endif

            ScreenDataBackgroundColor = new Color(0, 0, 0, 0.25f);
            ScreenDataFontSize = 12;
            ScreenDataFontColor = Color.white;
            ScreenDataVerbose = true;
            DisplayOnScreenData = IsDebugBuild;
        }
        
        #endregion <<---------- Initializers ---------->>
        
        
        
        
        #region <<---------- Properties and Fields ---------->>

        public static bool IsDebugBuild {
            get {
                #if UNITY_EDITOR
                return EditorUserBuildSettings.development;
                #else
                return UnityDebug.isDebugBuild;
                #endif
            }
        }
        
        public static RateLogLevel LogLevel { get; set; }

        public static bool DisplayOnScreenData { get; set; }

        public static bool ScreenDataVerbose { get; set; }

        public static Color ScreenDataBackgroundColor {
            get => _screenDataBackgroundColor;
            set {
                if (_screenDataBackgroundColor == value) return;
                _screenDataBackgroundColor = value;
                OnScreenDataBackgroundColorChanged(_screenDataBackgroundColor);
            }
        }
        private static Color _screenDataBackgroundColor;
        
        public static int ScreenDataFontSize {
            get => _screenDataFontSize;
            set {
                if (_screenDataFontSize == value) return;
                _screenDataFontSize = value;
                OnScreenDataFontSizeChanged(_screenDataFontSize);
            }
        }
        private static int _screenDataFontSize;

        public static Color ScreenDataFontColor {
            get => _screenDataFontColor;
            set {
                if (_screenDataFontColor == value) return;
                _screenDataFontColor = value;
                OnScreenDataFontColorChanged(_screenDataFontColor);
            }
        }
        private static Color _screenDataFontColor;

        private static Texture2D ScreenDataBackgroundTexture {
            get {
                if (_screenDataBackgroundTexture == null) {
                    _screenDataBackgroundTexture = CreateScreenDataBackgroundTexture(_screenDataBackgroundColor);
                }
                return _screenDataBackgroundTexture;
            }
        }
        private static Texture2D _screenDataBackgroundTexture;

        internal static GUIStyle ScreenDataStyle {
            get {
                if (_screenDataStyle == null) {
                    _screenDataStyle = CreateScreenDataStyle(
                        _screenDataFontSize,
                        _screenDataFontColor,
                        ScreenDataBackgroundTexture
                    );
                }
                return _screenDataStyle;
            }
        }
        private static GUIStyle _screenDataStyle;

        private const string LOG_PREFIX = "[UniRate] ";
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Callbacks ---------->>
        
        private static void OnScreenDataBackgroundColorChanged(Color color) {
            if (_screenDataBackgroundTexture != null) {
                UnityObject.Destroy(_screenDataBackgroundTexture);
                _screenDataBackgroundTexture = null;
            }
            if (_screenDataStyle == null) return;
            _screenDataStyle.normal.background = ScreenDataBackgroundTexture;
        }

        private static void OnScreenDataFontSizeChanged(int fontSize) {
            if (_screenDataStyle == null) return;
            _screenDataStyle.fontSize = fontSize;
        }

        private static void OnScreenDataFontColorChanged(Color color) {
            if (_screenDataStyle == null) return;
            _screenDataStyle.normal.textColor = color;
        }
        
        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>
        
        public static bool IsLogLevelActive(RateLogLevel level) {
            return (level >= LogLevel);
        }

        internal static void Log(RateLogLevel level, string message, UnityObject context = null) {
            switch (level) {
                case RateLogLevel.Trace: UnityDebug.Log($"{LOG_PREFIX}{message}", context); return;
                case RateLogLevel.Debug: UnityDebug.Log($"{LOG_PREFIX}{message}", context); return;
                case RateLogLevel.Info: UnityDebug.Log($"{LOG_PREFIX}{message}", context); return;
                case RateLogLevel.Warning: UnityDebug.LogWarning($"{LOG_PREFIX}{message}", context); return;
                case RateLogLevel.Error: UnityDebug.LogError($"{LOG_PREFIX}{message}", context); return;
                case RateLogLevel.Off: return;
                default: throw new NotImplementedException($"not handling {nameof(RateLogLevel)}.{level.ToString()}");
            }
        }
        
        private static Texture2D CreateScreenDataBackgroundTexture(Color color) {
            var texture = new Texture2D(1, 1);
            var colorArray = texture.GetPixels();
            for (int i = colorArray.Length - 1; i >= 0; i--) {
                colorArray[i] = color;
            }
            texture.SetPixels(colorArray);
            texture.Apply();
            return texture;
        }

        private static GUIStyle CreateScreenDataStyle(int fontSize, Color fontColor, Texture2D backgroundTexture) {
            var style = new GUIStyle(GUI.skin.label);
            style.fontSize = fontSize;
            style.normal.textColor = fontColor;
            style.normal.background = backgroundTexture;
            style.padding = new RectOffset(2, 2, 1, 1);
            return style;
        }

        internal static string GetCurrentStackTrace(int skipFrames) {
            var stackTraceObject = new StackTrace(skipFrames + 1, true);
            return stackTraceObject?.ToString();
        }

        #endregion <<---------- General ---------->>
    }
}