using UnityEditor;

namespace UniRate.Editor {

    [CustomEditor(typeof(RateRequestComponent), true)]
    [CanEditMultipleObjects]
    public class RateRequestComponentInspector : UnityEditor.Editor {
        
        #region <<---------- Properties and Fields ---------->>
        
        private SerializedProperty _propUpdateRatePresetOption;
        private SerializedProperty _propUpdateRateCustomValue;

        private SerializedProperty _propFixedUpdateRatePresetOption;
        private SerializedProperty _propFixedUpdateRateCustomValue;

        private SerializedProperty _propRenderIntervalPresetOption;
        private SerializedProperty _propRenderIntervalCustomValue;

        private readonly string[] _displayUpdateRate = new [] {
            $"{RateRequestComponent.PresetOptions.Ultra.ToString()}  ({RatePreset.Ultra.UpdateRate.ToString()})",
            $"{RateRequestComponent.PresetOptions.VeryHigh.ToString()}  ({RatePreset.VeryHigh.UpdateRate.ToString()})",
            $"{RateRequestComponent.PresetOptions.High.ToString()}  ({RatePreset.High.UpdateRate.ToString()})",
            $"{RateRequestComponent.PresetOptions.Medium.ToString()}  ({RatePreset.Medium.UpdateRate.ToString()})",
            $"{RateRequestComponent.PresetOptions.Low.ToString()}  ({RatePreset.Low.UpdateRate.ToString()})",
            $"{RateRequestComponent.PresetOptions.VeryLow.ToString()}  ({RatePreset.VeryLow.UpdateRate.ToString()})",
            RateRequestComponent.PresetOptions.Custom.ToString(),
        };

        private readonly string[] _displayFixedUpdateRate = new [] {
            $"{RateRequestComponent.PresetOptions.Ultra.ToString()}  ({RatePreset.Ultra.FixedUpdateRate.ToString()})",
            $"{RateRequestComponent.PresetOptions.VeryHigh.ToString()}  ({RatePreset.VeryHigh.FixedUpdateRate.ToString()})",
            $"{RateRequestComponent.PresetOptions.High.ToString()}  ({RatePreset.High.FixedUpdateRate.ToString()})",
            $"{RateRequestComponent.PresetOptions.Medium.ToString()}  ({RatePreset.Medium.FixedUpdateRate.ToString()})",
            $"{RateRequestComponent.PresetOptions.Low.ToString()}  ({RatePreset.Low.FixedUpdateRate.ToString()})",
            $"{RateRequestComponent.PresetOptions.VeryLow.ToString()}  ({RatePreset.VeryLow.FixedUpdateRate.ToString()})",
            RateRequestComponent.PresetOptions.Custom.ToString(),
        };

        private readonly string[] _displayRenderInterval = new [] {
            $"{RateRequestComponent.PresetOptions.Ultra.ToString()}  ({RatePreset.Ultra.RenderInterval.ToString()})",
            $"{RateRequestComponent.PresetOptions.VeryHigh.ToString()}  ({RatePreset.VeryHigh.RenderInterval.ToString()})",
            $"{RateRequestComponent.PresetOptions.High.ToString()}  ({RatePreset.High.RenderInterval.ToString()})",
            $"{RateRequestComponent.PresetOptions.Medium.ToString()}  ({RatePreset.Medium.RenderInterval.ToString()})",
            $"{RateRequestComponent.PresetOptions.Low.ToString()}  ({RatePreset.Low.RenderInterval.ToString()})",
            $"{RateRequestComponent.PresetOptions.VeryLow.ToString()}  ({RatePreset.VeryLow.RenderInterval.ToString()})",
            RateRequestComponent.PresetOptions.Custom.ToString(),
        };

        private const string STR_SPACE = " ";
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Editor ---------->>
        
        private void OnEnable() {
            this._propUpdateRatePresetOption = this.serializedObject.FindProperty("_updateRatePresetOption");
            this._propUpdateRateCustomValue = this.serializedObject.FindProperty("_updateRateCustomValue");
            
            this._propFixedUpdateRatePresetOption = this.serializedObject.FindProperty("_fixedUpdateRatePresetOption");
            this._propFixedUpdateRateCustomValue = this.serializedObject.FindProperty("_fixedUpdateRateCustomValue");
            
            this._propRenderIntervalPresetOption = this.serializedObject.FindProperty("_renderIntervalPresetOption");
            this._propRenderIntervalCustomValue = this.serializedObject.FindProperty("_renderIntervalCustomValue");
        }

        public override void OnInspectorGUI() {
            this.serializedObject.Update();
            this.DrawDefaultInspector();
            bool isCustom;

            EditorGUI.BeginChangeCheck();
            isCustom = this.DrawPresetOptionAndReturnIfCustom("Render Interval", this._displayRenderInterval, this._propRenderIntervalPresetOption);
            if (EditorGUI.EndChangeCheck()) {
                this.serializedObject.ApplyModifiedProperties();
            }
            if (isCustom) {
                EditorGUI.BeginChangeCheck();
                this._propRenderIntervalCustomValue.intValue = EditorGUILayout.IntField(STR_SPACE, this._propRenderIntervalCustomValue.intValue);
                if (EditorGUI.EndChangeCheck()) {
                    this.serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUI.BeginChangeCheck();
            isCustom = this.DrawPresetOptionAndReturnIfCustom("Update Rate", this._displayUpdateRate, this._propUpdateRatePresetOption);
            if (EditorGUI.EndChangeCheck()) {
                this.serializedObject.ApplyModifiedProperties();
            }
            if (isCustom) {
                EditorGUI.BeginChangeCheck();
                this._propUpdateRateCustomValue.intValue = EditorGUILayout.IntField(STR_SPACE, this._propUpdateRateCustomValue.intValue);
                if (EditorGUI.EndChangeCheck()) {
                    this.serializedObject.ApplyModifiedProperties();
                }
            }
            
            EditorGUI.BeginChangeCheck();
            isCustom = this.DrawPresetOptionAndReturnIfCustom("Fixed Update Rate", this._displayFixedUpdateRate, this._propFixedUpdateRatePresetOption);
            if (EditorGUI.EndChangeCheck()) {
                this.serializedObject.ApplyModifiedProperties();
            }
            if (isCustom) {
                EditorGUI.BeginChangeCheck();
                this._propFixedUpdateRateCustomValue.intValue = EditorGUILayout.IntField(STR_SPACE, this._propFixedUpdateRateCustomValue.intValue);
                if (EditorGUI.EndChangeCheck()) {
                    this.serializedObject.ApplyModifiedProperties();
                }
            }
            
            #if !UNITY_2019_3_OR_NEWER
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("render interval is only supported on Unity 2019.3 or newer", MessageType.Warning);
            #endif
        }
        
        #endregion <<---------- Editor ---------->>




        #region <<---------- General ---------->>
        
        private bool DrawPresetOptionAndReturnIfCustom(string label, string[] display, SerializedProperty property) {
            int enumIndex = (property.hasMultipleDifferentValues ? -1 : property.enumValueIndex);
            enumIndex = EditorGUILayout.Popup(label, enumIndex, display);
            var enumValue = (RateRequestComponent.PresetOptions)enumIndex;
            if (enumIndex != -1) {
                property.enumValueIndex = enumIndex;
            }
            return (enumValue == RateRequestComponent.PresetOptions.Custom);
        }
        
        #endregion <<---------- General ---------->>
    }
}