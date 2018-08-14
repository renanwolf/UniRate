using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PWR.LowPowerMemoryConsumption {

	public abstract class RateTriggerBase : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>

        protected bool isApplicationQuitting { get; private set; }

        #endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
			this.isApplicationQuitting = false;
		}

        protected virtual void OnEnable() {
			this.OnRateChangedCallback();
            this.StartListening();
        }

        protected virtual void OnDisable() {
            if (this.isApplicationQuitting) return;
            this.StopListening();
        }

        protected virtual void OnApplicationQuit() {
            this.isApplicationQuitting = true;
        }

		#if UNITY_EDITOR
		protected virtual void OnValidate() {
			if (!Application.isPlaying) return;
			this.OnRateChangedCallback();
		}
		#endif

        #endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- General ---------->>

		protected abstract void StartListening();

		protected abstract void StopListening();

		protected abstract int GetRate();

		protected abstract void OnRateChangedCallback(int rate);

		private void OnRateChangedCallback() {
			this.OnRateChangedCallback(this.GetRate());
		}

        #endregion <<---------- General ---------->>




		#region <<---------- Custom Inspector ---------->>
		#if UNITY_EDITOR
		[CustomEditor(typeof(RateTriggerBase), true)]
		[CanEditMultipleObjects]
		protected class CustomInspecotr : Editor {

			public override void OnInspectorGUI() {
				this.serializedObject.Update();
				this.DrawDefaultInspector();
				
				if (!FrameRateManager.IsSupported) {
					EditorGUILayout.HelpBox(FrameRateManager.NotSupportedMessage, MessageType.Warning);
					EditorGUILayout.Space();
				}
			}
		}
		#endif
		#endregion <<---------- Custom Inspector ---------->>
	}
}