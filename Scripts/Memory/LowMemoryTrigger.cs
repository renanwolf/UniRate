using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PWR.LowPowerMemoryConsumption {

    public class LowMemoryTrigger : MonoBehaviour {

        #region <<---------- Properties and Fields ---------->>

        [SerializeField] private UnityEvent _lowMemory;

        /// <summary>
        /// Event raised when <see cref="MemoryManager.LowMemory"/> is raised.
        /// </summary>
        public UnityEvent LowMemory {
            get {
                if (this._lowMemory == null) {
                    this._lowMemory = new UnityEvent();
                }
                return this._lowMemory;
            }
        }

        private bool _isApplicationQuitting = false;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>

        protected virtual void OnEnable() {
            MemoryManager.Instance.LowMemory += this.OnLowMemory;
        }

        protected virtual void OnDisable() {
            if (this._isApplicationQuitting) return;
            MemoryManager.Instance.LowMemory -= this.OnLowMemory;
        }

        protected virtual void OnApplicationQuit() {
            this._isApplicationQuitting = true;
        }

        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- General ---------->>

        protected virtual void OnLowMemory() {
            if (this._lowMemory == null) return;
            this._lowMemory.Invoke();
        }

        #endregion <<---------- General ---------->>




        #region <<---------- Legacy Support ---------->>

        #endregion <<---------- Legacy Support ---------->>




        #region <<---------- Custom Inspector ---------->>
		#if UNITY_EDITOR
		[CustomEditor(typeof(LowMemoryTrigger), true)]
        [CanEditMultipleObjects]
		protected class CustomInspectorBase : Editor {

			public override void OnInspectorGUI() {
				this.serializedObject.Update();
				this.DrawDefaultInspector();
				
				if (!MemoryManager.IsSupported) {
                    EditorGUILayout.HelpBox(MemoryManager.NotSupportedMessage, MessageType.Warning);
                    EditorGUILayout.Space();
                }
			}
		}
		#endif
		#endregion <<---------- Custom Inspector ---------->>
    }
}