using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PWR.LowPowerMemoryConsumption {

    public class LowMemoryTrigger : MonoBehaviour {

        #region <<---------- Properties and Fields ---------->>

        [SerializeField] private UnityEvent _lowMemoryEvent;

        /// <summary>
        /// Event raised when <see cref="MemoryManager.LowMemory"/> is raised.
        /// </summary>
        public UnityEvent LowMemoryEvent {
            get {
                if (this._lowMemoryEvent == null) {
                    this._lowMemoryEvent = new UnityEvent();
                }
                return this._lowMemoryEvent;
            }
        }

        private bool _isApplicationQuitting = false;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- MonoBehaviour ---------->>

        protected virtual void OnEnable() {
            MemoryManager.Instance.LowMemory += this.NotifyLowMemory;
        }

        protected virtual void OnDisable() {
            if (this._isApplicationQuitting) return;
            MemoryManager.Instance.LowMemory -= this.NotifyLowMemory;
        }

        protected virtual void OnApplicationQuit() {
            this._isApplicationQuitting = true;
        }

        #endregion <<---------- MonoBehaviour ---------->>




        #region <<---------- General ---------->>

        protected virtual void NotifyLowMemory() {
            if (this._lowMemoryEvent == null) return;
            this._lowMemoryEvent.Invoke();
        }

        #endregion <<---------- General ---------->>




        #region <<---------- Legacy Support ---------->>

        [System.Obsolete("use LowMemoryEvent instead", false)] // ObsoletedWarning 2018/08/22 - ObsoletedError 20##/##/##
		public UnityEvent OnLowMemoryEvent {
			get { return this.LowMemoryEvent; }
		}

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