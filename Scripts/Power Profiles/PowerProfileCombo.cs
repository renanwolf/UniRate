using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace PWR.LowPowerMemoryConsumption {

    [Obsolete]
    public class PowerProfileCombo : PowerProfile {

        #region <<---------- Properties and Fields ---------->>

        [SerializeField][HideInInspector] private List<PowerProfile> _profiles;
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Callbacks ---------->>
        
        protected override void OnIsRetainedChanged(bool isRetained) {
            if (this._profiles == null) return;
            int count = this._profiles.Count;
            if (count <= 0) return;
            PowerProfile profile;
            for (int i = 0; i < count; i++) {
                profile = this._profiles[i];
                if (profile == null) continue;
                if (profile == this) {
                    Debug.LogError("[" + this.GetType().Name + "/" + this.name + "] recursive interation, this object is inside its own profile list", this);
                    continue;
                }
                profile.RetainOrRelease(isRetained);
            }
        }
        
        #endregion <<---------- Callbacks ---------->>




        #region <<---------- Custom Inspector ---------->>
        #if UNITY_EDITOR
        [CustomEditor(typeof(PowerProfileCombo), true)]
        private class CustomInspector : Editor {
            private ReorderableList reordableProfiles;
            private void OnEnable() {
                this.reordableProfiles = new ReorderableList(
                    this.serializedObject,
                    this.serializedObject.FindProperty("_profiles"),
                    true,
                    true,
                    true,
                    true
                );
                this.reordableProfiles.drawHeaderCallback = (position) => {
					EditorGUI.LabelField(position, "Profile List");
				};
                this.reordableProfiles.elementHeight = (EditorGUIUtility.singleLineHeight * 1f) + (EditorGUIUtility.standardVerticalSpacing * 2f) + 8f;
                this.reordableProfiles.drawElementCallback = (rect, index, isActive, isFocused) => {
					rect.y += (EditorGUIUtility.standardVerticalSpacing * 1f) + 4f;
                    rect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.PropertyField(rect, this.reordableProfiles.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
				};
            }
            public override void OnInspectorGUI() {
                this.serializedObject.Update();
                this.DrawDefaultInspector();
                EditorGUI.BeginChangeCheck();
                this.reordableProfiles.DoLayoutList();
                if (EditorGUI.EndChangeCheck()) {
                    this.serializedObject.ApplyModifiedProperties();
                }
            }
        }
        #endif
        #endregion <<---------- Custom Inspector ---------->>
    }
}