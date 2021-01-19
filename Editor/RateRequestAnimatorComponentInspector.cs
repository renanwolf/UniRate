using UnityEditor;

namespace UniRate.Editor {

    [CustomEditor(typeof(RateRequestAnimatorComponent), true)]
    [CanEditMultipleObjects]
    public class RateRequestAnimatorComponentInspector : RateRequestComponentInspector {

        #region <<---------- Properties and Fields ---------->>
        
        private SerializedProperty _propActivateRequestsInTransitions;
        private SerializedProperty _propLayerName;
        private SerializedProperty _propStateName;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Editor ---------->>
        
        protected override void OnEnable() {
            base.OnEnable();
            this._propActivateRequestsInTransitions = this.serializedObject.FindProperty("_activateRequestsInTransitions");
            this._propLayerName = this.serializedObject.FindProperty("_layerName");
            this._propStateName = this.serializedObject.FindProperty("_stateName");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this._propActivateRequestsInTransitions);
            if (EditorGUI.EndChangeCheck()) {
                this.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this._propLayerName);
            if (EditorGUI.EndChangeCheck()) {
                this.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this._propStateName);
            if (EditorGUI.EndChangeCheck()) {
                this.serializedObject.ApplyModifiedProperties();
            }

            var hasAnyMultipleDifferentValues = (
                this._propActivateRequestsInTransitions.hasMultipleDifferentValues
                || this._propLayerName.hasMultipleDifferentValues
                || this._propStateName.hasMultipleDifferentValues
            );
            if (hasAnyMultipleDifferentValues) return;

            var helpMessage = this.GenerateHelpMessage(
                this._propActivateRequestsInTransitions.boolValue,
                this._propLayerName.stringValue,
                this._propStateName.stringValue
            );
            EditorGUILayout.HelpBox(helpMessage, MessageType.Info);
        }
        
        #endregion <<---------- Editor ---------->>




        #region <<---------- General ---------->>
        
        private string GenerateHelpMessage(bool activeRequestsInTransitions, string layerName, string stateName) {
            var hasLayer = !string.IsNullOrEmpty(layerName);
            var hasState = !string.IsNullOrEmpty(stateName);

            var helpMessage = "requests will be active if";

            if (activeRequestsInTransitions) {
                helpMessage += " any transition is ocurring on";
                if (hasLayer) helpMessage += $" layer '{layerName}'";
                else helpMessage += " any layer";
                helpMessage += "\nor";
            }

            if (hasState) helpMessage += $" state '{stateName}'";
            else helpMessage += " any state";

            helpMessage += " is playing on";

            if (hasLayer) helpMessage += $" layer '{layerName}'";
            else helpMessage += " any layer";

            return helpMessage;
        }
        
        #endregion <<---------- General ---------->>
    }
}