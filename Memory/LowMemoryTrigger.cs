using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PWR.LowPowerMemoryConsumption {

    public class LowMemoryTrigger : MonoBehaviour {

        #region <<---------- Properties ---------->>

        [SerializeField] private UnityEvent _onLowMemoryEvent;
        public UnityEvent onLowMemoryEvent {
            get {
                if (this._onLowMemoryEvent == null) {
                    this._onLowMemoryEvent = new UnityEvent();
                }
                return this._onLowMemoryEvent;
            }
        }

        private bool _isApplicationQuitting = false;

        #endregion <<---------- Properties ---------->>




        #region <<---------- MonoBehaviour ---------->>

        protected virtual void OnEnable() {
            MemoryManager.Instance.onLowMemory += this.OnLowMemoryCallback;
        }

        protected virtual void OnDisable() {
            if (this._isApplicationQuitting) return;
            MemoryManager.Instance.onLowMemory -= this.OnLowMemoryCallback;
        }

        protected virtual void OnApplicationQuit() {
            this._isApplicationQuitting = true;
        }

        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- General ---------->>

        protected virtual void OnLowMemoryCallback() {
            if (this._onLowMemoryEvent == null) return;
            this._onLowMemoryEvent.Invoke();
        }

        #endregion <<---------- General ---------->>




        #region <<---------- Custom Inspector ---------->>
		#if UNITY_EDITOR
		[CustomEditor(typeof(LowMemoryTrigger))]
		private class CustomInspector : Editor {

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