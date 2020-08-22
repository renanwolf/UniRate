using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PWR.LowPowerMemoryConsumption {

    [Serializable]
    [Obsolete]
    public class RenderIntervalManagerPointer {
        
        #region <<---------- Properties and Fields ---------->>
        
        [SerializeField] private bool _byReference = true;

        [SerializeField] private string _identifier;

        [SerializeField] private RenderIntervalManager _reference;

        /// <summary>
        /// Event raised when any field change.
        /// </summary>
        public event Action<RenderIntervalManagerPointer> Changed {
            add {
                this._changed -= value;
                this._changed += value;
            }
            remove {
                this._changed -= value;
            }
        }
        private Action<RenderIntervalManagerPointer> _changed;

        /// <summary>
        /// Access manager by referece or by identifier?
        /// </summary>
        public bool ByReference {
            get { return this._byReference; }
            set {
                if (this._byReference == value) return;
                this._byReference = value;
                this.OnChanged();
            }
        }

        /// <summary>
        /// Manager by reference.
        /// </summary>
        public RenderIntervalManager ManagerByReference {
            get { return this._reference; }
            set {
                if (this._reference == value) return;
                this._reference = value;
                this.OnChanged();
            }
        }

        /// <summary>
        /// Manager by identifier.
        /// </summary>
        public string ManagerByIdentifier {
            get { return this._identifier; }
            set {
                if (this._identifier == value) return;
                this._identifier = value;
                this.OnChanged();
            }
        }
        
        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Callbacks ---------->>
        
        protected virtual void OnChanged() {
            if (this._changed == null) return;
            this._changed(this);
        }
        
        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>
        
        public RenderIntervalManager GetManager() => null;
        
        #endregion <<---------- General ---------->>




        #region <<---------- Custom Drawer ---------->>
        #if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(RenderIntervalManagerPointer), true)]
        private class MyCustomDrawer : PropertyDrawer {
            private SerializedProperty GetPropByRef(SerializedProperty property) {
                return property.FindPropertyRelative("_byReference");
            }
            private SerializedProperty GetPropRef(SerializedProperty property) {
                return property.FindPropertyRelative("_reference");
            }
            private SerializedProperty GetPropId(SerializedProperty property) {
                return property.FindPropertyRelative("_identifier");
            }
            public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
				int lines = 0;
                //label
                if (label != GUIContent.none && !string.IsNullOrEmpty(label.text)) lines += 1;
                //toggle
                lines += 1;

                var propByRef = this.GetPropByRef(property);
                //id
                if (!propByRef.boolValue || propByRef.hasMultipleDifferentValues) lines += 1;
                //ref
                if (propByRef.boolValue || propByRef.hasMultipleDifferentValues) lines += 1;

                return ((float)lines * EditorGUIUtility.singleLineHeight) + ((float)(lines - 1) * EditorGUIUtility.standardVerticalSpacing);
			}
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
                label = EditorGUI.BeginProperty(position, label, property);

                var rectLine = position;
                rectLine.height = EditorGUIUtility.singleLineHeight;

                //label
                if (label != GUIContent.none && !string.IsNullOrEmpty(label.text)) {
                    EditorGUI.PrefixLabel(rectLine, GUIUtility.GetControlID(FocusType.Passive), label);
                    rectLine.y += rectLine.height + EditorGUIUtility.standardVerticalSpacing;
                }

                int originalIndent = EditorGUI.indentLevel;
                EditorGUI.indentLevel += 1;

                //toggle
                var propByRef = this.GetPropByRef(property);
                EditorGUI.BeginChangeCheck();
                if (propByRef.hasMultipleDifferentValues) {
                    EditorGUI.PropertyField(rectLine, propByRef, new GUIContent("by Reference"));
                }
                else {
                    propByRef.boolValue = EditorGUI.ToggleLeft(rectLine, "by Reference", propByRef.boolValue);
                }
                if (EditorGUI.EndChangeCheck()) {
                    property.serializedObject.ApplyModifiedProperties();
                }
                rectLine.y += rectLine.height + EditorGUIUtility.standardVerticalSpacing;

                //id
                if (!propByRef.boolValue || propByRef.hasMultipleDifferentValues) {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(rectLine, this.GetPropId(property));
                    if (EditorGUI.EndChangeCheck()) {
                        property.serializedObject.ApplyModifiedProperties();
                    }
                    rectLine.y += rectLine.height + EditorGUIUtility.standardVerticalSpacing;
                }

                //ref
                if (propByRef.boolValue || propByRef.hasMultipleDifferentValues) {
                    EditorGUI.BeginChangeCheck();
                    EditorGUI.PropertyField(rectLine, this.GetPropRef(property));
                    if (EditorGUI.EndChangeCheck()) {
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }

                EditorGUI.indentLevel = originalIndent;
                EditorGUI.EndProperty();
			}
        }
        #endif
        #endregion <<---------- Custom Drawer ---------->>
    }
}