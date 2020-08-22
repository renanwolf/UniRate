#if !UNITY_2019_3_OR_NEWER
using UnityEditor;

namespace UniRate.Editor {

    [CustomEditor(typeof(RateManager))]
    public class RateManagerInspector : UnityEditor.Editor {
        
        public override void OnInspectorGUI() {
            this.serializedObject.Update();
            this.DrawDefaultInspector();
            EditorGUILayout.HelpBox("render interval is only supported on Unity 2019.3 or newer", MessageType.Warning);
        }
    }
}
#endif