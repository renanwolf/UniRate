using UnityEditor;

namespace UniRate.Editor {

    [CustomEditor(typeof(RateRequestAnimationComponent), true)]
    [CanEditMultipleObjects]
    public class RateRequestAnimationComponentInspector : RateRequestComponentInspector {

        #region <<---------- Properties and Fields ---------->>
        
        private SerializedProperty _propClipName;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Editor ---------->>
        
        protected override void OnEnable() {
            base.OnEnable();
            this._propClipName = this.serializedObject.FindProperty("_clipName");
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if (!this._propClipName.hasMultipleDifferentValues) {
                var clipName = this._propClipName.stringValue;
                if (string.IsNullOrEmpty(clipName)) {
                    EditorGUILayout.HelpBox("rate requests will be active if any AnimationClip is playing", MessageType.Info);
                }
                else {
                    EditorGUILayout.HelpBox($"rate requests will be active only if AnimationClip named '{clipName}' is playing", MessageType.Info);
                }
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this._propClipName);
            if (EditorGUI.EndChangeCheck()) {
                this.serializedObject.ApplyModifiedProperties();
            }
        }
        
        #endregion <<---------- Editor ---------->>
    }
}