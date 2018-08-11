using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PWR.LowPowerMemoryConsumption {

    public class MemoryManager : MonoBehaviour {

        #region <<---------- Singleton ---------->>

        private static MemoryManager _instance = null;

		/// <summary>
		/// Singleton instance.
		/// </summary>
        public static MemoryManager Instance {
            get {
				if (_instance == null) {
					_instance = GameObject.FindObjectOfType<MemoryManager>();
					if (_instance == null) {
						var goInstance = new GameObject();
						_instance = goInstance.AddComponent<MemoryManager>();
					}
				}
                return _instance;
            }
        }

        #endregion <<---------- Singleton ---------->>




		#region <<---------- Properties ---------->>
		
		/// <summary>
		/// Should unload unused assets ( <see cref="Resources.UnloadUnusedAssets"/>) when application receives low memory warning?
		/// </summary>
		[HideInInspector] public bool unloadUnusedAssetsOnLowMemory = true;

		/// <summary>
		/// Should collect garbage ( <see cref="GC.Collect"/>) when application receives low memory warning?
		/// </summary>
		[HideInInspector] public bool collectGarbageOnLowMemory = true;

		[SerializeField][HideInInspector] private UnityEvent _onLowMemoryEvent;

		/// <summary>
		/// Event invoked when application receives low memory warning.
		/// </summary>
		public UnityEvent onLowMemoryEvent {
			get {
				if (this._onLowMemoryEvent == null) {
					this._onLowMemoryEvent = new UnityEvent();
				}
				return this._onLowMemoryEvent;
			}
		}

		/// <summary>
		/// Action invoked when application receives low memory warning.
		/// </summary>
		public Action onLowMemory;

		/// <summary>
		/// According to Unity documentation, lowMemory event is only supported on iOS and Android.
		/// </summary>
		/// <value>Returns true if Editor, iOS, or Android. Otherwise false.</value>
		public static bool IsSupported {
			get {
				#if (UNITY_IOS || UNITY_ANDROID)
				return true;
				#else
				return false;
				#endif
			}
		}

		private Action _onFinishedUnloadUnusedAssets;

		private Coroutine _coroutineUnloadUnusedAssets;

		public const string NotSupportedMessage = "According to Unity documentation, lowMemory event is only supported on iOS and Android.";

		private const string DefaultName = "Memory Manager";

		#endregion <<---------- Properties ---------->>




        #region <<---------- MonoBehaviour ---------->>

        private void Awake() {

			if (_instance == null) {
				_instance = this;
			}
			else if (_instance != this) {

				if (Debug.isDebugBuild) Debug.LogWarning("[" + typeof(MemoryManager).Name + "] trying to create another instance, destroying it", this);

				#if UNITY_EDITOR
				if (!Application.isPlaying) {
					DestroyImmediate(this);
					return;
				}
				#endif
				Destroy(this);
				return;
			}

			this.name = DefaultName;
			this.transform.SetParent(null, false);
			DontDestroyOnLoad(this);

			if (!MemoryManager.IsSupported) Debug.LogWarning("[" + typeof(MemoryManager).Name + "] " + MemoryManager.NotSupportedMessage, this);
        }

		private void OnEnable() {
			if (_instance != this) return;
			Application.lowMemory += this.OnApplicationLowMemoryCallback;
		}

		private void OnDisable() {
			if (_instance != this) return;
			Application.lowMemory -= this.OnApplicationLowMemoryCallback;
		}

		private void OnDestroy() {
			if (_instance != this) return;
			_instance = null;
		}

		#if UNITY_EDITOR
		private void OnValidate() {
			this.name = DefaultName;
		}
		private void Reset() {
			this.name = DefaultName;
		}
		#endif

        #endregion <<---------- MonoBehaviour ---------->>




		#region <<---------- Unused Assets Unload ---------->>

		/// <summary>
		/// Unload unused assets invoking <see cref="Resources.UnloadUnusedAssets"/> in a Coroutine.
		/// </summary>
		/// <param name="onFinishedHandler">Callback handler when unload finishes.</param>
		public void UnloadUnusedAssets(Action onFinishedHandler) {
			if (onFinishedHandler != null) this._onFinishedUnloadUnusedAssets += onFinishedHandler;
			if (this._coroutineUnloadUnusedAssets != null) return;
			this._coroutineUnloadUnusedAssets = this.StartCoroutine(this.CoroutineUnloadUnusedAssets());
		}

		/// <summary>
		/// Unload unused assets invoking <see cref="Resources.UnloadUnusedAssets"/> in a Coroutine.
		/// </summary>
		public void UnloadUnusedAssets() {
			this.UnloadUnusedAssets(null);
		}

		private IEnumerator CoroutineUnloadUnusedAssets() {

			yield return Resources.UnloadUnusedAssets();

			this._coroutineUnloadUnusedAssets = null;
			if (this._onFinishedUnloadUnusedAssets != null) {
				this._onFinishedUnloadUnusedAssets();
				this._onFinishedUnloadUnusedAssets = null;
			}
		}

		#endregion <<---------- Unused Assets Unload ---------->>




		#region <<---------- General ---------->>

		/// <summary>
		/// Collect garbage, same as <see cref="GC.Collect"/>.
		/// </summary>
		public void CollectGarbage() {
			GC.Collect();
		}

		/// <summary>
		/// Will invoke all callbacks registered to <see cref="onLowMemory"/> and <see cref="onLowMemoryEvent"/>. And also perform unused resources unload and garbage collect if they are enabled.
		/// </summary>
		public void SimulateLowMemory() {
			if (Debug.isDebugBuild) Debug.Log("[" + typeof(MemoryManager).Name + "] simulating low memory", this);
			this.OnApplicationLowMemoryCallback();
		}

		private void OnApplicationLowMemoryCallback() {
			if (Debug.isDebugBuild) Debug.LogWarning("[" + typeof(MemoryManager).Name + "] received application low memory callback", this);

			if (this.onLowMemory != null) this.onLowMemory();

			if (this._onLowMemoryEvent != null) this._onLowMemoryEvent.Invoke();

			if (this.unloadUnusedAssetsOnLowMemory) {
				this.UnloadUnusedAssets(() => {
					if (this == null) return;
					if (this.collectGarbageOnLowMemory) this.CollectGarbage();
				});
				return;
			}

			if (this.collectGarbageOnLowMemory) this.CollectGarbage();
		}

		#endregion <<---------- General ---------->>




		#region <<---------- Custom Inspector ---------->>
		#if UNITY_EDITOR
		[CustomEditor(typeof(MemoryManager))]
		private class CustomInspector : Editor {

			private SerializedProperty propUnloadUnusedAssetsOnLowMemory;
			private SerializedProperty propCollectGarbageOnLowMemory;
			private SerializedProperty propOnLowMemoryEvent;

			void OnEnable() {
				this.propUnloadUnusedAssetsOnLowMemory = this.serializedObject.FindProperty("unloadUnusedAssetsOnLowMemory");
				this.propCollectGarbageOnLowMemory = this.serializedObject.FindProperty("collectGarbageOnLowMemory");
				this.propOnLowMemoryEvent = this.serializedObject.FindProperty("_onLowMemoryEvent");
			}

			public override void OnInspectorGUI() {
				this.serializedObject.Update();
				this.DrawDefaultInspector();
				
				EditorGUI.BeginChangeCheck();
				this.propUnloadUnusedAssetsOnLowMemory.boolValue = EditorGUILayout.ToggleLeft("Unload Unused Assets On Low Memory", this.propUnloadUnusedAssetsOnLowMemory.boolValue);
				if (EditorGUI.EndChangeCheck()) {
					this.serializedObject.ApplyModifiedProperties();
				}

				EditorGUI.BeginChangeCheck();
				this.propCollectGarbageOnLowMemory.boolValue = EditorGUILayout.ToggleLeft("Collect Garbage On Low Memory", this.propCollectGarbageOnLowMemory.boolValue);
				if (EditorGUI.EndChangeCheck()) {
					this.serializedObject.ApplyModifiedProperties();
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(this.propOnLowMemoryEvent);
				if (EditorGUI.EndChangeCheck()) {
					this.serializedObject.ApplyModifiedProperties();
				}

				EditorGUILayout.Space();
				bool originalGUIEnabled = GUI.enabled;
				GUI.enabled = Application.isPlaying;
				if (GUILayout.Button("Simulate Low Memory")) {
					(this.target as MemoryManager).SimulateLowMemory();
				}
				GUI.enabled = originalGUIEnabled;
				EditorGUILayout.Space();


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