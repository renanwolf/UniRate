#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityDebug = UnityEngine.Debug;
using UnityObject = UnityEngine.Object;

namespace UniRate.Debug {

    public static class Debugger {

        #region <<---------- Initializers ---------->>
        
        static Debugger() {
            #if UNITY_EDITOR
            LogLevel = (IsDebugBuild ? LogLevel.Trace : LogLevel.Debug);
            #else
            LogLevel = (IsDebugBuild ? LogLevel.Debug : LogLevel.Info);
            #endif
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
        
        public static LogLevel LogLevel { get; set; }

        public static bool DisplayOnScreenData { get; set; }
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- General ---------->>
        
        public static bool IsLogLevelActive(LogLevel level) {
            return (level >= LogLevel);
        }

        internal static void Log(LogLevel level, string message, UnityObject context = null) {
            switch (level) {
                case LogLevel.Trace: UnityDebug.Log($"[UniRate] {message}", context); return;
                case LogLevel.Debug: UnityDebug.Log($"[UniRate] {message}", context); return;
                case LogLevel.Info: UnityDebug.Log($"[UniRate] {message}", context); return;
                case LogLevel.Warning: UnityDebug.LogWarning($"[UniRate] {message}", context); return;
                case LogLevel.Error: UnityDebug.LogError($"[UniRate] {message}", context); return;
            }
        }
        
        #endregion <<---------- General ---------->>
    }
}